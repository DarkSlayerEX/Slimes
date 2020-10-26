using RimWorld;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Slimes
{
	public class CompProperties_ExtraGraphics : CompProperties
	{
		public CompProperties_ExtraGraphics()
		{
			this.compClass = typeof(CompExtraGraphics);
		}
		public List<GrowthStage> growthStages;
	}

	public class CompExtraGraphics : ThingComp
	{
		public float innerMass;
		public float unprocessedMass;
		public GrowthStage curStage;
		public CompProperties_ExtraGraphics Props
		{
			get
			{
				return (CompProperties_ExtraGraphics)this.props;
			}
		}

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
			this.TryChangeGraphics();
		}

		public override void CompTick()
        {
            base.CompTick();
			if (Find.TickManager.TicksGame % 60 == 0 && unprocessedMass > 0f)
            {
				var massToGain = 0f;
				if (unprocessedMass > 1f)
                {
					massToGain = 1f;
					unprocessedMass -= 1f;
                }
				else
                {
					massToGain = unprocessedMass;
					unprocessedMass = 0;
				}
				innerMass += massToGain;
				Log.Message(this.parent + " gained " + innerMass);
			}
			this.TryChangeGraphics();
		}

		public void TryChangeGraphics()
        {
			if (Props.growthStages != null)
			{
				var stage = GetCurrentStage();
				if (stage != null && stage != curStage)
                {
					Log.Message(innerMass + " - " + unprocessedMass + " - " + stage.massStage);
					ChangeGraphics(stage);
					curStage = stage;
				}
			}
		} 
		public GrowthStage GetCurrentStage()
        {
			var keys = Props.growthStages;
			GrowthStage result = null;
			foreach (var key in keys)
			{
				if (key.massStage < innerMass)
				{
					result = key;
				}
			}
			return result;
		}
		public void ChangeGraphics(GrowthStage growthStage)
        {
			var pawn = this.parent as Pawn;
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
			pawn.Drawer.renderer.graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(growthStage.graphicPath, ShaderDatabase.CutoutSkin,
				new Vector2(growthStage.drawSize, growthStage.drawSize), growthStage.color);
		}

        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Values.Look(ref innerMass, "innerMass");
			Scribe_Values.Look(ref unprocessedMass, "unprocessedMass");
		}
	}
}
