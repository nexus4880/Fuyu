using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class EditNoteItemEventController : AbstractItemEventController<EditNoteItemEvent>
{
    private readonly IProfileRepository _profiles;

    public EditNoteItemEventController(IProfileRepository profiles) : base("EditNote")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, EditNoteItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var notes = profile.Pmc.Notes.Notes;

        if (request.Index < 0 || request.Index > notes.Count)
        {
            context.AppendInventoryError($"Notes index {request.Index} outside bounds of array");

            return;
        }

        notes[request.Index] = request.Note;
    }
}