using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GloryBot.Models
{
    public class UrlModel
    {
        public string Url {get; set;} = "";
        public string Name {get; set;} = "";
        public List<UrlModel> SubMenus { get; set; } = null;
        public bool Enabled { get; set; } = true;
    }
}