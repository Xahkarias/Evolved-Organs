using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace EvolvedOrgans
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches{
        static HarmonyPatches(){
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.evolvedorgans");

            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.FoodUtility), "AddFoodPoisoningHediff");

            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(EvolvedOrgans.HarmonyPatches).GetMethod("AddFoodPoisoningHediff_Prefix"));

            harmony.Patch(targetmethod, prefixmethod, null);
            Log.Message("Harmony successfully patched in Evolved Organ Methods", false);
        }

        public static bool AddFoodPoisoningHediff_Prefix(Pawn pawn)
            //do not run AddFoodPoisonHediff if pawn has immunity to food poisoning
        {
            if (pawn.health.immunity.GetImmunity(HediffDefOf.FoodPoisoning) > 0f){
                //Log.Message("Food poisoning set to immunizable will not get hediff added", true);
                return false;
            } return true;
        }
    }
}