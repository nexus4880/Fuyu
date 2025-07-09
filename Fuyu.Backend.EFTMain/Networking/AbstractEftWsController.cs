using System.Text.RegularExpressions;
using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.EFTMain.Networking;

public abstract class AbstractEftWsController : WsController
{
    public AbstractEftWsController(Regex path) : base(path)
    {
    }

    public AbstractEftWsController(string path) : base(path)
    {
    }
}
