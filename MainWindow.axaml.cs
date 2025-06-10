using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProtocolEditor.Editor;

namespace ProtocolEditor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ToExcelFileEditor_OnClick(object? sender, RoutedEventArgs e)
    {
        var editWindow = new EditWindow();
        editWindow.Show();
    }
}