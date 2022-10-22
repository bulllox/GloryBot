
namespace GloryBot.Models;

public class StreamModel
{
    public class SocialLink
    {
        public string type { get; set; } = "";
        public string url { get; set; } = "";
    }

    public bool is_live { get; set; } = false;
    public string category_id { get; set; } = "";
    public string category_name { get; set; } = "";
    public string live_title { get; set; } = "";
    public string audi_type { get; set; } = "";
    public string language_code { get; set; } = "";
    public string thumbnail { get; set; } = "";
    public int current_viewers { get; set; } = 0;
    public int followers { get; set; } = 0;
    public string streamer_info { get; set; } = "";
    public string profile_pic { get; set; } = "";
    public string channel_url { get; set; } = "";
    public string created_at { get; set; } = "";
    public int subscriber_num { get; set; } = 0;
    public string username { get; set; } = "";
    public List<SocialLink> social_links { get; set; } = new();
    public string started_at { get; set; } = "";
    public string ended_at { get; set; } = "";

    public void Save() {
        
    }

}