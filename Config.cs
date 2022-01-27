namespace SItemModifier;

public class ItemModification
{
    public ItemModification() { }
    public ItemModification(ItemAsset asset)
    {
        ID = asset.id;
        if (asset is ItemWeaponAsset weapon)
        {
            Durability = weapon.durability;
            Wear = weapon.wear;
        }
    }
    public ushort ID;
    public float
        Durability;
    public byte
        Wear;
    public void Modify(ItemAsset asset)
    {
        if (asset is ItemWeaponAsset weapon)
        {
            weapon.durability = Durability;
            weapon.wear = Wear;
        }
    }
}
public partial class Config : IRocketPluginConfiguration
{
    public List<ItemModification> Items = new List<ItemModification>();
    public void LoadDefaults()
    {
        Items.Add(new() { ID = 4, Durability = 1, Wear = 5 });
    }
}