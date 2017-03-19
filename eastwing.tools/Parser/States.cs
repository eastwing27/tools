namespace Eastwing.Tools.Parser
{
    public enum States : byte
    {
        New,
        Starting,
        Adding,
        BuildingNumber,
        BuildingWord,
        BuildingLine,
        BuildingLines,
        Ending,
        Done
    }
}
