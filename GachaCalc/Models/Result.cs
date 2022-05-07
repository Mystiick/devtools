public class Result
{
    public int TotalRequired { get; set; } = -1;
    public int Remainder { get; set; } = -1;
    public Tuple<Drop, int>[] Breakdown { get; set; } = new Tuple<Drop, int>[0];
}