using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class AddToWishListItemEventController : AbstractItemEventController<AddToWishListItemEvent>
{
    private readonly EftOrm _eftOrm;

    public AddToWishListItemEventController() : base("AddToWishList")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, AddToWishListItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var wishList = profile.Pmc.GetWishList();

        foreach ((var itemId, var wishlistGroup) in request.Items)
        {
            wishList[itemId] = wishlistGroup;
        }

        return Task.CompletedTask;
    }
}