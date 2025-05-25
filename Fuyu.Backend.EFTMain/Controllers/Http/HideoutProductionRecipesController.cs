using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutProductionRecipesController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public HideoutProductionRecipesController() : base("/client/hideout/production/recipes")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = _eftOrm.GetHideoutProductionRecipes();
        var text = response.ToString();
        return context.SendJsonAsync(text, true, true);
    }
}