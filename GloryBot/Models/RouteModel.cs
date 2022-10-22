namespace GloryBot.Models;
public class RouteModel {
    public string Name {get; set;} = "";
    public string Controller {get; set;} = "";
    public string Action {get; set;} = "";
    public List<string> Wildcards {get; set;} = new();
}