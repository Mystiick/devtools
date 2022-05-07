# GachaCalc
Simple application to calculate how many summons or drops are needed with a guaranteed drop mechanic. This does not take into account any drop chances, only calculates the maximum amount of drops required (and any gacha game is probably around there anyway).

 Example run:
```
Choose a summon type:
        ssr
        ssr+
ssr
How many total shards do you need:
42
A total of 85 summons are required, with 0 remaining
        1:      17
        2:      16
        3:      9
```

## Configuration
Update [appsettings.json](./appsettings.json) with the different types of summons, and how many summons are required per drop. An example of how to do this is already set up in the file for you. You may update appsettings.json, or use an appsettings.dev.json if running in a debug configuration.

## Compilation
Compile with:  `dotnet build -c release` or run it with `dotnet run`. Once compiled, follow the Configuration steps to build out your appsettings.json file that lives in the bin folder.

### Windows
Requires .NET Core 6.0 SDK and has only been tested on Windows 10.

### Linux
Follow the [Microsoft Guide](https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu) to install the dotnet SDK. This has only been tested on WSL 2 on Windows 10 but it should work on distros that can install the SDK.