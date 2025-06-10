using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProtocolEditor.ViewModel;

namespace ProtocolEditor.EditorControls.CompetitionSummary;

public partial class CompetitionSummary : UserControl
{
    private EditWindowViewModel viewModel;
    private int competitionCounter = 0;
    public CompetitionSummary()
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
        competitionCounter++;

        int insertIndex = 2;

        //int competitionNumber = (CompetitionDataGrid.Columns.Count - 4) / 2 + 1;
        CompetitionDataGrid.Columns.Insert(insertIndex, new DataGridTextColumn
        {
            Header = $"М",
            Binding = new Binding($"Competitions[{competitionCounter - 1}].Place") 
        });
        CompetitionDataGrid.Columns.Insert(insertIndex + 1, new DataGridTextColumn
        {
            Header = $"О",
            Binding = new Binding($"Competitions[{competitionCounter - 1}].Points")
        });

        var competitionHeader = new DataGridTextColumn
        {
            Header = $"Соревнование {competitionCounter}"
        };
        CompetitionDataGrid.Columns.Insert(insertIndex, competitionHeader);
    }

    private void SortByPlace_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ExportToExcel_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }


    private void RemoveTeam_Click(object? sender, RoutedEventArgs e)
    {
        if (CompetitionDataGrid.SelectedItem is Team selectedTeam)
        {
            viewModel.Teams.Remove(selectedTeam);
        }
    }

    private void RemoveCompetition_Click(object? sender, RoutedEventArgs e)
    {
        if (competitionCounter > 0)
        {
            int lastCompetitionIndex = CompetitionDataGrid.Columns.Count - 3;
            if (lastCompetitionIndex >= 2)
            {
                CompetitionDataGrid.Columns.RemoveAt(lastCompetitionIndex + 1);
                CompetitionDataGrid.Columns.RemoveAt(lastCompetitionIndex);
                competitionCounter--;
            }
        }
    }
}