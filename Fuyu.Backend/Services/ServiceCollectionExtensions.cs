using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.Core;
using Fuyu.Backend.Core.Controllers;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.EFT.Controllers.Http;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Controllers.Http;
using Fuyu.Backend.EFTMain.Controllers.ItemEvents;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.Security;
using Fuyu.Common.Backend.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Backend.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFuyuServices(this IServiceCollection services)
    {
        services.AddFuyuInfrastructure();
        services.AddFuyuServers();
        services.AddCoreHttpControllers();
        services.AddEftHttpControllers();
        services.AddItemEventControllers();

        return services;
    }

    private static IServiceCollection AddFuyuInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICertificateService, CertificateService>();
        services.AddHostedService<ModManagerService>();
        services.AddHostedService<DatabaseService>();
        services.AddHostedService<FuyuBackgroundService>();

        return services;
    }

    // Default servers
    // CoreServer (Launcher): 44300
    // EftMainServer (Game server): 44301
    private static IServiceCollection AddFuyuServers(this IServiceCollection services)
    {
        services.AddSingleton<FuyuServer, CoreServer>();
        services.AddSingleton<FuyuServer, EftMainServer>();

        return services;
    }

    /// These get used by <see cref="CoreServer"/>
    private static IServiceCollection AddCoreHttpControllers(this IServiceCollection services)
    {
        services.AddSingleton<AbstractCoreHttpController, PingController>();
        services.AddSingleton<AbstractCoreHttpController, AccountGameRegisterController>();
        services.AddSingleton<AbstractCoreHttpController, AccountGetController>();
        services.AddSingleton<AbstractCoreHttpController, AccountLoginController>();
        services.AddSingleton<AbstractCoreHttpController, AccountLogoutController>();
        services.AddSingleton<AbstractCoreHttpController, AccountRegisterController>();

        return services;
    }

    /// These get used by <see cref="EftMainServer"/>
    private static IServiceCollection AddEftHttpControllers(this IServiceCollection services)
    {
        // Internal controllers
        services.AddSingleton<AbstractEftHttpController, FuyuGameLoginController>();
        services.AddSingleton<AbstractEftHttpController, FuyuGameRegisterController>();

        // Game controllers
        services.AddSingleton<AbstractEftHttpController, AchievementListController>();
        services.AddSingleton<AbstractEftHttpController, AchievementStatisticController>();
        services.AddSingleton<AbstractEftHttpController, BuildsListController>();
        services.AddSingleton<AbstractEftHttpController, CheckVersionController>();
        services.AddSingleton<AbstractEftHttpController, CustomizationController>();
        services.AddSingleton<AbstractEftHttpController, CustomizationStorageController>();
        services.AddSingleton<AbstractEftHttpController, FriendListController>();
        services.AddSingleton<AbstractEftHttpController, FriendRequestListInboxController>();
        services.AddSingleton<AbstractEftHttpController, FriendRequestListOutboxController>();
        services.AddSingleton<AbstractEftHttpController, GameBotGenerateController>();
        services.AddSingleton<AbstractEftHttpController, GameKeepaliveController>();
        services.AddSingleton<AbstractEftHttpController, GameConfigController>();
        services.AddSingleton<AbstractEftHttpController, GameLogoutController>();
        services.AddSingleton<AbstractEftHttpController, GameModeController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileCreateController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileItemsMovingController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileListController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileNicknameReservedController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileNicknameValidateController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileSelectController>();
        services.AddSingleton<AbstractEftHttpController, GameStartController>();
        services.AddSingleton<AbstractEftHttpController, GameVersionValidateController>();
        services.AddSingleton<AbstractEftHttpController, GetMetricsConfigController>();
        services.AddSingleton<AbstractEftHttpController, PutHWMetricsController>();
        services.AddSingleton<AbstractEftHttpController, GlobalsController>();
        services.AddSingleton<AbstractEftHttpController, HandbookTemplatesController>();
        services.AddSingleton<AbstractEftHttpController, HideoutAreasController>();
        services.AddSingleton<AbstractEftHttpController, HideoutCustomizationOfferListController>();
        services.AddSingleton<AbstractEftHttpController, HideoutProductionRecipesController>();
        services.AddSingleton<AbstractEftHttpController, HideoutQteListController>();
        services.AddSingleton<AbstractEftHttpController, HideoutSettingsController>();
        services.AddSingleton<AbstractEftHttpController, ItemsController>();
        services.AddSingleton<AbstractEftHttpController, LanguagesController>();
        services.AddSingleton<AbstractEftHttpController, LocaleController>();
        services.AddSingleton<AbstractEftHttpController, LocalGameWeatherController>();
        services.AddSingleton<AbstractEftHttpController, LocationsController>();
        services.AddSingleton<AbstractEftHttpController, MailDialogListController>();
        services.AddSingleton<AbstractEftHttpController, MatchGroupCurrentController>();
        services.AddSingleton<AbstractEftHttpController, MatchGroupExitFromMenuController>();
        services.AddSingleton<AbstractEftHttpController, MatchGroupInviteCancelAllController>();
        services.AddSingleton<AbstractEftHttpController, MatchLocalEndController>();
        services.AddSingleton<AbstractEftHttpController, MatchLocalStartController>();
        services.AddSingleton<AbstractEftHttpController, MenuLocaleController>();
        services.AddSingleton<AbstractEftHttpController, NotifierChannelCreateController>();
        services.AddSingleton<AbstractEftHttpController, ProfileSettingsController>();
        services.AddSingleton<AbstractEftHttpController, ProfileStatusController>();
        services.AddSingleton<AbstractEftHttpController, PutMetricsController>();
        services.AddSingleton<AbstractEftHttpController, PrestigeListController>();
        services.AddSingleton<AbstractEftHttpController, QuestListController>();
        services.AddSingleton<AbstractEftHttpController, RaidConfigurationController>();
        services.AddSingleton<AbstractEftHttpController, RepeatableQuestActivityPeriodsController>();
        services.AddSingleton<AbstractEftHttpController, ServerListController>();
        services.AddSingleton<AbstractEftHttpController, SettingsController>();
        services.AddSingleton<AbstractEftHttpController, ClientSurveyViewController>();
        services.AddSingleton<AbstractEftHttpController, ClientSurveyOpinionController>();
        services.AddSingleton<AbstractEftHttpController, SurveyController>();
        services.AddSingleton<AbstractEftHttpController, TraderSettingsController>();
        services.AddSingleton<AbstractEftHttpController, WeatherController>();
        services.AddSingleton<AbstractEftHttpController, FilesController>();
        services.AddSingleton<AbstractEftHttpController, ClientItemsPriceController>();
        services.AddSingleton<AbstractEftHttpController, GetTraderAssortController>();
        services.AddSingleton<AbstractEftHttpController, ClientInsuranceItemsListCostController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileVoiceChangeController>();
        services.AddSingleton<AbstractEftHttpController, ProfileMagazineBuildSaveController>();
        services.AddSingleton<AbstractEftHttpController, ProfileBuildDeleteController>();
        services.AddSingleton<AbstractEftHttpController, ProfileEquipmentBuildSaveController>();
        services.AddSingleton<AbstractEftHttpController, ProfileWeaponBuildSaveController>();
        services.AddSingleton<AbstractEftHttpController, GameProfileNicknameChangeController>();
        services.AddSingleton<AbstractEftHttpController, GetOtherProfileController>();
        services.AddSingleton<AbstractEftHttpController, SearchOtherProfileController>();
        services.AddSingleton<AbstractEftHttpController, ClientRagfairFindController>();
        services.AddSingleton<AbstractEftHttpController, ClientRagfairItemMarketPriceController>();
        services.AddSingleton<AbstractEftHttpController, ClientMatchingAvailableController>();

        return services;
    }

    /// These get used by <see cref="GameProfileItemsMovingController"/>
    private static IServiceCollection AddItemEventControllers(this IServiceCollection services)
    {
        services.AddSingleton<IItemEventController, CustomizationBuyEventController>();
        services.AddSingleton<IItemEventController, EatItemEventController>();
        services.AddSingleton<IItemEventController, InsureEventController>();
        services.AddSingleton<IItemEventController, InterGameTransferEventController>();
        services.AddSingleton<IItemEventController, MoveItemEventController>();
        services.AddSingleton<IItemEventController, ReadEncyclopediaEventController>();
        services.AddSingleton<IItemEventController, SellAllFromSavageEventController>();
        services.AddSingleton<IItemEventController, TraderRepairEventController>();
        services.AddSingleton<IItemEventController, TradingConfirmEventController>();
        services.AddSingleton<IItemEventController, ApplyInventoryChangesItemEventController>();
        services.AddSingleton<IItemEventController, RemoveItemEventController>();
        services.AddSingleton<IItemEventController, FoldItemEventController>();
        services.AddSingleton<IItemEventController, BindItemEventController>();
        services.AddSingleton<IItemEventController, UnbindItemEventController>();
        services.AddSingleton<IItemEventController, AddToWishListItemEventController>();
        services.AddSingleton<IItemEventController, RemoveFromWishListItemEventController>();
        services.AddSingleton<IItemEventController, ChangeWishlistItemCategoryItemEventController>();
        services.AddSingleton<IItemEventController, AddNoteItemEventController>();
        services.AddSingleton<IItemEventController, EditNoteItemEventController>();
        services.AddSingleton<IItemEventController, DeleteNoteItemEventController>();
        services.AddSingleton<IItemEventController, ExamineItemEventController>();
        services.AddSingleton<IItemEventController, RecodeItemEventController>();
        services.AddSingleton<IItemEventController, TagItemEventController>();
        services.AddSingleton<IItemEventController, ToggleItemEventController>();
        services.AddSingleton<IItemEventController, RepairItemEventController>();
        services.AddSingleton<IItemEventController, RagFairBuyOfferItemEventController>();
        services.AddSingleton<IItemEventController, RagFairAddOfferItemEventController>();
        services.AddSingleton<IItemEventController, RagFairRemoveOfferItemEventController>();
        services.AddSingleton<IItemEventController, RagFairRenewOfferController>();
        services.AddSingleton<IItemEventController, TransferItemEventController>();
        services.AddSingleton<IItemEventController, MergeItemEventController>();
        services.AddSingleton<IItemEventController, SplitItemEventController>();
        services.AddSingleton<IItemEventController, PinLockItemEventController>();

        return services;
    }
}