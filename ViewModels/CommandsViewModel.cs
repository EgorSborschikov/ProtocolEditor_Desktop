using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls.Notifications;
using ProtocolEditor.Entities;
using ProtocolEditor.Models;

namespace ProtocolEditor.ViewModels;

public class CommandsViewModel : INotifyPropertyChanged
{
    private readonly ProtocolEditorDbContext _context;
    private ObservableCollection<Command> _commands;
    private Command _selectedCommand;
    private string? _errorMessage;

    public ObservableCollection<Command> Commands
    {
        get => _commands;
        set
        {
            _commands = value;
            OnPropertyChanged(nameof(Commands));
        }
    }

    public Command? SelectedCommand
    {
        get => _selectedCommand;
        set
        {
            _selectedCommand = value;
            OnPropertyChanged(nameof(SelectedCommand));
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public CommandsViewModel()
    {
        LoadCommands();
    }

    public void LoadCommands()
    {
        try
        {
            using var context = new ProtocolEditorDbContext();
            Commands = new ObservableCollection<Command>(context.Commands.ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки команд: {ex.Message}");
            Commands = new ObservableCollection<Command>();
        }
    }

    public void SaveChanges()
    {
        try
        {
            if (Commands.Any(c => string.IsNullOrWhiteSpace(c.CommandName)))
            {
                ErrorMessage = "Название команды не может быть пустым!";
                return;
            }

            ErrorMessage = null;
            
            using var context = new ProtocolEditorDbContext();
            
            // Удаление команд, удаленных в UI
            var dbIds = context.Commands.Select(c => c.IDCommand).ToList();
            var uiIds = Commands.Select(c => c.IDCommand).ToList();
            var toRemove = dbIds.Except(uiIds);
                
            foreach (var id in toRemove)
            {
                var entity = context.Commands.Find(id);
                if (entity != null)
                    context.Commands.Remove(entity);
            }

            // Обновление существующих команд
            foreach (var command in Commands.Where(c => c.IDCommand != 0))
            {
                var existing = context.Commands.Find(command.IDCommand);
                if (existing != null)
                    existing.CommandName = command.CommandName;
            }

            // Добавление новых команд
            var newCommands = Commands.Where(c => c.IDCommand == 0).ToList();
            context.Commands.AddRange(newCommands);

            context.SaveChanges();
            LoadCommands(); // Перезагружаем для получения ID
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка сохранения: {ex.Message}";
        }
    }

    public void AddNewCommand()
    {
        Commands.Add(new Command{ CommandName = "Новая команда"});
        SelectedCommand = Commands.Last();
    }

    public void RemoveSelectedCommand()
    {
        if (SelectedCommand != null)
        {
            Commands.Remove(SelectedCommand);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}