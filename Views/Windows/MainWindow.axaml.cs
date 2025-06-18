using Avalonia.Controls;
using Avalonia.Interactivity;
using ProtocolEditor.Views.Windows.Editor;

namespace ProtocolEditor.Views.Windows;

/// <summary>
/// Логика взаимодействия с MainWindow.axaml
/// </summary>

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CreateNewProtocol_Click(object? sender, RoutedEventArgs e)
    {
        var editorWindow = new EditorWindow();
        editorWindow.Show();
        this.Close();
    }
}