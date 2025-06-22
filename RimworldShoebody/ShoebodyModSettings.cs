using Verse;

namespace RimworldShoebody;

public class ShoebodyModSettings : ModSettings
{
    public static bool CurrentEnabledSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().ShoebodyEnabled;

    public static float CurrentVolumeSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().ShoebodyVolume;

    public static bool CurrentDoubleSpeedSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().DoubleSpeed;

    public static bool CurrentSilenceMusicSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().SilenceMusic;

    public static bool CurrentFreshCorpsesSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().OnlyFreshCorpses;

    public bool ShoebodyEnabled = true;

    public float ShoebodyVolume = 25f;

    public bool DoubleSpeed = true;

    public bool SilenceMusic = true;

    public bool OnlyFreshCorpses = true;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ShoebodyEnabled, "shoebodyEnabled", true);
        Scribe_Values.Look(ref ShoebodyVolume, "shoebodyVolume", 25f);
        Scribe_Values.Look(ref DoubleSpeed, "shoebodyDoubleSpeed", true);
        Scribe_Values.Look(ref SilenceMusic, "shoebodySilenceMusic", true);
        Scribe_Values.Look(ref OnlyFreshCorpses, "shoebodyOnlyFreshCorpses", true);
    }
}