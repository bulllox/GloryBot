using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GloryBot.Models;

public class PluginModel
{
    public string Name { get; set; } = "";
    public string Command { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsActive {get; set;} = false;
    public string ScriptPath {get; set;} = "";
}
