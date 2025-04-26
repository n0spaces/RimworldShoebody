using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace RimworldShoebody;

public static class ShoebodySustainerHelpers
{
    public static IEnumerable<Sustainer> FindAll()
    {
        var allSustainers = Find.SoundRoot.sustainerManager.AllSustainers;
        var shoebodySustainers = allSustainers.Where(s => !s.Ended && s.def.defName.StartsWith("Shoebody_Sound"));
        return shoebodySustainers;
    }
    
    public static Sustainer Create(TargetInfo maker)
    {
        var soundDef = ShoebodyModSettings.CurrentDoubleSpeedSetting
            ? SoundDef.Named("Shoebody_Sound2x")
            : SoundDef.Named("Shoebody_Sound");
        var s = new Sustainer(soundDef, SoundInfo.InMap(maker));
        s.externalParams["ShoebodySound_VolumeParam"] = ShoebodyModSettings.CurrentVolumeSetting;
        return s;
    }
    
    public static void EndAll()
    {
        foreach (var sustainer in FindAll())
        {
            sustainer.End();
        }
    }

    public static void UpdateVolume()
    {
        foreach (var sustainer in FindAll())
        {
            sustainer.externalParams["ShoebodySound_VolumeParam"] = ShoebodyModSettings.CurrentVolumeSetting;
        }
    }
}