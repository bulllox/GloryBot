namespace GloryBot.Models
{
    public class NodificationModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Message { get; set; } = "";
        public int Lines { get; set; } = 0;
        public int CallAfterLines { get; set; } = 0;
        public bool Active { get; set; } = true;

        public void Save() {
            using(var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<NodificationModel>("nodifications");
                col.Insert(this);
            }
        }

        public void Update()
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<NodificationModel>("nodifications");
                col.Update(this);
            }
        }

        public void Delete()
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<NodificationModel>("nodifications");
                if(col.Exists(Query.EQ("Name", this.Name)))
                {
                    col.Delete(this.Id);
                }
            }
        }

        public void TriggerMessage()
        {
            Chat.SendChatMessage(this.Message);
        }
    }

  
}
