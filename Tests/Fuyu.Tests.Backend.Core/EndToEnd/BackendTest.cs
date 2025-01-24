using Fuyu.Backend.Core;
using Fuyu.Common.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fuyu.Tests.Backend.Core.EndToEnd;

[TestClass]
public class BackendTest
{
    private static HttpClient _coreClient;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        // setup databases
        CoreLoader.Instance.Load();

        // setup server
        _ = new CoreServer();

        // create request clients
        _coreClient = new HttpClient("http://localhost:8000");
    }
}