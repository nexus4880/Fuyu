using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutCustomizationOfferListController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public HideoutCustomizationOfferListController() : base("/client/hideout/customization/offer/list")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2025-01-04
        var response = _eftOrm.GetHideoutCustomizationOffers();
        var text = response.ToString();
        return context.SendJsonAsync(text, true, true);
    }
}