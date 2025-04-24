using System.Text.RegularExpressions;

namespace Fuyu.Common.Backend.Networking;

public interface IRoutable
{
    Regex Matcher { get; }
}