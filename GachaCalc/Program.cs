using Microsoft.Extensions.Configuration;

public class Shard { public int Amount { get; set; } = -1; public int Summons { get; set; } = -1; }
public class Preset { public string Name { get; set; } = ""; public Shard[] Probabilities { get; set; } = new Shard[0]; }

class Program
{
    private static Shard[] Probability = {
        new Shard() { Amount = 1, Summons = 6 },
        new Shard() { Amount = 2, Summons = 16 },
        new Shard() { Amount = 5, Summons = 65 },
    };

    private static Preset[] Presets = new[] {
         new Preset() { Name = "ssr", Probabilities = Probability } ,
         new Preset() { Name = "ssr+", Probabilities = Probability } ,
    };

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

        Preset p = ChoosePreset(presets);
        int req = GetRequiredShards();

        for (int i = 1; true; i++)
        {
            var totals = p.Probabilities.Select(x => new { Probability = x, Summoned = i / x.Summons * x.Amount });

            if (totals.Sum(x => x.Summoned) >= req)
            {
                Console.WriteLine($"A total of {i} summons are required, with {totals.Sum(x => x.Summoned) - req} remaining");

                foreach (var t in totals)
                {
                    Console.WriteLine($"\t{t.Probability.Amount}:\t{t.Summoned}");
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
        int temp = 0;

        while (temp == 0)
        {
            Console.WriteLine("How many total shards do you need:");

            var input = Console.ReadLine();
            int.TryParse(input, out temp);
        }

        return temp;
    }
}
