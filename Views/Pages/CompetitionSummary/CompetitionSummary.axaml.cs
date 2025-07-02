using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ProtocolEditor.ViewModels;
using ProtocolEditor.Models;

namespace ProtocolEditor.Views.Pages.CompetitionSummary;

public partial class CompetitionSummary : UserControl
{
    private CompetitionSummaryViewModel? _viewModel;
    private CompetitionSummaryViewModel? ViewModel => _viewModel ??= DataContext as CompetitionSummaryViewModel;
    private int competitionCounter = 0;
    public CompetitionSummary()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.UpdateColumnsRequested += UpdateCompetitionColumns;
            ViewModel.Initialize();
        }
    }
    
    private void AddCompetition_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null) return;
        
        ViewModel.AddCompetition();
        UpdateCompetitionColumns();
    }
    
    private void UpdateCompetitionColumns()
    {
        // Удаляем все столбцы соревнований
        var columnsToRemove = CompetitionDataGrid.Columns
            .Where(c => c.Tag?.Equals("CompetitionColumn") == true)
            .ToList();
        
        foreach (var column in columnsToRemove)
        {
            CompetitionDataGrid.Columns.Remove(column);
        }

        // Добавляем новые столбцы для всех соревнований
        if (ViewModel != null)
        {
            for (int i = 0; i < ViewModel.Competitions.Count; i++)
            {
                AddCompetitionColumn(i);
            }
        }
    }
    
    private void AddCompetitionColumn(int competitionIndex)
    {
        if (ViewModel == null || competitionIndex >= ViewModel.Competitions.Count) return;
        
        var competition = ViewModel.Competitions[competitionIndex];
        int insertIndex = 2; // После колонок "№" и "Команда"

        var headerContainer = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Spacing = 0
        };
        
        headerContainer.Children.Add(new TextBlock
        {
            Text = $"\nМ",
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
            Text = $"\nО",
            TextAlignment = TextAlignment.Center,
            Width = 60,
            Padding = new Thickness(5)
        });

        var column = new DataGridTemplateColumn
        {
            Header = headerContainer,
            Tag = "CompetitionColumn",
            CellTemplate = new FuncDataTemplate<TeamViewModel>((team, scope) =>
            {
                if (team == null) return null;
                
                var cellContainer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 0
                };

                var placeBlock = new TextBlock
                {
                    Width = 60,
                    Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Center,
                    [!TextBlock.TextProperty] = new Binding($"CompetitionPlaces[{competition.IDCompetition}]")
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
                    [!TextBlock.TextProperty] = new Binding($"CompetitionPoints[{competition.IDCompetition}]")
                };
                
                cellContainer.Children.Add(placeBlock);
                cellContainer.Children.Add(separator);
                cellContainer.Children.Add(pointsBlock);
                
                return cellContainer;
            })
        };
        
        CompetitionDataGrid.Columns.Insert(insertIndex + competitionIndex, column);
    }

    private void SortByPlace_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ExportToExcel_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
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