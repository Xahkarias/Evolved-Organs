using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace EvolvedOrgans
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.evolvedorgans");

            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.FoodUtility), "AddFoodPoisoningHediff");

            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(EvolvedOrgans.HarmonyPatches).GetMethod("AddFoodPoisoningHediff_Prefix"));

            harmony.Patch(targetmethod, prefixmethod, null);
        }

        public static void AddFoodPoisoningHediff_Prefix(Pawn pawn)
        {
            if (pawn.health.immunity.GetImmunity(HediffDefOf.FoodPoisoning) > 0f)
            {
                return;
            }
        }
    }
}