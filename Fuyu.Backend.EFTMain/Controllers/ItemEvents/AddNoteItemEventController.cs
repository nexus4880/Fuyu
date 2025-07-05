using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class AddNoteItemEventController : AbstractItemEventController<AddNoteItemEvent>
{
    private readonly IProfileRepository _profiles;

    public AddNoteItemEventController(IProfileRepository profiles) : base("AddNote")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, AddNoteItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        profile.Pmc.Notes.Notes.Add(request.Note);
    }
}