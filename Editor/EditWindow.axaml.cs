using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using ProtocolEditor.ViewModel;

namespace ProtocolEditor.Editor;

public partial class EditWindow : Window
{
    private EditWindowViewModel viewModel;
    public EditWindow()
    {
        InitializeComponent();
        
        viewModel = new EditWindowViewModel();
        DataContext = viewModel;
        
        CompetitionDataGrid.ItemsSource = viewModel.Teams;
        
        viewModel.AddTeam();
    }
    
    private void AddTeam_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.AddTeam();
    }
    
    private void AddCompetition_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.AddCompetition();

        int competitionNumber = CompetitionDataGrid.Columns.Count / 2;
        CompetitionDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = $"Место {competitionNumber + 1}",
            Binding = new Binding($"Competitions[{competitionNumber}].Place") 
        });
        CompetitionDataGrid.Columns.Add( new DataGridTextColumn
        {
            Header = $"Очки {competitionNumber + 1}",
            Binding = new Binding($"Competitions[{competitionNumber}].Points")
        });
    }

    private void SortByPlace_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ExportToExcel_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    
}