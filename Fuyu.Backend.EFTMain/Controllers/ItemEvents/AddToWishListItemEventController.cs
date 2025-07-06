using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class AddToWishListItemEventController : AbstractItemEventController<AddToWishListItemEvent>
{
    private readonly IProfileRepository _profiles;

    public AddToWishListItemEventController(IProfileRepository profiles) : base("AddToWishList")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, AddToWishListItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var wishList = profile.Pmc.GetWishList();

        foreach ((var itemId, var wishlistGroup) in request.Items)
        {
            wishList[itemId] = wishlistGroup;
        }
    }
}