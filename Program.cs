using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;


namespace SampleLinuxService
{
    /// <summary>
    /// This program is a sample app that shows how to use the HostBuilder to setup and run a service oriented application. Below the square brackets
    /// indicate where in the program this is demonstrated (Mostly in the LinuxHostingService class).
    /// It demonstrates the following things:
    ///  - You must have the environment variable  ASPNETCORE_ENVIRONMENT = development set in order for it to read the appsettings.development.json file.
    ///    * In production this would be set to Production to read an appsettings.production.json file.
    ///  - It will read environment variables
    ///    * It will load any environment variable that is prefixed with SAMPLE_.  The prefix is removed and just a variable
    ///      with the part after the prefix is created.  For example if you in your environment set a variable SAMPLE_DB = aTest
    ///      then you will have a variable name DB set to "aTest".
    ///    * We read the environment variable in the LinuxHostingService class.
    ///  - [B] It will read settings from an AppSettings.json file using a JSON file Reader (Example B: in LinuxHostingService)
    ///  - [C, D] It will read settings from an AppSettings.Development.json file using a JSON file reader [D].  Any settings with the same
    ///    name that existed in the non environment specific AppSettings.Json file will be overriden with the value from the environment
    ///    specific version.  See Example C:
    ///  - Logging:  All logging is configured from the appsettings.development.json file.
    ///  - It will log to 3 sinks at the moment (It is important that the Name parameter of the WriteTo setting specifies the exact sink type or else
    ///    the config will not load properly.  So the name: RollingFile is not a symbolic name, but rather the actual name of the sink provider RollingFile.  
    ///    * Console.
    ///    * A rolling log file - RollingFile
    ///    * A permanent log file - File
    /// </summary>
    class Program
    {

	    static async Task Main (string [] args) {
		    try {
			    IHost host = new HostBuilder()

			                 // TODO Not sure we need this first ConfigureHostConfiguration as its interface is deprecated and it just passes its values onto the ConfigureAppConfiguration parameters anyway.
			                 .ConfigureHostConfiguration(configHost => {
				                 configHost.SetBasePath(Directory.GetCurrentDirectory());
				                 configHost.AddJsonFile("hostsettings.json", optional: true);
				                 configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
				                 configHost.AddCommandLine(args);
			                 })
			                 .ConfigureAppConfiguration((hostContext, configApp) => {
				                 configApp.SetBasePath(Directory.GetCurrentDirectory());
				                 configApp.AddJsonFile($"appsettings.json", true);
				                 configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
				                 configApp.AddEnvironmentVariables(prefix: "SAMPLE_");
				                 configApp.AddCommandLine(args);
			                 })
			                 .ConfigureLogging((hostContext, configLogging) => {
				                 configLogging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger());

// We NEVER want to log to Console in production - well at least if it is an API or Web app we do not - it will kill performance.
#if DEBUG
				                 //configLogging.AddConsole();
				                 configLogging.AddDebug();
#endif
			                 })
			                 .ConfigureServices((hostContext, services) => {
				                 services.AddLogging();
				                 services.AddHostedService<LinuxHostingService>();
			                 })
			                 .Build();

			    await host.RunAsync();

				// Remove this next statement for normal programs - just used to demonstrate an error situation.
				throw new ApplicationException("Test throwing an exception");

				// Exit the app.
			    Console.WriteLine("Flushing logs");
			    Log.CloseAndFlush();
			    Console.ReadKey();
		    } // End Try
		    catch ( Exception e ) {
			    string message =
				    "An unexpected error occurred during processing.  Because of where this error occured, there most likely will be no log of this error and the error message below will be the only copy available to provide to others.";
				Console.WriteLine(message);
				Log.Logger.Fatal(e,message);
				Log.CloseAndFlush();
				Console.WriteLine("Last Error was: " + e.Message);
				Console.WriteLine("Error specific data was: " + e.Data.ToString());
				Console.WriteLine("Full Error: " + e.ToString());
			    Console.WriteLine("Press the R key to exit once you have copied all error information ");

			    ConsoleKeyInfo key = new ConsoleKeyInfo('A',ConsoleKey.A,false,false,false);
			    while ( key.Key != ConsoleKey.R ) { key = Console.ReadKey(); }		    
		    }
	    }  // End Main Method
    }  // End Class
}
