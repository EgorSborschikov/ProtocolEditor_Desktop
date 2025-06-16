using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProtocolEditor.ViewModel;

namespace ProtocolEditor.EditorControls.CompetitionSummary;

public partial class CompetitionSummary : UserControl
{
    private CompetitionSummaryViewModel viewModel;
    private int competitionCounter = 0;
    public CompetitionSummary()
    {
        InitializeComponent();
        
        viewModel = new CompetitionSummaryViewModel();
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
        
        CompetitionDataGrid.Columns.Insert(insertIndex, new DataGridTextColumn
        {
            Header = $"Соревнование {competitionCounter}\nМ",
            Binding = new Binding($"Competitions[{competitionCounter - 1}].Place")
        });

        // Добавляем столбец для "Очки" с заголовком, включающим название соревнования
        CompetitionDataGrid.Columns.Insert(insertIndex + 1, new DataGridTextColumn
        {
            Header = $"Соревнование {competitionCounter}\nО",
            Binding = new Binding($"Competitions[{competitionCounter - 1}].Points")
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
            int competitionStartIndex = 2;
            
            if (CompetitionDataGrid.Columns.Count > competitionStartIndex + 1)
            {
                CompetitionDataGrid.Columns.RemoveAt(competitionStartIndex + 1);
                CompetitionDataGrid.Columns.RemoveAt(competitionStartIndex);

                competitionCounter--;
            }
        }
    }
}