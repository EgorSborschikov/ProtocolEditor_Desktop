using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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

        var headerContainer = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Spacing = 0
        };
        
        headerContainer.Children.Add(new TextBlock
        {
            Text = $"Соревнование {competitionCounter}\nM",
            TextAlignment = TextAlignment.Center,
            Width = 60,
            Padding = new Thickness(5)
        });
        
        headerContainer.Children.Add(new Rectangle
        {
            Fill = Brushes.Transparent,
            Width = 1,
            Margin = new Thickness(0, 5),
            VerticalAlignment = VerticalAlignment.Center,
        });
        
        headerContainer.Children.Add(new TextBlock
        {
            Text = $"Соревнование {competitionCounter}\nО",
            TextAlignment = TextAlignment.Center,
            Width = 60,
            Padding = new Thickness(5)
        });

        var column = new DataGridTemplateColumn
        {
            Header = headerContainer,
            CellTemplate = new FuncDataTemplate<object>((obj,scope) =>
            {
                var cellContainer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 0
                };

                var placeBook = new TextBlock
                {
                    Width = 60,
                    Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Center,
                    [!TextBlock.TextProperty] = new Binding($"Competition[{competitionCounter - 1}].Place")
                };
                
                var separator = new Rectangle 
                {
                    Fill = Brushes.LightGray,
                    Width = 1,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                
                var pointsBlock = new TextBlock
                {
                    Width = 60,
                    Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Center,
                    [!TextBlock.TextProperty] = new Binding($"Competitions[{competitionCounter - 1}].Points")
                };

                cellContainer.Children.Add(placeBook);
                cellContainer.Children.Add(separator);
                cellContainer.Children.Add(pointsBlock);
                
                return cellContainer;
            })
        };
        
        CompetitionDataGrid.Columns.Insert(insertIndex, column);
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
            
            int columnIndexToRemove = competitionStartIndex + (competitionCounter - 1);

            if (CompetitionDataGrid.Columns.Count > columnIndexToRemove)
            {
                // Удаляем одну объединенную колонку
                CompetitionDataGrid.Columns.RemoveAt(columnIndexToRemove);
                competitionCounter--;
            
                // Обновляем индексы в оставшихся колонках
                UpdateCompetitionColumnBindings();
            }
        }
    }

    private void UpdateCompetitionColumnBindings()
    {
        // Обновляем привязки во всех колонках соревнований
        for (int i = 0; i < competitionCounter; i++)
        {
            int columnIndex = 2 + i;
            if (columnIndex < CompetitionDataGrid.Columns.Count && 
                CompetitionDataGrid.Columns[columnIndex] is DataGridTemplateColumn column)
            {
                // Обновляем привязки в ячейке
                column.CellTemplate = new FuncDataTemplate<object>((obj, scope) =>
                {
                    var cellContainer = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 0 };
                    
                    var placeBlock = new TextBlock
                    {
                        Width = 60,
                        Padding = new Thickness(5),
                        TextAlignment = TextAlignment.Center,
                        [!TextBlock.TextProperty] = new Binding($"Competitions[{i}].Place")
                    };
                    
                    var separator = new Rectangle 
                    { 
                        Fill = Brushes.LightGray, 
                        Width = 1, 
                        VerticalAlignment = VerticalAlignment.Stretch 
                    };
                    
                    var pointsBlock = new TextBlock
                    {
                        Width = 60,
                        Padding = new Thickness(5),
                        TextAlignment = TextAlignment.Center,
                        [!TextBlock.TextProperty] = new Binding($"Competitions[{i}].Points")
                    };
                    
                    cellContainer.Children.Add(placeBlock);
                    cellContainer.Children.Add(separator);
                    cellContainer.Children.Add(pointsBlock);
                    
                    return cellContainer;
                });
                
                // Обновляем заголовок
                var headerContainer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 0
                };
                
                headerContainer.Children.Add(new TextBlock 
                {
                    Text = $"Соревнование {i + 1}\nМ",
                    TextAlignment = TextAlignment.Center,
                    Width = 60,
                    Padding = new Thickness(5)
                });
                
                headerContainer.Children.Add(new Rectangle 
                {
                    Fill = Brushes.Gray,
                    Width = 1,
                    Margin = new Thickness(0, 5),
                    VerticalAlignment = VerticalAlignment.Stretch
                });
                
                headerContainer.Children.Add(new TextBlock 
                {
                    Text = $"Соревнование {i + 1}\nО",
                    TextAlignment = TextAlignment.Center,
                    Width = 60,
                    Padding = new Thickness(5)
                });
                
                column.Header = headerContainer;
            }
        }
    }

    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}