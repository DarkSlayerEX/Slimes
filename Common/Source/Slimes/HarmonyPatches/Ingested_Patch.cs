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
    [HarmonyPatch(typeof(Thing), "Ingested")]
    public class Patch_Ingested
    {
        private static void Prefix(Thing __instance, Pawn ingester, float nutritionWanted)
        {
            if (ingester.IsSlime())
            {
                var comp = ingester.TryGetComp<CompExtraGraphics>();
                if (comp != null)
                {
                    Log.Message(ingester + " ingested " + __instance + " - " + __instance.def.BaseMass);
                    comp.unprocessedMass += __instance.def.BaseMass;
                }
            }
        }
    }
}
