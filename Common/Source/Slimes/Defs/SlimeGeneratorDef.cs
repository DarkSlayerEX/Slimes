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
    public class SlimeGeneratorDef : Def
    {
        public ThingDef sourceThingDef;
        public SlimeTypeDef slimeTypeDef;
        public Color color;
        public StatsModifiers statModifiers;
        public RaceModifiers raceModifiers;
        public ButcherThings butcherThings;
    }
}
