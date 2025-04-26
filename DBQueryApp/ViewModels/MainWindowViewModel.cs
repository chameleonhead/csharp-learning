using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Windows.Input;
using DBQueryApp.Services;

namespace DBQueryApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private List<TreeNode> _tableTree = [];
    private TreeNode? _selectedNode;
    private string _query = string.Empty;
    private string _resultMessage = string.Empty;
    private DataView? _resultTable = null;

    public List<TreeNode> TableTree
    {
        get => _tableTree;
        set
        {
            if (_tableTree != value)
            {
                _tableTree = value;
                OnPropertyChanged();
            }
        }
    }

    public TreeNode? SelectedNode
    {
        get => _selectedNode;
        set
        {
            if (_selectedNode != value)
            {
                _selectedNode = value;
                OnPropertyChanged();
            }
        }
    }

    public string Query
    {
        get => _query;
        set
        {
            if (_query != value)
            {
                _query = value;
                OnPropertyChanged();
            }
        }
    }

    public string ResultMessage
    {
        get => _resultMessage;
        set
        {
            if (_resultMessage != value)
            {
                _resultMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public DataView? ResultTable
    {
        get => _resultTable;
        set
        {
            if (_resultTable != value)
            {
                _resultTable = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand ExecuteCommand { get; }

    private readonly SqlService _sqlService;

    public MainWindowViewModel()
    {
        _sqlService = new SqlService("Server=localhost,1433;User Id=sa;Password=P@ssw0rd;Database=AdventureWorks;Encrypt=False");
        ExecuteCommand = new RelayCommand(ExecuteQuery);
        LoadTableTree();
    }

    private void LoadTableTree()
    {
        var tables = new List<TreeNode>();
        try
        {
            var schema = _sqlService.GetSchema();

            foreach (var row in schema)
            {
                var tableNode = new TreeNode
                {
                    Name = row.Name,
                    Children = [.. row.Columns.Select(c => new TreeNode() { Name = c.Name })]
                };

                // カラム一覧を取得
                tables.Add(tableNode);
            }
        }
        catch (Exception ex)
        {
            ResultMessage = $"Error loading tables: {ex.Message}";
        }

        TableTree = tables.OrderBy(t => t.Name).ToList();
    }

    private void ExecuteQuery()
    {
        try
        {
            var result = _sqlService.ExecuteQuery(Query);
            ResultTable = result.DefaultView;
            ResultMessage = $"Success: {ResultTable?.Count ?? 0} rows.";
        }
        catch (Exception ex)
        {
            ResultMessage = $"Error: {ex.Message}";
            ResultTable = null;
        }
    }
}