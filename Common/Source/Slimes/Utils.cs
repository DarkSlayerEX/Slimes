﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Slimes
{	public static class Utils
	{
		public static HashSet<PawnKindDef> slimePawnKindDefs = new HashSet<PawnKindDef>();

		public static bool IsSlime(this Pawn pawn)
        {
			if (slimePawnKindDefs.Contains(pawn.kindDef))
            {
				return true;
            }
			return false;
        }
	}

}
