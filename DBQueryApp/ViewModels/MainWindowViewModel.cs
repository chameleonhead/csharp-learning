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
    private string _query = string.Empty;
    private string _resultMessage = string.Empty;
    private DataView? _resultTable = null;

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