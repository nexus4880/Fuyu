using System.Collections.Generic;

namespace Fuyu.Common.Backend.Networking;

public class HttpRouter : Router<AbstractHttpController, HttpContext>
{
    public HttpRouter() : base()
    {
    }

    public HttpRouter(IEnumerable<AbstractHttpController> controllers) : base(controllers)
    {
    }
}