using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace RimworldShoebody;

[StaticConstructorOnStartup]
public static class ShoebodyDistRangeHelper
{
    static ShoebodyDistRangeHelper()
    {
        UpdateSubSoundDefDistRange();
    }
    
    /// <summary>
    /// Update all Shoebody SubSoundDef DistRanges to match the value set in the mod settings.
    /// This should be applied at startup (after defs are loaded) and any time the value changes.
    /// </summary>
    public static void UpdateSubSoundDefDistRange()
    {
        var intRange = ShoebodyModSettings.CurrentDistRangeSetting;
        var floatRange = new FloatRange(intRange.min, intRange.max);
        
        var soundDef1 = SoundDef.Named("Shoebody_Sound");
        var soundDef2 = SoundDef.Named("Shoebody_Sound2x");
        IEnumerable<SubSoundDef> subSoundDefs = [.. soundDef1.subSounds, .. soundDef2.subSounds];

        foreach (var sub in subSoundDefs)
        {
            sub.distRange = floatRange;
        }

        ThingPlaysShoebodyComp.StaticSoundRange = floatRange;
    }
}