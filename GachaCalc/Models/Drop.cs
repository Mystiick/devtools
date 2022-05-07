public class Drop
{
    public int AmountDropped { get; set; } = -1;
    public int SummonsRequired { get; set; } = -1;

    public int SimulateDrops(int numberOfDrops)
    {
        return numberOfDrops / SummonsRequired * AmountDropped;
    }
}
