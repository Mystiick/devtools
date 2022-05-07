using Microsoft.Extensions.Configuration;

class Program
{
    public static void Main(string[] args)
    {
        // Load config from appsettings
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
        Preset[] presets;

#if DEBUG
        builder.AddJsonFile("appsettings.dev.json", true);
#endif

        var config = builder.Build();
        presets = config.GetSection("Presets").Get<Preset[]>();

        DoWork(presets);
    }

    private static void DoWork(Preset[] presets)
    {
        // Get user input
        Preset preset = ChoosePreset(presets);
        int required = GetRequiredShards();

        // Quick Maths
        Result r = preset.CalculateRequiredDrops(required);

        // Print results
        Console.WriteLine($"A total of {r.TotalRequired} summons are required, with {r.Remainder} remaining");
        foreach (Tuple<Drop, int> b in r.Breakdown.OrderBy(x => x.Item1.AmountDropped))
        {
            Console.WriteLine($"\t{b.Item1.AmountDropped}:\t{b.Item2}");
        }
    }

    private static Preset ChoosePreset(Preset[] presets)
    {
        Preset? output = null;

        while (output == null)
        {
            Console.WriteLine("Choose a summon type:");
            foreach (var p in presets)
            {
                Console.WriteLine($"\t{p.Name}");
            }

            string? input = Console.ReadLine();
            output = presets.FirstOrDefault(x => x.Name.ToLower() == input?.ToLower());
        }

        return output;
    }

    private static int GetRequiredShards()
    {
        int output = 0;

        while (output == 0)
        {
            Console.WriteLine("How many total shards do you need:");

            var input = Console.ReadLine();
            int.TryParse(input, out output);
        }

        return output;
    }
}
