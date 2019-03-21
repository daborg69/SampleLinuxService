using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;


namespace SampleLinuxService {
    public class LinuxHostingService : IHostedService {
        IHostApplicationLifetime appLifetime;
        ILogger<LinuxHostingService> _logger;
        IHostEnvironment environment;
        IConfiguration configuration;
        

        public LinuxHostingService (IConfiguration configuration,
                                    IHostEnvironment environment,
                                    ILogger<LinuxHostingService> logger,
                                    IHostApplicationLifetime appLifetime
                                    ) {
            this.configuration = configuration;
            this._logger = logger;
            this.appLifetime = appLifetime;
            this.environment = environment;


            // A:  Retrieve an environment variable
            string sampleDB = configuration.GetValue<string> ("DB");
            Console.WriteLine("Environment variable [SAMPLE_DB] = " + sampleDB);


            // B:  Retrieve a config variable from the appsettings.json file - the non environment specific one.
            string globalSetting1 = configuration.GetValue<string> ("FromAppSettingBase:setting1");
            Console.WriteLine("Value from AppSettings.json:  [FromAppSettingBase:setting1] = " + globalSetting1);


            // C:  Retrieve a setting originally defined in appsettings.json file, but also exists in appsettings.[environment].json file.
            //     Will contain value from the environment version of the appsettings file.
            //     Value should be "Second"
            string overriddenSetting = configuration.GetValue<string> ("WillBeOverridden");
            Console.WriteLine("Overridden Value from appsettings.development.json:  [WillBeOverridden] = " + overriddenSetting);


            // D:  Retrieve a setting only defined in the environment specific version of the file - appsettings.development.json file.
            string onlyInDev = configuration.GetValue<string> ("UniqueToDevelopment");
            Console.WriteLine("Value only defined in appsettings.development.json file:  [UniqueToDevelopment] = " + onlyInDev);

            // Retrieve an Appsettings variable

            string connection = configuration.GetValue<string> ("SampleDB:Connection");
            
        }


        public Task StartAsync (CancellationToken cancellationToken) {
            _logger.LogInformation("StartAsync method called!");
            _logger.LogDebug("Debugging log statement.");


            // E:  Log with parameters that will be indexable on backends.
            string firstName = "George";
            string lastName = "Jetson";      
            _logger.LogInformation("Ooops {firstName} did it again {lastName}.", firstName, lastName);

            // F:  Log with Event ID
            EventId x = new EventId(500,"Bad");
            _logger.LogCritical(x,"Oh NOOOOO!");



            appLifetime.ApplicationStarted.Register (OnStarted);
            appLifetime.ApplicationStopping.Register (OnStopping);
            appLifetime.ApplicationStopped.Register (OnStopped);

            return Task.CompletedTask;

        }


        private void OnStarted () {
            _logger.LogInformation("OnStarted Method started.");

            // Post-startup code goes here  
        }


        private void OnStopping () {
            _logger.LogInformation("OnStopped method called");
            

            // On-stopping code goes here  
        }

        private void OnStopped() {
            _logger.LogInformation("OnStopped method called");

            // Post-stopped code goes here  
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync method called");
            //this._logger.LogInformation("StopAsync method called.");

            

            return Task.CompletedTask;
        }
    }
}
