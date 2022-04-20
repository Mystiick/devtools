public class Shard
{
    public int Amount { get; set; } = -1;
    public int SummonsRequired { get; set; } = -1;

    public int SimulateDrops(int numberOfDrops)
    {
        return numberOfDrops / SummonsRequired * Amount;
    }
}
