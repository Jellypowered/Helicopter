using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Helicopter
{
    [StaticConstructorOnStartup]
    internal static class HarmonyBoots
    {
        static HarmonyBoots()
        {
            var harmony = new Harmony("jelly.helicopter");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}