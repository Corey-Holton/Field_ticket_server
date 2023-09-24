using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using CA.Ticketing.Persistance.Models.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Sync
{
    public class SyncProcessor : EntityServiceBase, ISyncProcessor
    {
        private readonly IFileManagerService _fileManagerService;
        public SyncProcessor(CATicketingContext context, IMapper mapper, IFileManagerService fileManagerService) : base(context, mapper)
        {
            _fileManagerService = fileManagerService;
        }

        public async Task<(IEnumerable<object> Entities, DateTime LastModifiedDate)> GetEntities<T>(DateTime lastModifiedDate, bool isClientRequest)
        {
            if (typeof(T) == typeof(ApplicationUser))
            {
                var users = await _context.Users.ToListAsync();

                if (!isClientRequest)
                {
                    return (users, DateTime.UtcNow);
                }

                users = users.Where(x => x.LastModifiedDate > lastModifiedDate).ToList();

                if (users.Any())
                {
                    var employeeRole = await _context.Roles.FirstAsync(x => x.Name == RoleNames.ToolPusher);
                    var employeeUserIds = (await _context.UserRoles
                        .Where(x => x.RoleId == employeeRole.Id)
                        .ToListAsync())
                        .Select(x => x.UserId);

                    users = users
                        .Where(x => employeeUserIds.Contains(x.Id))
                        .ToList();
                }

                var lastModifiedDateResult = users.Select(x => x.LastModifiedDate).DefaultIfEmpty(DateTime.UtcNow).FirstOrDefault();
                return (users.Select(x => (object)x), lastModifiedDateResult);
            }

            if (typeof(T) == typeof(IdentityRole))
            {
                var roles = await _context.Roles.ToListAsync();
                return (roles.Select(x => (object)x), DateTime.UtcNow);
            }

            if (typeof(T) == typeof(IdentityUserRole<string>))
            {
                var userRoles = await _context.UserRoles.ToListAsync();
                return (userRoles.Select(x => (object)x), DateTime.UtcNow);
            }

            var methodInfo = GetType()
                .GetMethod(nameof(GetEntitiesFromDb))!
                .MakeGenericMethod(typeof(T));
            var (Entities, LastModifiedDate) = await (Task<(IEnumerable<T> Entities,DateTime LastModifiedDate)>)methodInfo!
                    .Invoke(this, new object[] { lastModifiedDate })!;

            return (Entities.Select(x => (object)x!), LastModifiedDate);
        }

        public async Task<(IEnumerable<T> Entities, DateTime LastModifiedDate)> GetEntitiesFromDb<T>(DateTime lastModifiedTime) where T : IdentityModel
        {
            var entities = await _context.Set<T>()
                .IgnoreQueryFilters()
                .Where(x => x.LastModifiedDate >= lastModifiedTime)
                .ToListAsync();

            var lastModifiedDate = entities.Select(x => x.LastModifiedDate).DefaultIfEmpty(DateTime.UtcNow).Max();

            if (typeof(T) == typeof(FieldTicket) || typeof(T) == typeof(EquipmentFile))
            {
                foreach (var entity in entities)
                {
                    if (entity.DeletedDate.HasValue)
                    {
                        continue;
                    }

                    if (entity is FieldTicket fieldTicket && !string.IsNullOrEmpty(fieldTicket.FileName))
                    {
                        fieldTicket.FileBytes = _fileManagerService.GetFileBytes(FilePaths.Tickets, fieldTicket.FileName);
                    }
                    
                    if (entity is EquipmentFile equipmentFile)
                    {
                        equipmentFile.FileBytes = _fileManagerService.GetFileBytes(Path.Combine(FilePaths.Equipment, equipmentFile.EquipmentId), equipmentFile.FileIndicator);
                    }
                }
            }

            return (entities, lastModifiedDate);
        }

        public async Task<DateTime?> UpdateEntities<T>(IEnumerable<object> entities, bool isServerUpdate)
        {
            if (!entities.Any())
            {
                return null;
            }

            if (typeof(T) == typeof(ApplicationUser))
            {
                var userEntities = entities.Select(x => CastToType<ApplicationUser>(x)).ToList();
                var localUsers = await _context.Users.ToListAsync();
                foreach (var entity in userEntities)
                {
                    var user = localUsers.FirstOrDefault(x => x.Id == entity.Id);
                    if (user != null)
                    {
                        if (user.LastModifiedDate >= entity.LastModifiedDate)
                        {
                            continue;
                        }

                        _mapper.Map(entity, user);
                    }
                    else if (isServerUpdate)
                    {
                        _context.Users.Add(entity);
                    }
                }

                if (isServerUpdate)
                {
                    localUsers
                        .Where(x => !userEntities.Select(x => x.Id).Contains(x.Id))
                        .ToList()
                        .ForEach(x => _context.Entry(x).State = EntityState.Deleted);
                }

                _context.SaveChanges();
                return userEntities.Select(x => x.LastModifiedDate).Max();
            }

            if (typeof(T) == typeof(IdentityRole))
            {
                var roleEntities = entities.Select(x => CastToType<IdentityRole>(x)).ToList();
                var allRoles = await _context.Roles.ToListAsync();
                foreach (var entity in roleEntities)
                {
                    var role = allRoles.FirstOrDefault(x => x.Id == entity.Id);
                    if (role != null)
                    {
                        _mapper.Map(entity, role);
                    }
                    else
                    {
                        _context.Roles.Add(entity);
                    }
                }

                allRoles
                    .Where(x => !roleEntities.Select(r => r.Id).Contains(x.Id))
                    .ToList()
                    .ForEach(x => _context.Entry(x).State = EntityState.Deleted);

                _context.SaveChanges();
                return DateTime.UtcNow;
            }

            if (typeof(T) == typeof(IdentityUserRole<string>))
            {
                var userRoleEntities = entities.Select(x => CastToType<IdentityUserRole<string>>(x)).ToList();
                var allUserRoles = await _context.UserRoles.ToListAsync();
                foreach (var entity in userRoleEntities)
                {
                    var userRole = allUserRoles
                        .FirstOrDefault(x => x.RoleId == entity.RoleId && x.UserId == entity.UserId);
                    if (userRole != null)
                    {
                        _mapper.Map(entity, userRole);
                    }
                    else
                    {
                        _context.UserRoles.Add(entity);
                    }
                }

                allUserRoles
                    .Where(x => !userRoleEntities.Any(ur => ur.RoleId == x.RoleId && ur.UserId == x.UserId))
                    .ToList()
                    .ForEach(x => _context.Entry(x).State = EntityState.Deleted);

                _context.SaveChanges();
                return DateTime.UtcNow;
            }
            var entitiesTransformed = entities.Select(x => CastToType<T>(x)).ToList();
            var methodInfo = GetType()
                .GetMethod(nameof(UpdateDbEntities))!
                .MakeGenericMethod(typeof(T));
            return await (Task<DateTime?>)methodInfo!.Invoke(this, new object[] { entitiesTransformed })!;
        }

        public async Task<DateTime?> UpdateDbEntities<T>(IEnumerable<T> entities) where T : IdentityModel
        {
            var dbSet = _context.Set<T>();

            foreach (var entity in entities)
            {
                var existingEntity = await dbSet
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == entity.Id);
                if (existingEntity != null)
                {
                    if (existingEntity.LastModifiedDate >= entity.LastModifiedDate)
                    {
                        continue;
                    }

                    _mapper.Map(entity, existingEntity);
                }
                else
                {
                    dbSet.Add(entity);
                }

                if (entity is IFileEntity fileEntity)
                {
                    FieldTicket? fieldTicket = null;
                    EquipmentFile? equipmentFile = null;

                    if (typeof(T) == typeof(FieldTicket))
                    {
                        fieldTicket = entity as FieldTicket;
                    }

                    if (typeof(T) == typeof(EquipmentFile))
                    {
                        equipmentFile = entity as EquipmentFile;
                    }

                    if (entity.DeletedDate.HasValue)
                    {
                        if (fieldTicket != null)
                        {
                            _fileManagerService.DeleteFile(FilePaths.Tickets, fieldTicket.FileName);
                        }
                        else if (equipmentFile != null)
                        {
                            _fileManagerService.DeleteFile(Path.Combine(FilePaths.Equipment, equipmentFile.EquipmentId), equipmentFile.FileIndicator);
                        }

                        continue;
                    }

                    if (fileEntity?.FileBytes == null)
                    {
                        continue;
                    }

                    if (fieldTicket != null)
                    {
                        _fileManagerService.SaveFile(fileEntity.FileBytes, FilePaths.Tickets, fieldTicket.FileName);
                    }
                    else if (equipmentFile != null)
                    {
                        _fileManagerService.SaveFile(fileEntity.FileBytes, Path.Combine(FilePaths.Equipment, equipmentFile.EquipmentId), equipmentFile.FileIndicator);
                    }
                }
            }

            _context.SaveChanges();

            _context.ChangeTracker.Clear();

            return entities.Select(x => x.LastModifiedDate).Max();
        }

        private static T CastToType<T>(object input) => JsonConvert.DeserializeObject<T>(input.ToString()!)!;
    }
}
