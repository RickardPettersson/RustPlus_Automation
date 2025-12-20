using RustPlusApi;
using RustPlusApi.Fcm;
using RustPlusApi.Fcm.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Windows.Foundation;
using static RustPlus_Automation.CredentialsReaderUtilities;
using Point = System.Drawing.Point;

namespace RustPlus_Automation
{
    public class Tools
    {
        private static bool smartSwitchState = true;
        public static async Task ConsoleLogPlayerPosition(RustPlus rustPlus, ulong steamId)
        {
            // Connect to the Rust game server
            await rustPlus.ConnectAsync();

            // Get some info about the server
            var info = await rustPlus.GetInfoAsync();

            // Take out the map size from the information
            int mapSize = (int)info.Data.MapSize;

            // Get the map data from the Rust+ API
            var mapRequest = await rustPlus.GetMapAsync();

            // Hold the last player position to be enable to check if it changed between the different checks
            Point lastPlayerPositionOnMapImage = new Point(0, 0);

            Console.WriteLine("Press Ctrl + C to stop the application, now we start logging player position in the game.");

            // Start timer every second
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync())
            {
                // Check if we are connected to the game server or connect to it
                if (!rustPlus.IsConnected())
                {
                    await rustPlus.ConnectAsync();
                }

                // To get patrol helicopter or ch47/chinook or cargoship positions
                var mapMarkesr = await rustPlus.GetMapMarkersAsync();

                // Get player info
                var playerMarker = mapMarkesr.Data.PlayerMarkers.Values.First(X => X.SteamId == steamId);

                // Convert the player marker position to a image pixel Point
                var playerPositionOnMapImage = MapHelper.WorldToImagePx(mapSize, (float)playerMarker.X, (float)playerMarker.Y, (int)mapRequest.Data.Width, (int)mapRequest.Data.Height);

                // Check if the player marker position been changed from last check
                if (lastPlayerPositionOnMapImage.X != playerPositionOnMapImage.X || lastPlayerPositionOnMapImage.Y != playerPositionOnMapImage.Y)
                {
                    // Update the latest player positon to be enabled to check if changed next time it check the position
                    lastPlayerPositionOnMapImage = playerPositionOnMapImage;

                    Console.WriteLine($"Position changed at {DateTime.Now} to {playerMarker.X} - {playerMarker.Y}");
                }
            }
        }


        public static async Task ConsoleLogPlayerPositionAndTryInRadiusFromBase(RustPlus rustPlus, ulong steamId, float baseLocationX, float baseLocationY, float radius)
        {
            // Connect to the Rust game server
            await rustPlus.ConnectAsync();

            // Get some info about the server
            var info = await rustPlus.GetInfoAsync();

            // Take out the map size from the information
            int mapSize = (int)info.Data.MapSize;

            // Get the map data from the Rust+ API
            var mapRequest = await rustPlus.GetMapAsync();

            // Get home position on map image
            var homePositionOnMapImage = MapHelper.WorldToImagePx(mapSize, baseLocationX, baseLocationY, (int)mapRequest.Data.Width, (int)mapRequest.Data.Height);

            // Hold the last player position to be enable to check if it changed between the different checks
            Point lastPlayerPositionOnMapImage = new Point(0, 0);

            Console.WriteLine("Press Ctrl + C to stop the application, now we start logging player position in the game and if it is in radius of your base location.");

            // Start timer every second
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync())
            {
                // Check if we are connected to the game server or connect to it
                if (!rustPlus.IsConnected())
                {
                    await rustPlus.ConnectAsync();
                }

                // To get patrol helicopter or ch47/chinook or cargoship positions
                var mapMarkesr = await rustPlus.GetMapMarkersAsync();

                // Get player info
                var playerMarker = mapMarkesr.Data.PlayerMarkers.Values.First(X => X.SteamId == steamId);

                // Convert the player marker position to a image pixel Point
                var playerPositionOnMapImage = MapHelper.WorldToImagePx(mapSize, (float)playerMarker.X, (float)playerMarker.Y, (int)mapRequest.Data.Width, (int)mapRequest.Data.Height);

                // Check if the player marker position been changed from last check
                if (lastPlayerPositionOnMapImage.X != playerPositionOnMapImage.X || lastPlayerPositionOnMapImage.Y != playerPositionOnMapImage.Y)
                {
                    // Update the latest player positon to be enabled to check if changed next time it check the position
                    lastPlayerPositionOnMapImage = playerPositionOnMapImage;

                    Console.WriteLine($"Position changed at {DateTime.Now} to {playerMarker.X} - {playerMarker.Y}");

                    // Check if the player marker is in radius of the base position
                    if (MapHelper.IsPointInCircle(playerPositionOnMapImage.X, playerPositionOnMapImage.Y, homePositionOnMapImage.X, homePositionOnMapImage.Y, radius))
                    {
                        Console.WriteLine("In the radius of your base");
                    }
                    else
                    {
                        Console.WriteLine("Not in the radius of base");
                    }
                }
            }
        }

        public static async Task ConsoleLogEvents(string rustPlusConfigPath)
        {
            Credentials credentials = new Credentials();
            try
            {
                var jsConfig = rustPlusConfigPath.ReadJavaScriptConfig();
                credentials = jsConfig.ConvertToCredentials();

                Console.WriteLine($"Loaded credentials - AndroidId: {credentials.Gcm.AndroidId}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Config file not found at: {rustPlusConfigPath}");
                Console.WriteLine("Please run 'npx @liamcottle/rustplus.js fcm-register' first and update the path above.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
                return;
            }

            var listener = new RustPlusFcm(credentials);

            listener.Connecting += (_, _) =>
            {
                Console.WriteLine($"[CONNECTING]: {DateTime.Now}");
            };

            listener.Connected += (_, _) =>
            {
                Console.WriteLine($"[CONNECTED]: {DateTime.Now}");
            };

            listener.SocketClosed += (_, _) =>
            {
                Console.WriteLine($"[SOCKET CLOSED]: {DateTime.Now}");
            };

            listener.ErrorOccurred += (_, error) =>
            {
                Console.WriteLine($"[ERROR]: {error}");
            };

            listener.Disconnecting += (_, _) =>
            {
                Console.WriteLine($"[DISCONNECTING]: {DateTime.Now}");
            };

            listener.Disconnected += (_, _) =>
            {
                Console.WriteLine($"[DISCONNECTED]: {DateTime.Now}");
            };

            /* Specials events */

            listener.OnParing += (_, pairing) =>
            {
                Console.WriteLine($"[PAIRING]:\n{JsonSerializer.Serialize(pairing, JsonUtilities.JsonOptions)}");
            };

            listener.OnServerPairing += (_, pairing) =>
            {
                Console.WriteLine($"[SERVER PAIRING]:\n{JsonSerializer.Serialize(pairing, JsonUtilities.JsonOptions)}");
            };

            listener.OnEntityParing += (_, pairing) =>
            {
                Console.WriteLine($"[ENTITY PAIRING]:\n{JsonSerializer.Serialize(pairing, JsonUtilities.JsonOptions)}");
            };

            listener.OnSmartSwitchParing += (_, pairing) =>
            {
                Console.WriteLine($"[SMART SWITCH PAIRING]:\n{JsonSerializer.Serialize(pairing, JsonUtilities.JsonOptions)}");
            };

            listener.OnStorageMonitorParing += (_, pairing) =>
            {
                Console.WriteLine($"[STORAGE MONITOR PAIRING]:\n{JsonSerializer.Serialize(pairing, JsonUtilities.JsonOptions)}");
            };

            listener.OnSmartAlarmParing += (_, pairing) =>
            {
                Console.WriteLine($"[SMART ALARM PAIRING]:\n{JsonSerializer.Serialize(pairing, JsonUtilities.JsonOptions)}");
            };

            listener.OnAlarmTriggered += (_, alarm) =>
            {
                Console.WriteLine($"[ALARM TRIGGERED]:\n{JsonSerializer.Serialize(alarm, JsonUtilities.JsonOptions)}");
            };

            await listener.ConnectAsync();

            Console.WriteLine(">>> Press enter to exit the application <<<");

            Console.ReadLine();
            listener.Disconnect();
        }

        public static async Task AutomationTurnOffSmartSwitch(RustPlus rustPlus, ulong steamId, float baseLocationX, float baseLocationY, float radius, uint smartSwitchId, bool smartSwitchStateToSet)
        {

            rustPlus.Connected += RustPlus_Connected;

            void RustPlus_Connected(object? sender, EventArgs e)
            {
                var smartSwitchInfo = rustPlus.GetSmartSwitchInfoAsync(smartSwitchId).Result;

                if (smartSwitchInfo.IsSuccess)
                {
                    // Data.IsActive can be get
                    Console.WriteLine($"[SmartSwitchInfo RECIEVED]:\n{JsonSerializer.Serialize(smartSwitchInfo.Data, JsonUtilities.JsonOptions)}");
                }
                else
                {
                    Console.WriteLine("ERROR try to get Smart Switch Info");
                }
            }



            rustPlus.MessageReceived += RustPlus_MessageReceived1;

            void RustPlus_MessageReceived1(object? sender, RustPlusContracts.AppMessage e)
            {
                //Console.WriteLine($"[MESSAGE RECEIVED]:\n{JsonSerializer.Serialize(e, JsonUtilities.JsonOptions)}");

                if (e.Broadcast != null && e.Broadcast.EntityChanged != null)
                {
                    var entityChanged = e.Broadcast.EntityChanged;
                    if (entityChanged.EntityId == smartSwitchId)
                    {
                        Console.WriteLine($"[SMART SWITCH STATE CHANGED]:\n{JsonSerializer.Serialize(entityChanged.Payload, JsonUtilities.JsonOptions)}");
                        if (entityChanged.Payload.HasValue)
                        {
                            smartSwitchState = entityChanged.Payload.Value;
                            Console.WriteLine($"[SMART SWITCH STATE]: {smartSwitchState}");
                        }
                    }
                }
                /*
                   {
                     "Broadcast": {
                       "EntityChanged": {
                         "EntityId": 16144467,
                         "HasEntityId": true,
                         "Payload": {
                           "Value": false,
                           "HasValue": false,
                           "Items": [],
                           "Capacity": 0,
                           "HasCapacity": false,
                           "HasProtection": false,
                           "HasHasProtection": false,
                           "ProtectionExpiry": 0,
                           "HasProtectionExpiry": false
                         }
                       }
                     }
                   }
                */
            }


            // Connect to the Rust game server
            await rustPlus.ConnectAsync();

            // Get some info about the server
            var info = await rustPlus.GetInfoAsync();

            // Take out the map size from the information
            int mapSize = (int)info.Data.MapSize;

            // Get the map data from the Rust+ API
            var mapRequest = await rustPlus.GetMapAsync();

            // Get home position on map image
            var homePositionOnMapImage = MapHelper.WorldToImagePx(mapSize, baseLocationX, baseLocationY, (int)mapRequest.Data.Width, (int)mapRequest.Data.Height);

            // Hold the last player position to be enable to check if it changed between the different checks
            Point lastPlayerPositionOnMapImage = new Point(0, 0);

            Console.WriteLine("Press Ctrl + C to stop the application, now we start logging player position in the game and if it is in radius of your base location.");

            // Start timer every second
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync())
            {
                // Check if we are connected to the game server or connect to it
                if (!rustPlus.IsConnected())
                {
                    await rustPlus.ConnectAsync();
                }

                // To get patrol helicopter or ch47/chinook or cargoship positions
                var mapMarkesr = await rustPlus.GetMapMarkersAsync();

                // Get player info
                var playerMarker = mapMarkesr.Data.PlayerMarkers.Values.First(X => X.SteamId == steamId);

                // Convert the player marker position to a image pixel Point
                var playerPositionOnMapImage = MapHelper.WorldToImagePx(mapSize, (float)playerMarker.X, (float)playerMarker.Y, (int)mapRequest.Data.Width, (int)mapRequest.Data.Height);

                // Check if the player marker position been changed from last check
                if (lastPlayerPositionOnMapImage.X != playerPositionOnMapImage.X || lastPlayerPositionOnMapImage.Y != playerPositionOnMapImage.Y)
                {
                    // Update the latest player positon to be enabled to check if changed next time it check the position
                    lastPlayerPositionOnMapImage = playerPositionOnMapImage;

                    //Console.WriteLine($"Position changed at {DateTime.Now} to {playerMarker.X} - {playerMarker.Y}");

                    // Check if the player marker is in radius of the base position
                    if (MapHelper.IsPointInCircle(playerPositionOnMapImage.X, playerPositionOnMapImage.Y, homePositionOnMapImage.X, homePositionOnMapImage.Y, radius))
                    {
                        //Console.WriteLine("In the radius of your base");
                    }
                    else
                    {
                        if (smartSwitchState)
                        {
                            if (smartSwitchStateToSet)
                            {
                                // Do nothing the smart switch is already on
                            }
                            else
                            {
                                Console.WriteLine("Not in the range of your base, turn off the smart switch");

                                var trigswitch = await rustPlus.SetSmartSwitchValueAsync(smartSwitchId, false);
                            }
                        }
                        else
                        {
                            if (smartSwitchStateToSet)
                            {
                                Console.WriteLine("Not in the range of your base, turn on the smart switch");

                                var trigswitch = await rustPlus.SetSmartSwitchValueAsync(smartSwitchId, true);
                            }
                            else
                            {
                                // Do nothing the smart switch is already off
                            }
                        }
                    }
                }
            }
        }
    }
}