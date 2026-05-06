using System.IO;
using System.Text.Json;
using LeetCodeManager.Models;

namespace LeetCodeManager.Services;

public class DataService
{
    private static readonly string AppDataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LeetCodeManager");

    private static readonly string UserProgressPath = Path.Combine(AppDataDir, "user_progress.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public List<ProblemMaster> LoadMasterData()
    {
        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "master_data.json");
            if (!File.Exists(path)) return new();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<ProblemMaster>>(json, JsonOptions) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public Dictionary<string, UserProgress> LoadUserProgress()
    {
        try
        {
            if (!File.Exists(UserProgressPath)) return new();
            string json = File.ReadAllText(UserProgressPath);
            return JsonSerializer.Deserialize<Dictionary<string, UserProgress>>(json, JsonOptions) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task SaveUserProgressAsync(Dictionary<string, UserProgress> progress)
    {
        try
        {
            Directory.CreateDirectory(AppDataDir);
            string json = JsonSerializer.Serialize(progress, JsonOptions);
            await File.WriteAllTextAsync(UserProgressPath, json);
        }
        catch
        {
            // Silently fail; next save will retry
        }
    }
}
