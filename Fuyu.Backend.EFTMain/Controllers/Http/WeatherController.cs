using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class WeatherController : AbstractEftHttpController
{
    private readonly WeatherService _weatherService;

    public WeatherController() : base("/client/weather")
    {
        _weatherService = WeatherService.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var localWeather = _weatherService.CreateLocalWeather();
        var response = new ResponseBody<WeatherResponse>()
        {
            data = localWeather
        };

        return context.SendResponseAsync(response, true, true);
    }
}