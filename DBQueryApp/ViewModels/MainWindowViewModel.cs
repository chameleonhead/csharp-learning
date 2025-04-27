using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

    public RelayCommand ExecuteCommand { get; }

    private readonly SqlService _sqlService;
    private AiService? _mcpService;
    private Task? _task;

    public MainWindowViewModel()
    {
        _sqlService = new SqlService("Server=localhost,1433;User Id=sa;Password=P@ssw0rd;Database=AdventureWorks;Encrypt=False");
        ExecuteCommand = new RelayCommand(ExecuteQuery, () => _task == null || _task.IsCompleted);
        LoadTableTree();
    }

    private void LoadTableTree()
    {
        var tables = new List<TreeNode>();
        try
        {
            var schema = _sqlService.GetSchema();
            _mcpService = new AiService(schema);

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

    private async void ExecuteQuery()
    {
        var tcs = new TaskCompletionSource();
        _task = tcs.Task;
        ExecuteCommand.RaiseCanExecuteChanged();
        try
        {
            var originalQuery = Query;
            if (Regex.IsMatch(originalQuery, "^\\s*SELECT"))
            {
                if (string.IsNullOrEmpty(originalQuery))
                {
                    ResultMessage = $"Error: could not parse query";
                    ResultTable = null;
                    tcs.SetResult();
                    return;
                }
                var result = _sqlService.ExecuteQuery(originalQuery);
                ResultTable = result.DefaultView;
            }
            else
            {
                var task = _mcpService?.QueryAsync(originalQuery);
                var sql = task == null ? "" : await task;
                if (string.IsNullOrEmpty(sql))
                {
                    ResultMessage = $"Error: could not parse query";
                    ResultTable = null;
                    tcs.SetResult();
                    return;
                }
                var result = _sqlService.ExecuteQuery(sql);
                ResultTable = result.DefaultView;
            }
            ResultMessage = $"Success: {ResultTable?.Count ?? 0} rows.";
            tcs.SetResult();
        }
        catch (Exception ex)
        {
            ResultMessage = $"Error: {ex.Message}";
            ResultTable = null;
            tcs.SetException(ex);
        }
        finally
        {
            _task = null;
            ExecuteCommand.RaiseCanExecuteChanged();
        }
    }
}
