global using static SItemModifier.Main;
using HarmonyLib;

namespace SItemModifier;

public sealed class Main : RocketPlugin<Config>
{
    public static Main Instance { get; private set; }

    internal static readonly List<ItemModification> Modifications = new();
    Harmony Harmony;

    protected override void Unload()
    {
        Level.onPostLevelLoaded -= PostLevelLoad;
        Harmony.UnpatchAll(Harmony.Id);
        ModifyAssets(Modifications);
        Modifications.Clear();
    }
    protected override void Load()
    {
        Instance = this;
        Level.onPostLevelLoaded += PostLevelLoad;
        if (Level.isLoaded)
            PostLevelLoad(0);
#if DEBUG
            Harmony.DEBUG = true;
#endif
        Harmony = new Harmony(nameof(SItemModifier));
        Harmony.PatchAll();
    }
    void PostLevelLoad(int l)
    {
        if (Modifications.Count != 0) 
            return;
        ModifyAssets(conf.Items, true);
    }
    public void ModifyAssets(IList<ItemModification> items, bool save = false)
    {
        foreach (var item in items)
        {
            var asset = Assets.find(EAssetType.ITEM, item.ID);
            if (asset is ItemAsset itemAsset)
            {
                if (save) Modifications.Add(new(itemAsset));
                item.Modify(itemAsset);
            }
        }
    }

    #region Translations
    public override TranslationList DefaultTranslations => DefaultTranslationList;

    public new string Translate(string key, params object[] args) => base.Translate(key.Trim(TranslationKeyTrimCharacters), args);
    #endregion
}