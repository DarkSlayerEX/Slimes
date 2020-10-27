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
    public class SlimeTypeDef : Def
    {
        public ThingDef originThingDef;
        public PawnKindDef originPawnKind;
        public GrowthStages growthStages;
        public float totalAbsorbableMass;
    }
}
