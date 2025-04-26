using System.Windows.Input;

namespace DBQueryApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _query = string.Empty;
    private string _result = string.Empty;

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

    public string Result
    {
        get => _result;
        set
        {
            if (_result != value)
            {
                _result = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand ExecuteCommand { get; }

    public MainWindowViewModel()
    {
        ExecuteCommand = new RelayCommand(ExecuteQuery);
    }

    private void ExecuteQuery()
    {
        // 仮実装: クエリをそのまま結果にコピー
        Result = $"Executed: {Query}";
    }
}