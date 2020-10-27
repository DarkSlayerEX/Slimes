using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Slimes
{
	[HarmonyPatch(typeof(Game), "UpdatePlay")]
	public class PawnGraphicSetPostfix_Patch
	{
		public static bool done = false;
		public static void Postfix()
		{
			if (!done)
            {
				foreach (var map in Find.Maps)
				{
					foreach (var pawn in map.mapPawns.AllPawns)
					{
						if (pawn.IsSlime())
						{
							var comp = pawn.TryGetComp<CompExtraGraphics>();
							if (comp != null)
							{
								if (comp.Props.growthStages != null)
								{
									var stage = comp.GetCurrentStage();
									if (stage != null)
									{
										comp.ChangeGraphics(stage);
									}
								}
							}
						}
					}
				}
				done = true;
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
	public class SpawnSetup_Patch
	{
		public static void Postfix(Pawn __instance)
		{
			if (PawnGraphicSetPostfix_Patch.done && __instance.IsSlime())
			{
				var comp = __instance.TryGetComp<CompExtraGraphics>();
				if (comp != null)
				{
					if (comp.Props.growthStages != null)
					{
						var stage = comp.GetCurrentStage();
						if (stage != null)
						{
							comp.ChangeGraphics(stage);
						}
					}
				}
			}
		}
	}
}
