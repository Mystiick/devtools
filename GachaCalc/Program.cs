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

        Preset preset = ChoosePreset(presets);
        int required = GetRequiredShards();

        for (int i = 1; true; i++)
        {
            var total = preset.Probabilities.Sum(x => x.SimulateDrops(i));
            if (total >= required)
            {
                Console.WriteLine($"A total of {i} summons are required, with {total - required} remaining");

                foreach (var prob in preset.Probabilities)
                {
                    Console.WriteLine($"\t{prob.Amount}:\t{prob.SimulateDrops(i)}");
                }

                break;
            }
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
