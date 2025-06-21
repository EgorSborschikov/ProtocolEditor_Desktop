using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtocolEditor.Models;

namespace ProtocolEditor.ViewModels;

/// <summary>
/// Интерфейс, содержащий логику управления опциями для работы с таблицей комбинированной эстафеты
/// </summary>

public class CombineRelayViewModel : INotifyPropertyChanged
{
    private readonly ProtocolEditorDbContext _context;
    
    public ObservableCollection<CombineRelayEntry> Entries { get; } = new();
    public List<Command> AvailableCommands { get; private set; } = new();

    public CombineRelayViewModel(ProtocolEditorDbContext context)
    {
        _context = context;
        LoadData();
    }

    public void LoadData()
    {
        AvailableCommands = _context.Commands.ToList();
        
        var existingEntries = _context.CombineRelays
            .Include(cr => cr.IDCommandNavigation)
            .ToList();
        
        Entries.Clear();

        foreach (var command in AvailableCommands)
        {
            var existingEntry = existingEntries.FirstOrDefault(e => e.IDCommand == command.IDCommand);
            
            Entries.Add(new CombineRelayEntry
            {
                IDCombineRelay = existingEntry?.IDCombineRelay ?? 0,
                IDCommand = command.IDCommand,
                TeamName = command.CommandName,
                Time = existingEntry?.Time,
                Place = existingEntry?.Place
            });
        }
    }

    public void SaveChanges()
    {
        var sortedEntries = Entries
            .OrderBy(e => e.Place ?? int.MaxValue)
            .ToList();
        
        Entries.Clear();
        foreach (var entry in sortedEntries)
        {
            Entries.Add(entry);
        }
        
        foreach (var entry in Entries)
        {
            if (entry.IDCombineRelay == 0)
            {
                _context.CombineRelays.Add(new CombineRelay
                {
                    IDCommand = entry.IDCommand,
                    Time = entry.Time?.ToUniversalTime(),
                    Place = entry.Place
                });
            }
            else
            {
                var existing = _context.CombineRelays.Find(entry.IDCombineRelay);
                if (existing != null)
                {
                    existing.Time = entry.Time?.ToUniversalTime();
                    existing.Place = entry.Place;
                }
            }
        }
        
        _context.SaveChanges();
        LoadData();
    }

    public void SortByPlace()
    {
        var sorted = Entries
            .OrderBy(e => e.Place ?? int.MaxValue)
            .ToList();
        
        Entries.Clear();
        foreach (var entry in sorted)
        {
            Entries.Add(entry);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class CombineRelayEntry : INotifyPropertyChanged
{
    public int IDCombineRelay {get; set;}
    public int IDCommand {get; set;}

    private string _teamName;

    public string TeamName
    {
        get => _teamName;
        set
        {
            _teamName = value;
            OnPropertyChanged(nameof(TeamName));
        }
    }

    private DateTime? _time;

    public DateTime? Time
    {
        get => _time;
        set
        {
            _time = value;
            OnPropertyChanged(nameof(Time));
        }
    }

    private int? _place;

    public int? Place
    {
        get => _place;
        set
        {
            _place = value;
            OnPropertyChanged(nameof(Place));
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}