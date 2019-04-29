using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SampleLinuxService
{
    /// <summary>
    /// Demonstrates a background service.  Is easier to implemented than directly inheriting from IHostedService.
    /// </summary>
    public class SampleBackgroundService : BackgroundService {
        private readonly ILogger<SampleBackgroundService> _log;
        private IConfiguration _configuration;


        public SampleBackgroundService (IConfiguration configuration, ILogger<SampleBackgroundService> logger) {
            _log = logger;
            _configuration = configuration;
        }



        /// <summary>
        /// The main execution routine for the service.
        /// </summary>
        /// <param name="stoppingToken">The Cancellation token used when the service is being stopped.</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation("SampleBackgroundService is starting");

            stoppingToken.Register(() => StopService());


            // This is the do work routine.
            while ( !stoppingToken.IsCancellationRequested ) {
                _log.LogDebug("SampleBackgroundService is doing background work.");

                // Do work here.
                
                // sleep...
                await Task.Delay (TimeSpan.FromSeconds (5), stoppingToken);
            }
            throw new NotImplementedException();
        }

        

        /// <summary>
        /// Called When the service is being cancelled.
        /// </summary>
        protected void StopService () {
            _log.LogInformation("Stopping SampleBackgroundService");
            
        }
    }
}
