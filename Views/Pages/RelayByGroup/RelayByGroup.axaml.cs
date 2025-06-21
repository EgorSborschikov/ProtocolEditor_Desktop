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
    private RelayByGroupsViewModel viewModel => (RelayByGroupsViewModel)DataContext!;
    
    public RelayByGroup()
    {
        InitializeComponent();
        var context = new ProtocolEditorDbContext();
        DataContext = new RelayByGroupsViewModel(context);
    }
    
    private void AddGroup_Click(object? sender, RoutedEventArgs e)
    {
        if (viewModel == null) return;
            
        // Проверяем наличие групп безопасно
        int newGroupId = viewModel.Groups.Any() 
            ? viewModel.Groups.Max(g => g.GroupId) + 1 
            : 1;
            
        viewModel.Groups.Add(new GroupViewModel 
        { 
            GroupId = newGroupId,
            GroupName = $"Группа {newGroupId}"
        });
    }

    private async void AddCommand_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel == null) 
        {
            Console.WriteLine("ViewModel не инициализирован");
            return;
        }
            
        if (!viewModel.Groups.Any())
        {
            AddGroup_Click(sender, e);
        }
            
        var dialog = new GroupSelectionDialog(viewModel.Groups);

        var dialogWindow = (Window)VisualRoot;
        var result = await dialog.ShowDialog<GroupViewModel?>(dialogWindow);
            
        if (result is null)
        {
            Console.WriteLine("Выбор группы отменен");
            return;
        }
        
        viewModel.AddCommandToGroup(result.GroupId);
    }

    private void RemoveCommand_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel?.SelectedCommand != null)
        {
            viewModel.RemoveCommand(viewModel.SelectedCommand);
        }
    }

    private async void MoveCommandFromGroup_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel?.SelectedCommand == null) return;
            
        var currentGroup = (GroupViewModel)((Button)sender).Tag;
            
        var otherGroups = new ObservableCollection<GroupViewModel>(
            viewModel.Groups.Where(g => g.GroupId != currentGroup.GroupId)
        );
            
        if (!otherGroups.Any()) 
        {
            // Если нет других групп, создаем новую
            int newGroupId = viewModel.Groups.Max(g => g.GroupId) + 1;
            var newGroup = new GroupViewModel 
            { 
                GroupId = newGroupId,
                GroupName = $"Группа {newGroupId}"
            };
                
            viewModel.Groups.Add(newGroup);
            otherGroups.Add(newGroup);
        }
            
        var dialog = new GroupSelectionDialog(otherGroups);
        
        var dialogWindow = (Window)VisualRoot;
        var result = await dialog.ShowDialog<GroupViewModel?>(dialogWindow);
            
        if (result != null)
        {
            viewModel.MoveCommandToGroup(viewModel.SelectedCommand, result.GroupId);
        }
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (viewModel != null && e.AddedItems.Count > 0 && e.AddedItems[0] is CommandViewModel command)
        {
            viewModel.SelectedCommand = command;
        }
    }

    private void SaveChanges_Click(object sender, RoutedEventArgs e)
    {
        viewModel?.SaveChanges();
    }
        

    private void ExportCombinedRelayToExcel_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}