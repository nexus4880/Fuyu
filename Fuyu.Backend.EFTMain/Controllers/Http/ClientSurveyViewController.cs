using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientSurveyViewController : AbstractEftHttpController<ClientSurveyViewRequest>
{
    private readonly IProfileRepository _profiles;
    private readonly IAccountRepository _accounts;

    public ClientSurveyViewController(IProfileRepository profiles, IAccountRepository accounts) : base("/client/survey/view")
    {
        _profiles = profiles;
        _accounts = accounts;
    }

    public override async Task RunAsync(EftHttpContext context, ClientSurveyViewRequest body)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var account = await _accounts.GetByIdAsync(profile.Pmc.aid);
        Terminal.WriteLine($"{account.Username} has viewed survey {body.SurveyId}");
    }
}