namespace GloryBot.Models;

public class AuthModel
{
    public int Id {get; set;} 
    public string username {get; set;} = "";
    public string access_token { get; set; } = "";
    public string token_type { get; set; } = "";
    public int expires_in { get; set; } = 0;
    public string refresh_token { get; set;} = ""; 
    public void Save(string table)
    {
        // try
        // {
        //     var jString = JsonConvert.SerializeObject(this, Formatting.Indented);
        //     File.WriteAllText(path, jString);
        //     Console.WriteLine($"{path} saved");
        //     Log($"[Auth Model]: {path} saved");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine(ex.Sysem);
        //     Log($"[Auth Model]: {path} cannot be saved Error: {ex.Sysem}");
        // }

        using(var db = new LiteDatabase(DbPath)) {
            var col = db.GetCollection<AuthModel>(table);
            col.Update(this);
        }
    }
}