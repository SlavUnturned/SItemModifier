using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace SItemModifier;

public static partial class Patches
{
    [Harmony]
    public static partial class Durability
    {
        [HarmonyPatch(typeof(UseableGun), "fire"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchGun(IEnumerable<CodeInstruction> originalInstructions) => PatchHasDurability(OpCodes.Ldarg_0, originalInstructions);

        [HarmonyPatch(typeof(UseableMelee), "fire"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchMelee(IEnumerable<CodeInstruction> originalInstructions) => PatchHasDurability(OpCodes.Ldarg_0, originalInstructions);
        public static IEnumerable<CodeInstruction> PatchHasDurability(OpCode playerCallerArgument, IEnumerable<CodeInstruction> originalInstructions)
        {
            var list = originalInstructions.ToList();
            foreach (var i in list.FindIndexes(x => x.LoadsField(AccessTools.Field(typeof(ItemsConfigData), nameof(ItemsConfigData.Has_Durability)))))
            {
                var idx = i;
                if (idx < 0)
                    continue;
                idx -= 2;
                list.Replace(idx, new CodeInstruction(playerCallerArgument))
                    .Replace(++idx, CodeInstruction.Call(typeof(PlayerCaller), $"get_{nameof(PlayerCaller.player)}"))
                    .Replace(++idx, CodeInstruction.Call(typeof(Durability), nameof(Check)));
            }
            return list;
        }
        public static bool Check(Player player)
        {
            var equipment = player.equipment;
            if (Modifications.Any(x => x.ID == equipment.itemID))
                return true;
            return Provider.modeConfigData.Items.Has_Durability;
        }
    }
}