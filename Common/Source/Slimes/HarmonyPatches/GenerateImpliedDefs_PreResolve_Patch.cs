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
			if (__instance.kindDef.defName.EndsWith(GenerateImpliedDefs_PreResolve_Patch.slimeDefPostfix))
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
		public static string slimeDefPostfix = "_Slime";
	
		public static List<StuffCategoryDef> allowedCategories = new List<StuffCategoryDef>
		{
			StuffCategoryDefOf.Stony,
			StuffCategoryDefOf.Metallic
		};

		public static void AddImpliedDef<T>(T def) where T : Def, new()
		{
			def.generated = true;
			def.modContentPack?.AddDef(def, "ImpliedDefs");
			def.PostLoad();
			DefDatabase<T>.Add(def);
		}
		public static void Prefix()
		{

			foreach (var slimeGenerator in DefDatabase<SlimeGeneratorDef>.AllDefs)
            {
				Log.Message("slimeGenerator: " + slimeGenerator + " - " + slimeGenerator.originThingDef);
				var newDef1 = BaseSlimeDef(slimeGenerator);
				var newDef2 = BaseSlimeKindDef(slimeGenerator);
				newDef2.race = newDef1;
				AddImpliedDef(newDef1);
				AddImpliedDef(newDef2);
			}
		}

		private static ThingDef BaseSlimeDef(SlimeGeneratorDef slimeGenerator)
		{
			var thingDef = new ThingDef();
			foreach (var fieldInfo in typeof(ThingDef).GetFields())
			{
				try
				{
					var newField = thingDef.GetType().GetField(fieldInfo.Name);
					newField.SetValue(thingDef, fieldInfo.GetValue(slimeGenerator.originThingDef));
				}
				catch { }
			}
			thingDef.defName = slimeGenerator.defName;
			thingDef.label = slimeGenerator.label;
			if (slimeGenerator.description?.Length > 0)
            {
				thingDef.description = slimeGenerator.description;
			}
			else
			{
				thingDef.description = slimeGenerator.originThingDef.description;
			}
			AssignNewVariables(ref thingDef, slimeGenerator.originThingDef);
			AdjustStatBases(ref thingDef, slimeGenerator.statModifiers);
			if (slimeGenerator.butcherThings.butcherBasic != null)
            {
				thingDef.butcherProducts = slimeGenerator.butcherThings.butcherBasic;
			}
			if (slimeGenerator.butcherThings.butcherSpecialized != null)
			{
				var compProperties = new CompProperties_ExtraButcherProducts
				{
					butcherSpecialized = slimeGenerator.butcherThings.butcherSpecialized
				};
				thingDef.comps.Add(compProperties);
			}
			return thingDef;
		}


		private static PawnKindDef BaseSlimeKindDef(SlimeGeneratorDef slimeGenerator)
		{
			var pawnKind = new PawnKindDef();
			foreach (var fieldInfo in typeof(PawnKindDef).GetFields())
			{
				try
				{
					var newField = pawnKind.GetType().GetField(fieldInfo.Name);
					newField.SetValue(pawnKind, fieldInfo.GetValue(slimeGenerator.originPawnKind));
				}
				catch { }
			}
			AssignNewVariables(ref pawnKind, slimeGenerator.originPawnKind);
			pawnKind.defName = slimeGenerator.defName;
			pawnKind.label = slimeGenerator.label;
			if (slimeGenerator.description?.Length > 0)
            {
				pawnKind.description = slimeGenerator.description;
			}
			else
            {
				pawnKind.description = slimeGenerator.originThingDef.description;
			}
			foreach (var lifeStage in pawnKind.lifeStages)
			{
				if (slimeGenerator.color != null)
                {
					lifeStage.bodyGraphicData.color = new Color(slimeGenerator.color.r, slimeGenerator.color.g, slimeGenerator.color.b, slimeGenerator.color.a);
				}
				else
                {
					lifeStage.bodyGraphicData.color = new Color(slimeGenerator.sourceThingDef.stuffProps.color.r, slimeGenerator.sourceThingDef.stuffProps.color.g,
						slimeGenerator.sourceThingDef.stuffProps.color.b, slimeGenerator.sourceThingDef.stuffProps.color.a);
				}
			}
			Utils.slimePawnKindDefs.Add(pawnKind);
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
