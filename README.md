# ArmoryBot
.NET Core Discord Bot for World of Warcraft Armory Lookups.

## Current Status: Working in Shadowlands 9.x

### Setup:
1. Sign up for [Blizzard API Access](https://develop.battle.net/), and register an application. You will need a **client_id** and **client_secret**.
2. Register a new [Discord Application](https://discord.com/developers/applications). Subsequently create a "Bot" for your application, and take note of the **token** for your created bot.
3. On the Oauth2 page of your discord application, use the supplied URL to join your bot to your server(s). The URL should look like `https://discord.com/api/oauth2/authorize?client_id=YOURCLIENTID&permissions=281600&scope=bot`   where YOURCLIENTID is the id listed on the "General Information" page.
4. Modify **blizzard.json** and **discord.json** with the above parameters that are bolded in parts 1 & 2. See [WoW Localizations](https://develop.battle.net/documentation/world-of-warcraft/guides/localization) for localization info, or leave as default (en_US). Non-English localizations are not fully supported.
5. Make sure you have [.NET 5.0](https://dotnet.microsoft.com/download) installed on the system that will be running your bot (.NET 5.0 is [cross-platform](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) so you can run this on Windows/macOS/Linux).
6. Launch the application. The bot should come online and respond to requests. [See Discord Usage](https://github.com/imerzan/ArmoryBot/tree/master#discord-usage)

**macOS/Linux:** When running from bash terminal, use the following commands:
```bash
chmod 755 ArmoryBot    ## Sets proper permissions, only need to do this once
./ArmoryBot    ## Starts the executable
```
![Console output](https://user-images.githubusercontent.com/42287509/113591776-f3866a80-95f9-11eb-891f-3fa6912d5f5a.jpg)

**NOTE:** When reporting issues with the bot, please utilize the [Issues Page](https://github.com/imerzan/ArmoryBot/issues), and include any error info from your Bot Console Output.

## Discord Usage:
```bash
!armory pve/pvp character-realm    ## Armory Lookup Command
!armory token    ## WoW Token Lookup Command
!armory help    ## Help Command
```
Example: ```!armory pvp Frostchiji-Sargeras``` **NOTE:** Spaces in realm name should have a ' - ' character.

![Example](https://user-images.githubusercontent.com/42287509/116110162-0f21e580-a67b-11eb-9b0f-4499062bf043.jpg)

![Example2](https://user-images.githubusercontent.com/42287509/113765312-b72e3980-96e1-11eb-9400-85f8c62b863b.jpg)
