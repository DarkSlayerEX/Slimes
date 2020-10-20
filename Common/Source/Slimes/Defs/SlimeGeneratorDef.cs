using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Slimes
{
    public class SlimeGeneratorDef : Def
    {
        public ThingDef originThingDef;
        public PawnKindDef originPawnKind;
        public StatsModifiers statModifiers;
        public RaceModifiers raceModifiers;
        public SourceMaterial material;
        public ButherThings butcherThings;
    }
}
