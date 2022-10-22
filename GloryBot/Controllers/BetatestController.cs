using GloryBot.Models.SaveModels;
using Microsoft.AspNetCore.Mvc;
using RequestHandler;
namespace GloryBot.Controllers
{
    public class BetatestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UseKey(BetaKeyModel model)
        {
            // Make Request to node js
            var request = new Request("http://localhost:3030/betakey", "POST");
            var header = new Dictionary<string, dynamic>
            {
                { "Accept", "application/json" }
            };
            var dict = new Dictionary<string, dynamic>
            {
                { "test", "{ \"key\": \"" + model.Key + "\"  }" }
            };
            request.SetHeader(header);
            request.SetContentType("application/json");
            request.SetContent(dict);
            request.Execute();
            return View();
        }
    }
}
