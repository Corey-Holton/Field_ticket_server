﻿using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CA.Ticketing.Persistance.Seed
{
    public class DatabaseInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly CATicketingContext _context;

        private const string _adminUserName = "admin";

        private const string _adminInitialPassword = "I@mAdm1nUs3r";

        public DatabaseInitializer(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            CATicketingContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task InitializeAsync(bool isMainServer)
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (!isMainServer)
            {
                return;
            }

            await CreateRoles();
            await CreateBaseUser();
            await InitiateCharges();
            await InitiateTypes();
            await InitiateSettings();
            await InitiateBackgroundJobs();
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

        private async Task InitiateTypes()
        {
            var defaultCharges = GetCharges();

            var defaultTypes = GetTypes();

            var typeNames = defaultTypes.Select(x => x.Name).ToList();

            var typesToRemove = await _context.TicketType
                .Where(x => !typeNames.Contains(x.Name))
                .ToListAsync();

            _context.TicketType.RemoveRange(typesToRemove);

            await _context.SaveChangesAsync();

            var existingTypes = await _context.TicketType.ToListAsync();

            var charges = await _context.Charges.ToListAsync();


            foreach (var ticketType in defaultTypes)
            {
                var existingType = existingTypes.SingleOrDefault(x => x.Name == ticketType.Name);
                var newType = ticketType;
                if (existingType == null)
                {
                    if (ticketType.Name == "Base")
                    {
                        foreach (var charge in charges)
                        {
                            if (charge.IncludeInTicketSpecs)
                            {
                                newType.IncludedCharges.Add(charge);
                            }

                        }
                        _context.TicketType.Add(newType);
                        continue;
                    }

                    if(ticketType.Name == "Well")
                    {
                        foreach (var charge in charges)
                        {
                            if (charge.Name != ChargeNames.SwabCups)
                            {
                                newType.IncludedCharges.Add(charge);
                            }
                            if (!charge.IncludeInTicketSpecs || charge.Name == ChargeNames.Labor)
                            {
                                newType.SpecialCharges.Add(charge);
                            }
                        }
                         _context.TicketType.Add(newType);
                        continue;
                    }
                }                   
            }

            await _context.SaveChangesAsync();
        }

        private async Task InitiateCharges()
        {
            var defaultCharges = GetCharges();

            var chargesNames = defaultCharges.Select(x => x.Name).ToList();

            var chargesToRemove = await _context.Charges
                .Where(x => !chargesNames.Contains(x.Name))
                .ToListAsync();

            _context.Charges.RemoveRange(chargesToRemove);

            await _context.SaveChangesAsync();

            var chargesGrouped = (await _context.Charges
                .ToListAsync()).GroupBy(x => x.Name);

            foreach (var chargeGroup in chargesGrouped)
            {
                if (chargeGroup.Count() > 1)
                {
                    foreach(var charge in chargeGroup.Skip(1).ToList())
                    {
                        _context.Entry(charge).State = EntityState.Deleted;
                    }
                }
            }

            await _context.SaveChangesAsync();

            var rigChargesGrouped = (await _context.EquipmentCharges.ToListAsync()).GroupBy(x => x.ChargeId);

            foreach(var rigChargeGroup in rigChargesGrouped)
            {
                if (rigChargeGroup.Count() > 1)
                {
                    foreach (var rigCharge in rigChargeGroup.Skip(1).ToList())
                    {
                        _context.Entry(rigCharge).State = EntityState.Deleted;
                    }
                }
            }

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
                TaxRate = 7.5,
                MileageCost = 1,
                OvertimePercentageIncrease = 10
            });

            await _context.SaveChangesAsync();
        }

        private async Task InitiateBackgroundJobs()
        {
            var backgroundJobs = await _context.BackgroundJobs.ToListAsync();

            var invoiceLateFeesJob = backgroundJobs.SingleOrDefault(x => x.Name == BusinessConstants.BackgroundJobNames.InvoiceLateFees);

            if (invoiceLateFeesJob == null)
            {
                _context.BackgroundJobs.Add(new BackgroundJob { Name = BusinessConstants.BackgroundJobNames.InvoiceLateFees });
                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateTicketTotals()
        {
            var isAnyTotalUpdated = _context.FieldTickets.Any(x => x.Total > 0);

            if (isAnyTotalUpdated)
            {
                return;
            }

            var allTickets = await _context.FieldTickets
                .Include(x => x.TicketSpecifications)
                .IgnoreQueryFilters()
                .ToListAsync();

            foreach (var ticket in allTickets)
            {
                ticket.Total = ticket.TicketSpecifications.Sum(x => x.Quantity * x.Rate);
            }

            _context.SaveChanges();
        }

        private async Task UpdateTimes()
        {
            var allUpdated = (await _context.FieldTickets
                .ToListAsync())
                .All(x => x.ExecutionDate.TimeOfDay.TotalSeconds == 0);

            if (allUpdated)
            {
                return;
            }

            var allTickets = await _context.FieldTickets
                .IgnoreQueryFilters()
                .ToListAsync();

            foreach (var ticket in allTickets)
            {
                ticket.ExecutionDate = ticket.ExecutionDate.Date;
                if (ticket.StartTime.HasValue)
                {
                    ticket.StartTime = new DateTime(ticket.ExecutionDate.Year, ticket.ExecutionDate.Month, ticket.ExecutionDate.Day, ticket.StartTime.Value.Hour, ticket.StartTime.Value.Minute, 0);
                }

                if (ticket.EndTime.HasValue)
                {
                    ticket.EndTime = new DateTime(ticket.ExecutionDate.Year, ticket.ExecutionDate.Month, ticket.ExecutionDate.Day, ticket.EndTime.Value.Hour, ticket.EndTime.Value.Minute, 0);
                }
            }

            await _context.SaveChangesAsync();
        }
        public static IEnumerable<TicketType> GetTypes()
        {
            return new List<TicketType>
            {
                new TicketType {Name = "Base"},
                new TicketType {Name = "Well" },
            };
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
                new Charge { Order = 28, Name = ChargeNames.PowerTbgTongs, DefaultRate = 10, UoM = UnitOfMeasure.EA, IsRigSpecific = false, IncludeInTicketSpecs = false, AllowRateAdjustment = false },
                new Charge { Order = 29, Name = ChargeNames.Labor, DefaultRate = 45, UoM = UnitOfMeasure.Hourly, IsRigSpecific = false, IncludeInTicketSpecs = true },
                new Charge { Order = 30, Name = ChargeNames.InsuranceFuelSurcharge, DefaultRate = 20, UoM = UnitOfMeasure.Hourly, IsRigSpecific = false, IncludeInTicketSpecs = false, AllowRateAdjustment = true },
                new Charge { Order = 31, Name = ChargeNames.RigTime, DefaultRate = 30, UoM = UnitOfMeasure.Hourly, IsRigSpecific = false, IncludeInTicketSpecs = false, AllowRateAdjustment = true }
            };
        }
    }
}
