# RustPlus Automation  (Unofficial)
This is an **unofficial** project built by a gamer of Rust, not from the game developer Facepunch Studios. 

It is a tool to make an automation in Rust so if a player going outside the base range a smart switch turns off. 

The project built as a C# .Net 10 Console application, i have not test in other operating systems then windows.

## Configuration
Provide settings in `appsettings.json` under the `AppSettings` section. Required keys (used per command):

- `RustPlusConfigPath` — Path to RustPlus config (rustplus.config.json)  
- `ServerIP` — Rust server IP or hostname  
- `RustPlusPort` — Rust+ port  
- `SteamId` — Steam ID used by the RustPlus client  
- `PlayerToken` — Player token for authentication  
- `BaseLocationX` — You base position X coordinate
- `BaseLocationY` — You base position Y coordinate
- `Radius` — Radius for proximity checks, this is number of pixels on the rust game server map as image
- `SmartSwitchId` — Device id of smart switch to toggle (automation)
- `SmartSwitchStateToSet` — What state you like the smart switch to be on when you leave your base? true = on, false = off


## How to get all configuration values

### Prepare with installing Node.JS

- To get the rustplus.config.json file you need to instal node.js on a computer, i run it on windows
- You need to install node.js from: https://nodejs.org/en/download/ i working in Windows but should be the same for linux and mac i guess

### Get the rustplus.config.json

- After you have installed node.js you should create a empty folder example C:\temp
- Then run a command line prompt and go to path of the folder you created where you like your rusptlus.config.json to be created
- Run the command: npm install @liamcottle/rustplus.js
- When its done you then run this command: npx @liamcottle/rustplus.js fcm-register
- A chrome window opening and you need to authorize with steam and after its says hello to you its done and you have the config file in the folder to use

The rustplus.config.json you easiest put in the same folder as the RustPlus_Automation.exe and appsettings.json file

### Get server information and user token by pairing Rust server

- Next step is to get the server connection info you need to get from the rust server
- Run the command: npx @liamcottle/rustplus.js fcm-listen
- When the command is running you need to start the game and connect to the server you play on
- When you in the game you now need to press ESC and then press the Pairing button to get the server information sent to your command prompt, i also recommend to have the mobile app för Rust+ open and accept the pairing if you like to use the Rust+ mobile app.
- When you pressed the pairing button you should see the server information in the command prompt window where you run the fcm-listen command
- In the appData with key body you have server information you need like server ip, port, player/steam id and playerToken
- The server info below you should write down and put in the appsettings.json file for the console application

Now you have all you need to run this C# example below

## Functions in the console application

The following functions you put the name as argument to the console application exe file (RustPlus_Automation.exe).

To run the automation you need to do the following functions one at the time in the order i written them below.

Example open a command line prompt and go to the folder where you have the RustPlus_Automation.exe file and the run the command: RustPlus_Automation.exe <function>

### LogPlayerPosition

Log in Console the player position in game when it changing
 
This function you use to get the position of your base, i recommend to find the center of the base and then copy the player position from the console logs from this function and put it in the appsetitngs.json file för base position.

### ConsoleLogPlayerPositionAndTryInRadiusFromBase

Console log player position and show if the position in radius from your base position

This function helps you to test out the radius settings in appsettings.json start the application with radius set to 10 is my recommendation and try it out.

Its logging every player position and say if its in the radius of the base position and radius set in the appsettings.json file.

### ConsoleLogEvents

Console log events from the Rust game server

This function is only to get the Smart Switch Entity Id, so start the function go to a smart switch and use the wire tool and hold use button and then select Pair.

You going to get some different events but look for the last one named "SMART SWITCH PAIRING" and copy the value of property Data, for me a 8 characters long number.

Copy the number to the appsettings.json and the value of SmartSwitchId.

### AutomationTurnOffSmartSwitch

Automation to turn off a smart switch in rust game when player is out of range from the base.

If you followed and run all the functions above you now is ready to start the automation function and test go outside your base range and test it.

# Thanks to developer of RustPlus C# projects

Using the C# library named RustPlusAPI to communicate with the Rust Game Server and also test and inspect code of Rust+ Desktop APP, so thanks to them behind the code of this two repositories:

- RustPlusApi - https://github.com/HandyS11/RustPlusApi
- Rust+ Desktop App - https://github.com/Pronwan/rustplus-desktop/tree/master
