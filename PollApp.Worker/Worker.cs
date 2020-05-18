using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PollApp.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPollTabulation _pollTabulation;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IPollTabulation pollTabulation)
        {
            _logger = logger;
            _configuration = configuration;
            _pollTabulation = pollTabulation;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _pollTabulation.RunPollTabulationProcess();
        }
    }
}
