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
	[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
	public static class SpawnSetup
	{
		public static void Postfix(ref Pawn __instance)
		{
			if (__instance.kindDef.defName.StartsWith(GenerateImpliedDefs_PreResolve_Patch.slimeDefPrefix))
            {
				foreach (var statBase in __instance.def.statBases)
				{
					Log.Message("TEST 1: " + __instance.kindDef + " - " + statBase);
				}
			}
		}
	}

	[HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
	public static class GenerateImpliedDefs_PreResolve_Patch
	{
		public static string slimeDefPrefix = "Slime_";
	
		public static List<StuffCategoryDef> allowedCategories = new List<StuffCategoryDef>
		{
			StuffCategoryDefOf.Stony,
			StuffCategoryDefOf.Metallic
		};
	
		public static void Prefix()
		{
			var list = DefDatabase<ThingDef>.AllDefs.Where(x => x.stuffProps?.categories != null &&
				x.stuffProps.categories.Where(y => allowedCategories.Contains(y)).Any()).ToList();
			foreach (var thingDef in list)
			{
				var newDef1 = BaseSlimeDef(thingDef);
				var newDef2 = BaseSlimeKindDef(thingDef);
				newDef2.race = newDef1;
				AddImpliedDef(newDef1);
				AddImpliedDef(newDef2);
			}
		}
	
		public static void AddImpliedDef<T>(T def) where T : Def, new()
		{
			def.generated = true;
			def.modContentPack?.AddDef(def, "ImpliedDefs");
			def.PostLoad();
			DefDatabase<T>.Add(def);
		}
		private static ThingDef BaseSlimeDef(ThingDef baseThingDef)
		{
			var thingDef = new ThingDef();
			var baseSlime = SlimeDefOf.Slime;
			foreach (var fieldInfo in typeof(ThingDef).GetFields())
			{
				try
				{
					var newField = thingDef.GetType().GetField(fieldInfo.Name);
					newField.SetValue(thingDef, fieldInfo.GetValue(baseSlime));
				}
				catch { }
			}
			AssignNewVariables(ref thingDef, baseSlime);
			thingDef.defName = slimeDefPrefix + baseThingDef.defName;
			thingDef.label = slimeDefPrefix + " " + baseThingDef.label;
			foreach (var stuffCategoryModifier in DefDatabase<StuffCategoryModifiers>.AllDefs)
            {
				if (baseThingDef.stuffProps.categories.Where(x => x.defName == stuffCategoryModifier.defName).Any())
                {
					AdjustStatBases(ref thingDef, stuffCategoryModifier.statsModifiers);
					//foreach (var t in stuffCategoryModifier.raceModifiers.offsets.GetType())
                    //{
					//
                    //}
				}
            }

			foreach (var stuffModifier in DefDatabase<StuffModifiers>.AllDefs)
			{
				if (thingDef.defName == stuffModifier.defName)
				{
					AdjustStatBases(ref thingDef, stuffModifier.statsModifiers);
				}
			}
			return thingDef;
		}


		private static PawnKindDef BaseSlimeKindDef(ThingDef baseThingDef)
		{
			var pawnKind = new PawnKindDef();
			var baseSlime = SlimePawnKindDefOf.Slime;
			foreach (var fieldInfo in typeof(PawnKindDef).GetFields())
			{
				try
				{
					var newField = pawnKind.GetType().GetField(fieldInfo.Name);
					newField.SetValue(pawnKind, fieldInfo.GetValue(baseSlime));
				}
				catch { }
			}
			AssignNewVariables(ref pawnKind, baseSlime);
			pawnKind.defName = slimeDefPrefix + baseThingDef.defName;
			pawnKind.label = slimeDefPrefix + " " + baseThingDef.label;
			foreach (var lifeStage in pawnKind.lifeStages)
			{
				lifeStage.bodyGraphicData.color = new Color(baseThingDef.stuffProps.color.r, baseThingDef.stuffProps.color.g, baseThingDef.stuffProps.color.b, baseThingDef.stuffProps.color.a);
			}
			return pawnKind;
		}

		private static void AdjustStatBases(ref ThingDef target, StatsModifiers statsModifiers)
        {
			if (statsModifiers.offsets != null)
            {
				foreach (var offset in statsModifiers.offsets)
				{
					foreach (var statBase in target.statBases)
					{
						if (statBase.stat == offset.stat)
						{
							statBase.value += offset.value;
							Log.Message("Adding: " + target + " - Offset: " + offset.stat + " - " + offset.value);
						}
					}
				}
			}

			if (statsModifiers.multipliers != null)
            {
				foreach (var multiplier in statsModifiers.multipliers)
				{
					foreach (var statBase in target.statBases)
					{
						if (statBase.stat == multiplier.stat)
						{
							statBase.value *= multiplier.value;
							Log.Message("Adding: " + target + " - Multiplier: " + multiplier.stat + " - " + multiplier.value);
						}
					}
				}
			}

			if (statsModifiers.fixedValues != null)
            {
				foreach (var fixedValue in statsModifiers.fixedValues)
				{
                    foreach (var statBase in target.statBases)
					{
						if (statBase.stat == fixedValue.stat)
						{
							statBase.value = fixedValue.value;
							Log.Message("Adding: " + target + " - Fixed value: " + fixedValue.stat + " - " + fixedValue.value);
						}
					}
				}
			}
		}

		private static void AssignNewVariables(ref ThingDef target, ThingDef source)
		{
			target.statBases = new List<StatModifier>();
			foreach (var statBase in source.statBases)
			{
				target.statBases.Add(new StatModifier 
				{
					stat = statBase.stat,
					value = statBase.value
				});
			}
		}
		private static void AssignNewVariables(ref PawnKindDef target, PawnKindDef source)
        {
			target.lifeStages = new List<PawnKindLifeStage>();
			foreach (var lifeStage in source.lifeStages)
            {
				target.lifeStages.Add(new PawnKindLifeStage
				{
					bodyGraphicData = new GraphicData
					{
						shaderType = lifeStage.bodyGraphicData.shaderType,
						texPath = lifeStage.bodyGraphicData.texPath,
						shadowData = lifeStage.bodyGraphicData.shadowData,
						drawSize = lifeStage.bodyGraphicData.drawSize,
						colorTwo = lifeStage.bodyGraphicData.colorTwo,
						color = lifeStage.bodyGraphicData.color
					},
					dessicatedBodyGraphicData = new GraphicData
					{
						shaderType = lifeStage.dessicatedBodyGraphicData.shaderType,
						texPath = lifeStage.dessicatedBodyGraphicData.texPath,
						shadowData = lifeStage.dessicatedBodyGraphicData.shadowData,
						drawSize = lifeStage.dessicatedBodyGraphicData.drawSize,
						colorTwo = lifeStage.dessicatedBodyGraphicData.colorTwo,
						color = lifeStage.dessicatedBodyGraphicData.color
					},
				});
			}
		}
	}
}
