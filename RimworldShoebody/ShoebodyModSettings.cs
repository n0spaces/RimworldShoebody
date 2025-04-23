using Verse;

namespace RimworldShoebody;

public class ShoebodyModSettings : ModSettings
{
    public static bool CurrentEnabledSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().ShoebodyEnabled;
    
    public static float CurrentVolumeSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().ShoebodyVolume;
    
    public bool ShoebodyEnabled = true;

    public float ShoebodyVolume = 25f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ShoebodyEnabled, "shoebodyEnabled", true);
        Scribe_Values.Look(ref ShoebodyVolume, "shoebodyVolume", 25f);
    }
}