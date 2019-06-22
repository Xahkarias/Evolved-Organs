using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
            Log.Message("Harmony successfully patched in Evolved Organ Methods", false);
        }

        [HarmonyPatch(typeof(FoodUtility))]
        [HarmonyPatch(nameof(FoodUtility.AddFoodPoisoningHediff))]

        static class PreventFoodPoisoningIfImmune_Patch
        //do not run AddFoodPoisonHediff if pawn has immunity to food poisoning
        {
            [HarmonyPrefix]
            public static bool AddFoodPoisoningHediff_Prefix(Pawn pawn)
            {
                if (pawn.health.immunity.GetImmunity(HediffDefOf.FoodPoisoning) > 0f)
                {
                    return false;
                }
                return true;
            }
        }

        static class PreventAgeHediffsIfImmortal_Patch
        {
            [HarmonyPatch(typeof(AgeInjuryUtility), "RandomHediffsToGainOnBirthday", new Type[] { typeof(Pawn), typeof(int) })]
            public static bool PreventAgeInjuries_Prefix(Pawn pawn, ref IEnumerable<HediffGiver_Birthday> __result)
            {

                if (pawn.health.hediffSet.HasHediff(HediffDef.Named("ImmortalOrgan")))
                {
                    Log.Message("no hediff added, pawn is immortal" + __result);
                    __result = System.Linq.Enumerable.Empty<HediffGiver_Birthday>();
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }
        //try to warn the user before they add a bill to overwrite a implant.
        //static class PreventMultipleImplants_Patch
        //{
        //    [HarmonyPatch(typeof(Recipe_InstallImplant))]
        //    [HarmonyPatch(nameof(Recipe_InstallImplant.GetPartsToApplyOn))]

        //    public static bool PreventMultipleImplants_Prefix(Pawn pawn, RecipeDef recipe)
        //    {
        //        for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
        //        {
        //            Log.Message(i.ToString());
        //            Log.Message(pawn.health.hediffSet.hediffs.ToString());
        //            Log.Message(recipe.appliedOnFixedBodyParts.ToString());
        //            Log.Message(pawn.health.hediffSet.hediffs.ToString());
        //            if (pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part.ToString() == recipe.appliedOnFixedBodyParts.ToString())
        //                && recipe.label.Contains(pawn.health.hediffSet.hediffs.ToString()))
        //            {
        //                Messages.Message("This pawn already has a implant in their" + recipe.appliedOnFixedBodyParts.ToString()+".  Remove the existing implant before trying to install a new one.", pawn, MessageTypeDefOf.CautionInput);
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //}
    }

}
