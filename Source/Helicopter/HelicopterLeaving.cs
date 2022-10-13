﻿using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace Helicopter
{
    public class HelicopterLeaving : Skyfaller, IActiveDropPod, IThingHolder
    {
        private static readonly List<Thing> tmpActiveDropPods = new List<Thing>();
        public int groupID = -1;
        public int destinationTile = -1;
        public TransportPodsArrivalAction arrivalAction;
        private bool alreadyLeft;

        public ActiveDropPodInfo Contents
        {
            get
            {
                return ((ActiveDropPod)this.innerContainer[0]).Contents;
            }
            set
            {
                ((ActiveDropPod)this.innerContainer[0]).Contents = value;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.groupID, "groupID", 0, false);
            Scribe_Values.Look<int>(ref this.destinationTile, "destinationTile", 0, false);
            Scribe_Deep.Look<TransportPodsArrivalAction>(ref this.arrivalAction, "arrivalAction");
            Scribe_Values.Look<bool>(ref this.alreadyLeft, "alreadyLeft", false, false);
        }

        protected override void LeaveMap()
        {
            if (this.alreadyLeft)
                base.LeaveMap();
            else if (this.groupID < 0)
            {
                Log.Error("Drop pod left the map, but its group ID is " + (object)this.groupID);
                this.Destroy(DestroyMode.Vanish);
            }
            else if (this.destinationTile < 0)
            {
                Log.Error("Drop pod left the map, but its destination tile is " + (object)this.destinationTile);
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                Lord lord = TransporterUtility.FindLord(this.groupID, this.Map);
                if (lord != null)
                    this.Map.lordManager.RemoveLord(lord);
                TravelingTransportPods travelingTransportPods = (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("TravelingHelicopters", true));
                travelingTransportPods.Tile = this.Map.Tile;
                travelingTransportPods.SetFaction(Faction.OfPlayer);
                travelingTransportPods.destinationTile = this.destinationTile;
                travelingTransportPods.arrivalAction = this.arrivalAction;

                Find.WorldObjects.Add(travelingTransportPods);
                HelicopterLeaving.tmpActiveDropPods.Clear();
                HelicopterLeaving.tmpActiveDropPods.AddRange((IEnumerable<Thing>)this.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));
                //travelingTransportPods.flyingThing = tmpActiveDropPods.Find(x => (x as SRTSLeaving)?.groupID == this.groupID);
                for (int index = 0; index < HelicopterLeaving.tmpActiveDropPods.Count; ++index)
                {
                    if (HelicopterLeaving.tmpActiveDropPods[index] is HelicopterLeaving tmpActiveDropPod && tmpActiveDropPod.groupID == this.groupID)
                    {
                        tmpActiveDropPod.alreadyLeft = true;
                        travelingTransportPods.AddPod(tmpActiveDropPod.Contents, true);
                        tmpActiveDropPod.Contents = (ActiveDropPodInfo)null;
                        tmpActiveDropPod.Destroy(DestroyMode.Vanish);
                    }
                }
            }
        }
    }
}