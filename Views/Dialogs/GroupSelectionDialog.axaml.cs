using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using ProtocolEditor.ViewModels;

namespace ProtocolEditor.Views.Dialogs;

public partial class GroupSelectionDialog : Window
{
    public GroupViewModel? SelectedGroup { get; private set; }
    private GroupViewModel viewModel => (GroupViewModel)DataContext!;
    
    public GroupSelectionDialog()
    {
        InitializeComponent();
        DataContext = this;
    }

    public GroupSelectionDialog(ObservableCollection<GroupViewModel> groups) : this()
    {
        GroupsComboBox.ItemsSource = groups.ToList();

        if (groups.Any())
        {
            GroupsComboBox.SelectedIndex = 0;
        }
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        SelectedGroup = GroupsComboBox.SelectedItem as GroupViewModel;

        if (SelectedGroup != null)
        {
            MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Пожалуйста, выберите группу!").ShowAsync();
        }
        
        Close(SelectedGroup);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}