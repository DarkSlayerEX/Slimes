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
				Log.Message(__instance + " - " + __instance.ageTracker?.CurKindLifeStage?.bodyGraphicData?.color);
				foreach (var lifeStage in __instance.kindDef.lifeStages)
				{
					Log.Message("TEST 1: " + __instance.kindDef + " - " + lifeStage.bodyGraphicData.color);
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
				Log.Message("Adding: " + newDef1);
				foreach (var d in DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.StartsWith(slimeDefPrefix)))
				{
					foreach (var lifeStage in d.lifeStages)
					{
						Log.Message("TEST 2: " + d + " - " + lifeStage.bodyGraphicData.color);
					}
				}
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
			thingDef.defName = slimeDefPrefix + baseThingDef.defName;
			thingDef.label = slimeDefPrefix + " " + baseThingDef.label;
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
			SetCopyOf(baseSlime, ref pawnKind);
			pawnKind.defName = slimeDefPrefix + baseThingDef.defName;
			pawnKind.label = slimeDefPrefix + " " + baseThingDef.label;
			foreach (var lifeStage in pawnKind.lifeStages)
			{
				lifeStage.bodyGraphicData.color = new Color(baseThingDef.stuffProps.color.r, baseThingDef.stuffProps.color.g, baseThingDef.stuffProps.color.b, baseThingDef.stuffProps.color.a);
				Log.Message(pawnKind + " - " + lifeStage.bodyGraphicData.color);
			}
			return pawnKind;
		}

		private static void SetCopyOf(PawnKindDef source, ref PawnKindDef target)
        {
			target.lifeStages = new List<PawnKindLifeStage>
			{
				new PawnKindLifeStage()
				{
					bodyGraphicData = new GraphicData()
					{
						drawSize = Vector2.one,
						texPath = "Things/Pawn/Animal/Slime",
						color = Color.white,
						shaderType = ShaderTypeDefOf.CutoutComplex
					},
					dessicatedBodyGraphicData = new GraphicData()
					{
						drawSize = Vector2.one,
						texPath = "Things/Pawn/Animal/Dessicated_Slime",
					}
				},
				new PawnKindLifeStage()
				{
					bodyGraphicData = new GraphicData()
					{
						drawSize = Vector2.one,
						texPath = "Things/Pawn/Animal/Slime",
						color = Color.white,
						shaderType = ShaderTypeDefOf.CutoutComplex
					},
					dessicatedBodyGraphicData = new GraphicData()
					{
						drawSize = Vector2.one,
						texPath = "Things/Pawn/Animal/Dessicated_Slime",
					}
				},
				new PawnKindLifeStage()
				{
					bodyGraphicData = new GraphicData()
					{
						drawSize = Vector2.one,
						texPath = "Things/Pawn/Animal/Slime",
						color = Color.white,
						shaderType = ShaderTypeDefOf.CutoutComplex
					},
					dessicatedBodyGraphicData = new GraphicData()
                    {
						drawSize = Vector2.one,
						texPath = "Things/Pawn/Animal/Dessicated_Slime",
					}
				}
			};
		}
	}
}
