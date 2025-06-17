using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.Models.Sqlite;
using ProtocolEditor.ViewModels;

namespace ProtocolEditor.Views.ScoreTable;

/// <summary>
/// Логика взаимодействия с ScoreTable.axaml через методы класса ScoreTableViewModel
/// </summary>

public partial class ScoreTable : UserControl
{
    private ScoreTableViewModel viewModel => (ScoreTableViewModel)DataContext!;
    public ScoreTable()
    {
        InitializeComponent();
        DataContext = new ScoreTableViewModel();
    }

    private ScoreTableType GetCurrentTableType()
    {
        return ScoreTabs.SelectedIndex switch
        {
            0 => ScoreTableType.OtherSports,
            1 => ScoreTableType.CombinedRelay,
            2 => ScoreTableType.GroupRelay,
            _ => ScoreTableType.OtherSports
        };
    }

    private void AddScoreEntry_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.AddEntry(GetCurrentTableType());
    }

    private void RemoveScoreEntry_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.RemoveEntry(GetCurrentTableType());
    }

    private void SaveData_Click(object? sender, RoutedEventArgs e)
    {
        viewModel.SaveAllData();
    }
}