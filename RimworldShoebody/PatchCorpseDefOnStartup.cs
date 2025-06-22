using Verse;

namespace RimworldShoebody;

/// <summary>
/// Patches the Corpse_Human things so they get the ThingPlaysShoebodyComp component.
/// This is needed because corpses are defined dynamically at startup, rather than in XML.
/// </summary>
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