namespace GloryBot.Utils;

public class Lang
{

    public List<LangDbModel> LangDb { get; set; } = new();


    // Load Lang Database
    public void LoadLangDb()
    {
        Console.WriteLine("load lang db");
        Log("Loading Lang Db", LogTypes.System);
        LangDb = TryLoadJson<List<LangDbModel>>("Lang/LangDb.json");
        Console.WriteLine("lang db loaded");
        Log("Lang Db load", LogTypes.System);
    }

    // Get Language for Selection
    public string GetLanguage()
    {
        var options = "";
        foreach (var lang in LangDb)
        {
            if (SelectedLang == lang.Name)
            {
                options += $"<option selected>{lang.Name}</option>";
            }
            else
            {
                options += $"<option>{lang.Name}</option>";
            }
        }
        return options;
    }

}
