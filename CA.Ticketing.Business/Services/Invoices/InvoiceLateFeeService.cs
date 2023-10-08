using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CA.Ticketing.Business.Services.Invoices
{
    public class InvoiceLateFeeService : IHostedService
    {
        private readonly Timer _timer;

        private readonly IServiceProvider _serviceProvider;

        private readonly double _timerInterval = TimeSpan.FromHours(1).TotalMilliseconds;

        private ILogger<InvoiceLateFeeService> _logger;

        public InvoiceLateFeeService(IServiceProvider serviceProvider, ILogger<InvoiceLateFeeService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _timer = new Timer(_timerInterval)
            {
                AutoReset = false
            };

            _timer.Elapsed += VerifyLateinvoicesAction;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            return Task.CompletedTask;
        }

        private void VerifyLateinvoicesAction(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var verificationTask = VerifyLateInvoices();
                verificationTask.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error while executing check invoice function", ex);
            }
            finally
            {
                _timer.Start();
            }
        }

        private async Task VerifyLateInvoices()
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<CATicketingContext>();

            var backgroundJob = await context.BackgroundJobs.SingleOrDefaultAsync(x => x.Name == BusinessConstants.BackgroundJobNames.InvoiceLateFees);

            if (backgroundJob == null)
            {
                return;
            }

            if (backgroundJob.LastRunOn.HasValue && (backgroundJob.LastRunOn.Value - DateTime.UtcNow).TotalDays > 1)
            {
                return;
            }

            var unpaidInvoices = await context.Invoices
                .Include(x => x.Customer)
                .Where(x => !x.Paid && DateTime.UtcNow > x.DueDate)
                .ToListAsync();

            var modifiedInvoices = new List<Invoice>();

            foreach (var unpaidInvoice in unpaidInvoices)
            {
                var daysSinceDueDate = (DateTime.UtcNow - unpaidInvoice.DueDate).TotalDays;
                var monthsLate = (int)Math.Floor(daysSinceDueDate / 30);

                var currentLateFeesCount = await context.InvoiceLateFees
                    .IgnoreQueryFilters()
                    .CountAsync(x => x.InvoiceId == unpaidInvoice.Id);

                if (currentLateFeesCount < monthsLate && unpaidInvoice.SentToCustomer.HasValue)
                {
                    modifiedInvoices.Add(unpaidInvoice);
                }

                while (currentLateFeesCount < monthsLate)
                {
                    unpaidInvoice.InvoiceLateFees.Add(new InvoiceLateFee() { SentToCustomer = true });
                    currentLateFeesCount++;
                }
            }

            await context.SaveChangesAsync();

            if (modifiedInvoices.Any())
            {
                var invoicesService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                
                foreach (var invoice in modifiedInvoices)
                {
                    await invoicesService.SendToCustomer(invoice.Id);
                }
            }

            backgroundJob.LastRunOn = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }
    }
}
