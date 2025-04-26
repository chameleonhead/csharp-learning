using System.ComponentModel;
using System.Data;
using Avalonia.Controls;
using Avalonia.Data;
using DBQueryApp.ViewModels;

namespace DBQueryApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.PropertyChanged += OnViewModelPropertyChanged;
            }
        };
        DataContext = new MainWindowViewModel();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.ResultTable))
        {
            if (DataContext is MainWindowViewModel vm && vm.ResultTable != null)
            {
                BuildDataGridColumns(vm.ResultTable);
            }
        }
    }

    private void BuildDataGridColumns(DataView view)
    {
        ResultDataGrid.Columns.Clear();
        ResultDataGrid.ItemsSource = null;

        if (view.Table!.Columns.Count == 0)
            return;

        foreach (DataColumn column in view.Table.Columns)
        {
            if (column.DataType == typeof(bool))
            {
                ResultDataGrid.Columns.Add(new DataGridCheckBoxColumn
                {
                    Header = column.ColumnName,
                    Binding = new Binding($"Row.ItemArray[{column.Ordinal}]")
                });
            }
            else
            {
                ResultDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = column.ColumnName,
                    Binding = new Binding($"Row.ItemArray[{column.Ordinal}]")
                });
            }
        }

        ResultDataGrid.ItemsSource = view;
    }

    private void OnTreeViewDoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (TableTreeView.SelectedItem is TreeNode node)
        {
            if (node.Children.Count > 0)
            {
                // これはテーブルノード（子を持っている＝テーブル）
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.Query = $"SELECT * FROM [{string.Join("].[", node.Name.Split("."))}]";
                }
            }
            else
            {
                // これはカラムノード（子がいない＝カラム）
                // カラムを選んだときの動作は今は特にしない（必要なら拡張できる）
            }
        }
    }
}