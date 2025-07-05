using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class ExamineItemEventController : AbstractItemEventController<ExamineItemEvent>
{
    private readonly IProfileRepository _profiles;

    public ExamineItemEventController(IProfileRepository profiles) : base("Examine")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, ExamineItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        profile.Pmc.Encyclopedia[request.TemplateId] = true;
    }
}