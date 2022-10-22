namespace GloryBot.Models;



public class BotData {
        public string Channel {get; set;} = "";  
}
public class SettingsData {
    public string Lang {get; set;} = "German";
}
public class DbModel {

    public BotData BotData {get; set;} = new();
    public SettingsData Settings {get; set; } = new();
    public List<LastCatData> LastCategorys {get; set;} = new();

    public void Save() {
        var path = PublicFolder() + @"\Assets\Database";       
        File.WriteAllText(path + @"\db.json", JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}

