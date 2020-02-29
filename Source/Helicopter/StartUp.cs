using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Verse;

namespace Helicopter
{
    [StaticConstructorOnStartup]
    public static class StartUp
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static StartUp()
        {
            var harmony = new Harmony("Helicopter.AKreedz"); 
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // ((Texture2D[])typeof(Thing).Assembly.GetType("Verse.TexButton").GetField("SpeedButtonTextures").GetValue(null))[4] = ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Ultrafast", true);
        }
    }
}
