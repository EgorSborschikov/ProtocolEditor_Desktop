using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.ViewModels;

namespace ProtocolEditor.Views.Pages.Commands;

public partial class CommandsPage : UserControl
{
    private CommandsViewModel ViewModel => (CommandsViewModel)DataContext!;
    
    public CommandsPage()
    {
        InitializeComponent();
        DataContext = new CommandsViewModel();
    }

    private void AddCommand_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.AddNewCommand();
    }

    private void RemoveCommand_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.RemoveSelectedCommand();
    }

    private void SaveCommands_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.SaveChanges();
    }
}