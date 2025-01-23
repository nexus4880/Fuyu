using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Common;

[DataContract]
public class Config<T> where T : Config<T>
{
    public virtual string FileName { get; }

    public void Load()
    {
        var filepath = $"Fuyu/Configs/{FileName}";

        if (!VFS.Exists(filepath))
        {
            // file doesn't exist, generate it
            Save();
        }

        // read config
        var json = VFS.ReadTextFile(filepath);
        var config = Json.Parse<T>(json);

        // assign values from config
        var instance = (T)this;

        foreach (var pi in typeof(T).GetProperties())
        {
            var found = pi.GetCustomAttributes().Where(x => x.GetType() == typeof(DataMemberAttribute));

            if (found.Any())
            {
                var value = pi.GetValue(config);
                pi.SetValue(instance, value);
            }
        }
    }

    public void Save()
    {
        var instance = (T)this;
        var json = Json.Stringify(instance);
        VFS.WriteTextFile($"Fuyu/Configs/{FileName}", json);
    }
}