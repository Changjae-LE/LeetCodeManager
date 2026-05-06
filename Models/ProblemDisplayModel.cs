using CommunityToolkit.Mvvm.ComponentModel;

namespace LeetCodeManager.Models;

public class ProblemDisplayModel : ObservableObject
{
    public ProblemMaster Master { get; }
    public UserProgress Progress { get; }

    private string _status;
    private int _rowNumber;

    public int RowNumber
    {
        get => _rowNumber;
        set => SetProperty(ref _rowNumber, value);
    }

    public string Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
            {
                Progress.Status = value;

                OnPropertyChanged(nameof(LastSolvedDateDisplay));
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public string TopicsDisplay => string.Join(", ", Master.Topics);
    public string IncludedInDisplay => string.Join(", ", Master.IncludedIn);
    public int ListCount => Master.ListCount;

    public string LastSolvedDateDisplay =>
        Progress.LastSolvedDate.HasValue
            ? Progress.LastSolvedDate.Value.ToString("yyyy-MM-dd")
            : "";

    public void MarkOpenedToday()
    {
        Progress.LastSolvedDate = DateTime.Today;
        OnPropertyChanged(nameof(LastSolvedDateDisplay));
    }

    public event EventHandler? StatusChanged;

    public ProblemDisplayModel(ProblemMaster master, UserProgress progress)
    {
        Master = master;
        Progress = progress;
        _status = progress.Status ?? "Not Started";
    }
}
