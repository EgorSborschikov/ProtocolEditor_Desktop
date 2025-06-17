using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ProtocolEditor.Models.Sqlite;
using ProtocolEditor.Services;

namespace ProtocolEditor.ViewModels;

/// <summary>
/// Интерфейс, содержащий логику взаимодействия с локальной базой данных
/// для хранения данных таблиц соответствия очков
/// </summary>

public class ScoreTableViewModel : INotifyPropertyChanged
{
    private readonly ScoreTableService _dbService = new();
        
    public ObservableCollection<ScoreEntry> OtherSportsScores { get; } = new();
    public ObservableCollection<ScoreEntry> CombinedRelayScores { get; } = new();
    public ObservableCollection<ScoreEntry> GroupRelayScores { get; } = new();

    public ScoreTableViewModel()
    {
        LoadData();
    }

    private void LoadData()
    {
        LoadCollection(OtherSportsScores, ScoreTableType.OtherSports);
        LoadCollection(CombinedRelayScores, ScoreTableType.CombinedRelay);
        LoadCollection(GroupRelayScores, ScoreTableType.GroupRelay);
    }

    private void LoadCollection(ObservableCollection<ScoreEntry> collection, ScoreTableType tableType)
    {
        collection.Clear();
        var entries = _dbService.LoadEntries(tableType);

        foreach (var entry in entries)
        {
            collection.Add(entry);
        }

        if (!collection.Any())
        {
            //
        }
    }

    public void AddEntry(ScoreTableType tableType)
    {
        var collection = GetCollection(tableType);
        if (collection == null) return;
        
        int newPlace = collection.Count + 1;
        int newPoints = collection.Count > 0
            ? collection.Last().Points - 10
            : 100;
        
        collection.Add(new ScoreEntry
        {
            Place = newPlace,
            Points = newPoints,
            TableType = tableType
        });
        
        SaveCollection(collection);
    }

    public void RemoveEntry(ScoreTableType tableType)
    {
        var collection = GetCollection(tableType);
        if (collection == null || collection.Count == 0) return;

        collection.RemoveAt(collection.Count - 1);
        SaveCollection(collection);   
    }

    public void SaveAllData()
    {
        SaveCollection(OtherSportsScores);
        SaveCollection(CombinedRelayScores);
        SaveCollection(GroupRelayScores);
    }

    public void SaveCollection(ObservableCollection<ScoreEntry> collection)
    {
        if (collection.Any())
        {
            _dbService.SaveEntries(collection);
        }
    }

    private ObservableCollection<ScoreEntry> GetCollection(ScoreTableType tableType)
    {
        return (tableType switch
        {
            ScoreTableType.OtherSports => OtherSportsScores,
            ScoreTableType.CombinedRelay => CombinedRelayScores,
            ScoreTableType.GroupRelay => GroupRelayScores,
            _ => null
        })!;
    }
    
    private void AddDefaultEntries(ObservableCollection<ScoreEntry> collection, ScoreTableType tableType)
    {
        int count = tableType switch
        {
            ScoreTableType.OtherSports => 10,
            ScoreTableType.CombinedRelay => 8,
            ScoreTableType.GroupRelay => 6,
            _ => 5
        };

        int basePoints = tableType switch
        {
            ScoreTableType.OtherSports => 100,
            ScoreTableType.CombinedRelay => 90,
            ScoreTableType.GroupRelay => 120,
            _ => 100
        };

        int step = tableType switch
        {
            ScoreTableType.OtherSports => 10,
            ScoreTableType.CombinedRelay => 10,
            ScoreTableType.GroupRelay => 20,
            _ => 10
        };

        for (int i = 1; i <= count; i++)
        {
            collection.Add(new ScoreEntry
            {
                Place = i,
                Points = basePoints - (i * step),
                TableType = tableType
            });
        }
            
        SaveCollection(collection);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnProperryChandged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}