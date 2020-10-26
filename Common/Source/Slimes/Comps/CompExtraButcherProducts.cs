using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Slimes
{
	public class CompProperties_ExtraButcherProducts : CompProperties
	{
		public CompProperties_ExtraButcherProducts()
		{
			this.compClass = typeof(CompExtraButcherProducts);
		}

		public ButcherSpecialized butcherSpecialized;
	}

	public class CompExtraButcherProducts : ThingComp
	{
		public CompProperties_ExtraButcherProducts Props
		{
			get
			{
				return (CompProperties_ExtraButcherProducts)this.props;
			}
		}

        public override void CompTick()
        {
            base.CompTick();
        }
    }
}
