using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.ViewModels;
using ProtocolEditor.Views.Dialogs;

namespace ProtocolEditor.Views.Pages.RelayByGroup;

/// <summary>
/// Логика взаимодействия с RelayByGroup.axaml
/// </summary>

public partial class RelayByGroup : UserControl
{
    private RelayByGroupsViewModel viewModel => (RelayByGroupsViewModel)DataContext!;
    
    public RelayByGroup()
    {
        InitializeComponent();
    }
    
    private void AddGroup_Click(object? sender, RoutedEventArgs e)
    {
        int newGroupId = viewModel.Groups.Any()
            ? viewModel.Groups.Max(g => g.GroupId) + 1
            : 1;
        
        viewModel.Groups.Add(new GroupViewModel {GroupId = newGroupId});
    }

    private async void AddCommand_Click(object? sender, RoutedEventArgs e)
    {
        if (!viewModel.Groups.Any()) 
        {
            AddGroup_Click(sender, e);
        }
    
        var dialog = new GroupSelectionDialog(viewModel.Groups);
        var result = await dialog.ShowDialog<GroupViewModel?>(new Window());
    
        if (result != null)
        {
            viewModel.AddCommandToGroup(result.GroupId);
        }
    }
    
    private void RemoveCommand_Click(object? sender, RoutedEventArgs e)
    {
        if (viewModel.SelectedCommand != null)
        {
            viewModel.RemoveCommand(viewModel.SelectedCommand);
        }
    }

    private void ExportCombinedRelayToExcel_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }

    private async void MoveCommandFromGroup_Click(object? sender, RoutedEventArgs e)
    {
        if (viewModel.SelectedCommand == null) return;
            
        var currentGroup = (GroupViewModel)((Button)sender).Tag;
            
        var dialog = new GroupSelectionDialog(viewModel.Groups);
        var result = await dialog.ShowDialog<GroupViewModel>(new Window());
            
        if (result != null && result != currentGroup)
        {
            viewModel.MoveCommandToGroup(viewModel.SelectedCommand, result.GroupId);
        }
    }

    private void DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is CommandViewModel command)
        {
            viewModel.SelectedCommand = command;
        }
    }
}