# ArmoryBot

## Discontinued - I haven't played World of Warcraft in ages, and do not plan to continue further development on this.

.NET Core Discord Bot for World of Warcraft Armory Lookups.
### Supports new M+ Rating in 9.1!

## Current Status: Working in Shadowlands 9.x

### Setup:
1. Sign up for [Blizzard API Access](https://develop.battle.net/), and register an application. You will need a `client_id` and `client_secret`.
2. Register a new [Discord Application](https://discord.com/developers/applications). Subsequently create a "Bot" for your application, and take note of the `token` for your created bot.
3. On the Oauth2 page of your discord application, use the supplied URL to join your bot to your server(s). The URL should look like `https://discord.com/api/oauth2/authorize?client_id=YOURCLIENTID&permissions=281600&scope=bot`   where YOURCLIENTID is the id listed on the "General Information" page.
4. Modify `appsettings.json` with the above parameters that are highlighted in parts 1 & 2. See [WoW Localizations](https://develop.battle.net/documentation/world-of-warcraft/guides/localization) for localization info, or leave as default (en_US). Non-English localizations are not fully supported. If you lost your appsettings.json file, you can download a default one [here](https://github.com/imerzan/ArmoryBot/files/6446475/appsettings.zip).
5. Make sure you have [.NET 6.0](https://dotnet.microsoft.com/download) installed on the system that will be running your bot (.NET 6.0 is [cross-platform](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) so you can run this on Windows/macOS/Linux).
6. Launch the application. The bot should come online and respond to requests. [See Discord Usage](https://github.com/imerzan/ArmoryBot/tree/master#discord-usage), [See Advanced Setup](https://github.com/imerzan/ArmoryBot/tree/master#advanced-setup)

**macOS/Linux:** When running from bash terminal, use the following commands:
```bash
chmod 755 ArmoryBot    ## Sets proper permissions, only need to do this once
./ArmoryBot    ## Starts the executable
```
![Console output](https://user-images.githubusercontent.com/42287509/117552409-b5250680-b010-11eb-9294-b44706417c2a.jpg)

**NOTE:** When reporting issues with the bot, please utilize the [Issues Page](https://github.com/imerzan/ArmoryBot/issues), and include any error info from your Bot Console Output.

## Discord Usage:
```bash
!armory pve/pvp character-realm    ## Armory Lookup Command
!armory token    ## WoW Token Lookup Command
!armory help    ## Help Command
```
Example: ```!armory pvp Frostchiji-Sargeras``` **NOTE:** Spaces in realm name should have a ' - ' character.

![Example](https://user-images.githubusercontent.com/42287509/123978874-d60d3680-d985-11eb-92e0-51cc473f565a.jpg)

![Example2](https://user-images.githubusercontent.com/42287509/113765312-b72e3980-96e1-11eb-9400-85f8c62b863b.jpg)

## Advanced Setup:
### Windows Service:
1. To run this as a Windows Service (will run in the background and upon system reboot), move the .Exe file and appsettings.json file to a directory of your choosing, for example `C:\ArmoryBot\`
2. Open a command prompt (Cmd.exe) as **Administrator** (Right-Click Run-as-Admin).
3. Enter the following commands: 
```
sc create ArmoryBot binPath=C:\ArmoryBot\ArmoryBot.exe
sc config ArmoryBot start=auto
sc start ArmoryBot
```
4. Your service is now configured and will Startup automatically. You can view logs using Event Viewer.

### Linux Service (systemd):
1. To run this as a Linux Service, move the executable and appsetings.json files to a directory of your choosing. Make sure to `chmod 755` the executable as well.
2. Create a file called `armorybot.service` in the folder ``/etc/systemd/system/`` with a text editor of your choosing. Save the following contents:
```bash
[Unit]
Description=ArmoryBot Service

[Service]
Type=notify
WorkingDirectory=/Path    ## Folder Path that executable is contained in
ExecStart=/Path/ArmoryBot   ## Full path to ArmoryBot Executable
Environment=DOTNET_ROOT=/DotnetPath    ## You can look this up using: echo $DOTNET_ROOT

[Install]
WantedBy=multi-user.target
```
3. Enter the following terminal commands:
```
sudo systemctl daemon-reload
sudo systemctl enable armorybot
sudo systemctl start armorybot
```
4. Your service is now configured and will Startup automatically. You can view logs using `journalctl -u armorybot`.
