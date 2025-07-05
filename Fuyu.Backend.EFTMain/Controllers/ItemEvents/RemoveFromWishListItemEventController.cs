using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RemoveFromWishListItemEventController : AbstractItemEventController<RemoveFromWishListItemEvent>
{
    private readonly IProfileRepository _profiles;

    public RemoveFromWishListItemEventController(IProfileRepository profiles) : base("RemoveFromWishList")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, RemoveFromWishListItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var wishList = profile.Pmc.GetWishList();

        foreach (var itemToRemove in request.Items)
        {
            wishList.Remove(itemToRemove);
        }
    }
}