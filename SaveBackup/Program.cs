using System.Diagnostics;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main(string[] args) 
    {
        // Load config from appsettings
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
        var appConfig = config.GetSection(nameof(AppConfig)).Get<AppConfig>();

        // Copy all files from SaveLocation to BackupLocation
        Console.WriteLine($"Copying files from {appConfig.SaveLocation} to {appConfig.BackupLocation}");
        foreach (string fileName in Directory.GetFiles(appConfig.SaveLocation))
        {
            FileInfo fi = new FileInfo(fileName);
            File.Copy(fileName, appConfig.BackupLocation + fi.Name, true);
        }

        // If we are using Git, commit and push the latest files
        if (appConfig.UseGit)
        {
            ProcessStartInfo psi = new ProcessStartInfo() 
            {
                WorkingDirectory = appConfig.BackupLocation, // Important to run the command in the backup location
                FileName = "git",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process proc = new Process() { StartInfo = psi };

            await InitalizeRepo(proc, psi);

            await CommitAndPush(proc, psi);
        }
    }

    private static async Task<string> RunCommand(string command, Process proc, ProcessStartInfo psi)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        psi.Arguments = command;
        proc.Start();

        while(!proc.StandardOutput.EndOfStream)
        {
            sb.AppendLine(await proc.StandardOutput.ReadLineAsync());
        }

        return sb.ToString();
    }

    private static async Task InitalizeRepo(Process proc, ProcessStartInfo psi)
    {
        // Check if the destination folder is already a git repo
        psi.Arguments = "status";
        proc.Start();

        if (!proc.StandardError.EndOfStream)
        {
            var err = await proc.StandardError.ReadToEndAsync();

            // Not a git repo? Make one and add all the files
            if (err.Contains("not a git repository"))
            {
                Console.WriteLine("Destination folder is not a git repository, initializing an empty one now.");
                await RunCommand("init", proc, psi);
                await RunCommand("add .", proc, psi);
            }
        }
    }

    private static async Task CommitAndPush(Process proc, ProcessStartInfo psi)
    {
        // Commit file to repo
        Console.WriteLine("Commiting files");
        await RunCommand($"commit -am \"Commiting file on {DateTime.Now}\"", proc, psi);        

        // If this is a remote repo, push as well
        string remote = await RunCommand("remote -v", proc, psi);

        // Expecting output like:  "origin  https://github.com/Mystiick/tools.git (push)"
        if (remote.Contains("(push)"))
        {
            Console.WriteLine("Pushing to remote.");
            await RunCommand("push", proc, psi);
        }
    }
}