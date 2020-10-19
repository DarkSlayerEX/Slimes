using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Slimes
{
	public class Controller : Mod
	{
		public Controller(ModContentPack content) : base(content)
		{
			new Harmony("Slimes.Mod").PatchAll();
		}
	}
}
