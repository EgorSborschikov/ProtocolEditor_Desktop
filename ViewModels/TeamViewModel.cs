using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ProtocolEditor.ViewModels;

public class TeamViewModel : INotifyPropertyChanged
{
    public int TeamId { get; }
    public string TeamName { get; }
    
    // Очки по соревнованиям [ID соревнования: очки]
    public Dictionary<int, int> CompetitionPoints { get; } = new();
    
    // Места по соревнованиям [ID соревнования: место]
    public Dictionary<int, int> CompetitionPlaces { get; } = new();
    
    // Суммарные очки (вычисляемое свойство)
    public int TotalPoints => CompetitionPoints.Values.Sum();
    
    // Текущее место в общем зачете
    private int _place;
    public int Place
    {
        get => _place;
        set
        {
            if (_place != value)
            {
                _place = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public TeamViewModel(int teamId, string teamName)
    {
        TeamId = teamId;
        TeamName = teamName;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}