using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace Helicopter
{
    public static class HelicopterStatic
    {

        public static IEnumerable<FloatMenuOption> GetFM(WorldObject wobj, IEnumerable<IThingHolder> ih, CompLaunchableHelicopter comp, Caravan car)
        {

            if (wobj is Caravan)
            {
                return Enumerable.Empty<FloatMenuOption>();
            }
            else if (wobj is Site)
            {
                return GetSite(wobj as Site, ih, comp, car);

            }
            else if (wobj is Settlement)
            {
                return GetSettle(wobj as Settlement, ih, comp, car);
            }
            else if (wobj is MapParent)
            {
                return GetMapParent(wobj as MapParent, ih, comp, car);
            }

            return Enumerable.Empty<FloatMenuOption>();

        }



        public static IEnumerable<FloatMenuOption> GetMapParent(MapParent mapparent, IEnumerable<IThingHolder> pods, CompLaunchableHelicopter representative, Caravan car)
        {
            /*
            foreach (FloatMenuOption o in mapparent.GetFloatMenuOptions())
            {
                yield return o;
            }
            */

            if (TransportPodsArrivalAction_LandInSpecificCell.CanLandInSpecificCell(pods, mapparent))
            {
                yield return new FloatMenuOption("LandInExistingMap".Translate(mapparent.Label), delegate
                {

                    Map myMap;
                    if (car == null)
                        myMap = representative.parent.Map;
                    else
                        myMap = null;

                    Map map = mapparent.Map;
                    Current.Game.CurrentMap = map;
                    CameraJumper.TryHideWorld();
                    Find.Targeter.BeginTargeting(TargetingParameters.ForDropPodsDestination(), delegate (LocalTargetInfo x)
                    {
                        representative.TryLaunch(mapparent.Tile, new TransportPodsArrivalAction_LandInSpecificCell(mapparent, x.Cell), car);
                    }, null, delegate
                    {

                        if (myMap != null && Find.Maps.Contains(myMap))
                        {
                            Current.Game.CurrentMap = myMap;
                        }

                    }, CompLaunchable.TargeterMouseAttachment);
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            yield break;
        }

        public static IEnumerable<FloatMenuOption> GetSite(Site site, IEnumerable<IThingHolder> pods, CompLaunchableHelicopter representative, Caravan car)
        {
            foreach (FloatMenuOption o in GetMapParent(site, pods, representative, car))
            {
                yield return o;
            }
            foreach (FloatMenuOption o2 in GetVisitSite(representative, pods, site, car))
            {
                yield return o2;
            }
            yield break;
        }

        public static IEnumerable<FloatMenuOption> GetVisitSite(CompLaunchableHelicopter representative, IEnumerable<IThingHolder> pods, Site site, Caravan car)
        {
            foreach (FloatMenuOption f in HelicoptersArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_VisitSite>(() => TransportPodsArrivalAction_VisitSite.CanVisit(pods, site), () => new TransportPodsArrivalAction_VisitSite(site, PawnsArrivalModeDefOf.EdgeDrop), "DropAtEdge".Translate(), representative, site.Tile, car))
            {
                yield return f;
            }
            foreach (FloatMenuOption f2 in HelicoptersArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_VisitSite>(() => TransportPodsArrivalAction_VisitSite.CanVisit(pods, site), () => new TransportPodsArrivalAction_VisitSite(site, PawnsArrivalModeDefOf.CenterDrop), "DropInCenter".Translate(), representative, site.Tile, car))
            {
                yield return f2;
            }
            yield break;
        }

        public static IEnumerable<FloatMenuOption> GetSettle(Settlement bs, IEnumerable<IThingHolder> pods, CompLaunchableHelicopter representative, Caravan car)
        {
            foreach (FloatMenuOption o in GetMapParent(bs, pods, representative, car))
            {
                yield return o;
            }

            foreach (FloatMenuOption f in HelicoptersArrivalActionUtility.GetVisitFloatMenuOptions(representative, pods, bs, car))
            {
                yield return f;
            }

            foreach (FloatMenuOption f2 in HelicoptersArrivalActionUtility.GetGIFTFloatMenuOptions(representative, pods, bs, car))
            {
                yield return f2;
            }
            foreach (FloatMenuOption f3 in HelicoptersArrivalActionUtility.GetATKFloatMenuOptions(representative, pods, bs, car))
            {
                yield return f3;
            }
            yield break;
        }

        public static void HelicopterDestroy(Thing thing, DestroyMode mode = DestroyMode.Vanish)
        {
            if (!Thing.allowDestroyNonDestroyable && !thing.def.destroyable)
            {
                Log.Error("Tried to destroy non-destroyable thing " + thing);
                return;
            }
            if (thing.Destroyed)
            {
                Log.Error("Tried to destroy already-destroyed thing " + thing);
                return;
            }
            //bool spawned = thing.Spawned;
            //Map map = thing.Map;
            if (thing.Spawned)
            {
                thing.DeSpawn(mode);
            }
            Type typ = typeof(Thing);
            FieldInfo finfo = typ.GetField("mapIndexOrState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            SByte sbt = -2;
            finfo.SetValue(thing, sbt);



            if (thing.def.DiscardOnDestroyed)
            {
                thing.Discard(false);
            }

            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.Notify_ContainedItemDestroyed(thing);
            }
            MethodInfo minfo = typ.GetMethod("RemoveAllReservationsAndDesignationsOnThis", BindingFlags.NonPublic | BindingFlags.Instance);
            minfo.Invoke(thing, null);

            if (!(thing is Pawn))
            {
                thing.stackCount = 0;
            }
        }
    }
}
