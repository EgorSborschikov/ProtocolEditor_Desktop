using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProtocolEditor.ViewModels;

namespace ProtocolEditor.Views.Pages.CombineRelay;

/// <summary>
/// Логика взаимодействия с CombineRelay.axaml
/// </summary>

public partial class CombineRelay : UserControl
{
    private CombineRelayViewModel viewModel => (CombineRelayViewModel)DataContext!;
    
    public CombineRelay()
    {
        InitializeComponent();
        DataContext = new CombineRelayViewModel();
    }

    private void AddCombinedRelayTeam_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.AddNewEntry();
    }

    private void RemoveCombinedRelayTeam_Click(object? sender, RoutedEventArgs e)
    {
        if (CombinedRelayDataGrid.SelectedItem is CombineRelayEntry selected)
        {
            viewModel.RemoveEntry(selected);
        }
    }
    
    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.SaveChanges();
    }
    
    private void ExportCombinedRelayToExcel_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}