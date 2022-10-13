using Verse;

namespace Helicopter
{
    public class CompProperties_LaunchableHelicopter : CompProperties
    {
        // Token: 0x06000A9A RID: 2714 RVA: 0x00054A5B File Offset: 0x00052E5B
        public CompProperties_LaunchableHelicopter()
        {
            this.compClass = typeof(CompLaunchableHelicopter);
        }

        public float fuelPerTile = 2.25f;
    }
}