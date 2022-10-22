using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GloryBot.Models
{
    public class AlertUploadModel
    {
        public int Id {get; set;}
        public string Name { get; set; } = "";
        public string Text {get; set;} = "";
        public string AlertDuration {get; set;} = "0";
        public IFormFile SoundFile { get; set; }
        public IFormFile ImageFile { get; set; }
        public string AlertVolume { get; set; } = "";
        public string Animation { get; set; } = "none";
        public string TextColor { get; set; } = "white";
    
    }
}