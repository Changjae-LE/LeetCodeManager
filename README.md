# LeetCode Study Manager

A Windows desktop app for managing personal LeetCode study progress.  
It helps you track problem-solving status and quickly find problems that need review based on popular problem lists such as Blind 75, NeetCode 75, NeetCode 150, NeetCode 250, Grind 169, and Top Interview 150.

---

## Screenshot

> _(Add a screenshot of the app here after running it.)_

---

## Features

| Feature                     | Description                                                                                        |
| --------------------------- | -------------------------------------------------------------------------------------------------- |
| **Status Management**       | Select a problem and mark it as `Solved` or `Need Review` using the header buttons                 |
| **Status Toggle**           | Clicking the same status button again resets the problem back to `Not Started`                     |
| **Automatic Date Tracking** | The latest attempt date is recorded when a problem status changes or when a problem link is opened |
| **Automatic Save**          | Progress is automatically saved to `%AppData%\Roaming\LeetCodeManager\user_progress.json`          |
| **Combined Filtering**      | Filter problems by title, difficulty, status, problem list, and topics                             |
| **Problem List Filter**     | View only problems included in specific lists such as Blind 75, NeetCode 150, or Grind 169         |
| **Multi-Topic Selection**   | Select multiple algorithm topics using the Topics dropdown                                         |
| **ListCount Sorting**       | Problems included in more well-known lists are displayed higher by default                         |
| **Open LeetCode URL**       | Click `Open` in the Link column to open the problem page in the default browser                    |
| **Keyboard Shortcut**       | Press `Esc` to clear all filters                                                                   |

---

## Requirements

- Windows 10 / 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  - Required for development and building
- .NET 8 Runtime
  - Required when running as a framework-dependent app

---

## Installation and Run

### Option 1 — Run with `dotnet run`

```bash
git clone https://github.com/<your-username>/LeetCodeManager.git
cd LeetCodeManager
dotnet restore
dotnet run
```

### Option 2 — Build a Windows executable

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

After publishing, the executable is usually created at:

```text
bin\Release\net8.0-windows\win-x64\publish\LeetCodeManager.exe
```

Run `LeetCodeManager.exe` to start the app.

---

## How to Use

### Change Problem Status

1. Select a problem row in the DataGrid.
2. Click `✓ Solved` or `⚑ Mark Need Review` in the header.
3. Click the same button again to reset the status back to `Not Started`.

| Current Status | Button Clicked       | Result      |
| -------------- | -------------------- | ----------- |
| Not Started    | `✓ Solved`           | Solved      |
| Solved         | `✓ Solved`           | Not Started |
| Not Started    | `⚑ Mark Need Review` | Need Review |
| Need Review    | `⚑ Mark Need Review` | Not Started |

---

## Filtering

You can combine multiple filters from the top filter bar.

- **Search** — Search by problem title or canonical slug
- **Difficulty** — All / Easy / Medium / Hard
- **Status** — All / Solved / Need Review
- **Problem List** — All / Blind 75 / NeetCode 75 / NeetCode 150 / NeetCode 250 / Grind 169 / Top Interview 150
- **Topics** — Select multiple algorithm topics
- **✕ Clear Filters** or `Esc` — Clear all filters

Filters are applied using AND logic.

For example, selecting:

```text
Difficulty = Medium
Problem List = Blind 75
Topic = Graphs
```

will show only **Medium Graphs problems included in Blind 75**.

---

## Customizing Problem Data

You can customize the problem list by editing `master_data.json` in the same directory as the app executable.

Example:

```json
[
  {
    "CanonicalSlug": "two-sum",
    "Title": "Two Sum",
    "Difficulty": "Easy",
    "Topics": ["Arrays & Hashing"],
    "IncludedIn": ["Blind 75", "NeetCode 150", "Grind 169"],
    "ListCount": 3,
    "Url": "https://leetcode.com/problems/two-sum/"
  }
]
```

| Field           | Description                                                                              |
| --------------- | ---------------------------------------------------------------------------------------- |
| `CanonicalSlug` | Unique problem identifier from the LeetCode URL. Used as the key for saved user progress |
| `Title`         | Problem title displayed in the app                                                       |
| `Difficulty`    | `Easy`, `Medium`, or `Hard`                                                              |
| `Topics`        | Array of algorithm topic tags                                                            |
| `IncludedIn`    | Array of problem lists that include this problem                                         |
| `ListCount`     | Number of lists in `IncludedIn`. Used for sorting                                        |
| `Url`           | LeetCode problem URL                                                                     |

> User progress is stored separately in `%AppData%\LeetCodeManager\user_progress.json`.  
> Therefore, replacing `master_data.json` will preserve progress as long as the same `CanonicalSlug` values are used.

---

## User Progress Storage

User progress is saved at:

```text
%AppData%\Roaming\LeetCodeManager\user_progress.json
```

Example saved data:

```json
{
  "two-sum": {
    "Status": "Solved",
    "LastSolvedDate": "2026-05-06T00:00:00"
  }
}
```

---

## Project Structure

```text
LeetCodeManager/
├── Models/
│   ├── ProblemMaster.cs                 # Master problem data model
│   ├── UserProgress.cs                  # User progress model
│   └── ProblemDisplayModel.cs           # Combined model for UI binding
├── ViewModels/
│   └── MainViewModel.cs                 # Filtering, sorting, status updates, and save logic
├── Services/
│   └── DataService.cs                   # JSON read/write service
├── Converters/
│   ├── StatusToForegroundConverter.cs
│   ├── DifficultyToForegroundConverter.cs
│   └── NonEmptyStringToVisibilityConverter.cs
├── MainWindow.xaml                      # Main UI
├── MainWindow.xaml.cs                   # UI event handling
├── App.xaml                             # Global style resources
├── master_data.json                     # Master problem data
└── LeetCodeManager.csproj
```

---

## Tech Stack

- **C# / .NET 8**
- **WPF** — Desktop UI framework
- **MVVM Pattern** — UI and business logic separation
- **CommunityToolkit.Mvvm** — Observable properties and relay commands
- **System.Text.Json** — JSON serialization and deserialization

---

## License

MIT
