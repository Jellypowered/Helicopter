using HarmonyLib;
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
    //helicopter direct place
    [HarmonyPatch(typeof(ActiveDropPod), "PodOpen", new Type[] { })]
    public static class HarmonyTest_dp
    {
        public static void Prefix(ActiveDropPod __instance)
        {
            ActiveDropPodInfo activeDropPodInfo = Traverse.Create((object)__instance).Field("contents").GetValue<ActiveDropPodInfo>();
            for (int index = activeDropPodInfo.innerContainer.Count - 1; index >= 0; --index)
            {
                Thing thing = activeDropPodInfo.innerContainer[index];
                if (thing?.TryGetComp<CompLaunchableHelicopter>() != null)
                {
                    GenSpawn.Spawn(thing, __instance.Position, __instance.Map, thing.Rotation);
                    break;
                }
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
        if (contents == null || contents.Count <= 0) return;

        for (int i = 0; i < contents.Count; i++)
        {
            if (contents[i].def.defName != "Building_Helicopter")
            {
                newResult.Add(contents[i]);
            }
        }
        tv.Field("playerCaravanAllPawnsAndItems").SetValue(newResult);
    }
}

//helicopter incoming, Edge Code thanks to SmashPhil and Neceros of SRTS-Expanded!
[HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt", new Type[] { typeof(IntVec3), typeof(Map), typeof(ActiveDropPodInfo) })]
public static class HarmonyTest
{
    public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
    {
        for (int index = 0; index < info.innerContainer.Count; index++)
        {
            if (info.innerContainer[index].TryGetComp<Helicopter.CompLaunchableHelicopter>() != null)
            {
                _ = info.innerContainer[index];
                ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named("ActiveHelicopter"), null);
                activeDropPod.Contents = info;

                EnsureInBounds(ref c, info.innerContainer[index].def, map);
                SkyfallerMaker.SpawnSkyfaller(ThingDef.Named("HelicopterIncoming"), activeDropPod, c, map);
                return false;
            }
        }
        return true;
    }

    private static void EnsureInBounds(ref IntVec3 c, ThingDef helicopter, Map map)
    {
        if (helicopter is null)
        {
            throw new ArgumentNullException(nameof(helicopter));
        }

        int x = (int)9;
        int y = (int)9;
        int offset = x > y ? x : y;

        if (c.x < offset)
        {
            c.x = offset;
        }
        else if (c.x >= (map.Size.x - offset))
        {
            c.x = map.Size.x - offset;
        }
        if (c.z < offset)
        {
            c.z = offset;
        }
        else if (c.z > (map.Size.z - offset))
        {
            c.z = map.Size.z - offset;
        }
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
            if (lpc.parent.TryGetComp<Helicopter.CompLaunchableHelicopter>() != null)
            {
                Map map = tv.Field("map").GetValue<Map>();
                List<Pawn> list = CaravanFormingUtility.AllSendablePawns(map, true, true, true, true);
                for (int i = 0; i < list.Count; i++)
                {
                    Type typ = __instance.GetType();
                    MethodInfo minfo = typ.GetMethod("AddToTransferables", BindingFlags.NonPublic | BindingFlags.Instance);
                    minfo.Invoke(__instance, new object[] { list[i] });
                    // __instance.AddToTransferables(list[i]);
                }
                return false;
            }
        }
        return true;
    }
}

/* Akreedz original patch */

[HarmonyPatch(typeof(TransportPodsArrivalAction_LandInSpecificCell), "Arrived", new System.Type[] { typeof(List<ActiveDropPodInfo>), typeof(int) })]
public static class HarmonyTest_AJ
{
    public static bool Prefix(TransportPodsArrivalAction_LandInSpecificCell __instance, List<ActiveDropPodInfo> pods, int tile)
    {
        foreach (ActiveDropPodInfo pod in pods)
        {
            for (int index = 0; index < pod.innerContainer.Count; index++)
            {
                if (pod.innerContainer[index].TryGetComp<Helicopter.CompLaunchableHelicopter>() != null || DefDatabase<ThingDef>.GetNamed(pod.innerContainer[index]?.def?.defName?.Split('_')[0], false)?.GetCompProperties<Helicopter.CompProperties_LaunchableHelicopter>() != null)
                {
                    Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(pods);
                    Traverse traverse = Traverse.Create((object)__instance);
                    IntVec3 c = traverse.Field("cell").GetValue<IntVec3>();
                    Map map = traverse.Field("mapParent").GetValue<MapParent>().Map;
                    TransportPodsArrivalActionUtility.RemovePawnsFromWorldPawns(pods);
                    for (int i = 0; i < pods.Count; ++i)
                    {
                        pods[i].openDelay = 0;
                        DropPodUtility.MakeDropPodAt(c, map, pods[i]);
                    }
                    Messages.Message("MessageTransportPodsArrived".Translate(), (LookTargets)lookTarget, MessageTypeDefOf.TaskCompletion, true);
                    return false;
                }
            }
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
    public static void Postfix(Caravan __instance, ref IEnumerable<Gizmo> __result)
    {
        float masss = 0;
        foreach (Pawn pawn in __instance.pawns.InnerListForReading)
        {
            for (int j = 0; j < pawn.inventory.innerContainer.Count; j++)
            {
                if (pawn.inventory.innerContainer[j].def.defName != "Building_Helicopter")
                    masss += pawn.inventory.innerContainer[j].def.BaseMass * pawn.inventory.innerContainer[j].stackCount;
            }
        }

        foreach (Pawn pawn in __instance.pawns.InnerListForReading)
        {
            Pawn_InventoryTracker pinv = pawn.inventory;
            for (int i = 0; i < pinv.innerContainer.Count; i++)
            {
                if (pinv.innerContainer[i].def.defName == "Building_Helicopter")
                {
                    Command_Action launch = new Command_Action
                    {
                        defaultLabel = "CommandLaunchGroup".Translate(),
                        defaultDesc = "CommandLaunchGroupDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true),
                        alsoClickIfOtherInGroupClicked = false,
                        action = delegate
                        {
                            float maxmass = pinv.innerContainer[i].TryGetComp<CompTransporter>().Props.massCapacity;
                            if (masss <= maxmass)
                                pinv.innerContainer[i].TryGetComp<Helicopter.CompLaunchableHelicopter>().WorldStartChoosingDestination(__instance);
                            else
                                Messages.Message("TooBigTransportersMassUsage".Translate() + "(" + (maxmass - masss) + "KG)", MessageTypeDefOf.RejectInput, false);
                        }
                    };

                    List<Gizmo> newr = __result.ToList();
                    newr.Add(launch);

                    Command_Action addFuel = new Command_Action
                    {
                        defaultLabel = "CommandAddFuel".Translate(),
                        defaultDesc = "CommandAddFuelDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("Things/Item/Resource/Chemfuel", true),
                        alsoClickIfOtherInGroupClicked = false,
                        action = delegate
                        {
                            bool hasAddFuel = false;
                            int fcont = 0;
                            CompRefuelable comprf = pinv.innerContainer[i].TryGetComp<CompRefuelable>();
                            List<Thing> list = CaravanInventoryUtility.AllInventoryItems(__instance);
                            _ = pinv.innerContainer.Count;
                            for (int j = 0; j < list.Count; j++)
                            {
                                if (list[j].def == ThingDefOf.Chemfuel)
                                {
                                    fcont = list[j].stackCount;
                                    Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(__instance, list[j]);
                                    float need = comprf.Props.fuelCapacity - comprf.Fuel;

                                    if (need < 1f && need > 0)
                                    {
                                        fcont = 1;
                                    }
                                    if (fcont * 1f >= need)
                                    {
                                        fcont = (int)need;
                                    }

                                    // Log.Warning("f&n is "+fcont+"/"+need);
                                    if (list[j].stackCount * 1f <= fcont)
                                    {
                                        list[j].stackCount -= fcont;
                                        Thing thing = list[j];
                                        ownerOf.inventory.innerContainer.Remove(thing);
                                        thing.Destroy(DestroyMode.Vanish);
                                    }
                                    else
                                    {
                                        if (fcont != 0)
                                            list[j].SplitOff(fcont).Destroy(DestroyMode.Vanish);
                                    }

                                    Type crtype = comprf.GetType();
                                    FieldInfo finfo = crtype.GetField("fuel", BindingFlags.NonPublic | BindingFlags.Instance);
                                    finfo.SetValue(comprf, comprf.Fuel + fcont);
                                    hasAddFuel = true;
                                    break;
                                }
                            }
                            if (hasAddFuel)
                            {
                                Messages.Message("AddFuelDoneMsg".Translate(fcont, comprf.Fuel), MessageTypeDefOf.PositiveEvent, false);
                            }
                            else
                            {
                                Messages.Message("NonOilMsg".Translate(), MessageTypeDefOf.RejectInput, false);
                            }
                        }
                    };

                    newr.Add(addFuel);

                    Helicopter.Gizmo_MapRefuelableFuelStatus fuelStat = new Helicopter.Gizmo_MapRefuelableFuelStatus
                    {
                        nowFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Fuel,
                        maxFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.fuelCapacity,
                        compLabel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.FuelGizmoLabel
                    };

                    newr.Add(fuelStat);

                    __result = newr;
                    return;
                }
            }
        }
    }
}