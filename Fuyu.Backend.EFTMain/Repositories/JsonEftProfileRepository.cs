using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Backend.Configuration;
using Fuyu.Common.Collections;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fuyu.Backend.EFTMain.Repositories;

public class JsonEftProfileRepository : IProfileRepository
{
    private readonly ThreadList<EftProfile> _profiles;
    private readonly EftConfiguration _config;
    private readonly IAccountRepository _accountRepository;
    private readonly ISessionRepository _sessionRepository;

    public JsonEftProfileRepository(
        ILogger<JsonEftProfileRepository> logger,
        IOptions<EftConfiguration> config,
        IAccountRepository accountRepository,
        ISessionRepository sessionRepository,
        ItemFactoryService itemFactoryService,
        ItemService itemService,
        IItemTemplateRepository itemTemplateRepository
        )
    {
        _config = config.Value;
        _accountRepository = accountRepository;
        _sessionRepository = sessionRepository;
        _profiles = new ThreadList<EftProfile>();

        if (!VFS.DirectoryExists(_config.ProfilesPath))
        {
            VFS.CreateDirectory(_config.ProfilesPath);
        }

        var files = VFS.GetFiles(_config.ProfilesPath);
        foreach (var filepath in files)
        {
            var json = VFS.ReadTextFile(filepath);
            var profile = Json.Parse<EftProfile>(json);
            _profiles.Add(profile);

            if (profile.Pmc.Inventory is not null)
            {
                foreach (var item in profile.Pmc.Inventory.Items)
                {
                    var itemTemplate = itemTemplateRepository.GetItemTemplateAsync(item.TemplateId).GetAwaiter().GetResult();
                    var props = itemFactoryService.GetItemProperties<CompoundItemItemProperties>(itemTemplate);

                    if (props.Grids.Count > 0)
                    {
                        var items = itemService.GetItemAndChildren(profile.Pmc.Inventory.Items, item);
                        item.InitializeMatrices(props.Grids, items);
                    }
                }
            }
        }
    }

    public Task<List<EftProfile>> GetAllAsync()
    {
        return Task.FromResult(_profiles.ToList());
    }

    public async Task<EftProfile> GetByIdAsync(string profileId)
    {
        var profiles = await GetAllAsync();
        return profiles.FirstOrDefault(p => p.Pmc._id == profileId);
    }

    public async Task<EftProfile> GetActiveProfileAsync(string sessionId)
    {
        var accountId = await _sessionRepository.GetAccountIdAsync(sessionId);
        var account = await _accountRepository.GetByIdAsync(accountId);

        string profileId = account.CurrentSession switch
        {
            ESessionMode.Regular => account.PvpId,
            ESessionMode.Pve => account.PveId,
            _ => throw new Exception($"Unhandled session mode: {account.CurrentSession}")
        };

        return await GetByIdAsync(profileId);
    }

    public async Task AddOrUpdateAsync(EftProfile profile)
    {
        var profiles = await GetAllAsync();
        var existingIndex = -1;

        for (var i = 0; i < profiles.Count; i++)
        {
            if (profiles[i].Pmc._id == profile.Pmc._id)
            {
                existingIndex = i;
                break;
            }
        }

        if (existingIndex >= 0)
        {
            _profiles.TrySet(existingIndex, profile);
        }
        else
        {
            _profiles.Add(profile);
        }

        await SaveAsync(profile);
    }

    public async Task RemoveAsync(EftProfile profile)
    {
        var profiles = await GetAllAsync();
        for (var i = 0; i < profiles.Count; i++)
        {
            if (profiles[i].Pmc._id == profile.Pmc._id)
            {
                _profiles.TryRemoveAt(i);
                break;
            }
        }

        // Remove file
        var filepath = $"{_config.ProfilesPath}{profile.Pmc._id}.json";
        if (VFS.Exists(filepath))
        {
            VFS.DeleteFile(filepath);
        }
    }

    public Task SaveAsync(EftProfile profile)
    {
        var json = Json.Stringify(profile);
        var filepath = $"{_config.ProfilesPath}{profile.Pmc._id}.json";
        VFS.WriteTextFile(filepath, json);
        return Task.CompletedTask;
    }
}