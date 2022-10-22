using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GloryBot.Models
{
    public class Subscription
    {
        public User user { get; set; }  = new User();
        public int sub_created_at { get; set; } = 0;
        public string sub_lv { get; set; } = "";
        public string sub_tier { get; set; } = "";
    }

    public class User
    {
        public string user_id { get; set; } = "";
        public string username { get; set; } = "";
        public string display_name { get; set; } = "";
        public string profile_pic { get; set; } = "";
        public string created_at { get; set; } = "";
    }
    public class SubscriberModel
    {
        public int total {get; set;} = 0;
        public List<Subscription> subscriptions {get; set;} = new();
    }
}