using System.IO;
namespace GloryBot.Utils;
public static class Translation
{
    public static string SelectedLang { get; private set; } = "German";
    public static Dictionary<string, string> Items { get; set; } = new();

    // Läd Sprach Datei die in den Settings angegeben sind 
    public static void LoadTranslation(string langName)
    {
        var res = false;
        Console.WriteLine($"Check for Translation \"{langName}\"");
        Log("Check for Translation", LogTypes.System);
        var lang = GetLang(langName);
        if (lang.Name == langName)
        {
            Console.WriteLine($"Got lang file {lang.File}");
            Log($"Got lang: {lang.File}");
            if (File.Exists($"Lang/Langs/{lang.File}"))
            {
                SelectedLang = langName;
                Items = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"Lang/Langs/{lang.File}"));
                Console.WriteLine($"{lang.File} loaded");
                Log($"{lang.File} Loaded");
            }
        }
    }
    // gets List of Languages and checks if its match with selected LangInstance and returns LangDbModel
    private static LangDbModel GetLang(string langName)
    {
        var lang = LangInstance.LangDb.Find(l => l.Name == langName);
        
        return (lang != null) ? lang : null;
    }

    // Change to Selected Sysem
    public static void ChangeTranslation(string langName)
    {
        LoadTranslation(langName);
        DashboardInstance.SettingsModel.Lang = langName;
        DashboardInstance.SettingsModel.Save();
        return;

    }
    // Returns Translated Text
    public static string Translate(string key, string defaultText = "")
    {
        if (Items.ContainsKey(key))
        {
            return Items[key];
        }
        else
        {
            if (!string.IsNullOrEmpty(defaultText))
            {
                return defaultText;
            }

            throw new Exception($"{key} not found in translation, check the Current selected Lang file if the Key exists");
        }
    }
}