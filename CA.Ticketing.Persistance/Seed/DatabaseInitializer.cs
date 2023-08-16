using AutoMapper;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Persistance.Seed
{
    public class DatabaseInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly CATicketingContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;

        private const string _adminUserName = "admin";

        private const string _adminInitialPassword = "I@mAdm1nUs3r";

        private readonly IMapper _mapper;

        public DatabaseInitializer(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            CATicketingContext context, 
            IWebHostEnvironment hostEnvironment,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        public async Task InitializeAsync()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }

            await CreateRoles();
            await CreateBaseUser();
            await InitiateCharges();
            await InitiateSettings();
        }

        private async Task CreateRoles()
        {
            foreach (string roleName in StringExtensions.GetRoleNames())
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    continue;
                }

                role = new IdentityRole(roleName);

                await _roleManager.CreateAsync(role);
            }
        }

        private async Task CreateBaseUser()
        {
            var adminUser = await _userManager.FindByNameAsync(_adminUserName);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser()
                {
                    Email = "info@readydev.tech",
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = _adminUserName,
                    TicketIdentifier = "SA"
                };

                await _userManager.CreateAsync(adminUser, _adminInitialPassword);
            }

            var adminRoles = await _userManager.GetRolesAsync(adminUser);

            if (!adminRoles.Contains(RoleNames.Admin))
            {
                await _userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
            }
        }

        private async Task InitiateCharges()
        {
            var defaultCharges = GetCharges();
            
            var chargesNames = defaultCharges.Select(x => x.Name).ToList();

            var chargesToRemove = await _context.Charges
                .Where(x => chargesNames.Contains(x.Name))
                .ToListAsync();

            _context.Charges.RemoveRange(chargesToRemove);

            await _context.SaveChangesAsync();

            var existingCharges = await _context.Charges.ToListAsync();

            foreach (var charge in defaultCharges)
            {
                var existingCharge = existingCharges.SingleOrDefault(x => x.Name == charge.Name);
                if (existingCharge == null)
                {
                    _context.Charges.Add(charge);
                }
            }

            await _context.SaveChangesAsync();

            var rigCharges = await _context.Charges
                .Where(x => x.IsRigSpecific)
                .ToListAsync();

            var rigChargesIds = rigCharges
                .Select(x => x.Id)
                .ToList();

            var rigs = await _context.Equipment
                .Include(x => x.Charges)
                .Where(x => x.Category == EquipmentCategory.Rig)
                .ToListAsync();

            foreach (var rig in rigs)
            {
                var rigChargesToRemove = rig.Charges.Where(x => !rigChargesIds.Contains(x.ChargeId)).ToList();
                rigChargesToRemove.ForEach(x => _context.Entry(x).State = EntityState.Deleted);

                var currentChargesIds = rig.Charges
                    .Select(x => x.ChargeId)
                    .ToList();

                var chargesToAdd = rigCharges
                    .Where(x => !currentChargesIds.Contains(x.Id))
                    .ToList();

                foreach (var chargeToAdd in chargesToAdd)
                {
                    rig.Charges.Add(new EquipmentCharge { ChargeId = chargeToAdd.Id, Rate = chargeToAdd.DefaultRate });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task InitiateSettings()
        {
            var setting = await _context.Settings.FirstOrDefaultAsync();

            if (setting != null)
            {
                return;
            }

            _context.Settings.Add(new Setting 
            { 
                FuelCalculationMultiplier = 15,
                TaxRate = 7.5,
                MileageCost = 1,
                OvertimePercentageIncrease = 10
            });

            await _context.SaveChangesAsync();
        }

        private static IEnumerable<Charge> GetCharges()
        {
            return new List<Charge>()
            {
                new Charge { Order = 1, Name = ChargeNames.Permit, DefaultRate = 50, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 2, Name = ChargeNames.Rig, DefaultRate = 350, UoM = UnitOfMeasure.Hourly, IsRigSpecific = true, IncludeInTicketSpecs = true },
                new Charge { Order = 3, Name = ChargeNames.PumpOrTank, DefaultRate = 35, UoM = UnitOfMeasure.Hourly, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 4, Name = ChargeNames.BOP, DefaultRate = 200, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 5, Name = ChargeNames.PowerSwivelOrSub, DefaultRate = 1200, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 6, Name = ChargeNames.TTWValve, DefaultRate = 0, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 7, Name = ChargeNames.HydraulicRodTongs, DefaultRate = 100, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 8, Name = ChargeNames.ToolPusher, DefaultRate = 500, UoM = UnitOfMeasure.Daily, IsRigSpecific = true, IncludeInTicketSpecs = true },
                new Charge { Order = 9, Name = ChargeNames.ExtraHand, DefaultRate = 45, UoM = UnitOfMeasure.Hourly, IsRigSpecific = true, IncludeInTicketSpecs = true },
                new Charge { Order = 10, Name = ChargeNames.Fuel, DefaultRate = 4, UoM = UnitOfMeasure.Gallon, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 11, Name = ChargeNames.OvershotStanding, DefaultRate = 50, UoM = UnitOfMeasure.EA, IsRigSpecific = true, IncludeInTicketSpecs = true },
                new Charge { Order = 12, Name = ChargeNames.Valve, DefaultRate = 0, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 13, Name = ChargeNames.PipeDope, DefaultRate = 105, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 14, Name = ChargeNames.RodStripperRubber, DefaultRate = 150, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 15, Name = ChargeNames.PerDiem, DefaultRate = 150, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 16, Name = ChargeNames.TbgStripperRubber, DefaultRate = 195, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 17, Name = ChargeNames.TubingWiper, DefaultRate = 130, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 18, Name = ChargeNames.SwabCups, DefaultRate = 39, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 19, Name = ChargeNames.OilSaverRubber, DefaultRate = 35, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 20, Name = ChargeNames.Paint, DefaultRate = 50, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 21, Name = ChargeNames.TravelTime, DefaultRate = 190, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 22, Name = ChargeNames.CrewTruck, DefaultRate = 100, UoM = UnitOfMeasure.Daily, IsRigSpecific = true, IncludeInTicketSpecs = true },
                new Charge { Order = 23, Name = ChargeNames.PipeTrailer, DefaultRate = 200, UoM = UnitOfMeasure.Daily, IsRigSpecific = true, IncludeInTicketSpecs = true },
                new Charge { Order = 24, Name = ChargeNames.PipeRacks, DefaultRate = 25, UoM = UnitOfMeasure.Daily, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 25, Name = ChargeNames.ThirdParty, DefaultRate = 0, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true, AllowRateAdjustment = true },
                new Charge { Order = 26, Name = ChargeNames.Trucking, DefaultRate = 0, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = true, AllowRateAdjustment = true },
                new Charge { Order = 27, Name = ChargeNames.Other, DefaultRate = 0, UoM = UnitOfMeasure.None, IsRigSpecific = false, IncludeInTicketSpecs = true, AllowRateAdjustment = true, AllowUoMChange = true },
                new Charge { Order = 28, Name = ChargeNames.Labor, DefaultRate = 45, UoM = UnitOfMeasure.Hourly, IsRigSpecific = false, IncludeInTicketSpecs = true }
            };
        }
    }
}
