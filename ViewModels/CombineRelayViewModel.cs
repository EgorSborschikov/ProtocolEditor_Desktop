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
    private readonly ProtocolEditorDbContext _context = new();
    private ObservableCollection<CombineRelayEntry> _entries = new();
    private List<Command>_allCommands = new();
    private CombineRelayEntry? _selectedEntry;

    public ObservableCollection<CombineRelayEntry> Entries
    {
        get => _entries;
        set
        {
            _entries = value;
            OnPropertyChanged(nameof(Entries));
        }
    }
    
    public List<Command> AllCommands => _allCommands;

    public CombineRelayEntry? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            _selectedEntry = value;
            OnPropertyChanged(nameof(SelectedEntry));
        }
    }

    public CombineRelayViewModel()
    {
        LoadData();
    }

    public void LoadData()
    {
        try
        {
            _allCommands = _context.Commands.ToList();
            
            Entries.Clear();
            var relayData = _context.CombineRelays
                .AsNoTracking()
                .Include(cr => cr.IDCommandNavigation)
                .ToList();
            
            Console.WriteLine($"Загружено {relayData.Count} Записей о комбинированных эстафетах");

            foreach (var item in relayData)
            {
                Console.WriteLine($"Record: ID={item.IDCombineRelay}, CommandID={item.IDCommand}, Time={item.Time}, Place={item.Place}");
            
                Entries.Add(new CombineRelayEntry
                {
                    IDCombineRelay = item.IDCombineRelay,
                    IDCommand = item.IDCommand,
                    TeamName = item.IDCommandNavigation?.CommandName ?? "Команда не найдена",
                    Time = item.Time,
                    Place = item.Place,
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            // Добавьте вывод полного стека исключений для отладки
            Console.WriteLine(ex.StackTrace);
        }
    }

    public void SaveChanges()
    {
        try
        {
            var currentIds = Entries.Select(e => e.IDCombineRelay).ToList();
            
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
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }
    
    public void AddNewEntry()
    {
        Entries.Add(new CombineRelayEntry
        {
            Time = DateTime.UtcNow
        });
    }

    public void RemoveSelectedEntry()
    {
        if (SelectedEntry != null)
        {
            Entries.Remove(SelectedEntry);
        }
    }
    
    // Обновление команды по ID
    public void UpdateTeam(int entryId, int commandId)
    {
        var entry = Entries.FirstOrDefault(e => e.IDCombineRelay == entryId);
        if (entry == null) return;
        
        var command = _allCommands.FirstOrDefault(c => c.IDCommand == commandId);
        if (command != null)
        {
            entry.IDCommand = commandId;
            entry.TeamName = command.CommandName;
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
    private string _teamName = "Выберите команду";
    private DateTime? _time;
    private int? _place;

    public int IDCombineRelay { get; set; }
    public int IDCommand { get; set; }
    
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
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}