using System;
using Microsoft.AspNetCore.Builder;

namespace Fuyu.Backend;

public interface IConfigureKestrel
{
    static void ConfigureApp(WebApplication app)
    {
        throw new NotImplementedException();
    }
}