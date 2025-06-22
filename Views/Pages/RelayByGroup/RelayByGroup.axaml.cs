using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.Models;
using ProtocolEditor.ViewModels;
using ProtocolEditor.Views.Dialogs;

namespace ProtocolEditor.Views.Pages.RelayByGroup;

/// <summary>
/// Логика взаимодействия с RelayByGroup.axaml
/// </summary>

public partial class RelayByGroup : UserControl
{
    private RelayByGroupsViewModel ViewModel => (RelayByGroupsViewModel)DataContext!;
    
    public RelayByGroup()
    {
        InitializeComponent();
        DataContext = new RelayByGroupsViewModel();
        Console.WriteLine("DataContext установлен");
    }
    
    private void AddRelayGroupTeam_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.AddNewEntry();
    }

    private void RemoveRelayGroupTeam_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.RemoveSelectedEntry();
    }

    private async void ChangeTeam_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedEntry == null) return;
        
        var dialog = new TeamSelectionDialog(ViewModel.AllCommands);
        
        var dialogWindow = (Window)VisualRoot;
        var result = await dialog.ShowDialog<Command?>(dialogWindow);
        
        if (result != null)
        {
            ViewModel.SelectedEntry.IDCommand = result.IDCommand;
            ViewModel.SelectedEntry.TeamName = result.CommandName;
        }
    }

    private void SortByPlace_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.SortByPlace();
    }

    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.SaveChanges();
    }
}