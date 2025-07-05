using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class DeleteNoteItemEventController : AbstractItemEventController<DeleteNoteItemEvent>
{
    private readonly IProfileRepository _profiles;

    public DeleteNoteItemEventController(IProfileRepository profiles) : base("DeleteNote")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, DeleteNoteItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var notes = profile.Pmc.Notes.Notes;

        if (request.Index < 0 || request.Index > notes.Count)
        {
            context.AppendInventoryError($"Notes index {request.Index} outside bounds of array");

            return;
        }

        notes.RemoveAt(request.Index);
    }
}