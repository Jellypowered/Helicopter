using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Helicopter
{

    //helicopter direct place
    [HarmonyPatch(typeof(ActiveDropPod), "PodOpen", new Type[]
    {
      
    })]
    public static class HarmonyTest_dp
    {
       
        public static void Prefix(ActiveDropPod __instance)
        {
            Traverse tv = Traverse.Create(__instance);
            ActiveDropPodInfo contents = tv.Field("contents").GetValue<ActiveDropPodInfo>();
            for (int i = contents.innerContainer.Count - 1; i >= 0; i--)
            {
                Thing thing = contents.innerContainer[i];
                Thing thing2;
                if (thing != null && thing.def.defName == "Building_Helicopter") {
                    GenPlace.TryPlaceThing(thing, __instance.Position, __instance.Map, ThingPlaceMode.Direct, out thing2, delegate (Thing placedThing, int count)
                    {
                        if (Find.TickManager.TicksGame < 1200 && TutorSystem.TutorialMode && placedThing.def.category == ThingCategory.Item)
                        {
                            Find.TutorialState.AddStartingItem(placedThing);
                        }
                    }, null);
                    break;
                }
            }



            }

    }



    //helicopter's mass will not appear in trade window
    [HarmonyPatch(typeof(Dialog_Trade), "SetupPlayerCaravanVariables", new Type[]
    {

    })]
    public static class HarmonyTest_trade
    {

        public static void Postfix(Dialog_Trade __instance)
        {
            Traverse tv = Traverse.Create(__instance);
            List<Thing> contents = tv.Field("playerCaravanAllPawnsAndItems").GetValue<List<Thing>>();
            List<Thing> newResult = new List<Thing>();
            if (contents==null ||contents.Count <= 0) return;

            for(int i = 0;i<contents.Count;i++)
            {
                
                if (contents[i].def.defName != "Building_Helicopter")
                {
                    newResult.Add(contents[i]);
                }
            }
            tv.Field("playerCaravanAllPawnsAndItems").SetValue(newResult);



        }

    }



    //helicopter incoming
    [HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt", new Type[]
    {
        typeof(IntVec3), typeof(Map), typeof(ActiveDropPodInfo)
    })]
    public static class HarmonyTest
    {
        public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            if (info.innerContainer.Contains(ThingDef.Named("Building_Helicopter")))
            {
                ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"), null);
                activeDropPod.Contents = info;
                SkyfallerMaker.SpawnSkyfaller(ThingDef.Named("HelicopterIncoming"), activeDropPod, c, map);
                return false;
            }
            return true;


        }

    }


    //helicopter can take down and mad pawn
    [HarmonyPatch(typeof(Dialog_LoadTransporters), "AddPawnsToTransferables", new Type[]
    {
      
    })]
    public static class HarmonyTest_C
    {
        public static bool Prefix(Dialog_LoadTransporters __instance)
        {
            Traverse tv = Traverse.Create(__instance);
            List<CompTransporter> lp = tv.Field("transporters").GetValue<List<CompTransporter>>();
            foreach (CompTransporter lpc in lp)
            {
                if (lpc.parent.TryGetComp <CompLaunchableHelicopter>()!=null)
                {
                    Map map = tv.Field("map").GetValue<Map>();
                    List<Pawn> list = CaravanFormingUtility.AllSendablePawns(map, true, true,true,true);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Type typ = __instance.GetType();
                        MethodInfo minfo = typ.GetMethod("AddToTransferables",BindingFlags.NonPublic|BindingFlags.Instance);
                        minfo.Invoke(__instance,new object[] { list[i]});
                      // __instance.AddToTransferables(list[i]);
                    }
                    return false;
                }
            }
            return true;


        }

    }

    //helicopter arrive jingzhun
    [HarmonyPatch(typeof(TransportPodsArrivalAction_LandInSpecificCell), "Arrived", new Type[]
    {
        typeof(List<ActiveDropPodInfo>), typeof(int)
    })]
    public static class HarmonyTest_AJ
    {
        public static bool Prefix(TransportPodsArrivalAction_LandInSpecificCell __instance,List<ActiveDropPodInfo> pods, int tile)
        {
            foreach(ActiveDropPodInfo info in pods)
            if (info.innerContainer.Contains(ThingDef.Named("Building_Helicopter")))
            {
                    Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(pods);
                    Traverse tv = Traverse.Create(__instance);
                    IntVec3 c = tv.Field("cell").GetValue <IntVec3>();
                    Map map = tv.Field("mapParent").GetValue<MapParent>().Map;
                    TransportPodsArrivalActionUtility.RemovePawnsFromWorldPawns(pods);
                    for (int i = 0; i < pods.Count; i++)
                    {
                        
                        DropPodUtility.MakeDropPodAt(c, map, pods[i]);
                    }
                    Messages.Message("MessageTransportPodsArrived".Translate(), lookTarget, MessageTypeDefOf.TaskCompletion, true);
                    return false;
            }
            return true;


        }

    }

    //caravan gizmo
    [HarmonyPatch(typeof(Caravan), "GetGizmos", new Type[]
    {
        
    })]
    public static class HarmonyTest_BB
    {
        public static void Postfix(Caravan __instance,ref IEnumerable<Gizmo> __result)
        {
            float masss = 0;
            foreach (Pawn pawn in __instance.pawns.InnerListForReading)
            {
                
                for(int j = 0; j < pawn.inventory.innerContainer.Count; j++)
                {
                    if (pawn.inventory.innerContainer[j].def.defName != ("Building_Helicopter"))
                        masss += (pawn.inventory.innerContainer[j].def.BaseMass * pawn.inventory.innerContainer[j].stackCount);
                }
            }






            foreach(Pawn pawn in __instance.pawns.InnerListForReading)
            {
                Pawn_InventoryTracker pinv = pawn.inventory;
                for (int i=0;i< pinv.innerContainer.Count;i++) {
                    if (pinv.innerContainer[i].def.defName==("Building_Helicopter"))
                    {
                        Command_Action launch = new Command_Action();
                        launch.defaultLabel = "CommandLaunchGroup".Translate();
                        launch.defaultDesc = "CommandLaunchGroupDesc".Translate();
                         launch.icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true);
                        launch.alsoClickIfOtherInGroupClicked = false;
                        launch.action = delegate
                        {
                            float maxmass = pinv.innerContainer[i].TryGetComp<CompTransporter>().Props.massCapacity;
                            if (masss <= maxmass)
                                pinv.innerContainer[i].TryGetComp<CompLaunchableHelicopter>().WorldStartChoosingDestination(__instance);
                            else
                                Messages.Message("TooBigTransportersMassUsage".Translate()+"("+(maxmass-masss)+"KG)", MessageTypeDefOf.RejectInput, false);
                        };

                        List<Gizmo> newr = __result.ToList();
                        newr.Add(launch);

                        Command_Action addFuel = new Command_Action();
                        addFuel.defaultLabel = "CommandAddFuel".Translate();
                        addFuel.defaultDesc = "CommandAddFuelDesc".Translate();
                        addFuel.icon = ContentFinder<Texture2D>.Get("Things/Item/Resource/Chemfuel", true);
                        addFuel.alsoClickIfOtherInGroupClicked = false;
                        addFuel.action = delegate
                        {
                            bool hasAddFuel = false;
                            int fcont = 0;
                            CompRefuelable comprf = pinv.innerContainer[i].TryGetComp<CompRefuelable>();
                            List<Thing> list = CaravanInventoryUtility.AllInventoryItems(__instance);
                            //pinv.innerContainer.Count
                            for (int j = 0; j < list.Count; j++)
                            {

                                if (list[j].def == ThingDefOf.Chemfuel)
                                {
                                    fcont = list[j].stackCount;
                                    Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(__instance, list[j]);
                                    float need = comprf.Props.fuelCapacity-comprf.Fuel;

                                    if (need < 1f && need > 0)
                                    {
                                        fcont = 1;
                                    }
                                    if (fcont * 1f >= need)
                                    {
                                        fcont = (int)need;
                                    }



                                   // Log.Warning("f&n is "+fcont+"/"+need);
                                    if (list[j].stackCount*1f <= fcont)
                                    {
                                        list[j].stackCount -= fcont;
                                        Thing thing = list[j];
                                        ownerOf.inventory.innerContainer.Remove(thing);
                                        thing.Destroy(DestroyMode.Vanish);
                                    }
                                    else
                                    {
                                        if(fcont!=0)
                                         list[j].SplitOff(fcont).Destroy(DestroyMode.Vanish);
                                       
                                    }


                                    Type crtype = comprf.GetType();
                                    FieldInfo finfo = crtype.GetField("fuel",BindingFlags.NonPublic|BindingFlags.Instance);
                                    finfo.SetValue(comprf,comprf.Fuel+fcont);
                                    hasAddFuel = true;
                                    break;

                                }
                            }
                        if (hasAddFuel)
                        {
                                Messages.Message("AddFuelDoneMsg".Translate(fcont, comprf.Fuel), MessageTypeDefOf.PositiveEvent, false);
                            }else
                            {
                                Messages.Message("NonOilMsg".Translate(), MessageTypeDefOf.RejectInput, false);
                            }
                            
                        };

                        newr.Add(addFuel);
                        
                       Gizmo_MapRefuelableFuelStatus fuelStat= new Gizmo_MapRefuelableFuelStatus
                        {
                           nowFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Fuel,
                           maxFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.fuelCapacity,
                           compLabel =  pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.FuelGizmoLabel

                       };
                       
                     
                        newr.Add(fuelStat);

                        __result = newr;
                        return;
                    }
                }
                
            }


        }

    }

    /*
    //helicopter no group action
    [HarmonyPatch(typeof(CompTransporter), "CompGetGizmosExtra", new Type[]
    {
        
    })]
    public static class HarmonyTest_A
    {
        public static void postfix(CompTransporter __instance,ref IEnumerable<Gizmo> __result)
        {
            if (__instance.parent.TryGetComp<CompLaunchableHelicopter>()!=null)
            {
                List<Gizmo> glist = new List<Gizmo>();//__result.ToList();
                if (__instance.LoadingInProgressOrReadyToLaunch)
                {
                    glist.Add(new Command_Action
                    {
                        defaultLabel = "CommandCancelLoad".Translate(),
                        defaultDesc = "CommandCancelLoadDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                        action = delegate
                        {
                            SoundDefOf.Designate_Cancel.PlayOneShotOnCamera(null);
                            __instance.CancelLoad();
                        }
                      });
                }
                else
                {
                    Command_LoadToTransporter loadGroup = new Command_LoadToTransporter();
                    int selectedTransportersCount = 0;
                    for (int i = 0; i < Find.Selector.NumSelected; i++)
                    {
                        Thing thing = Find.Selector.SelectedObjectsListForReading[i] as Thing;
                        if (thing != null && thing.def == __instance.parent.def)
                        {
                            CompLaunchable compLaunchable = thing.TryGetComp<CompLaunchable>();
                            if (compLaunchable == null || (compLaunchable.FuelingPortSource != null && compLaunchable.FuelingPortSourceHasAnyFuel))
                            {
                                selectedTransportersCount++;
                                break;
                            }
                        }
                    }

                    loadGroup.defaultLabel = "CommandLoadTransporter".Translate(new object[]
                    {
                       selectedTransportersCount.ToString()
                    });
                    loadGroup.defaultDesc = "CommandLoadTransporterDesc".Translate();
                    loadGroup.icon = ContentFinder<Texture2D>.Get("UI/Commands/LoadTransporter", true);
                    loadGroup.transComp = __instance;
                    CompLaunchable launchable = __instance.Launchable;
                    if (launchable != null)
                    {
                        if (!launchable.ConnectedToFuelingPort)
                        {
                            loadGroup.Disable("CommandLoadTransporterFailNotConnectedToFuelingPort".Translate());
                        }
                        else if (!launchable.FuelingPortSourceHasAnyFuel)
                        {
                            loadGroup.Disable("CommandLoadTransporterFailNoFuel".Translate());
                        }
                    }
                    glist.Add(loadGroup);
                }



                __result = glist.AsEnumerable<Gizmo>();
            }


        }

    }
    */
}
