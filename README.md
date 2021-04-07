# ArmoryBot
.NET Core Discord Bot for World of Warcraft Armory Lookups.

## Current Status: Working in Shadowlands 9.x

### Setup:
1. Sign up for [Blizzard API Access](https://develop.battle.net/), and register an application. You will need a **client_id** and **client_secret**.
2. Register a new [Discord Application](https://discord.com/developers/applications). Subsequently create a "Bot" for your application, and take note of the **token** for your created bot.
3. On the Oauth2 page of your discord application, use the supplied URL to join your bot to your server(s). The URL should look like `https://discord.com/api/oauth2/authorize?client_id=YOURCLIENTID&permissions=281600&scope=bot`   where YOURCLIENTID is the id listed on the "General Information" page.
4. Modify **blizzard.json** and **discord.json** with the above parameters that are bolded in parts 1 & 2. See [WoW Localizations](https://develop.battle.net/documentation/world-of-warcraft/guides/localization) for localization info, or leave as default (en_US). Non-English localizations are not fully supported.
5. Make sure you have [.NET Core 3.1 or newer runtime](https://dotnet.microsoft.com/download) installed on the system that will be running your bot (.NET Core is [cross-platform](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) so you can run this on Windows/macOS/Linux).
6. Launch the application. The bot should come online and respond to requests. [See Discord Usage](https://github.com/imerzan/ArmoryBot/tree/master#discord-usage)

**macOS/Linux:** When running from bash terminal, use the following commands:
```bash
chmod 755 ArmoryBot    ## Sets proper permissions, only need to do this once
./ArmoryBot    ## Starts the executable
```
![Console output](https://user-images.githubusercontent.com/42287509/113591776-f3866a80-95f9-11eb-891f-3fa6912d5f5a.jpg)

**NOTE:** When reporting issues with the bot, please utilize the [Issues Page](https://github.com/imerzan/ArmoryBot/issues), and include any error info from your Bot Console Output.

### Docker Setup
1. *(Optional)* For easier setup, download the pre-compiled release in [Releases](https://github.com/imerzan/ArmoryBot/releases), reference README.txt in the docker folder. Otherwise, continue to step 2.
2. Clone this repo to your system. Open a 'Developer Command Prompt for VS' terminal. Change directory to the project folder with the Dockerfile and .csproj file. Run the following commands:
```csharp
dotnet publish -c Release
```
3. In a separate window, navigate to the `bin\Release\netcoreapp3.1\publish` directory. Execute ArmoryBot.exe (or ArmoryBot.dll) to have a blank `discord.json` and `blizzard.json` config file created. Modify these files with your configuration, and save.
4. Back in your Command Prompt, execute the remaining commands:
```console
docker build -t armorybot-image -f Dockerfile .
docker create --name armorybot armorybot-image --restart always --net host
docker start armorybot
```
5. ArmoryBot should be good to go within Docker!

**NOTE:** I do not officially support Docker for this project. My testing indicated that it seems to work very well in Docker for Windows, but I encountered some intermittent issues when running this on Debian (Linux arm32). If you run into issues, running this outside of Docker is *VERY* stable, and I would recommend that instead ([See Setup](https://github.com/imerzan/ArmoryBot/tree/master#setup)). I will be unable to provide Docker assistance.

## Discord Usage:
```docker
!armory character-realm pve/pvp    ## Armory Lookup Command
!armory token    ## WoW Token Lookup Command
!armory help    ## Help Command
```
Example: ```!armory Frostchiji-Sargeras pvp``` **NOTE:** Spaces in realm name should have a ' - ' character.

![Example](https://user-images.githubusercontent.com/42287509/113754051-6bc15e80-96d4-11eb-8aac-ac5ab74c4620.jpg)

![Example2](https://user-images.githubusercontent.com/42287509/113765312-b72e3980-96e1-11eb-9400-85f8c62b863b.jpg)
