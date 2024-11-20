using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CameraRentalApp.Services
{
    public class TransactionStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionStatusUpdater> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public TransactionStatusUpdater(IServiceProvider serviceProvider, ILogger<TransactionStatusUpdater> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateTransactionStatusesAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task UpdateTransactionStatusesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var transactionService = scope.ServiceProvider.GetRequiredService<TransactionService>();

                _logger.LogInformation("Checking and updating transaction statuses...");
                await transactionService.CheckAndUpdateTransactionStatusesAsync();
            }
        }
    }
}
