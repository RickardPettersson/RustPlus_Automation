using Microsoft.Extensions.Configuration;
using RustPlus_Automation;
using RustPlusApi;
using System.Net.Http.Headers;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var appSettings = config
    .GetSection("AppSettings")
    .Get<AppSettings>();

#if DEBUG
if (args.Length == 0)
{
    Console.WriteLine("Running in DEBUG mode and no arguments, we set first argument to LogPlayerPosition");
    args = new string[] { "LogPlayerPosition" };
}
#endif

// Check that we got any arguments
if (args.Length > 0)
{
    if (args[0].ToLower() == "LogPlayerPosition".ToLower())
    {
        if (string.IsNullOrEmpty(appSettings.ServerIP) || appSettings.RustPlusPort == 0 || appSettings.SteamId == 0 || appSettings.PlayerToken == 0)
        {
            Console.WriteLine("Please fill in all the required fields in the appsettings.json file before running the application.");
        }
        else
        {
            Console.WriteLine("Starting player position logging...");

            // Setup client
            var rustPlus = new RustPlus(appSettings.ServerIP, appSettings.RustPlusPort, appSettings.SteamId, appSettings.PlayerToken, false);

            // Start the player position logging
            await Tools.ConsoleLogPlayerPosition(rustPlus, appSettings.SteamId);
        }
    }
    else if (args[0].ToLower() == "ConsoleLogPlayerPositionAndTryInRadiusFromBase".ToLower())
    {
        if (string.IsNullOrEmpty(appSettings.ServerIP) || appSettings.RustPlusPort == 0 || appSettings.SteamId == 0 || appSettings.PlayerToken == 0 || appSettings.BaseLocationX == 0 || appSettings.BaseLocationY == 0 || appSettings.Radius == 0)
        {
            Console.WriteLine("Please fill in all the required fields in the appsettings.json file before running the application.");
        }
        else
        {
            Console.WriteLine("Starting player position check in radius of base position");

            // Setup client
            var rustPlus = new RustPlus(appSettings.ServerIP, appSettings.RustPlusPort, appSettings.SteamId, appSettings.PlayerToken, false);

            // Start the player position in radius of base position logging
            await Tools.ConsoleLogPlayerPositionAndTryInRadiusFromBase(rustPlus, appSettings.SteamId, appSettings.BaseLocationX, appSettings.BaseLocationY, appSettings.Radius);
        }
    }
    else if (args[0].ToLower() == "ConsoleLogEvents".ToLower())
    {
        Console.WriteLine("Starting to log RustPlus events...");

        await Tools.ConsoleLogEvents(appSettings.RustPlusConfigPath);
    }
    else if (args[0].ToLower() == "AutomationTurnOffSmartSwitch".ToLower())
    {
        if (string.IsNullOrEmpty(appSettings.ServerIP) || appSettings.RustPlusPort == 0 || appSettings.SteamId == 0 || appSettings.PlayerToken == 0 || appSettings.BaseLocationX == 0 || appSettings.BaseLocationY == 0 || appSettings.Radius == 0 || appSettings.SmartSwitchId == 0)
        {
            Console.WriteLine("Please fill in all the required fields in the appsettings.json file before running the application.");
        }
        else
        {
            Console.WriteLine("Starting autoamtion to automatic turn of smart switch when player position is out of the radius of base position");

            // Setup client
            var rustPlus = new RustPlus(appSettings.ServerIP, appSettings.RustPlusPort, appSettings.SteamId, appSettings.PlayerToken, false);

            // Start the autoamtion to automatic turn of smart switch when player position is out of the radius of base position
            await Tools.AutomationTurnOffSmartSwitch(rustPlus, appSettings.SteamId, appSettings.BaseLocationX, appSettings.BaseLocationY, appSettings.Radius, appSettings.SmartSwitchId);
        }
    }
    else
    {
        Console.WriteLine("Unknown argument passed to the application.");
    }
}
else
{
    Console.WriteLine("You didnt pass any parameters to the application, please add arguments ");
}
