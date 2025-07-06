using System;
using System.Collections.Generic;
using Fuyu.Common.IO;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Services;

public class LocationService
{
    private readonly Dictionary<string, JObject> _locationLoot;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public LocationService()
    {
        _locationLoot = new Dictionary<string, JObject>()
        {
            { "bigmap",         JObject.Parse(Resx.GetText("eft", "database.locations.bigmap.json"))          },
            { "factory4_day",   JObject.Parse(Resx.GetText("eft", "database.locations.factory4_day.json"))    },
            { "factory4_night", JObject.Parse(Resx.GetText("eft", "database.locations.factory4_night.json"))  },
            { "interchange",    JObject.Parse(Resx.GetText("eft", "database.locations.interchange.json"))     },
            { "laboratory",     JObject.Parse(Resx.GetText("eft", "database.locations.laboratory.json"))      },
            { "lighthouse",     JObject.Parse(Resx.GetText("eft", "database.locations.lighthouse.json"))      },
            { "rezervbase",     JObject.Parse(Resx.GetText("eft", "database.locations.rezervbase.json"))      },
            { "sandbox",        JObject.Parse(Resx.GetText("eft", "database.locations.sandbox.json"))         },
            { "shoreline",      JObject.Parse(Resx.GetText("eft", "database.locations.shoreline.json"))       },
            { "tarkovstreets",  JObject.Parse(Resx.GetText("eft", "database.locations.tarkovstreets.json"))   },
            { "woods",          JObject.Parse(Resx.GetText("eft", "database.locations.woods.json"))           }
        };
    }

    // TODO: generate this
    // --seionmoya, 2024-11-18
    public JObject GetLoot(string location)
    {
        return _locationLoot[location];
    }
}