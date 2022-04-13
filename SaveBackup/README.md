# SaveBackup
Console app that copies from SaveLocation to BackupLocation.

## Usage
Update appconfig.json Save and Backup locations to your target and backup locations.  
Run with `dotnet run` or `dotnet publish -c release` to create a runnable executable.

I like to run `dotnet publish -p:PublishSingleFile=true -r win-x64 -c Release --self-contained false` to publish a single file instead of all dependent .dll files as individually

### Git integration
If UseGit in `appconfig.json` is set to true, the app will automatically commit to git. If the BackupLocation folder is not a git repository, it will automatically `git init` the folder to create a repo before comitting to it. Remote repositories will also be automatically pushed to.
