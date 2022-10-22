using System;  
using System.Collections.Generic;  
using System.ComponentModel;  
using System.ComponentModel.DataAnnotations;  
using System.Linq;  
using System.Web;
using Microsoft.AspNetCore.Http;

namespace GloryBot.Models
{
    
    public class AlertModel
    {
        public int Id {get; set;}
        public string Name { get; set; } = "";
        public string AlertText {get; set;} = "";
        public string AlertDuration {get; set;} = "3";
        public string SoundFilePath { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string AlertVolume { get; set; } = "";
        public string Animation { get; set; } = "none";
        public string TextColor { get; set; } = "white";
        public void Save() {
            using(var db = new LiteDatabase(DbPath)) {
                var col = db.GetCollection<AlertModel>("alerts");
                if(col.Exists(Query.EQ("Name", Name))) {
                    col.Update(this);
                } else {
                    col.Insert(this);
                }
            }
        }

        public void Delete() {
            using(var db = new LiteDatabase(DbPath)) {
                var col = db.GetCollection<AlertModel>("alerts");
                if(col.Exists(Query.EQ("Name", Name))) {
                    col.Delete(Id);
                }
            }
        }

    }
}