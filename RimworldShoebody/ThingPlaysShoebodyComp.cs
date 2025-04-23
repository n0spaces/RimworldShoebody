using RimWorld;
using Verse;

namespace RimworldShoebody;

public class ThingPlaysShoebodyComp : CompPlaysMusic
{
    public override bool Playing => Find.CurrentMap.GetComponent<ShoebodyMapComp>()?.CurrentShoebodyThing == parent;

    public override FloatRange SoundRange { get; } = SoundDef.Named("Shoebody_Sound").subSounds[0].distRange;
}