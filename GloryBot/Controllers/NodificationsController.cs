using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GloryBot.Controllers
{
    public class NodificationsController : Controller
    {
        public IActionResult Index()
        {
            Electron.IpcMain.RemoveAllListeners("setNodyState");
            Electron.IpcMain.On("setNodyState", SaveNodyState);
            return View();
        }

        

        private void SaveNodyState(object data)
        {
            var jString = JsonConvert.SerializeObject(data, Formatting.Indented);
            var JData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jString);
            var model = ChatInstance.NodyList.Where(x => x.Id == JData["id"].ToInt()).First();

            if (JData["state"] == "on")
            {
                model.Active = true;
                model.Update();

                var dict = new Dictionary<string, string>{
                    {"title", "Success"},
                    {"msg", Translate("nodification.nodificationActivated", "Nodification Activated")}
                };
                Electron.IpcMain.Send(MainWindow, "window:nodifacation", JsonConvert.SerializeObject(dict, Formatting.Indented));
            }
            else
            {
                model.Active = false;
                model.Update();
                var dict = new Dictionary<string, string>{
                    {"title", "Success"},
                    {"msg", Translate("nodification.nodificationDeactivated", "Nodification Deactivated")}
                };
                Electron.IpcMain.Send(MainWindow, "window:nodifacation", JsonConvert.SerializeObject(dict, Formatting.Indented));
            }

        }

        public IActionResult Edit(int id)
        {
            Electron.IpcMain.RemoveAllListeners("nodyUpdate");
            Electron.IpcMain.On("nodyUpdate", UpdateNodification);
            ViewBag.Nodification = ChatInstance.NodyList.Where(x => x.Id == id).First();
            return View();
        }

        private void UpdateNodification(object data)
        {
            var jString = JsonConvert.SerializeObject(data, Formatting.Indented);
            var JData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jString);
            var id = JData["nodyId"].ToInt();
            var name = JData["nodyName"];
            var message = JData["nodyMessage"];
            var lines = JData["nodyLines"].ToInt();
            var model = ChatInstance.NodyList.Where((x) => x.Id == id).First();
            model.Name = name;
            model.Message = message;
            model.CallAfterLines = lines;
            model.Update();


            var dict = new Dictionary<string, string>{
                    {"title", "Success"},
                    {"msg", "Erfolgreich bearbeitet"},
                    {"uri", "/nodifications/index"},
                    {"time", "3"}
                };

            Electron.IpcMain.Send(MainWindow, "window-wd", JsonConvert.SerializeObject(dict, Formatting.Indented));


        }

        public RedirectToActionResult Delete(int id)
        {
            Console.WriteLine(id);

            var model = ChatInstance.NodyList.Where((x) => x.Id == id).First();
            var tmp = model;
            ChatInstance.NodyList.Remove(tmp);
            model.Delete();
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            return View();
        }
        public IActionResult SaveNodification(NodyModel model)
        {
            var nodyModel = new NodificationModel
            {
                Name = model.Name,
                Message = model.Message,
                CallAfterLines = model.Lines.ToInt(),
                Active = true
            };
            nodyModel.Save();
            ChatInstance.NodyList.AddIfNotExists(nodyModel);
            return Redirect("/nodifications/index");
        }
       


    }
}
