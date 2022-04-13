using System.Diagnostics;
using Microsoft.Extensions.Configuration;

public class Program
{
    private static AppConfig _appconfig = new AppConfig();

    public static async Task Main(string[] args)
    {
        // Load config from appsettings
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);

#if DEBUG
        builder.AddJsonFile("appsettings.dev.json", true);
#endif

        var config = builder.Build();
        _appconfig = config.GetSection(nameof(AppConfig)).Get<AppConfig>();

        Log($"Copying files from \"{_appconfig.SaveLocation}\" to \"{_appconfig.BackupLocation}\"");
        CopyDirectory(_appconfig.SaveLocation, _appconfig.BackupLocation);

        // If we are using Git, commit and push the latest files
        if (_appconfig.UseGit)
        {
            if (await InitalizeRepo())
            {
                await CommitAndPush();
            }
            else
            {
                Console.WriteLine("Nothing to commit.");
            }
        }

        if (_appconfig.PauseOnFinish)
        {
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }
    }

    #region | File Management |
    private static void CopyDirectory(string source, string destination)
    {
        if (!Directory.Exists(destination))
        {
            LogVerbose($"\tCREATE:\t\"{destination}{Path.DirectorySeparatorChar}\"");
            Directory.CreateDirectory(destination);
        }

        // Copy all files from SaveLocation to BackupLocation
        foreach (string fileName in Directory.GetFiles(source))
        {
            FileInfo fi = new FileInfo(fileName);
            var temp = Path.Combine(destination, fi.Name);

            LogVerbose($"\tCOPY:\t\"{fileName}\"\tto \"{temp}\"");

            File.Copy(fileName, temp, true);
        }

        // Remove all files from destination that are no longer in source
        CleanupFiles(source, destination);
        CleanupDirectories(source, destination);

        // Recursively copy directories
        foreach (string directory in Directory.GetDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(destination, Path.GetRelativePath(source, directory)));
        }
    }

    private static void CleanupFiles(string source, string destination)
    {
        var sourceNames = Directory.GetFiles(source).Select(x => Path.GetFileName(x));
        var destinationNames = Directory.GetFiles(destination).Select(x => Path.GetFileName(x));

        // Remove all files in destination that are not in source
        foreach (string fileName in destinationNames.Except(sourceNames))
        {
            var path = Path.Combine(destination, fileName);
            LogVerbose($"\tDELETE:\t\"{path}\"");
            File.Delete(path);
        }
    }

    private static void CleanupDirectories(string source, string destination)
    {
        // Calling GetFileName here isn't a typo, GetFileName returns the destination folder name, where GetDirectoryName returns the parent directory
        var sourceNames = Directory.GetDirectories(source).Select(x => Path.GetFileName(x));
        var destinationNames = Directory.GetDirectories(destination).Select(x => Path.GetFileName(x));

        // Remove all folders from destination that are no longer in source (except /.git/)
        foreach (string? directory in destinationNames.Except(sourceNames).Except(new[] { ".git" }))
        {
            if (!string.IsNullOrEmpty(directory))
            {
                var path = Path.Combine(destination, directory);
                LogVerbose($"\tDELETE:\t\"{path}{Path.DirectorySeparatorChar}*\"");
                Directory.Delete(path, true);
            }
        }
    }
    #endregion

    #region | Git Management |
    private static async Task<string> RunCommand(string command)
    {
        using Process proc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                WorkingDirectory = _appconfig.BackupLocation, // Important to run the command in the backup location
                FileName = "git",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        proc.StartInfo.Arguments = command;
        proc.Start();

        while (!proc.StandardOutput.EndOfStream)
        {
            sb.AppendLine(await proc.StandardOutput.ReadLineAsync());
        }
        while (!proc.StandardError.EndOfStream)
        {
            sb.AppendLine(await proc.StandardError.ReadLineAsync());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Runs `git status` to determine if the current folder is already a repo. If not, it runs `git init` and returns true. 
    /// Returns true/false based on if there are changes to commit
    /// <returns>true if there are changes to commit, false if there are none</returns>
    private static async Task<bool> InitalizeRepo()
    {
        // Check if the destination folder is already a git repo
        string msg = await RunCommand("status");

        // Not a git repo? Make one and add all the files
        if (msg.Contains("not a git repository"))
        {
            Log("Destination folder is not a git repository, initializing an empty one now.");
            await RunCommand("init");

            // If we had to init, we know it has changes to commit
            return true;
        }
        else
        {
            // true if there are changes to commit, false if there are none
            return !msg.Contains("nothing to commit");
        }
    }

    /// <summary>Commits and pushes (if applicable) files in the destination folder</summary>
    private static async Task CommitAndPush()
    {
        // Commit file to repo
        Log("Commiting files");

        await RunCommand("add .");
        await RunCommand($"commit -am \"Commiting file on {DateTime.Now}\"");

        // If this is a remote repo, push as well
        string remote = await RunCommand("remote -v");

        // Expecting output like:  "origin  https://github.com/Mystiick/tools.git (push)"
        if (remote.Contains("(push)"))
        {
            Log("Pushing to remote.");
            await RunCommand("push");
        }
    }
    #endregion

    #region | Logging |
    private static void Log(string message)
    {
        Console.WriteLine(message);
    }
    private static void LogVerbose(string message)
    {
        if (_appconfig.Verbose)
        {
            Console.WriteLine(message);
        }
    }
    #endregion

}