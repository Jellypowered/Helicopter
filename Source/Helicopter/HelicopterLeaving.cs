using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace Helicopter
{
    public class HelicopterLeaving : Skyfaller, IActiveDropPod, IThingHolder
    {
        // Token: 0x170005F0 RID: 1520
        // (get) Token: 0x0600271B RID: 10011 RVA: 0x00129CA0 File Offset: 0x001280A0
        // (set) Token: 0x0600271C RID: 10012 RVA: 0x00129CB8 File Offset: 0x001280B8
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

        // Token: 0x0600271D RID: 10013 RVA: 0x00129CD4 File Offset: 0x001280D4
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.groupID, "groupID", 0, false);
            Scribe_Values.Look<int>(ref this.destinationTile, "destinationTile", 0, false);
            Scribe_Deep.Look<TransportPodsArrivalAction>(ref this.arrivalAction, "arrivalAction", new object[0]);
            Scribe_Values.Look<bool>(ref this.alreadyLeft, "alreadyLeft", false, false);
        }

        // Token: 0x0600271E RID: 10014 RVA: 0x00129D34 File Offset: 0x00128134
        protected override void LeaveMap()
        {
            if (this.alreadyLeft)
            {
                base.LeaveMap();
                return;
            }
            if (this.groupID < 0)
            {
                Log.Error("Drop pod left the map, but its group ID is " + this.groupID, false);
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.destinationTile < 0)
            {
                Log.Error("Drop pod left the map, but its destination tile is " + this.destinationTile, false);
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            Lord lord = TransporterUtility.FindLord(this.groupID, base.Map);
            if (lord != null)
            {
                base.Map.lordManager.RemoveLord(lord);
            }
            TravelingTransportPods travelingTransportPods = (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("TravelingHelicopters", true));
            travelingTransportPods.Tile = base.Map.Tile;
            travelingTransportPods.SetFaction(Faction.OfPlayer);
            travelingTransportPods.destinationTile = this.destinationTile;
            travelingTransportPods.arrivalAction = this.arrivalAction;
            Find.WorldObjects.Add(travelingTransportPods);
            HelicopterLeaving.tmpActiveDropPods.Clear();
            HelicopterLeaving.tmpActiveDropPods.AddRange(base.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));
            for (int i = 0; i < HelicopterLeaving.tmpActiveDropPods.Count; i++)
            {
                HelicopterLeaving HelicopterLeaving = HelicopterLeaving.tmpActiveDropPods[i] as HelicopterLeaving;
                if (HelicopterLeaving != null && HelicopterLeaving.groupID == this.groupID)
                {
                    HelicopterLeaving.alreadyLeft = true;
                    travelingTransportPods.AddPod(HelicopterLeaving.Contents, true);
                    HelicopterLeaving.Contents = null;
                    HelicopterLeaving.Destroy(DestroyMode.Vanish);
                }
            }
        }

        // Token: 0x04001624 RID: 5668
        public int groupID = -1;

        // Token: 0x04001625 RID: 5669
        public int destinationTile = -1;

        // Token: 0x04001626 RID: 5670
        public TransportPodsArrivalAction arrivalAction;

        // Token: 0x04001627 RID: 5671
        private bool alreadyLeft;

        // Token: 0x04001628 RID: 5672
        private static List<Thing> tmpActiveDropPods = new List<Thing>();
    }
}
