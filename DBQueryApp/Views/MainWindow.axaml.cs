using Avalonia.Controls;
using DBQueryApp.ViewModels;

namespace DBQueryApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}