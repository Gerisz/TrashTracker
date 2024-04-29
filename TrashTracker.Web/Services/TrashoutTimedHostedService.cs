using System.Diagnostics;
using System.Text.Json;
using TrashTracker.Web.Services;

namespace TrachTracker.Web.Services
{
    public class TrashoutTimedHostedService : BackgroundService
    {
        private Int32 executionCount = 0;
        private readonly ILogger<TrashoutTimedHostedService> _logger;

        public IServiceProvider Services { get; set; }

        public TrashoutTimedHostedService(IServiceProvider services,
        ILogger<TrashoutTimedHostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            try
            {
                await DoWork(stoppingToken);
            }
            catch (HttpRequestException)
            {
                _logger.LogError("No internet connection, trying again in 30 minutes.");
            }

            using PeriodicTimer timer = new(TimeSpan.FromMinutes(30));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork(stoppingToken);
                }
            }
            catch (HttpRequestException)
            {
                _logger.LogError("No internet connection, trying again in 30 minutes.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        /// <summary>
        /// The periodical syncing of database executed by the timer of this class
        /// </summary>
        /// <param name="state"></param>
        private async Task DoWork(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var count = Interlocked.Increment(ref executionCount);

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<TrashoutService>();
                try
                {
                    await scopedProcessingService.GetPlaceListAsync(null);
                }
                catch (JsonException e)
                {
                    _logger.LogInformation($"Error: {e.Message}");
                }
            }

            stopwatch.Stop();

            _logger.LogInformation($"Timed Hosted Service is working.\n" +
                $"Count: {executionCount}\n" +
                $"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Stops the periodical syncing of database
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
