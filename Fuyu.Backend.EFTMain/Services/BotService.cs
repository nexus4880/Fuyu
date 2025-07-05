using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.Models.Bots;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Services;

// NOTE: regarding how BotService is designed;
// * The goal is to make a very simple bot generator that can easily be
//   replaced by mods
// * Profiles are not stored in EftDatabase because ideally a completely
//   different generation system not depending on this data
// -- seionmoya, 2024-10-21
public class BotService
{
    private readonly Dictionary<EWildSpawnType, string> _profiles;
    private readonly InventoryService _inventoryService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public BotService(InventoryService inventoryService)
    {
        _profiles = new Dictionary<EWildSpawnType, string>()
        {
            { EWildSpawnType.marksman,                    Resx.GetText("eft", "database.bots.marksman.json")                  },
            { EWildSpawnType.assault,                     Resx.GetText("eft", "database.bots.assault.json")                   },
            { EWildSpawnType.bossTest,                    string.Empty                                                        },    // BSG internal
            { EWildSpawnType.bossBully,                   Resx.GetText("eft", "database.bots.bossbully.json")                 },
            { EWildSpawnType.followerTest,                string.Empty                                                        },    // BSG internal
            { EWildSpawnType.followerBully,               Resx.GetText("eft", "database.bots.followerbully.json")             },
            { EWildSpawnType.bossKilla,                   Resx.GetText("eft", "database.bots.bosskilla.json")                 },
            { EWildSpawnType.bossKojaniy,                 Resx.GetText("eft", "database.bots.bosskojaniy.json")               },
            { EWildSpawnType.followerKojaniy,             Resx.GetText("eft", "database.bots.followerkojaniy.json")           },
            { EWildSpawnType.pmcBot,                      Resx.GetText("eft", "database.bots.pmcbot.json")                    },
            { EWildSpawnType.cursedAssault,               string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.bossGluhar,                  Resx.GetText("eft", "database.bots.bossgluhar.json")                },
            { EWildSpawnType.followerGluharAssault,       Resx.GetText("eft", "database.bots.followergluharassault.json")     },
            { EWildSpawnType.followerGluharSecurity,      Resx.GetText("eft", "database.bots.followergluharsecurity.json")    },
            { EWildSpawnType.followerGluharScout,         Resx.GetText("eft", "database.bots.followergluharscout.json")       },
            { EWildSpawnType.followerGluharSnipe,         string.Empty                                                        },    // TODO: missing?
            { EWildSpawnType.followerSanitar,             Resx.GetText("eft", "database.bots.followersanitar.json")           },
            { EWildSpawnType.test,                        string.Empty                                                        },    // BSG internal
            { EWildSpawnType.assaultGroup,                string.Empty                                                        },    // TODO: missing?
            { EWildSpawnType.sectantWarrior,              Resx.GetText("eft", "database.bots.sectantwarrior.json")            },
            { EWildSpawnType.sectantPriest,               Resx.GetText("eft", "database.bots.sectantpriest.json")             },
            { EWildSpawnType.bossTagilla,                 Resx.GetText("eft", "database.bots.bosstagilla.json")               },
            { EWildSpawnType.followerTagilla,             string.Empty                                                        },    // TODO: missing?
            { EWildSpawnType.exUsec,                      Resx.GetText("eft", "database.bots.exusec.json")                    },
            { EWildSpawnType.gifter,                      Resx.GetText("eft", "database.bots.gifter.json")                    },
            { EWildSpawnType.bossKnight,                  Resx.GetText("eft", "database.bots.bossknight.json")                },
            { EWildSpawnType.followerBigPipe,             Resx.GetText("eft", "database.bots.followerbigpipe.json")           },
            { EWildSpawnType.followerBirdEye,             Resx.GetText("eft", "database.bots.followerbirdeye.json")           },
            { EWildSpawnType.bossZryachiy,                Resx.GetText("eft", "database.bots.bosszryachiy.json")              },
            { EWildSpawnType.followerZryachiy,            Resx.GetText("eft", "database.bots.followerzryachiy.json")          },
            { EWildSpawnType.bossBoar,                    Resx.GetText("eft", "database.bots.bossboar.json")                  },
            { EWildSpawnType.followerBoar,                Resx.GetText("eft", "database.bots.followerboar.json")              },
            { EWildSpawnType.arenaFighter,                string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.arenaFighterEvent,           string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.bossBoarSniper,              Resx.GetText("eft", "database.bots.bossboarsniper.json")            },
            { EWildSpawnType.crazyAssaultEvent,           string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.peacefullZryachiyEvent,      string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.sectactPriestEvent,          string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.ravangeZryachiyEvent,        string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.followerBoarClose1,          Resx.GetText("eft", "database.bots.followerboarclose1.json")        },
            { EWildSpawnType.followerBoarClose2,          Resx.GetText("eft", "database.bots.followerboarclose2.json")        },
            { EWildSpawnType.bossKolontay,                Resx.GetText("eft", "database.bots.bosskolontay.json")              },
            { EWildSpawnType.followerKolontayAssault,     Resx.GetText("eft", "database.bots.followerkolontayassault.json")   },
            { EWildSpawnType.followerKolontaySecurity,    Resx.GetText("eft", "database.bots.followerkolontaysecurity.json")  },
            { EWildSpawnType.shooterBTR,                  Resx.GetText("eft", "database.bots.shooterbtr.json")                },
            { EWildSpawnType.bossPartisan,                Resx.GetText("eft", "database.bots.bosspartisan.json")              },
            { EWildSpawnType.spiritWinter,                string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.spiritSpring,                string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.peacemaker,                  string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.pmcBEAR,                     Resx.GetText("eft", "database.bots.pmcbear.json")                   },    // TODO: missing
            { EWildSpawnType.pmcUSEC,                     Resx.GetText("eft", "database.bots.pmcusec.json")                   },    // TODO: missing
            { EWildSpawnType.skier,                       string.Empty                                                        },    // TODO: missing
            { EWildSpawnType.sectantPredvestnik,          Resx.GetText("eft", "database.bots.sectantpredvestnik.json")        },
            { EWildSpawnType.sectantPrizrak,              Resx.GetText("eft", "database.bots.sectantprizrak.json")            },
            { EWildSpawnType.sectantOni,                  Resx.GetText("eft", "database.bots.sectantoni.json")                },
            { EWildSpawnType.infectedAssault,             Resx.GetText("eft", "database.bots.infectedassault.json")           },
            { EWildSpawnType.infectedPmc,                 Resx.GetText("eft", "database.bots.infectedpmc.json")               },
            { EWildSpawnType.infectedCivil,               Resx.GetText("eft", "database.bots.infectedcivil.json")             },
            { EWildSpawnType.infectedLaborant,            Resx.GetText("eft", "database.bots.infectedlaborant.json")          },
            { EWildSpawnType.infectedTagilla,             string.Empty                                                        },    // TODO: missing
        };

        _inventoryService = inventoryService;
    }

    public Profile[] GetBots(BotCondition[] conditions)
    {
        var profiles = new List<Profile>();

        foreach (var condition in conditions)
        {
            // generate amount for profile type
            for (var i = 0; i < condition.Limit; ++i)
            {
                var profile = GenerateBot(condition.Role, condition.Difficulty);
                profiles.Add(profile);
            }
        }

        return profiles.ToArray();
    }

    private Profile GenerateBot(EWildSpawnType role, EBotDifficulty difficulty)
    {
        var profile = Json.Parse<Profile>(_profiles[role]);

        // TODO: remove this when new dumps are acquired
        profile.Prestige = new Dictionary<MongoId, int>();

        // regenerate all ids
        profile._id = MongoId.Generate();
        _inventoryService.RegenerateIds(profile.Inventory);

        // set difficulty
        profile.Info.Settings.BotDifficulty = difficulty;

        return profile;
    }
}