public class Preset
{
    public string Name { get; set; } = "";
    public Drop[] GuaranteedDrops { get; set; } = new Drop[0];

    public Result CalculateRequiredDrops(int required)
    {
        var output = new Result();

        for (int i = 1; true; i++)
        {
            var total = this.GuaranteedDrops.Sum(x => x.SimulateDrops(i));
            output.Breakdown = this.GuaranteedDrops.Select(x => new Tuple<Drop, int>(x, x.SimulateDrops(i))).ToArray();

            if (output.Breakdown.Sum(x => x.Item2) >= required)
            {
                output.TotalRequired = i;
                output.Remainder = total - required;

                break;
            }
        }

        return output;
    }
}
