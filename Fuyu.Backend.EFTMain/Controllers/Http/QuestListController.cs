using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class QuestListController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public QuestListController() : base("/client/quest/list")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = _eftOrm.GetQuests();
        var text = response.ToString();
        return context.SendJsonAsync(text, true, true);
    }
}