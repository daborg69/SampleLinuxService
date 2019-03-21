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
        IApplicationLifetime appLifetime;
        //ILogger<LinuxHostingService> logger;
        IHostingEnvironment environment;
        IConfiguration configuration;
        

        public LinuxHostingService (IConfiguration configuration,
                                    IHostingEnvironment environment,
                                    ILogger<LinuxHostingService> logger,
                                    IApplicationLifetime appLifetime
                                    ) {
            this.configuration = configuration;
            
            
            //this.logger = logger;
            

            //Log.Logger = new LoggerConfiguration();
            LoggerConfiguration lc = new LoggerConfiguration();
            lc.WriteTo.Console();
            lc.WriteTo.File ("C:\\temp\\log.txt");
            //lc.CreateLogger();
            Log.Logger = lc.CreateLogger();

            
            //Log.Logger = new LoggerConfiguration().CreateLogger();
            
            

            this.appLifetime = appLifetime;
            this.environment = environment;


            // A:  Retrieve an environment variable
            string sampleDB = configuration.GetValue<string> ("DB");
            Console.WriteLine("Environment variable [SAMPLE_DB] = " + sampleDB);


            // B:  Retrieve a config variable from the appsettings.json file - the non environment specific one.
            string globalSetting1 = configuration.GetValue<string> ("FromAppSettingBase:setting1");
            Console.WriteLine("Value from AppSettings.json:  [FromAppSettingBase:setting1] = " + globalSetting1);


            // C:  Retrieve a setting originall defined in appsettings.json file, but also exists in appsettings.[environment].json file.
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
            Log.Information("StartAsync method called.");
//            this.logger.LogInformation ("StartAsync method called.");

            this.appLifetime.ApplicationStarted.Register (OnStarted);
            this.appLifetime.ApplicationStopping.Register (OnStopping);
            this.appLifetime.ApplicationStopped.Register (OnStopped);

            return Task.CompletedTask;

        }


        private void OnStarted () {
            Log.Information("OnStarted called.");
            //this.logger.LogInformation ("OnStarted method called.");

            // Post-startup code goes here  
        }


        private void OnStopping () {
            Log.Information ("OnStopped method called");
            //this.logger.LogInformation ("OnStopping method called.");

            // On-stopping code goes here  
        }

        private void OnStopped() {
            Log.Information ("OnStopped method called");
            Log.CloseAndFlush();
                //this.logger.LogInformation("OnStopped method called.");

            // Post-stopped code goes here  
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("StopAsync method called");
            //this.logger.LogInformation("StopAsync method called.");

            
            return Task.CompletedTask;
        }
    }
}
