using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            harmony.PatchAll();
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

        //[HarmonyPatch(typeof(Pawn))]
        //[HarmonyPatch(nameof(Pawn.TickRare))]

        //static class AddFoodFromSun_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void AddFoodFromSun_Postfix()
        //    {
        //        Log.Message("running postfix for sun");
        //        Map map = Current.Game.CurrentMap;
        //        IEnumerable<Pawn> pawnlist = map.mapPawns.AllPawns;
        //        for (int i = 0; i < map.mapPawns.AllPawnsCount; i++)
        //        {
        //            Pawn pawn = (Pawn)pawnlist;//Somehow iterate thru pawnlist and set Pawn pawn = each pawn in the forloop
        //            Log.Message(pawn.ToString());
        //            if (pawn.health.hediffSet.HasHediff(HediffDef.Named("PhotosyntheticSkin")))
        //            {
        //                float sunlevel = pawn.Map.skyManager.CurSkyGlow;
        //                double worncoverage = 0;
        //                float foodadded = 0f;
        //                if (pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.UpperHead) || pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.FullHead))
        //                {
        //                    worncoverage += .1;
        //                    Log.Message(worncoverage.ToString());
        //                }
        //                if (pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.Torso))
        //                {
        //                    worncoverage += .4;
        //                    Log.Message(worncoverage.ToString());
        //                }
        //                if (pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.Legs))
        //                {
        //                    worncoverage += .3;
        //                    Log.Message(worncoverage.ToString());
        //                }
        //                if (pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.LeftHand))
        //                {
        //                    worncoverage += .1;
        //                    Log.Message(worncoverage.ToString());
        //                }
        //                if (pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.RightHand))
        //                {
        //                    worncoverage += .1;
        //                    Log.Message(worncoverage.ToString());
        //                }
        //                Log.Message(pawn.Name.ToString() + "has hediff photosynthesis, light level is : " + sunlevel.ToString());
        //                if (sunlevel > .01)
        //                {
        //                    Log.Message("light level is : " + sunlevel.ToString());
        //                    if (worncoverage < 1)
        //                    {
        //                        Log.Message("worncoverage is : " + (worncoverage).ToString());
        //                        foodadded = Math.Max(sunlevel * (float)(worncoverage * -1), 0);//some number to increase food need by every 250 ticks?
        //                        Log.Message("adding food need via photosynthesis " + foodadded.ToString());
        //                        pawn.needs.food.CurLevel += foodadded;
        //                    }

        //                }
        //            }
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(Recipe_InstallImplant), "GetPartsToApplyOn", new Type[] { typeof(Pawn), typeof(RecipeDef)})]
        public class AddImplantstoEvolvedOrgans_Patch
        {
            [HarmonyPostfix]
            public static IEnumerable<BodyPartRecord> GetPartsToApplyOn_Postfix(Pawn pawn, RecipeDef recipe)
            {
                Log.Message("running patch for GetPartsToApplyOn");
                if (recipe.defName.Contains("EVORG_"))
                {
                    Log.Message("pawn target has installed evolved organ, allowing implant");
                    for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
                    {
                        BodyPartDef part = recipe.appliedOnFixedBodyParts[i];
                        List<BodyPartRecord> bpList = pawn.RaceProps.body.AllParts;
                        for (int j = 0; j < bpList.Count; j++)
                        {
                            BodyPartRecord record = bpList[j];
                            if (record.def == part)
                            {
                                if (pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(record))
                                {
                                    if (!pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && x.def == recipe.addsHediff))
                                    {
                                       yield return record;
                                    }
                                }
                            }
                        }
                    }
                } yield break;
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
