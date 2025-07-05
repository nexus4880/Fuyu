using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Services;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemInstance
{
    [DataMember(Name = "_id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "_tpl")]
    public MongoId TemplateId { get; set; }

    // emits when 'null'
    // NOTE: This is a string instead of a MongoId as "hideout" is valid
    [DataMember(Name = "parentId", EmitDefaultValue = false)]
    public string ParentId { get; set; }

    // emits when 'null'
    [DataMember(Name = "slotId", EmitDefaultValue = false)]
    public string SlotId { get; set; }

    // emits when 'null'
    [DataMember(Name = "location", EmitDefaultValue = false)]
    [UnionMappings(JTokenType.Object, JTokenType.Integer)]
    public Union<LocationInGrid, int> Location { get; set; }

    // emits when 'null'
    [DataMember(Name = "upd", EmitDefaultValue = false)]
    public ItemUpdatable Updatable { get; set; }

    /// <summary>
    /// Do not access directly, use <see cref="ItemService.CalculateItemSize(System.Collections.Generic.List{ItemInstance})"/>
    /// </summary>
    public ValueTuple<int, int>? Size { get; set; }

    public async Task<ItemUpdatable> GetOrCreateUpdatableAsync(ItemFactoryService itemFactoryService)
    {
        if (Updatable == null)
        {
            Updatable = await itemFactoryService.CreateItemUpdatableAsync(TemplateId);
            if (Updatable == null)
            {
                Updatable = new ItemUpdatable();
            }
        }

        return Updatable;
    }

    public async Task<T> GetOrCreateUpdatableAsync<T>(ItemFactoryService itemFactoryService) where T : class
    {
        await GetOrCreateUpdatableAsync(itemFactoryService);

        // NOTE: Intentionally letting this throw here. The idea is that GetOrCreateUpdatable should
        // create T if it doesn't exist meaning most usage would be GetOrCreateUpdatable<Upd>().Value
        // which means a null check after calling this would be undesirable
        // -- nexus4880, 2024-10-27
        var field = Updatable.GetType()
            .GetProperties()
            .First(f => f.PropertyType == typeof(T));

        var value = field.GetValue(Updatable) as T;

        return value;
    }

    public void InitializeMatrices(IList<Grid> grids, IList<ItemInstance> children)
    {
        Matrices = new MatricesClass(this, grids, children);
    }

    public MatricesClass Matrices { get; private set; }
}

// TODO: figure this out at a later date
// -- nexus4880, 2025-3-10
public class MatricesClass
{
    private readonly ItemInstance _owner;
    private readonly IList<Grid> _grids;
    private readonly IList<ItemInstance> _children;
    private readonly Dictionary<string, bool[,]> _cachedMatrices = [];

    public MatricesClass(ItemInstance owner, IList<Grid> grids, IList<ItemInstance> children)
    {
        _owner = owner;
        _grids = grids;
        _children = children;
    }

    public async Task<bool[,]> GetMatrixAsync(ItemService itemService, string name)
    {
        if (_cachedMatrices.TryGetValue(name, out var result))
        {
            return result;
        }

        var grid = _grids.FirstOrDefault(g => g.Name == name);
        if (grid == null)
        {
            return null;
        }

        var width = grid.Properties.CellsHorizontal;
        var height = grid.Properties.CellsVertical;
        var matrix = new bool[width, height];

        foreach (var itemInGrid in _children.Where(i => i.ParentId == _owner.Id && i.SlotId == grid.Name))
        {
            if (!itemInGrid.Location.IsValue1)
            {
                throw new Exception("!itemInGrid.Location.IsValue1");
            }

            var itemsInGrid = itemService.GetItemAndChildren(_children.ToList(), itemInGrid);
            (int itemWidth, int itemHeight) = await itemService.CalculateItemSizeAsync(itemsInGrid, itemInGrid.Location.Value1.r);

            for (var dx = 0; dx < itemWidth; dx++)
            {
                for (var dy = 0; dy < itemHeight; dy++)
                {
                    var x = itemInGrid.Location.Value1.x + dx;
                    var y = itemInGrid.Location.Value1.y + dy;

                    matrix[x, y] = true;
                }
            }
        }

        _cachedMatrices[grid.Name] = matrix;

        return matrix;
    }
}