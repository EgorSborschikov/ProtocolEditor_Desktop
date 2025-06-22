using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtocolEditor.Models;

namespace ProtocolEditor.ViewModels;

/// <summary>
/// Интерфейс, содержащий логику управления опциями с таблицей эстафет по группам
/// </summary>

public class RelayByGroupsViewModel : INotifyPropertyChanged
{
    private readonly ProtocolEditorDbContext _context = new();
    private ObservableCollection<RelayGroupEntry> _entries = new();
    private List<Command> _allCommands = new();
    private RelayGroupEntry? _selectedEntry;

    public ObservableCollection<RelayGroupEntry> Entries
    {
        get => _entries;
        set
        {
            _entries = value;
            OnPropertyChanged(nameof(Entries));
        }
    }
    
    public List<Command> AllCommands => _allCommands;

    public RelayGroupEntry? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            _selectedEntry = value;
            OnPropertyChanged(nameof(SelectedEntry));
        }
    }

    public RelayByGroupsViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        try
        {
            _allCommands = _context.Commands.ToList();
            Console.WriteLine($"Загружено команд: {_allCommands.Count}");
            
            Entries.Clear();
            var relayData = _context.CommandsForRelays
                .Include(cfr => cfr.IDCommandNavigation)
                .ToList();
            
            Console.WriteLine($"Загружено записей эстафеты: {relayData.Count}");

            foreach (var item in relayData)
            {
                Console.WriteLine($"ID: {item.IDCommandForRelay}, Команда: {item.IDCommandNavigation?.CommandName}");
                
                Entries.Add(new RelayGroupEntry
                {
                    IDCommandForRelay = item.IDCommandForRelay,
                    IDCommand = item.IDCommand,
                    TeamName = item.IDCommandNavigation?.CommandName ?? "Команда не найдена",
                    Time = item.Time,
                    Place = item.Place,
                    Points = item.Points
                });
            }
            Console.WriteLine($"Добавлено в коллекцию: {Entries.Count} элементов");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    public void SaveChanges()
    {
        try
        {
            var currentIds = Entries.Select(e => e.IDCommandForRelay)
                .ToList();
            
            var toRemove = _context.CommandsForRelays
                .Where(cfr => !currentIds.Contains(cfr.IDCommandForRelay))
                .ToList();
            
            _context.CommandsForRelays.RemoveRange(toRemove);
            
            foreach (var entry in Entries)
            {
                if (entry.IDCommandForRelay == 0) // Новая запись
                {
                    _context.CommandsForRelays.Add(new CommandsForRelay
                    {
                        IDCommand = entry.IDCommand,
                        Time = entry.Time,
                        Place = entry.Place,
                        Points = entry.Points
                    });
                }
                else // Существующая запись
                {
                    var existing = _context.CommandsForRelays.Find(entry.IDCommandForRelay);
                    if (existing != null)
                    {
                        existing.IDCommand = entry.IDCommand;
                        existing.Time = entry.Time;
                        existing.Place = entry.Place;
                        existing.Points = entry.Points;
                    }
                }
            }
            
            _context.SaveChanges();
            LoadData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        
            // Добавляем вывод внутреннего исключения
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
            
                // Для DbUpdateException выводим дополнительные детали
                if (ex.InnerException is DbUpdateException dbEx)
                {
                    foreach (var entry in dbEx.Entries)
                    {
                        Console.WriteLine($"Ошибка в сущности: {entry.Entity.GetType().Name}");
                    
                        if (entry.State == EntityState.Added)
                            Console.WriteLine("Операция: Добавление");
                        else if (entry.State == EntityState.Modified)
                            Console.WriteLine("Операция: Обновление");
                    
                        // Выводим значения свойств
                        foreach (var prop in entry.CurrentValues.Properties)
                        {
                            var value = entry.CurrentValues[prop];
                            Console.WriteLine($"{prop.Name}: {value ?? "null"}");
                        }
                    }
                }
            }
        }
    }
    
    public void AddNewEntry()
    {
        Entries.Add(new RelayGroupEntry());
    }

    public void RemoveSelectedEntry()
    {
        if (SelectedEntry != null)
        {
            Entries.Remove(SelectedEntry);
        }
    }

    public void UpdateTeam(int entryId, int commandId)
    {
        var entry = Entries.FirstOrDefault(e => e.IDCommandForRelay == entryId);
        if (entry == null) return;
        
        var command = _allCommands.FirstOrDefault(c => c.IDCommand == commandId);
        if (command != null)
        {
            entry.IDCommand = commandId;
            entry.TeamName = command.CommandName;
        }
    }
    
    public void SortByPlace()
    {
        var sorted = new ObservableCollection<RelayGroupEntry>(
            Entries.OrderBy(e => e.Place ?? int.MaxValue));
        
        Entries = sorted;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


/// <summary>
/// Вспомогательные классы (интерфейсы)
/// </summary>
public class RelayGroupEntry : INotifyPropertyChanged
{
    private string _teamName = "Выберите команды";
    private DateTime? _time;
    private int? _place;
    private int? _points;

    public int IDCommandForRelay {get; set;}
    public int IDCommand {get; set;}

    public string TeamName
    {
        get => _teamName;
        set
        {
            _teamName = value;
            OnPropertyChanged(nameof(TeamName));
        }
    }

    public DateTime? Time
    {
        get => _time;
        set
        {
            _time = value;
            OnPropertyChanged(nameof(Time));
        }
    }

    public int? Place
    {
        get => _place;
        set
        {
            _place = value;
            OnPropertyChanged(nameof(Place));
        }
    }
    
    public int? Points
    {
        get => _points;
        set
        {
            _points = value;
            OnPropertyChanged(nameof(Points));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}