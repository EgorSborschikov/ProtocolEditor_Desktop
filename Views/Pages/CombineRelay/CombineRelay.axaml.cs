using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProtocolEditor.Models;
using ProtocolEditor.ViewModels;
using ProtocolEditor.Views.Dialogs;

namespace ProtocolEditor.Views.Pages.CombineRelay;

/// <summary>
/// Логика взаимодействия с CombineRelay.axaml
/// </summary>

public partial class CombineRelay : UserControl
{
    private CombineRelayViewModel? ViewModel => (CombineRelayViewModel)DataContext!;
    
    public CombineRelay()
    {
        InitializeComponent();
        
        // Инициализация контекста данных
        var context = new ProtocolEditorDbContext();
        DataContext = new CombineRelayViewModel(context);
            
        // Подписка на загрузку данных для номеров строк
        if (CombineRelayDataGrid != null)
        {
            CombineRelayDataGrid.LoadingRow += (s, e) => 
            {
                e.Row.Header = e.Row.GetIndex() + 1;
            };
        }
    }
    

    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.SaveChanges();
    }

    private void SortByPlace_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.SortByPlace();
    }
}