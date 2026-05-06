using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeetCodeManager.Models;
using LeetCodeManager.Services;

namespace LeetCodeManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DataService _dataService = new();
    private readonly ObservableCollection<ProblemDisplayModel> _allProblems = new();
    private readonly ICollectionView _problemsView;
    private QuickFilter _activeQuickFilter = QuickFilter.None;

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private string _selectedDifficulty = "All";
    [ObservableProperty] private string _selectedStatus = "All";
    [ObservableProperty] private string _selectedProblemList = "All";
    [ObservableProperty] private ProblemDisplayModel? _selectedProblem;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _solvedCount;
    [ObservableProperty] private int _needReviewCount;
    [ObservableProperty] private int _filteredCount;
    [ObservableProperty] private string _activeQuickFilterLabel = "";

    public ObservableCollection<TopicFilterItem> TopicFilters { get; } = new();
    public ObservableCollection<string> ProblemLists { get; } = new();
    public ICollectionView ProblemsView => _problemsView;

    public string SelectedTopicsText
    {
        get
        {
            int n = TopicFilters.Count(t => t.IsSelected);
            return n == 0 ? "Topics ▼" : $"Topics ({n}) ▼";
        }
    }

    public MainViewModel()
    {
        _problemsView = CollectionViewSource.GetDefaultView(_allProblems);
        _problemsView.SortDescriptions.Add(
            new SortDescription(nameof(ProblemDisplayModel.ListCount), ListSortDirection.Descending));
        _problemsView.Filter = FilterPredicate;

        LoadData();
    }

    private void LoadData()
    {
        var masters = _dataService.LoadMasterData();
        var progress = _dataService.LoadUserProgress();
        var allTopics = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var allLists = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        int rowNumber = 1;

        foreach (var master in masters)
        {
            var p = progress.TryGetValue(master.CanonicalSlug, out var saved)
                ? saved
                : new UserProgress();

            var model = new ProblemDisplayModel(master, p);
            model.RowNumber = rowNumber++;

            model.StatusChanged += OnStatusChanged;
            _allProblems.Add(model);

            foreach (var t in master.Topics)
                allTopics.Add(t);

            foreach (var list in master.IncludedIn)
                allLists.Add(list);
        }

        foreach (var topic in allTopics)
            TopicFilters.Add(new TopicFilterItem(topic, RefreshFilter));

        ProblemLists.Add("All");
        foreach (var list in allLists)
            ProblemLists.Add(list);

        UpdateStats();
        RefreshFilter();
    }

    private bool FilterPredicate(object obj)
    {
        if (obj is not ProblemDisplayModel item) return false;

        if (_activeQuickFilter == QuickFilter.Yesterday)
            return item.Progress.LastSolvedDate?.Date == DateTime.Today.AddDays(-1);

        if (SelectedDifficulty != "All" && item.Master.Difficulty != SelectedDifficulty)
            return false;

        if (SelectedStatus != "All" && item.Status != SelectedStatus)
            return false;

        if (SelectedProblemList != "All" &&
            !item.Master.IncludedIn.Contains(SelectedProblemList))
            return false;

        var selected = TopicFilters.Where(t => t.IsSelected).Select(t => t.Name).ToHashSet();
        if (selected.Count > 0 && !item.Master.Topics.Any(t => selected.Contains(t)))
            return false;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string q = SearchText.ToLowerInvariant();
            if (!item.Master.Title.ToLowerInvariant().Contains(q) &&
                !item.Master.CanonicalSlug.ToLowerInvariant().Contains(q))
                return false;
        }

        return true;
    }

    [RelayCommand]
    private void ShowAll()
    {
        ClearAllFilters();
    }


    [RelayCommand]
    private void ToggleSolved()
    {
        if (SelectedProblem is null)
            return;

        SelectedProblem.Status =
            SelectedProblem.Status == "Solved"
                ? "Not Started"
                : "Solved";

        UpdateStats();
        RefreshFilter();
        _ = SaveAsync();
    }

    [RelayCommand]
    private void ToggleNeedReview()
    {
        if (SelectedProblem is null)
            return;

        SelectedProblem.Status =
            SelectedProblem.Status == "Need Review"
                ? "Not Started"
                : "Need Review";

        UpdateStats();
        RefreshFilter();
        _ = SaveAsync();
    }


    [RelayCommand]
    private void ShowYesterday()
    {
        ResetFiltersOnly();
        _activeQuickFilter = QuickFilter.Yesterday;
        ActiveQuickFilterLabel = "Yesterday";
        RefreshFilter();
    }

    [RelayCommand]
    private void ClearAllFilters()
    {
        _activeQuickFilter = QuickFilter.None;
        ActiveQuickFilterLabel = "";
        SearchText = "";
        SelectedDifficulty = "All";
        SelectedStatus = "All";
        SelectedProblemList = "All";
        foreach (var t in TopicFilters) t.IsSelectedSilent = false;
        OnPropertyChanged(nameof(SelectedTopicsText));
        RefreshFilter();
    }

    private void ResetFiltersOnly()
    {
        SearchText = "";
        SelectedDifficulty = "All";
        SelectedStatus = "All";
        SelectedProblemList = "All";
        foreach (var t in TopicFilters) t.IsSelectedSilent = false;
        OnPropertyChanged(nameof(SelectedTopicsText));
    }

    private void OnStatusChanged(object? sender, EventArgs e)
    {
        UpdateStats();
        _ = SaveAsync();
    }

    public void MarkProblemOpened(ProblemDisplayModel problem)
    {
        problem.MarkOpenedToday();
        _ = SaveAsync();
    }

    private async Task SaveAsync()
    {
        var dict = _allProblems.ToDictionary(p => p.Master.CanonicalSlug, p => p.Progress);
        await _dataService.SaveUserProgressAsync(dict);
    }

    private void UpdateStats()
    {
        TotalCount = _allProblems.Count;
        SolvedCount = _allProblems.Count(p => p.Status == "Solved");
        NeedReviewCount = _allProblems.Count(p => p.Status == "Need Review");
    }

    private void RefreshFilter()
    {
        _problemsView.Refresh();

        int row = 1;
        foreach (ProblemDisplayModel problem in _problemsView)
        {
            problem.RowNumber = row++;
        }

        FilteredCount = row - 1;
        OnPropertyChanged(nameof(SelectedTopicsText));
    }

    partial void OnSelectedProblemListChanged(string value) => RefreshFilter();
    partial void OnSearchTextChanged(string value) => RefreshFilter();
    partial void OnSelectedDifficultyChanged(string value) => RefreshFilter();
    partial void OnSelectedStatusChanged(string value) => RefreshFilter();
}

public enum QuickFilter { None, NeedReview, Yesterday }

public class TopicFilterItem : ObservableObject
{
    private bool _isSelected;
    private readonly Action _onChanged;

    public string Name { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
                _onChanged();
        }
    }

    // Silent setter used when clearing all filters (avoids N refreshes for N topics)
    public bool IsSelectedSilent
    {
        set => SetProperty(ref _isSelected, value, nameof(IsSelected));
    }

    public TopicFilterItem(string name, Action onChanged)
    {
        Name = name;
        _onChanged = onChanged;
    }
}
