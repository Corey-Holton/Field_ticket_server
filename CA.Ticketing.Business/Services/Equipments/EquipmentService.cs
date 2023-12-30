using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Equipments.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Business.Services.Removal;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Business.Authentication;

namespace CA.Ticketing.Business.Services.Equipments
{
    public class EquipmentService : EntityServiceBase, IEquipmentService
    {
        private readonly IUserContext _userContext;

        private readonly IFileManagerService _fileManagerService;

        private readonly IRemovalService _removalService;

        public EquipmentService(
            CATicketingContext context,
            IMapper mapper,
            IUserContext userContext,
            IFileManagerService fileManagerService,
            IRemovalService removalService) : base (context, mapper) 
        {
            _userContext = userContext;
            _fileManagerService = fileManagerService;
            _removalService = removalService;
        }

        public async Task<IEnumerable<EquipmentDto>> GetAll()
        {
            return (await _context.Equipment
                .ToListAsync()).Select(x => _mapper.Map<EquipmentDto>(x));
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllByCategory(int equipmentCategory)
        {
            return (await _context.Equipment
                .Where(x => (int)x.Category == equipmentCategory)
                .ToListAsync()).Select(x => _mapper.Map<EquipmentDto>(x));
        }

        public async Task<EntityDtoBase?> GetByEmployeeAssigned()
        {
            var userId = _userContext.User!.Id;

            var user = await _context.Users.SingleAsync(x => x.Id == userId);
            if (string.IsNullOrEmpty(user.EmployeeId))
            {
                return null;
            }
            var employee = _context.Employees.SingleOrDefault(x => x.Id == user.EmployeeId);

            var equipment = await _context.Equipment.FirstOrDefaultAsync(x => x.Id == employee!.AssignedRigId);

            if (equipment == null)
            {
                return null;
            }

            return _mapper.Map<EntityDtoBase>(equipment);
        }

        public async Task<EquipmentDetailsDto> GetById(string? id)
        {
            var equipment = await GetEquipment(id);
            return _mapper.Map<EquipmentDetailsDto>(equipment);
        }

        public async Task<string> Create(EquipmentDetailsDto entity)
        {
            var equipment = _mapper.Map<Equipment>(entity);
            
            if (equipment.Category == EquipmentCategory.Rig)
            {
                var allEquipmentCharges = await _context.Charges
                    .Where(x => x.IsRigSpecific)
                    .ToListAsync();

                foreach (var equipmentCharge in allEquipmentCharges)
                {
                    equipment.Charges.Add(new EquipmentCharge
                    {
                        ChargeId = equipmentCharge.Id,
                        Rate = equipmentCharge.DefaultRate
                    });
                }
            }
            
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
            return equipment.Id;
        }

        public async Task Update(EquipmentDetailsDto entity)
        {
            var equipment = await GetEquipment(entity.Id);

            _mapper.Map(entity, equipment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var equipment = await _context.Equipment
                .Include(x => x.Crew)
                .Include(x => x.Files)
                .Include(x => x.Tickets)
                .Include(x => x.ScheduledJobs)
                .Include(x => x.Charges)
                .AsSplitQuery()
                .SingleAsync(x => x.Id == id);

            _removalService.Remove(equipment);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EquipmentChargeDto>> GetEquipmentCharges(string? id)
        {
            var equipmentCharges = await _context.EquipmentCharges
                .Include(x => x.Charge)
                .Where(x => x.EquipmentId == id)
                .ToListAsync();
            return equipmentCharges.Select(x => _mapper.Map<EquipmentChargeDto>(x));
        }

        public async Task UpdateEquipmentCharges(IEnumerable<EquipmentChargeDto> chargesDto)
        {
            var chargesIds = chargesDto
                .Select(x => x.Id)
                .ToList();

            var charges = await _context.EquipmentCharges
                .Where(x => chargesIds.Contains(x.Id))
                .ToListAsync();

            foreach (var chargeDto in chargesDto)
            {
                var charge = charges.Single(x => x.Id == chargeDto.Id);
                _mapper.Map(chargeDto, charge);
            }
            
            await _context.SaveChangesAsync();
        }

        private async Task<Equipment> GetEquipment(string? id)
        {
            var equipment = await _context.Equipment
                .SingleOrDefaultAsync(x => x.Id == id);

            if (equipment == null)
            {
                throw new KeyNotFoundException(nameof(Equipment));
            }

            return equipment!;
        }

        public async Task<IEnumerable<EquipmentDetailsDto>> GetExpiringPermitEquipment()
        {
            return (await _context.Equipment.ToListAsync())
                .Where(x => DateTimeExtensions.IsWithinMonth(x.PermitExpirationDate))
                .Select(x => _mapper.Map<EquipmentDetailsDto>(x));
        }

        public async Task<IEnumerable<RigWithNextJobDto>> GetRigsWithJobData(DateTime today)
        {
            return (await _context.Equipment
                .Where(x => x.Category == EquipmentCategory.Rig)
                .ToListAsync())
                .Select(rig =>
                {
                    var nextJob = _context.Scheduling
                        .OrderBy(schedule => schedule.StartTime)
                        .FirstOrDefault(x => x.EquipmentId == rig.Id && x.EndTime >= today);

                    var lastJob = _context.FieldTickets
                        .OrderByDescending(x => x.ExecutionDate)
                        .FirstOrDefault(x => x.EquipmentId == rig.Id);

                    var rigDto = _mapper.Map<EquipmentDto>(rig);

                    var getDaysUntilNextJob = () => 
                    { 
                        if (nextJob == null)
                        {
                            return -1;
                        }

                        if (nextJob.StartTime > today)
                        {
                            return (nextJob.StartTime - today).Days;
                        }

                        return 0;
                    };

                    return new RigWithNextJobDto
                    {
                        Rig = rigDto,
                        DaysUntilNextJob = getDaysUntilNextJob(),
                        DaysSinceLastJob = (today - lastJob?.ExecutionDate)?.Days ?? -1
                    };
                });
        }

        public async Task UploadFile(Stream fileStream, string equipmentId, string assignedName, string fileName, string contentType)
        {
            var equipment = await _context.Equipment.SingleAsync(x => x.Id == equipmentId);

            var equipmentFile = new EquipmentFile 
            { 
                ContentType = contentType,
                DisplayName = assignedName,
                FileName = fileName,
                FileIndicator = Path.GetRandomFileName()
            };

            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();

            _fileManagerService.SaveFile(fileBytes, Path.Combine(FilePaths.Equipment, equipment.Id), equipmentFile.FileIndicator);

            equipment.Files.Add(equipmentFile);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EquipmentFileDto>> GetFilesList(string equipmentId)
        {
            var equipment = await _context.Equipment
                .Include(x => x.Files)
                .SingleAsync(x => x.Id == equipmentId);

            return equipment.Files
                .Select(x => _mapper.Map<EquipmentFileDto>(x)).ToList();
        }

        public async Task<(byte[] FileBytes, EquipmentFileDto FileDto)> DownloadFile(string fileId)
        {
            var equipmentFile = await _context.EquipmentFiles.SingleAsync(x => x.Id == fileId);
            var fileBytes = _fileManagerService.GetFileBytes(Path.Combine(FilePaths.Equipment, equipmentFile.EquipmentId), equipmentFile.FileIndicator);
            return (fileBytes, _mapper.Map<EquipmentFileDto>(equipmentFile));
        }

        public async Task DeleteFile(string fileId)
        {
            var equipmentFile = await _context.EquipmentFiles.SingleAsync(x => x.Id == fileId);
            _context.EquipmentFiles.Remove(equipmentFile);
            _fileManagerService.DeleteFile(Path.Combine(FilePaths.Equipment, equipmentFile.EquipmentId), equipmentFile.FileIndicator);
            await _context.SaveChangesAsync();
        }
    }
}
