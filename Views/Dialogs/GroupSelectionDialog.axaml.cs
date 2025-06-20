using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.ViewModels;

namespace ProtocolEditor.Views.Dialogs;

public partial class GroupSelectionDialog : Window
{
    public GroupViewModel? SelectedGroup { get; private set; }
    
    public GroupSelectionDialog()
    {
        InitializeComponent();
    }

    public GroupSelectionDialog(ObservableCollection<GroupViewModel> groups) : this()
    {
        GroupsComboBox.ItemsSource = groups.ToList();
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        SelectedGroup = GroupsComboBox.SelectedItem as GroupViewModel;
        Close(SelectedGroup != null);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}