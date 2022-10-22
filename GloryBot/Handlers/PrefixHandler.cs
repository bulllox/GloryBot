namespace GloryBot.Handlers;

public class PrefixHandler {
    
    private Dictionary<string, dynamic> PrefixList = new();

    // public static void RegisterPrefix(string PrefixName, Action callback) {
    //     PrefixList.Add(PrefixName, callback);
    // }
    
    // public static void ReplacePrefix(string prefix) {
    //     foreach(var prefixName in PrefixList.Keys) {
    //         if(prefix == prefixName) {
    //             PrefixList[prefixName].Invoke();
    //         }
    //     }
    // }
    public void RegisterPrefix(string PrefixName, string PrefixValue) {
        PrefixList.AddIfNotExists(PrefixName, PrefixValue);
    }
    
    public string HandlePrefix(string item, string username = "")
    {
        if(PrefixList.ContainsKey(item)) {
            return PrefixList[item];
        }
        return "";
    }

    internal void RegisterPrefixAction(string key, Func<string> func)
    {
        PrefixList.AddIfNotExists(key, func.Invoke());
    }

    internal void RegisterPrefixAction(string key, Func<string, string> func)
    {
        throw new NotImplementedException();
    }

    internal void RegisterPrefixAction(string key, Func<int> func)
    {
        PrefixList.AddIfNotExists(key, func.Invoke());
    }
}