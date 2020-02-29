using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Helicopter
{
    [StaticConstructorOnStartup]
    public class CompLaunchableHelicopter : ThingComp
    {

        // (get) Token: 0x060028C4 RID: 10436 RVA: 0x00134D20 File Offset: 0x00133120
        public Building FuelingPortSource
        {
            get
            {
                return (Building)this.parent;//FuelingPortUtility.FuelingPortGiverAtFuelingPortCell(this.parent.Position, this.parent.Map);
            }
        }

        // Token: 0x17000625 RID: 1573
        // (get) Token: 0x060028C5 RID: 10437 RVA: 0x00134D3D File Offset: 0x0013313D
        public bool ConnectedToFuelingPort
        {
            get
            {
                return this.FuelingPortSource != null;
            }
        }

        // Token: 0x17000626 RID: 1574
        // (get) Token: 0x060028C6 RID: 10438 RVA: 0x00134D4B File Offset: 0x0013314B
        public bool FuelingPortSourceHasAnyFuel
        {
            get
            {
                return this.ConnectedToFuelingPort && this.FuelingPortSource.GetComp<CompRefuelable>().HasFuel;
            }
        }

        // Token: 0x17000627 RID: 1575
        // (get) Token: 0x060028C7 RID: 10439 RVA: 0x00134D6B File Offset: 0x0013316B
        public bool LoadingInProgressOrReadyToLaunch
        {
            get
            {
                return this.Transporter.LoadingInProgressOrReadyToLaunch;
            }
        }

        // Token: 0x17000628 RID: 1576
        // (get) Token: 0x060028C8 RID: 10440 RVA: 0x00134D78 File Offset: 0x00133178
        public bool AnythingLeftToLoad
        {
            get
            {
                return this.Transporter.AnythingLeftToLoad;
            }
        }

        // Token: 0x17000629 RID: 1577
        // (get) Token: 0x060028C9 RID: 10441 RVA: 0x00134D85 File Offset: 0x00133185
        public Thing FirstThingLeftToLoad
        {
            get
            {
                return this.Transporter.FirstThingLeftToLoad;
            }
        }

        // Token: 0x1700062A RID: 1578
        // (get) Token: 0x060028CA RID: 10442 RVA: 0x00134D92 File Offset: 0x00133192
        public List<CompTransporter> TransportersInGroup
        {
            get
            {
                List<CompTransporter> result = new List<CompTransporter>();
                result.Add(this.parent.TryGetComp<CompTransporter>());
                return result;//this.Transporter.TransportersInGroup(this.parent.Map);
            }
        }

        // Token: 0x1700062B RID: 1579
        // (get) Token: 0x060028CB RID: 10443 RVA: 0x00134DAA File Offset: 0x001331AA
        public bool AnyInGroupHasAnythingLeftToLoad
        {
            get
            {
                return this.Transporter.AnyInGroupHasAnythingLeftToLoad;
            }
        }

        // Token: 0x1700062C RID: 1580
        // (get) Token: 0x060028CC RID: 10444 RVA: 0x00134DB7 File Offset: 0x001331B7
        public Thing FirstThingLeftToLoadInGroup
        {
            get
            {
                return this.Transporter.FirstThingLeftToLoadInGroup;
            }
        }

        // Token: 0x1700062D RID: 1581
        // (get) Token: 0x060028CD RID: 10445 RVA: 0x00134DC4 File Offset: 0x001331C4
        public bool AnyInGroupIsUnderRoof
        {
            get
            {
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    if (transportersInGroup[i].parent.Position.Roofed(this.parent.Map))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // Token: 0x1700062E RID: 1582
        // (get) Token: 0x060028CE RID: 10446 RVA: 0x00134E18 File Offset: 0x00133218
        public CompTransporter Transporter
        {
            get
            {
                if (this.cachedCompTransporter == null)
                {
                    this.cachedCompTransporter = this.parent.GetComp<CompTransporter>();
                }
                return this.cachedCompTransporter;
            }
        }

        // Token: 0x1700062F RID: 1583
        // (get) Token: 0x060028CF RID: 10447 RVA: 0x00134E3C File Offset: 0x0013323C
        public float FuelingPortSourceFuel
        {
            get
            {
                if (!this.ConnectedToFuelingPort)
                {
                    return 0f;
                }
                return this.parent.GetComp<CompRefuelable>().Fuel;
            }
        }

        // Token: 0x17000630 RID: 1584
        // (get) Token: 0x060028D0 RID: 10448 RVA: 0x00134E60 File Offset: 0x00133260
        public bool AllInGroupConnectedToFuelingPort
        {
            get
            {
                /*
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    if (!transportersInGroup[i].Launchable.ConnectedToFuelingPort)
                    {
                        return false;
                    }
                }
                */
                return true;
            }
        }

        // Token: 0x17000631 RID: 1585
        // (get) Token: 0x060028D1 RID: 10449 RVA: 0x00134EA4 File Offset: 0x001332A4
        public bool AllFuelingPortSourcesInGroupHaveAnyFuel
        {
            get
            {
                /*
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    if (!transportersInGroup[i].Launchable.FuelingPortSourceHasAnyFuel)
                    {
                        return false;
                    }
                }
                */
                return true;
            }
        }

        // Token: 0x17000632 RID: 1586
        // (get) Token: 0x060028D2 RID: 10450 RVA: 0x00134EE8 File Offset: 0x001332E8
        private float FuelInLeastFueledFuelingPortSource
        {
            get
            {
                //List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                float num = 0f;
                bool flag = false;
              //  for (int i = 0; i < transportersInGroup.Count; i++)
              //  {
                    float fuelingPortSourceFuel = //transportersInGroup[i].Launchable.
                    FuelingPortSourceFuel;
                    if (!flag || fuelingPortSourceFuel < num)
                    {
                        num = fuelingPortSourceFuel;
                        flag = true;
                    }
              //  }
                if (!flag)
                {
                    return 0f;
                }
                return num;
            }
        }

        // Token: 0x17000633 RID: 1587
        // (get) Token: 0x060028D3 RID: 10451 RVA: 0x00134F4E File Offset: 0x0013334E
        private int MaxLaunchDistance
        {
            get
            {
                if(this.parent.Spawned)
                if (!this.LoadingInProgressOrReadyToLaunch )
                {
                    return 0;
                }
                return CompLaunchableHelicopter.MaxLaunchDistanceAtFuelLevel(this.FuelInLeastFueledFuelingPortSource);
            }
        }

        // Token: 0x17000634 RID: 1588
        // (get) Token: 0x060028D4 RID: 10452 RVA: 0x00134F68 File Offset: 0x00133368
        private int MaxLaunchDistanceEverPossible
        {
            get
            {
                if (!this.LoadingInProgressOrReadyToLaunch)
                {
                    return 0;
                }
                //List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                float num = 0f;
                //for (int i = 0; i < transportersInGroup.Count; i++)
                //{
                    Building fuelingPortSource = //transportersInGroup[i].Launchable
                    this.FuelingPortSource;
                    if (fuelingPortSource != null)
                    {
                        num = Mathf.Max(num, fuelingPortSource.GetComp<CompRefuelable>().Props.fuelCapacity);
                    }
                //}
                return CompLaunchableHelicopter.MaxLaunchDistanceAtFuelLevel(num);
            }
        }

        // Token: 0x17000635 RID: 1589
        // (get) Token: 0x060028D5 RID: 10453 RVA: 0x00134FDC File Offset: 0x001333DC
        private bool PodsHaveAnyPotentialCaravanOwner
        {
            get
            {
                List<CompTransporter> transportersInGroup = this.TransportersInGroup;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    ThingOwner innerContainer = transportersInGroup[i].innerContainer;
                    for (int j = 0; j < innerContainer.Count; j++)
                    {
                        Pawn pawn = innerContainer[j] as Pawn;
                        if (pawn != null && CaravanUtility.IsOwner(pawn, Faction.OfPlayer))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        // Token: 0x060028D6 RID: 10454 RVA: 0x00135054 File Offset: 0x00133454
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }
            if (this.LoadingInProgressOrReadyToLaunch)
            {
                Command_Action launch = new Command_Action();
                launch.defaultLabel = "CommandLaunchGroup".Translate();
                launch.defaultDesc = "CommandLaunchGroupDesc".Translate();
                launch.icon = CompLaunchableHelicopter.LaunchCommandTex;
                launch.alsoClickIfOtherInGroupClicked = false;
                launch.action = delegate
                {
                    if (this.AnyInGroupHasAnythingLeftToLoad)
					{
                         Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmSendNotCompletelyLoadedPods".Translate(this.FirstThingLeftToLoadInGroup.LabelCapNoCount), new Action(this.StartChoosingDestination), false, null));
                    }
					else
					{
                        this.StartChoosingDestination();
                    }
                };
                if (!this.AllInGroupConnectedToFuelingPort)
                {
                    launch.Disable("CommandLaunchGroupFailNotConnectedToFuelingPort".Translate());
                }
                else if (!this.AllFuelingPortSourcesInGroupHaveAnyFuel)
                {
                    launch.Disable("CommandLaunchGroupFailNoFuel".Translate());
                }
                else if (this.AnyInGroupIsUnderRoof)
                {
                    launch.Disable("CommandLaunchGroupFailUnderRoof".Translate());
                }
                yield return launch;
            }
            yield break;
        }

        // Token: 0x060028D7 RID: 10455 RVA: 0x00135078 File Offset: 0x00133478
        public override string CompInspectStringExtra()
        {
            if (!this.LoadingInProgressOrReadyToLaunch)
            {
                return null;
            }
            if (!this.AllInGroupConnectedToFuelingPort)
            {
                return "NotReadyForLaunch".Translate() + ": " + "NotAllInGroupConnectedToFuelingPort".Translate() + ".";
            }
            if (!this.AllFuelingPortSourcesInGroupHaveAnyFuel)
            {
                return "NotReadyForLaunch".Translate() + ": " + "NotAllFuelingPortSourcesInGroupHaveAnyFuel".Translate() + ".";
            }
            if (this.AnyInGroupHasAnythingLeftToLoad)
            {
                return "NotReadyForLaunch".Translate() + ": " + "TransportPodInGroupHasSomethingLeftToLoad".Translate() + ".";
            }
            return "ReadyForLaunch".Translate();
        }

        private void StartChoosingDestination()
        {
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.parent));
            Find.WorldSelector.ClearSelection();
            int tile = this.parent.Map.Tile;
            this.carr = null;
            Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, CompLaunchableHelicopter.TargeterMouseAttachment, true, delegate
            {
                GenDraw.DrawWorldRadiusRing(tile, this.MaxLaunchDistance);
            }, delegate (GlobalTargetInfo target)
            {
                if (!target.IsValid)
                {
                    return null;
                }
                int num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile, true, int.MaxValue);
                if (num > this.MaxLaunchDistance)
                {
                    GUI.color = Color.red;
                    if (num > this.MaxLaunchDistanceEverPossible)
                    {
                        return "TransportPodDestinationBeyondMaximumRange".Translate();
                    }
                    return "TransportPodNotEnoughFuel".Translate();
                }
                else
                {
                    IEnumerable<FloatMenuOption> transportPodsFloatMenuOptionsAt = this.GetTransportPodsFloatMenuOptionsAt(target.Tile);
                    if (!transportPodsFloatMenuOptionsAt.Any<FloatMenuOption>())
                    {
                        if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
                        {
                           
                            return ("MessageTransportPodsDestinationIsInvalid".Translate());

                        }
                        return string.Empty;
                    }
                    if (transportPodsFloatMenuOptionsAt.Count<FloatMenuOption>() == 1)
                    {
                        if (transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Disabled)
                        {
                            GUI.color = Color.red;
                        }
                        return transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Label;
                    }
                    MapParent mapParent = target.WorldObject as MapParent;
                    if (mapParent != null)
                    {
                        return "ClickToSeeAvailableOrders_WorldObject".Translate(mapParent.LabelCap);
                    }
                    return "ClickToSeeAvailableOrders_Empty".Translate();
                }
            });
        }




        // Token: 0x060028D8 RID: 10456 RVA: 0x0013512C File Offset: 0x0013352C
        public void WorldStartChoosingDestination(Caravan car)
        {
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(car));
            Find.WorldSelector.ClearSelection();
            int tile = car.Tile;
            this.carr = car;
            Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, CompLaunchableHelicopter.TargeterMouseAttachment, false, delegate
            {
                GenDraw.DrawWorldRadiusRing(car.Tile, this.MaxLaunchDistance);
            }, delegate (GlobalTargetInfo target)
            {
                if (!target.IsValid)
                {
                    return null;
                }
                int num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile, true, int.MaxValue);
                if (num > this.MaxLaunchDistance)
                {
                    GUI.color = Color.red;
                    if (num > this.MaxLaunchDistanceEverPossible)
                    {
                        return "TransportPodDestinationBeyondMaximumRange".Translate();
                    }
                    return "TransportPodNotEnoughFuel".Translate();
                }
                else
                {
                   
                    IEnumerable<FloatMenuOption> transportPodsFloatMenuOptionsAt = this.GetTransportPodsFloatMenuOptionsAt(target.Tile,car);
                    if (!transportPodsFloatMenuOptionsAt.Any<FloatMenuOption>())
                    {
                        if (Find.WorldGrid[target.Tile].biome.impassable||Find.World.Impassable(target.Tile))
                        {
                            return ("MessageTransportPodsDestinationIsInvalid".Translate());
                           
                        }
                        return string.Empty;
                    }
                    if (transportPodsFloatMenuOptionsAt.Count<FloatMenuOption>() == 1)
                    {
                        if (transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Disabled)
                        {
                            GUI.color = Color.red;
                        }
                        return transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Label;
                    }
                    MapParent mapParent = target.WorldObject as MapParent;
                    if (mapParent != null)
                    {
                        return "ClickToSeeAvailableOrders_WorldObject".Translate(mapParent.LabelCap);
                    }
                    return "ClickToSeeAvailableOrders_Empty".Translate();
                    
                    //return "DI!";

                }
            });
        }

        // Token: 0x060028D9 RID: 10457 RVA: 0x001351B0 File Offset: 0x001335B0
        private bool ChoseWorldTarget(GlobalTargetInfo target)
        {
           
            if(this.carr==null)
            if (!this.LoadingInProgressOrReadyToLaunch)
            {
                return true;
            }
            
            if (!target.IsValid)
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            /*
            if (!target.HasWorldObject)
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            */


            int myTile = -2;
            if (this.carr == null)
            {
                myTile = this.parent.Map.Tile;
            }else
            {
                myTile = carr.Tile;
            }

            int num = Find.WorldGrid.TraversalDistanceBetween(myTile, target.Tile, true, int.MaxValue);
            if (num > this.MaxLaunchDistance)
            {
                Messages.Message("MessageTransportPodsDestinationIsTooFar".Translate(CompLaunchableHelicopter.FuelNeededToLaunchAtDist((float)num).ToString("0.#")), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
            {

                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }

            MapParent mapParent = Find.WorldObjects.MapParentAt(target.Tile);


            IEnumerable<FloatMenuOption> transportPodsFloatMenuOptionsAt = this.GetTransportPodsFloatMenuOptionsAt(target.Tile,carr);
           
            
            if (!transportPodsFloatMenuOptionsAt.Any<FloatMenuOption>())
            {
                if (Find.WorldGrid[target.Tile].biome.impassable || Find.World.Impassable(target.Tile))
                {
                    
                    Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                this.TryLaunch(target.Tile, null,null);
                return true;
            }
            else
            {
                if (transportPodsFloatMenuOptionsAt.Count<FloatMenuOption>() == 1)
                {
                    if (!transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().Disabled)
                    {
                        transportPodsFloatMenuOptionsAt.First<FloatMenuOption>().action();
                    }
                    return false;
                }
                Find.WindowStack.Add(new FloatMenu(transportPodsFloatMenuOptionsAt.ToList<FloatMenuOption>()));
                return false;
            }
            

        }




        // Token: 0x060028DA RID: 10458 RVA: 0x001352F0 File Offset: 0x001336F0
        public void TryLaunch(int destinationTile, TransportPodsArrivalAction arrivalAction,Caravan cafr=null)
        {
            //Log.Warning("CARR:" + this.carr+"/"+cafr);
            if (cafr==null)
            if (!this.parent.Spawned)
            {
                Log.Error("Tried to launch " + this.parent + ", but it's unspawned.", false);
                return;
            }
            /*
            List<CompTransporter> transportersInGroup = this.TransportersInGroup;
            if (transportersInGroup == null)
            {
                Log.Error("Tried to launch " + this.parent + ", but it's not in any group.", false);
                return;
            }
            */
            if (this.parent.Spawned) {
                if (!this.LoadingInProgressOrReadyToLaunch)
                {
                    return;
                }
            }
            if (!this.AllInGroupConnectedToFuelingPort || !this.AllFuelingPortSourcesInGroupHaveAnyFuel)
            {
                
                return;
            }
            if (cafr==null)
            {
            Map map = this.parent.Map;
            int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, destinationTile, true, int.MaxValue);
            if (num > this.MaxLaunchDistance)
            {
                return;
            }
            this.Transporter.TryRemoveLord(map);
            int groupID = this.Transporter.groupID;
            float amount = Mathf.Max(CompLaunchableHelicopter.FuelNeededToLaunchAtDist((float)num), 1f);
            //for (int i = 0; i < transportersInGroup.Count; i++)
            
                CompTransporter compTransporter = this.FuelingPortSource.TryGetComp<CompTransporter>();//transportersInGroup[i];
                Building fuelingPortSource = this.FuelingPortSource;//compTransporter.Launchable.FuelingPortSource;
                if (fuelingPortSource != null)
                {
                    fuelingPortSource.TryGetComp<CompRefuelable>().ConsumeFuel(amount);
                }
                ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();

                Thing helicopter = ThingMaker.MakeThing(ThingDef.Named("Building_Helicopter"));
                helicopter.SetFactionDirect(Faction.OfPlayer);

                CompRefuelable compr= helicopter.TryGetComp<CompRefuelable>();
                Type tcr = compr.GetType();
                FieldInfo finfos = tcr.GetField("fuel",BindingFlags.NonPublic|BindingFlags.Instance);
                finfos.SetValue(compr, fuelingPortSource.TryGetComp<CompRefuelable>().Fuel);

                helicopter.stackCount = 1;
                directlyHeldThings.TryAddOrTransfer(helicopter);

                ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"), null);
                activeDropPod.Contents = new ActiveDropPodInfo();
                activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true, true);
                HelicopterLeaving dropPodLeaving = (HelicopterLeaving)SkyfallerMaker.MakeSkyfaller(ThingDef.Named("HelicopterLeaving"), activeDropPod);
                dropPodLeaving.groupID = groupID;
                dropPodLeaving.destinationTile = destinationTile;
                dropPodLeaving.arrivalAction = arrivalAction;
                compTransporter.CleanUpLoadingVars(map);
            //compTransporter.parent
                IntVec3 poc = fuelingPortSource.Position;
            // fuelingPortSource.Destroy(DestroyMode.Vanish);
            HelicopterStatic.HelicopterDestroy(fuelingPortSource,DestroyMode.Vanish);
            GenSpawn.Spawn(dropPodLeaving, poc, map, WipeMode.Vanish);
            
            CameraJumper.TryHideWorld();
            }else
            {
                int num = Find.WorldGrid.TraversalDistanceBetween(carr.Tile, destinationTile, true, int.MaxValue);
                if (num > this.MaxLaunchDistance)
                {
                    return;
                }
                float amount = Mathf.Max(CompLaunchableHelicopter.FuelNeededToLaunchAtDist((float)num), 1f);
                if (FuelingPortSource != null)
                {
                    FuelingPortSource.TryGetComp<CompRefuelable>().ConsumeFuel(amount);
                }

                
                ThingOwner<Pawn> directlyHeldThings = (ThingOwner<Pawn>)cafr.GetDirectlyHeldThings();
                Thing helicopter = null;
                foreach (Pawn pawn in directlyHeldThings.InnerListForReading)
                {
                    Pawn_InventoryTracker pinv = pawn.inventory;
                    for (int i = 0; i < pinv.innerContainer.Count; i++)
                    {
                        if (pinv.innerContainer[i].def.defName == ("Building_Helicopter"))
                        {
                            helicopter = pinv.innerContainer[i];
                            pinv.innerContainer[i].holdingOwner.Remove(pinv.innerContainer[i]);
                           
                            break;
                        }
                    }
                }

                ThingOwner<Thing> finalto = new ThingOwner<Thing>();
                List<Pawn> lpto = directlyHeldThings.AsEnumerable<Pawn>().ToList();
                foreach(Pawn p in lpto)
                {
                    finalto.TryAddOrTransfer(p);
                }


                if (helicopter != null)
                {
                   // Log.Warning("TRY ADD"+helicopter);
                    if(helicopter.holdingOwner==null)
                        //Log.Warning("NULL");
                    //directlyHeldThings.
                        finalto.TryAddOrTransfer(helicopter,false);
                }
               

                ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"), null);
                activeDropPod.Contents = new ActiveDropPodInfo();
                activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(
                    //directlyHeldThings
                    finalto, true, true);
                
                cafr.RemoveAllPawns();
                if (cafr.Spawned)
                {
                    Find.WorldObjects.Remove(cafr);
                }
                
                TravelingTransportPods travelingTransportPods = (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("TravelingHelicopters", true));
                travelingTransportPods.Tile = cafr.Tile;
                travelingTransportPods.SetFaction(Faction.OfPlayer);
                travelingTransportPods.destinationTile = destinationTile;
                travelingTransportPods.arrivalAction = arrivalAction;
                Find.WorldObjects.Add(travelingTransportPods);
                travelingTransportPods.AddPod(activeDropPod.Contents, true);
                activeDropPod.Contents = null;
                activeDropPod.Destroy(DestroyMode.Vanish);
                // CameraJumper.TryHideWorld();
                Find.WorldTargeter.StopTargeting();
            }

        }

        // Token: 0x060028DB RID: 10459 RVA: 0x001354B0 File Offset: 0x001338B0
        public void Notify_FuelingPortSourceDeSpawned()
        {
            if (this.Transporter.CancelLoad())
            {
                Messages.Message("MessageTransportersLoadCanceled_FuelingPortGiverDeSpawned".Translate(), this.parent, MessageTypeDefOf.NegativeEvent, true);
            }
        }

        // Token: 0x060028DC RID: 10460 RVA: 0x001354E2 File Offset: 0x001338E2
        public static int MaxLaunchDistanceAtFuelLevel(float fuelLevel)
        {
            return Mathf.FloorToInt(fuelLevel / 2.25f);
        }

        // Token: 0x060028DD RID: 10461 RVA: 0x001354F0 File Offset: 0x001338F0
        public static float FuelNeededToLaunchAtDist(float dist)
        {
            return 2.25f * dist;
        }

        // Token: 0x060028DE RID: 10462 RVA: 0x001354FC File Offset: 0x001338FC
        public IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptionsAt(int tile,Caravan car=null)
        {
            bool anything = false;
            IEnumerable<IThingHolder> pods = this.TransportersInGroup.Cast<IThingHolder>();
            if (car != null)
            {
                List<Caravan> rliss = new List<Caravan>();
                rliss.Add(car);
                pods = rliss.Cast<IThingHolder>();

               
            }

            if (car == null)
            {
                if (TransportPodsArrivalAction_FormCaravan.CanFormCaravanAt(pods, tile) && !Find.WorldObjects.AnySettlementBaseAt(tile) && !Find.WorldObjects.AnySiteAt(tile))
                {
                    anything = true;
                    yield return new FloatMenuOption("FormCaravanHere".Translate(), delegate
                    {
                        this.TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan(), car);

                    }, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
            }else
            {
                if(!Find.WorldObjects.AnySettlementBaseAt(tile) && !Find.WorldObjects.AnySiteAt(tile)&&!Find.World.Impassable(tile))
                {
                    anything = true;
                    yield return new FloatMenuOption("FormCaravanHere".Translate(), delegate
                    {
                        this.TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan(), car);

                    }, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
            }
            
            List<WorldObject> worldObjects = Find.WorldObjects.AllWorldObjects;
            for (int i = 0; i < worldObjects.Count; i++)
            {
                if (worldObjects[i].Tile == tile)
                {
                    IEnumerable<FloatMenuOption> nowre = HelicopterStatic.getFM(worldObjects[i], pods, this,car);
                    if (nowre.ToList().Count < 1)
                    {
                        yield return new FloatMenuOption("FormCaravanHere".Translate(), delegate
                        {
                            this.TryLaunch(tile, new TransportPodsArrivalAction_FormCaravan(), car);
                        }, MenuOptionPriority.Default, null, null, 0f, null, null);
                    }
                    else
                    foreach (FloatMenuOption o in nowre)//worldObjects[i].GetTransportPodsFloatMenuOptions(this.TransportersInGroup.Cast<IThingHolder>(), this))
                    {
                        anything = true;
                        yield return o;
                    }
                }
            }

            
            if (!anything && !Find.World.Impassable(tile))
            {
                yield return new FloatMenuOption("TransportPodsContentsWillBeLost".Translate(), delegate
                {
                    this.TryLaunch(tile, null);
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
           
            yield break;
        }

        // Token: 0x040016BC RID: 5820
        private CompTransporter cachedCompTransporter;

        // Token: 0x040016BD RID: 5821
        public static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment", true);

        // Token: 0x040016BE RID: 5822
        private static readonly Texture2D LaunchCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true);

        // Token: 0x040016BF RID: 5823
        private const float FuelPerTile = 2.25f;

        private Caravan carr ;
    }
}
