using Harmony;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;

namespace EvolvedOrgans
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.evolvedOrgans");

            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.FoodUtility), "AddFoodPoisoningHediff");

            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(EvolvedOrgans.HarmonyPatches).GetMthod("AddFoodPoisoningHediff_Prefix"));

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