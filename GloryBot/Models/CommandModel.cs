namespace GloryBot.Models;

public class CommandModel {
    public int Id {get; set;}
    public string Command {get; set;} = "";
    public string CommandDesc {get; set;} = "";
    public string CommandText {get; set;} = "";
    public List<UserRoles> Roles {get; set;} = new();

    public bool Save() {
        using(var db = new LiteDatabase(DbPath)) {
            var col = db.GetCollection<CommandModel>("commands");
            if(!col.Exists(Query.EQ("Command", Command))) {
                col.Insert(this);
                return true;
            } else {
                return false;
            }
        }
    }

    public void Update() {
        using(var db = new LiteDatabase(DbPath)) {
            var col = db.GetCollection<CommandModel>("commands");

            col.Update(this);
        }
    }

    public void Delete() {
        using(var db = new LiteDatabase(DbPath)) {
            var col = db.GetCollection<CommandModel>("commands");
            if(col.Exists(Query.EQ("Command", Command))) {
                col.Delete(Id);
            }
         }
    }
}