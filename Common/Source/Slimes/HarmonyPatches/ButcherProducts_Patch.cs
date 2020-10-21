using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Slimes
{
    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    internal static class MakeRecipeProducts_Patch
    {
        private static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
        {
            if (recipeDef == SlimeDefOf.ButcherSlime)
            {
                List<Thing> list = (__result as List<Thing>) ?? __result.ToList<Thing>();
                var corpseSlimes = ingredients.Where(x => x is Corpse corpse && corpse.InnerPawn.IsSlime()).Cast<Corpse>().ToList();
                foreach (var t in GenerateExtraButcherProducts(corpseSlimes))
                {
                    list.Add(t);
                }
                __result = list;
            }
        }

        private static List<Thing> GenerateExtraButcherProducts(List<Corpse> corpseSlimes)
        {
            Log.Message(" - GenerateExtraButcherProducts - var things = new List<Thing>(); - 1", true);
            var things = new List<Thing>();
            Log.Message(" - GenerateExtraButcherProducts - foreach (var corpse in corpseSlimes) - 2", true);
            foreach (var corpse in corpseSlimes)
            {
                Log.Message(" - GenerateExtraButcherProducts - var butcherComp = corpse.InnerPawn.GetComp<CompExtraButcherProducts>(); - 3", true);
                var butcherComp = corpse.InnerPawn.GetComp<CompExtraButcherProducts>();
                Log.Message(" - GenerateExtraButcherProducts - if (butcherComp != null) - 4", true);
                if (butcherComp != null)
                {
                    Log.Message(" - GenerateExtraButcherProducts - if (butcherComp.Props.butcherSpecialized.oneOf != null && butcherComp.Props.butcherSpecialized.oneOf.Count > 0) - 5", true);
                    if (butcherComp.Props.butcherSpecialized.oneOf != null && butcherComp.Props.butcherSpecialized.oneOf.Count > 0)
                    {
                        Log.Message(" - GenerateExtraButcherProducts - var butcherData = butcherComp.Props.butcherSpecialized.oneOf.RandomElement(); - 6", true);
                        var butcherData = butcherComp.Props.butcherSpecialized.oneOf.RandomElement();
                        Log.Message(" - GenerateExtraButcherProducts - Thing thing = null; - 7", true);
                        Thing thing = null;
                        Log.Message(" - GenerateExtraButcherProducts - if (butcherData.yieldRange != null) - 8", true);
                        if (butcherData.yieldRange != null)
                        {
                            Log.Message(" - GenerateExtraButcherProducts - thing = ThingMaker.MakeThing(butcherData.thingDef); - 9", true);
                            thing = ThingMaker.MakeThing(butcherData.thingDef);
                            Log.Message(" - GenerateExtraButcherProducts - thing.stackCount = butcherData.yieldRange.RandomInRange; - 10", true);
                            thing.stackCount = butcherData.yieldRange.RandomInRange;
                        }
                        else if (butcherData.baseYield > 0)
                        {
                            Log.Message(" - GenerateExtraButcherProducts - thing = ThingMaker.MakeThing(butcherData.thingDef); - 12", true);
                            thing = ThingMaker.MakeThing(butcherData.thingDef);
                            Log.Message(" - GenerateExtraButcherProducts - thing.stackCount = butcherData.baseYield; - 13", true);
                            thing.stackCount = butcherData.baseYield;
                        }
                        Log.Message(" - GenerateExtraButcherProducts - if (thing != null) - 14", true);
                        if (thing != null)
                        {
                            Log.Message(" - GenerateExtraButcherProducts - things.Add(thing); - 15", true);
                            things.Add(thing);
                        }
                    }
                    if (butcherComp.Props.butcherSpecialized.allOf != null && butcherComp.Props.butcherSpecialized.allOf.Count > 0)
                    {
                        foreach (var butcherData in butcherComp.Props.butcherSpecialized.allOf)
                        {
                            Thing thing = null;
                            if (butcherData.yieldRange != null)
                            {
                                thing = ThingMaker.MakeThing(butcherData.thingDef);
                                thing.stackCount = butcherData.yieldRange.RandomInRange;
                            }
                            else if (butcherData.baseYield > 0)
                            {
                                thing = ThingMaker.MakeThing(butcherData.thingDef);
                                thing.stackCount = butcherData.baseYield;
                            }
                            if (thing != null)
                            {
                                things.Add(thing);
                            }
                        }
                    }
                }
            }
            return things;
        }
    }
}
