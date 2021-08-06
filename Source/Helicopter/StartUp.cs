using HarmonyLib;
using System.Reflection;
using Verse;

namespace Helicopter
{
    [StaticConstructorOnStartup]
    public static class StartUp
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static StartUp()
        {
            var harmony = new Harmony("Jellypowered.TransportCargoPod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // ((Texture2D[])typeof(Thing).Assembly.GetType("Verse.TexButton").GetField("SpeedButtonTextures").GetValue(null))[4] = ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Ultrafast", true);
        }
        public class HelicopterMod : Mod
        {
            public static HelicopterMod mod;

            public HelicopterMod(ModContentPack content) : base(content)
            {
                //this.settings = GetSettings<SRTS_ModSettings>();
                mod = this;
            }
        }
    }
}
