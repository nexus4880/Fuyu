using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Repositories;

public class JsonGameDataRepository : IGameDataRepository
{
    private readonly ThreadDictionary<string, string> _languages;
    private readonly ThreadDictionary<string, MenuLocaleResponse> _menuLocales;
    private readonly ThreadDictionary<string, Dictionary<string, string>> _globalLocales;
    private readonly ThreadDictionary<string, CustomizationTemplate> _customizations;
    private readonly ThreadList<CustomizationStorageEntry> _customizationStorage;
    private readonly ThreadDictionary<string, Dictionary<EPlayerSide, Profile>> _wipeProfiles;

    private readonly ThreadObject<AchievementStatisticResponse> _achievementStatistic;
    private readonly ThreadObject<BuildsListResponse> _defaultBuilds;
    private readonly ThreadObject<HideoutSettingsResponse> _hideoutSettings;
    private readonly ThreadObject<JObject> _achievements;
    private readonly ThreadObject<JObject> _globals;
    private readonly ThreadObject<HandbookTemplates> _handbook;
    private readonly ThreadObject<JArray> _hideoutAreas;
    private readonly ThreadObject<JObject> _hideoutCustomizationOffers;
    private readonly ThreadObject<JObject> _hideoutProductionRecipes;
    private readonly ThreadObject<JArray> _hideoutQtes;
    private readonly ThreadObject<JObject> _settings;
    private readonly ThreadObject<JObject> _prestige;
    private readonly ThreadObject<JArray> _quests;
    private readonly ThreadObject<JArray> _traders;
    private readonly ThreadObject<JObject> _weather;
    private readonly ThreadObject<JObject> _worldMap;

    public JsonGameDataRepository()
    {
        _languages = new ThreadDictionary<string, string>();
        _menuLocales = new ThreadDictionary<string, MenuLocaleResponse>();
        _globalLocales = new ThreadDictionary<string, Dictionary<string, string>>();
        _customizations = new ThreadDictionary<string, CustomizationTemplate>();
        _customizationStorage = new ThreadList<CustomizationStorageEntry>();
        _wipeProfiles = new ThreadDictionary<string, Dictionary<EPlayerSide, Profile>>();
        _achievementStatistic = new ThreadObject<AchievementStatisticResponse>(null);
        _defaultBuilds = new ThreadObject<BuildsListResponse>(null);
        _hideoutSettings = new ThreadObject<HideoutSettingsResponse>(null);
        _achievements = new ThreadObject<JObject>(null);
        _globals = new ThreadObject<JObject>(null);
        _handbook = new ThreadObject<HandbookTemplates>(null);
        _hideoutAreas = new ThreadObject<JArray>(null);
        _hideoutCustomizationOffers = new ThreadObject<JObject>(null);
        _hideoutProductionRecipes = new ThreadObject<JObject>(null);
        _hideoutQtes = new ThreadObject<JArray>(null);
        _settings = new ThreadObject<JObject>(null);
        _prestige = new ThreadObject<JObject>(null);
        _quests = new ThreadObject<JArray>(null);
        _traders = new ThreadObject<JArray>(null);
        _weather = new ThreadObject<JObject>(null);
        _worldMap = new ThreadObject<JObject>(null);

        // load locales
        LoadLanguages();
        LoadGlobalLocales();
        LoadMenuLocales();

        // load templates
        LoadCustomizations();
        LoadCustomizationStorage();
        LoadDefaultBuilds();
        LoadWipeProfiles();
        LoadWorldMap();
        LoadHideoutSettings();
        LoadAchievementStatistics();

        // JOBJECT
        LoadAchievements();
        LoadGlobals();
        LoadHandbook();
        LoadHideoutAreas();
        LoadHideoutCustomizationOffers();
        LoadHideoutProductionRecipes();
        LoadHideoutQtes();
        LoadLocalWeather();
        LoadPrestige();
        LoadQuests();
        LoadSettings();
        LoadTraders();
        LoadWeather();
    }

    public Task<JObject> GetAchievementsAsync()
    {
        return Task.FromResult(_achievements.Get());
    }

    public Task<AchievementStatisticResponse> GetAchievementStatisticsAsync()
    {
        return Task.FromResult(_achievementStatistic.Get());
    }

    public Task<BuildsListResponse> GetDefaultBuildsAsync()
    {
        return Task.FromResult(_defaultBuilds.Get());
    }

    public Task<JObject> GetGlobalsAsync()
    {
        return Task.FromResult(_globals.Get());
    }

    public Task<HandbookTemplates> GetHandbookAsync()
    {
        return Task.FromResult(_handbook.Get());
    }

    public Task<JArray> GetHideoutAreasAsync()
    {
        return Task.FromResult(_hideoutAreas.Get());
    }

    public Task<JObject> GetHideoutCustomizationOffersAsync()
    {
        return Task.FromResult(_hideoutCustomizationOffers.Get());
    }

    public Task<JObject> GetHideoutProductionRecipesAsync()
    {
        return Task.FromResult(_hideoutProductionRecipes.Get());
    }

    public Task<JArray> GetHideoutQtesAsync()
    {
        return Task.FromResult(_hideoutQtes.Get());
    }

    public Task<HideoutSettingsResponse> GetHideoutSettingsAsync()
    {
        return Task.FromResult(_hideoutSettings.Get());
    }

    public Task<JObject> GetLocalWeatherAsync()
    {
        return Task.FromResult(_weather.Get());
    }

    public Task<JObject> GetPrestigeAsync()
    {
        return Task.FromResult(_prestige.Get());
    }

    public Task<JArray> GetQuestsAsync()
    {
        return Task.FromResult(_quests.Get());
    }

    public Task<JObject> GetSettingsAsync()
    {
        return Task.FromResult(_settings.Get());
    }

    public Task<JArray> GetTradersAsync()
    {
        return Task.FromResult(_traders.Get());
    }

    public Task<JObject> GetWeatherAsync()
    {
        return Task.FromResult(_weather.Get());
    }

    public Task<JObject> GetWorldMapAsync()
    {
        return Task.FromResult(_worldMap.Get());
    }

    public Task SetAchievementsAsync(JObject achievements)
    {
        _achievements.Set(achievements);
        return Task.CompletedTask;
    }

    public Task SetAchievementStatisticsAsync(AchievementStatisticResponse statistics)
    {
        _achievementStatistic.Set(statistics);
        return Task.CompletedTask;
    }

    public Task SetDefaultBuildsAsync(BuildsListResponse builds)
    {
        _defaultBuilds.Set(builds);
        return Task.CompletedTask;
    }

    public Task SetGlobalsAsync(JObject globals)
    {
        _globals.Set(globals);
        return Task.CompletedTask;
    }

    public Task SetHandbookAsync(HandbookTemplates handbook)
    {
        _handbook.Set(handbook);
        return Task.CompletedTask;
    }

    public Task SetHideoutAreasAsync(JArray areas)
    {
        _hideoutAreas.Set(areas);
        return Task.CompletedTask;
    }

    public Task SetHideoutCustomizationOffersAsync(JObject offers)
    {
        _hideoutCustomizationOffers.Set(offers);
        return Task.CompletedTask;
    }

    public Task SetHideoutProductionRecipesAsync(JObject recipes)
    {
        _hideoutProductionRecipes.Set(recipes);
        return Task.CompletedTask;
    }

    public Task SetHideoutQtesAsync(JArray qtes)
    {
        _hideoutQtes.Set(qtes);
        return Task.CompletedTask;
    }

    public Task SetHideoutSettingsAsync(HideoutSettingsResponse settings)
    {
        _hideoutSettings.Set(settings);
        return Task.CompletedTask;
    }

    public Task SetLocalWeatherAsync(JObject weather)
    {
        _weather.Set(weather);
        return Task.CompletedTask;
    }

    public Task SetPrestigeAsync(JObject prestige)
    {
        _prestige.Set(prestige);
        return Task.CompletedTask;
    }

    public Task SetQuestsAsync(JArray quests)
    {
        _quests.Set(quests);
        return Task.CompletedTask;
    }

    public Task SetSettingsAsync(JObject settings)
    {
        _settings.Set(settings);
        return Task.CompletedTask;
    }

    public Task SetTradersAsync(JArray traders)
    {
        _traders.Set(traders);
        return Task.CompletedTask;
    }

    public Task SetWeatherAsync(JObject weather)
    {
        _weather.Set(weather);
        return Task.CompletedTask;
    }

    public Task SetWorldMapAsync(JObject worldMap)
    {
        _worldMap.Set(worldMap);
        return Task.CompletedTask;
    }

    private void LoadCustomizations()
    {
        var json = Resx.GetText("eft", "database.client.customization.json");
        var response = Json.Parse<Dictionary<string, CustomizationTemplate>>(json);

        foreach (var kvp in response)
        {
            SetOrAddCustomization(kvp.Key, kvp.Value);
        }
    }

    private void LoadCustomizationStorage()
    {
        var json = Resx.GetText("eft", "database.client.customization.storage.json");
        var response = Json.Parse<CustomizationStorageEntry[]>(json);

        foreach (var entry in response)
        {
            SetOrAddCustomizationStorage(entry);
        }
    }

    private void LoadLanguages()
    {
        var json = Resx.GetText("eft", $"database.locales.client.languages.json");
        var response = Json.Parse<Dictionary<string, string>>(json);

        foreach (var kvp in response)
        {
            SetOrAddLanguage(kvp.Key, kvp.Value);
        }
    }

    private void LoadGlobalLocales()
    {
        foreach (var (languageId, _) in _languages)
        {
            var json = Resx.GetText("eft", $"database.locales.client.locale-{languageId}.json");
            var response = Json.Parse<Dictionary<string, string>>(json);
            SetOrAddGlobalLocale(languageId, response);
        }
    }

    private void LoadMenuLocales()
    {
        foreach (var (languageId, _) in _languages)
        {
            var json = Resx.GetText("eft", $"database.locales.client.menu.locale-{languageId}.json");
            var response = Json.Parse<MenuLocaleResponse>(json);

            SetOrAddMenuLocale(languageId, response);
        }
    }

    private void LoadDefaultBuilds()
    {
        var json = Resx.GetText("eft", "database.client.builds.list.json");
        var response = Json.Parse<BuildsListResponse>(json);
        _defaultBuilds.Set(response);
    }

    private void LoadWipeProfiles()
    {
        // profile
        var bearJson = Resx.GetText("eft", "database.profiles.player.unheard-bear.json");
        var usecJson = Resx.GetText("eft", "database.profiles.player.unheard-usec.json");
        var savageJson = Resx.GetText("eft", "database.profiles.player.savage.json");

        SetOrAddWipeProfile("unheard", new Dictionary<EPlayerSide, Profile>()
        {
            { EPlayerSide.Bear, Json.Parse<Profile>(bearJson) },
            { EPlayerSide.Usec, Json.Parse<Profile>(usecJson) },
            { EPlayerSide.Savage, Json.Parse<Profile>(savageJson) }
        });
    }

    private void LoadAchievementStatistics()
    {
        var json = Resx.GetText("eft", "database.client.achievement.statistic.json");
        var statistics = Json.Parse<AchievementStatisticResponse>(json);
        _achievementStatistic.Set(statistics);
    }

    // TODO: parse from model
    // -- seionmoya, 2024-01-09
    private void LoadWorldMap()
    {
        var json = Resx.GetText("eft", "database.client.locations.json");
        //var worldmap = Json.Parse<WorldMap>(json);
        var worldmap = JObject.Parse(json);
        _worldMap.Set(worldmap);
    }

    private void LoadHideoutSettings()
    {
        var json = Resx.GetText("eft", "database.client.hideout.settings.json");
        var hideoutSettings = Json.Parse<HideoutSettingsResponse>(json);
        _hideoutSettings.Set(hideoutSettings);
    }

    private void LoadAchievements()
    {
        var json = Resx.GetText("eft", "database.client.achievement.list.json");
        var achievements = JObject.Parse(json);
        _achievements.Set(achievements);
    }

    private void LoadGlobals()
    {
        var json = Resx.GetText("eft", "database.client.globals.json");
        var globals = JObject.Parse(json);
        _globals.Set(globals);
    }

    private void LoadHandbook()
    {
        var json = Resx.GetText("eft", "database.client.handbook.templates.json");
        var handbook = Json.Parse<HandbookTemplates>(json);
        _handbook.Set(handbook);
    }

    private void LoadHideoutAreas()
    {
        var json = Resx.GetText("eft", "database.client.hideout.areas.json");
        var areas = JArray.Parse(json);
        _hideoutAreas.Set(areas);
    }

    private void LoadHideoutCustomizationOffers()
    {
        var json = Resx.GetText("eft", "database.client.hideout.customization.offer.list.json");
        var offers = JObject.Parse(json);
        _hideoutCustomizationOffers.Set(offers);
    }

    private void LoadHideoutProductionRecipes()
    {
        var json = Resx.GetText("eft", "database.client.hideout.production.recipes.json");
        var recipes = JObject.Parse(json);
        _hideoutProductionRecipes.Set(recipes);
    }

    private void LoadHideoutQtes()
    {
        var json = Resx.GetText("eft", "database.client.hideout.qte.list.json");
        var qtes = JArray.Parse(json);
        _hideoutQtes.Set(qtes);
    }

    private void LoadLocalWeather()
    {
        var json = Resx.GetText("eft", "database.client.localGame.weather.json");
        var weather = JObject.Parse(json);
        _weather.Set(weather);
    }

    private void LoadPrestige()
    {
        var json = Resx.GetText("eft", "database.client.prestige.list.json");
        var prestige = JObject.Parse(json);
        _prestige.Set(prestige);
    }

    private void LoadQuests()
    {
        var json = Resx.GetText("eft", "database.client.quest.list.json");
        var quests = JArray.Parse(json);
        _quests.Set(quests);
    }

    private void LoadSettings()
    {
        var json = Resx.GetText("eft", "database.client.settings.json");
        var settings = JObject.Parse(json);
        _settings.Set(settings);
    }

    private void LoadTraders()
    {
        var json = Resx.GetText("eft", "database.client.trading.api.traderSettings.json");
        var traders = JArray.Parse(json);
        _traders.Set(traders);
    }

    private void LoadWeather()
    {
        var json = Resx.GetText("eft", "database.client.weather.json");
        var weather = JObject.Parse(json);
        _weather.Set(weather);
    }

    public void SetOrAddWipeProfile(string edition, Dictionary<EPlayerSide, Profile> profiles)
    {
        if (_wipeProfiles.ContainsKey(edition))
        {
            _wipeProfiles.Set(edition, profiles);
        }
        else
        {
            _wipeProfiles.Set(edition, profiles);
        }
    }

    public void SetOrAddCustomizationStorage(CustomizationStorageEntry entry)
    {
        for (var i = 0; i < _customizationStorage.Count && _customizationStorage.TryGet(i, out var thisEntry); ++i)
        {
            if (thisEntry.Id == entry.Id)
            {
                _customizationStorage.TrySet(i, entry);
                return;
            }
        }

        _customizationStorage.Add(entry);
    }

    public void SetOrAddMenuLocale(string languageId, MenuLocaleResponse menuLocale)
    {
        if (_menuLocales.ContainsKey(languageId))
        {
            _menuLocales.Set(languageId, menuLocale);
        }
        else
        {
            _menuLocales.Set(languageId, menuLocale);
        }
    }

    public void SetOrAddGlobalLocale(string languageId, Dictionary<string, string> globalLocale)
    {
        if (_globalLocales.ContainsKey(languageId))
        {
            _globalLocales.Set(languageId, globalLocale);
        }
        else
        {
            _globalLocales.Set(languageId, globalLocale);
        }
    }

    public void SetOrAddCustomization(string customizationId, CustomizationTemplate template)
    {
        if (_customizations.ContainsKey(customizationId))
        {
            _customizations.Set(customizationId, template);
        }
        else
        {
            _customizations.Set(customizationId, template);
        }
    }

    public void SetOrAddLanguage(string languageId, string name)
    {
        if (_languages.ContainsKey(languageId))
        {
            _languages.Set(languageId, name);
        }
        else
        {
            _languages.Set(languageId, name);
        }
    }

    public Task<Dictionary<string, string>> GetLanguagesAsync()
    {
        return Task.FromResult(_languages.ToDictionary());
    }

    public Task<Dictionary<string, Dictionary<string, string>>> GetGlobalLocaleAsync(string languageId)
    {
        return Task.FromResult(_globalLocales.ToDictionary());
    }

    public Task<Dictionary<string, MenuLocaleResponse>> GetMenuLocaleAsync(string languageId)
    {
        return Task.FromResult(_menuLocales.ToDictionary());
    }

    public Task<List<CustomizationStorageEntry>> GetCustomizationStorageAsync()
    {
        return Task.FromResult(_customizationStorage.ToList());
    }

    public Task<Dictionary<string, CustomizationTemplate>> GetCustomizationsAsync()
    {
        return Task.FromResult(_customizations.ToDictionary());
    }

    public Task<CustomizationTemplate> GetCustomizationAsync(string voiceId)
    {
        if (!_customizations.TryGet(voiceId, out var customizationTemplate))
        {
            throw new Exception($"Failed to get customization {voiceId}");
        }

        return Task.FromResult(customizationTemplate);
    }

    public Task<Dictionary<EPlayerSide, Profile>> GetWipeProfilesAsync(string edition)
    {
        if (!_wipeProfiles.TryGet(edition, out var profiles))
        {
            throw new Exception($"Failed to get profiles for edition {edition}");
        }

        return Task.FromResult(profiles);
    }

    Task<Dictionary<string, string>> IGameDataRepository.GetGlobalLocaleAsync(string languageId)
    {
        if (_globalLocales.TryGet(languageId, out var globalLocale))
        {
            return Task.FromResult(globalLocale);
        }

        throw new Exception($"Faild to get global locale {languageId}");
    }

    Task<MenuLocaleResponse> IGameDataRepository.GetMenuLocaleAsync(string languageId)
    {
        if (_menuLocales.TryGet(languageId, out var menuLocale))
        {
            return Task.FromResult(menuLocale);
        }

        throw new Exception($"Failed to get menu locale {languageId}");
    }
}