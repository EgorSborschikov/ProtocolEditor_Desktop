using System;
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
    private readonly ProtocolEditorDbContext _context;
    
    public ObservableCollection<GroupViewModel> Groups { get; } = new ();
    public CommandViewModel? SelectedCommand { get; set; }

    public RelayByGroupsViewModel(ProtocolEditorDbContext context)
    {
        _context = context;
        LoadData();
    }

    public void LoadData()
    {
        Groups.Clear();

        var dbGroups = _context.GroupsForRelays
            .Include(g => g.IDCommandForRelayNavigation)
            .ThenInclude(c => c.IDCommandNavigation)
            .ToList()
            .GroupBy(g => g.IDGroupForRelay);

        foreach (var group in dbGroups)
        {
            var groupViewModel = new GroupViewModel
            {
                GroupId = group.Key,
            };

            foreach (var item in group)
            {
                var command = item.IDCommandForRelayNavigation;
                groupViewModel.Commands.Add(new CommandViewModel
                {
                    Id = command.IDCommandForRelay,
                    CommandId = command.IDCommand,
                    TeamName = command.IDCommandNavigation?.CommandName ?? "Неизвестно",
                    
                    Time = command.Time,
                    Points = command.Points,
                    Place = command.Place,
                });
            }
            
            groupViewModel.SortCommands();
            Groups.Add(groupViewModel);
        }
    }

    public void AddCommandToGroup(int groupId)
    {
        if (Groups == null) return;
        
        var group = Groups.FirstOrDefault(g => g.GroupId == groupId);
        if (group == null)
        {
            group = new GroupViewModel
            {
                GroupId = groupId,
                GroupName = $"Группа{groupId}"
            };
            Groups.Add(group);
        }
        
        group.Commands.Add(new CommandViewModel
        {
            Time = DateTime.UtcNow,
        });
    }
    
    public void RemoveCommand(CommandViewModel command)
    {
        foreach (var group in Groups)
        {
            if (group.Commands.Contains(command))
            {
                group.Commands.Remove(command);
                return;
            }
        }
    }
    
    public void MoveCommandToGroup(CommandViewModel command, int targetGroupId)
    {
        // Удаляем команду из текущей группы
        foreach (var group in Groups)
        {
            if (group.Commands.Contains(command))
            {
                group.Commands.Remove(command);
                break;
            }
        }
            
        // Добавляем в новую группу
        var targetGroup = Groups.FirstOrDefault(g => g.GroupId == targetGroupId);
        if (targetGroup == null)
        {
            targetGroup = new GroupViewModel 
            { 
                GroupId = targetGroupId,
                GroupName = $"Группа {targetGroupId}"
            };
            Groups.Add(targetGroup);
        }
            
        targetGroup.Commands.Add(command);
    }
    
    public void SaveChanges()
    {
        // Удаление всех существующих данных
        _context.GroupsForRelays.RemoveRange(_context.GroupsForRelays);
        _context.CommandsForRelays.RemoveRange(_context.CommandsForRelays);
            
        // Сохранение новых данных
        foreach (var group in Groups)
        {
            foreach (var command in group.Commands)
            {
                var time = command.Time.Kind == DateTimeKind.Utc 
                    ? command.Time 
                    : command.Time.ToUniversalTime();
                
                var dbCommand = new CommandsForRelay
                {
                    IDCommand = command.CommandId,
                    Time = time,
                    Place = command.Place,
                    Points = command.Points
                };
                    
                _context.CommandsForRelays.Add(dbCommand);
                _context.SaveChanges(); // Для получения ID
                    
                _context.GroupsForRelays.Add(new GroupsForRelay
                {
                    IDGroupForRelay = group.GroupId,
                    IDCommandForRelay = dbCommand.IDCommandForRelay
                });
            }
        }
            
        _context.SaveChanges();
        LoadData(); // Перезагружаем данные
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
public class GroupViewModel : INotifyPropertyChanged
{
    private string _groupName;
    public string GroupName
    {
        get => _groupName;
        set
        {
            _groupName = value;
            OnPropertyChanged(nameof(GroupName));
        }
    }
    
    public int GroupId { get; set; }
    public ObservableCollection<CommandViewModel> Commands { get; } = new();
        
    public void SortCommands()
    {
        var sorted = Commands.OrderBy(c => c.Place).ToList();
        Commands.Clear();
        foreach (var command in sorted)
        {
            Commands.Add(command);
        }
    }

    public override string ToString()
    {
        return GroupName ?? $"Группа {GroupId}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class CommandViewModel : INotifyPropertyChanged
{
    public int Id { get; set; }
    public int CommandId { get; set; }
        
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
        
    private int _points;
    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            OnPropertyChanged(nameof(Points));
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