using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtocolEditor.Entities;
using ProtocolEditor.Models;
using ProtocolEditor.Views.Pages.CombineRelay;
using CombineRelay = ProtocolEditor.Models.CombineRelay;

namespace ProtocolEditor.ViewModels;

/// <summary>
/// Интерфейс, содержащий логику управления опциями для работы с таблицей комбинированной эстафеты
/// </summary>

public class CombineRelayViewModel : INotifyPropertyChanged
{
    private readonly ProtocolEditorDbContext _context = new();

    private ObservableCollection<CombineRelayEntry> _entries = new();
    public ObservableCollection<CombineRelayEntry> Entries
    {
        get => _entries;
        set
        {
            _entries = value;
            OnPropertyChanged(nameof(Entries));
        }
    }
    
    private List<Command> _avaliableCommands = new();

    public List<Command> AvaliableCommands
    {
        get => _avaliableCommands;
        set
        {
            _avaliableCommands = value;
            OnPropertyChanged(nameof(AvaliableCommands));
        }
    }

    public CombineRelayViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        AvaliableCommands = _context.Commands.ToList<Command>();
        OnPropertyChanged(nameof(AvaliableCommands));
        
        Entries.Clear();
        var relayData = _context.CombineRelays.ToList();

        foreach (var item in relayData)
        {
            var command = AvaliableCommands.FirstOrDefault(c => c.IDCommand == item.IDCommand);
                
            Entries.Add(new CombineRelayEntry
            {
                IDCombineRelay = item.IDCombineRelay,
                IDCommand = item.IDCommand,
                TeamName = command?.CommandName ?? "Неизвестно",
                Time = item.Time,
                Place = item.Place
            });
        }
    }

    public void SaveChanges()
    {
        // Собираем все ID для проверки удаленных записей
        var currentIds = Entries.Select(e => e.IDCombineRelay).ToList();

        // Удаление отсутствующих записей
        var toRemove = _context.CombineRelays
            .Where(cr => !currentIds.Contains(cr.IDCombineRelay))
            .ToList();
            
        _context.CombineRelays.RemoveRange(toRemove);

        // Обновление и добавление
        foreach (var entry in Entries)
        {
            if (entry.IDCombineRelay == 0) // Новая запись
            {
                _context.CombineRelays.Add(new CombineRelay
                {
                    IDCommand = entry.IDCommand,
                    Time = entry.Time,
                    Place = entry.Place,
                });
            }
            else // Существующая запись
            {
                var existing = _context.CombineRelays.Find(entry.IDCombineRelay);
                if (existing != null)
                {
                    existing.IDCommand = entry.IDCommand;
                    existing.Time = entry.Time;
                    existing.Place = entry.Place;
                }
            }
        }

        _context.SaveChanges();
        LoadData(); // Перезагружаем данные
    }
    
    public void AddNewEntry()
    {
        Entries.Add(new CombineRelayEntry
        {
            Time = DateTime.UtcNow // Значение по умолчанию
        });
    }

    public void RemoveEntry(CombineRelayEntry entry)
    {
        if (entry != null)
        {
            Entries.Remove(entry);
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
    public int IDCombineRelay { get; set; }
    public int IDCommand { get; set; }
        
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
        
    private DateTime _time;
    public DateTime Time
    {
        get => _time;
        set
        {
            _time = value;
            OnPropertyChanged(nameof(Time));
        }
    }
        
    private int _place;
    public int Place
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