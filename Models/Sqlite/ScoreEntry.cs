namespace ProtocolEditor.Models.Sqlite;

public class ScoreEntry
{
    public int Id { get; set; }
    public int Place { get; set; }
    public int Points { get; set; }
    public ScoreTableType TableType { get; set; }
}

public enum ScoreTableType
{
    OtherSports,
    CombinedRelay,
    GroupRelay
}