using System;

namespace Fuyu.Common.Backend.Networking;

public class RouteNotFoundException : Exception
{
    public RouteNotFoundException(string message) : base(message)
    {
    }
}