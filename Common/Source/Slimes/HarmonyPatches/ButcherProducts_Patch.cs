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
                var corpseSlimes = ingredients.Where(x => x is Corpse corpse && corpse.InnerPawn.IsSlime()).Cast<Corpse>().ToList();
                __result = __result.ToList();
                foreach (var t in GenerateExtraButcherProducts(corpseSlimes))
                {
                    __result.AddItem(t);
                }
            }
        }

        private static List<Thing> GenerateExtraButcherProducts(List<Corpse> corpseSlimes)
        {
            var things = new List<Thing>();
            foreach (var corpse in corpseSlimes)
            {
                var butcherComp = corpse.InnerPawn.GetComp<CompExtraButcherProducts>();
                if (butcherComp != null)
                {
                    if (butcherComp.Props.butcherSpecialized.oneOf != null && butcherComp.Props.butcherSpecialized.oneOf.Count > 0)
                    {
                        var butcherData = butcherComp.Props.butcherSpecialized.oneOf.RandomElement();
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
                    else if (butcherComp.Props.butcherSpecialized.allOf != null && butcherComp.Props.butcherSpecialized.allOf.Count > 0)
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
