using Verse;

namespace RimworldShoebody;

[StaticConstructorOnStartup]
public static class PatchCorpseDefOnStartup
{
    static PatchCorpseDefOnStartup()
    {
        var corpseDef = ThingDef.Named("Corpse_Human");
        if (!corpseDef.comps.Any(comp => comp.compClass == typeof(ThingPlaysShoebodyComp)))
        {
            corpseDef.comps.Add(new CompProperties(typeof(ThingPlaysShoebodyComp)));
        }
    }
}