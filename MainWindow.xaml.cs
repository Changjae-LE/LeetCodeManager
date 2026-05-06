using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LeetCodeManager.Models;
using LeetCodeManager.ViewModels;
using System.Windows.Documents;

namespace LeetCodeManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Single-click to begin editing the Status cell
    private void ProblemsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        var dg = (DataGrid)sender;
        // Only begin edit when exactly one cell is selected and it is the Status column (index 3)
        if (dg.SelectedCells.Count == 1 &&
            dg.SelectedCells[0].Column.DisplayIndex == 3)
        {
            dg.Dispatcher.BeginInvoke(new Action(() => dg.BeginEdit()),
                System.Windows.Threading.DispatcherPriority.Input);
        }
    }

    // Row number display
    private void ProblemsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        //e.Row.Header = $"{e.Row.GetIndex() + 1}.";
    }

    // Open URL in default browser
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

            if (sender is Hyperlink hyperlink &&
                hyperlink.DataContext is ProblemDisplayModel problem &&
                DataContext is MainViewModel vm)
            {
                vm.MarkProblemOpened(problem);
            }
        }
        catch
        {
            /* ignore */
        }

        e.Handled = true;
    }
}
