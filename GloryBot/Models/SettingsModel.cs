namespace GloryBot.Models;

public class SettingsModel
{
    public int Id { get; set; }
    public string Lang { get; set; } = "German";
    public string Channel { get; set; } = "";
    public string AlertSceneName { get; set; } = "-Alerts-";
    public string ObsUrl { get; set; } = "";
    public string ObsPassword { get; set; } = "";
    public int ObsActive { get; set; } = 0;
    public string DiscordClientId { get; set; } = "";
    public string DiscordLargeImage { get; set; } = "";
    public string DiscordSmallImage {get; set; } = " ";

    public void Save() {
        using(var db = new LiteDatabase(DbPath)) {
            var col = db.GetCollection<SettingsModel>("settings");
            col.Update(this);
        }
    }
}
