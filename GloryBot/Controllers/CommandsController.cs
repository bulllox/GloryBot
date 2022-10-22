using System.Linq;
using GloryBot.Models.SaveModels;
using Microsoft.Extensions.Logging;
namespace GloryBot.Controllers;

public class CommandsController : Controller
{
    private BrowserWindow editBrowser { get; set; }
    private readonly ILogger<CommandsController> _logger;

    public CommandsController(ILogger<CommandsController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        SetChatState(false);
        ViewData["Commands"] = ChatInstance.CommandInstance.GetCommandList();
        IsHomeActive = false;
               
        return View();
    }

    public IActionResult Add()
    {
        Console.WriteLine("SHow Add View");
        return View("Add");
    }

    [HttpPost]
    public IActionResult AddCommand(CommandSaveModel model)
    {
        IActionResult res = null;
        try
        {
            var command = new CommandModel
            {
                Command = model.Name,
                CommandDesc = model.Description,
                CommandText = model.Text
            };
            var roles = model.Roles.Split(",").ToStringList();
            foreach(var role in roles)
            {
                Enum.TryParse(role, out UserRoles userRole);
                command.Roles.Add(userRole);
            }

            if(command.Save())
            {
                ChatInstance.CommandInstance.RegisterCommand(command.Command, command);
                res = Redirect("/commands/index");
            }
        } 
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            Log(ex.ToString(), LogTypes.Error);
            ViewBag.State = "error";
            res = View("Add");
        }
        return res;
    }
   
    private void UpdateCommand(object obj)
    {
        try
        {
            var JString = JsonConvert.SerializeObject(obj);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(JString);
           
            var model = ChatInstance.CommandInstance.Find(x => x.Command == data["oldName"]);
            if (model != null)
            {
                model.Command = data["commandName"];
                model.CommandDesc = data["commandDesc"];
                model.CommandText = data["commandText"];
                model.Update();
                ChatInstance.CommandInstance.UpdateCommand(model.Command, model);
                var dict = new Dictionary<string, string>{
                    {"title", "Success"},
                    {"msg", Translate("CommandUpdated", "Command was updated")}
                };
                ViewData["Commands"] = ChatInstance.CommandInstance.GetCommandList();
                
                Electron.IpcMain.Send(TDGlobals.MainWindow, "window:nodifacation", JsonConvert.SerializeObject(dict, Formatting.Indented));
                System.Threading.Thread.Sleep(7000);
                MainWindow.Reload();
            }
            else
            {
                var dict = new Dictionary<string, string>{
                    {"title", "Success"},
                    {"msg", Translate("CommandUpdated", "Command was updated")}
                };
                Electron.IpcMain.Send(TDGlobals.MainWindow, "window:nodifacation", JsonConvert.SerializeObject(dict, Formatting.Indented));
                System.Threading.Thread.Sleep(7000);
                MainWindow.Reload();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public IActionResult Delete(string commandName)
    {
        var command = ChatInstance.CommandInstance.Find(x => x.Command == commandName);
        command.Delete();
        ChatInstance.CommandInstance.DeleteCommand(commandName);
        
        return Redirect("/commands/index");
    }

    public IActionResult Edit(string commandName)
    {
        var command = ChatInstance.CommandInstance.Find(x => x.Command == commandName);
        ViewBag.Command = command;
        return View();
    }


    public IActionResult EditCommand(CommandSaveModel model)
    {
        
        var command = ChatInstance.CommandInstance.Find(x => x.Command == model.OldName);
        if (command != null)
        {
            command.Command = model.Name;
            command.CommandDesc = model.Description;
            command.CommandText = model.Text;
            var roles = model.Roles.Split(",");
                command.Roles.Clear();
            foreach(var role in roles)
            {
                Enum.TryParse(role, out UserRoles userRole);
                command.Roles.Add(userRole);
            }
            command.Update();
        }
        TempData["state"] = 1;
        TempData["msg"] = "Erfolgreich bearbeitet";
        return RedirectToAction("Index", "Commands");
    }
}