using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Repositories.Abstractions;

public interface IGameDataRepository
{
    Task<Dictionary<string, string>> GetLanguagesAsync();
    Task<Dictionary<string, string>> GetGlobalLocaleAsync(string languageId);
    Task<MenuLocaleResponse> GetMenuLocaleAsync(string languageId);

    Task<List<CustomizationStorageEntry>> GetCustomizationStorageAsync();
    Task<Dictionary<string, CustomizationTemplate>> GetCustomizationsAsync();
    Task<CustomizationTemplate> GetCustomizationAsync(string voiceId);
    Task<Dictionary<EPlayerSide, Profile>> GetWipeProfilesAsync(string edition);

    Task<BuildsListResponse> GetDefaultBuildsAsync();
    Task<AchievementStatisticResponse> GetAchievementStatisticsAsync();
    Task<HideoutSettingsResponse> GetHideoutSettingsAsync();
    Task<HandbookTemplates> GetHandbookAsync();

    Task<JObject> GetAchievementsAsync();
    Task<JObject> GetGlobalsAsync();
    Task<JArray> GetQuestsAsync();
    Task<JArray> GetTradersAsync();
    Task<JObject> GetWeatherAsync();
    Task<JObject> GetLocalWeatherAsync();
    Task<JObject> GetWorldMapAsync();
    Task<JObject> GetSettingsAsync();
    Task<JObject> GetPrestigeAsync();
    Task<JArray> GetHideoutAreasAsync();
    Task<JObject> GetHideoutProductionRecipesAsync();
    Task<JObject> GetHideoutCustomizationOffersAsync();
    Task<JArray> GetHideoutQtesAsync();

    Task SetDefaultBuildsAsync(BuildsListResponse builds);
    Task SetAchievementStatisticsAsync(AchievementStatisticResponse statistics);
    Task SetHideoutSettingsAsync(HideoutSettingsResponse settings);
    Task SetHandbookAsync(HandbookTemplates handbook);
    Task SetAchievementsAsync(JObject achievements);
    Task SetGlobalsAsync(JObject globals);
    Task SetQuestsAsync(JArray quests);
    Task SetTradersAsync(JArray traders);
    Task SetWeatherAsync(JObject weather);
    Task SetLocalWeatherAsync(JObject weather);
    Task SetWorldMapAsync(JObject worldMap);
    Task SetSettingsAsync(JObject settings);
    Task SetPrestigeAsync(JObject prestige);
    Task SetHideoutAreasAsync(JArray areas);
    Task SetHideoutProductionRecipesAsync(JObject recipes);
    Task SetHideoutCustomizationOffersAsync(JObject offers);
    Task SetHideoutQtesAsync(JArray qtes);
}