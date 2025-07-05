using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class ChangeWishlistItemCategoryItemEventController : AbstractItemEventController<ChangeWishlistItemCategoryItemEvent>
{
    private readonly IProfileRepository _profiles;

    public ChangeWishlistItemCategoryItemEventController(IProfileRepository profiles) : base("ChangeWishlistItemCategory")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, ChangeWishlistItemCategoryItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var wishList = profile.Pmc.GetWishList();

        wishList[request.Item] = request.Category;
    }
}