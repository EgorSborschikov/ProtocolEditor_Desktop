using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProtocolEditor.Models;
using ProtocolEditor.ViewModels;
using ProtocolEditor.Views.Dialogs;

namespace ProtocolEditor.Views.Pages.CombineRelay;

/// <summary>
/// Логика взаимодействия с CombineRelay.axaml
/// </summary>

public partial class CombineRelay : UserControl
{
    private CombineRelayViewModel ViewModel => (CombineRelayViewModel)DataContext!;
    
    public CombineRelay()
    {
        InitializeComponent();
    }

    private void AddCombinedRelayTeam_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.AddNewEntry();
    }

    private void RemoveCombinedRelayTeam_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.RemoveSelectedEntry();
    }

    private async void ChangeTeam_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedEntry == null) return;
        
        // Создаем диалог выбора команды
        var dialog = new TeamSelectionDialog(ViewModel.AllCommands);
        
        var dialogWindow = (Window)VisualRoot;
        var result = await dialog.ShowDialog<Command?>(dialogWindow);
        
        if (result != null)
        {
            ViewModel.SelectedEntry.IDCommand = result.IDCommand;
            ViewModel.SelectedEntry.TeamName = result.CommandName;
        }
    }

    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.SaveChanges();
    }
}