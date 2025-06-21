using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.Models;

namespace ProtocolEditor.Views.Dialogs;

public partial class TeamSelectionDialog : Window
{
    private readonly List<Command>  _commands;
    public TeamSelectionDialog(List<Command> commands)
    {
        InitializeComponent();
        _commands = commands;
        CommandsList.ItemsSource= commands;

        CommandsList.DoubleTapped += CommandsList_DoubleTapped;
        SearchBox.TextChanged += SearchBox_TextChanged;
    }
    
    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text?.ToLower() ?? "";
        CommandsList.ItemsSource = _commands
            .Where(c => c.CommandName.ToLower().Contains(searchText))
            .ToList();
    }
    
    private void CommandsList_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        OK_Click(sender, e);
    }

    private void OK_Click(object? sender, RoutedEventArgs e)
    {
        if (CommandsList.SelectedItem is Command selectedCommand)
        {
            Close(selectedCommand);
        }
        else
        {
            Close(null);
        }
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}