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
		public GrowthStages growthStages;
		public float totalAbsorbableMass;
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
			curStage = GetCurrentStage();
        }
        public override void CompTick()
        {
            base.CompTick();
			if (innerMass < Props.totalAbsorbableMass && curStage != null && Find.TickManager.TicksGame % curStage.massConversionRate == 0)
            {
				if (unprocessedMass > 0f)
				{
					var massToGain = 0f;
					Log.Message(this.parent + " trying to process " + unprocessedMass + " - total current mass: " + innerMass);
					if (unprocessedMass > 1f)
					{
						massToGain = 1f * curStage.massConversionEfficiency;
						unprocessedMass -= 1f;
					}
					else
					{
						massToGain = unprocessedMass * curStage.massConversionEfficiency;
						unprocessedMass = 0;
					}
					innerMass += massToGain;
					Log.Message(this.parent + " gained " + massToGain + " - total current mass: " + innerMass + " - unprocessed mass: " + unprocessedMass);
				}
				this.TryChangeGraphics();
			}
		}

		public void TryChangeGraphics()
        {
			if (Props.growthStages != null)
			{
				var stage = GetCurrentStage();
				if (stage != null && stage != curStage)
                {
					ChangeGraphics(stage);
					curStage = stage;
				}
			}
		}

		public GrowthStage GetCurrentStage()
        {
			var keys = Props.growthStages.growthStages.OrderBy(x => x.massStage);
			GrowthStage result = null;
			foreach (var key in keys)
			{
				var progress = innerMass / Props.totalAbsorbableMass;
				if (key.massStage <= progress)
				{
					result = key;
				}
			}
			return result;
		}
		public void ChangeGraphics(GrowthStage growthStage)
        {
			Log.Message("Changed graphics: " + growthStage.graphicPath);
			Log.Message("Result: " + growthStage.massStage + " - innerMass: " + innerMass + " - Props.totalAbsorbableMass: " + Props.totalAbsorbableMass);

			var pawn = this.parent as Pawn;
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
			pawn.Drawer.renderer.graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(growthStage.graphicPath, ShaderDatabase.CutoutSkin,
				new Vector2(growthStage.drawSize, growthStage.drawSize), growthStage.color);
			if (growthStage.baseHungerRate > 0f)
            {
				ChangeHungerRate(growthStage);
			}
		}

		public void ChangeHungerRate(GrowthStage growthStage)
        {
			//var hediffDef = new HediffDef()
			//{
			//	defName = "SlimeHungerRate" + this.parent.ThingID,
			//	label = "SlimeHungerRate" + this.parent.ThingID,
			//	stages = new List<HediffStage>
			//	{
			//		new HediffStage()
			//		{
			//			hungerRateFactor = growthStage.baseHungerRate
			//		}
			//	}
			//};
			//var pawn = this.parent as Pawn;
			//var hungerHediff = HediffMaker.MakeHediff(hediffDef, pawn);
			//var oldHediff = pawn.health.hediffSet.hediffs.Where(x => x.def.defName.StartsWith("SlimeHungerRate")).FirstOrDefault();
			//if (oldHediff != null)
            //{
			//	pawn.health.hediffSet.hediffs.Remove(oldHediff);
            //}
			//pawn.health.AddHediff(hungerHediff);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Values.Look(ref innerMass, "innerMass");
			Scribe_Values.Look(ref unprocessedMass, "unprocessedMass");
		}
	}
}
