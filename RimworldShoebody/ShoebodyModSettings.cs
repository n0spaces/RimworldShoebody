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

    public static IntRange CurrentDistRangeSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().DistRange;

    public static bool CurrentPsychicRitualSetting =>
        LoadedModManager.GetMod<ShoebodyMod>().GetSettings<ShoebodyModSettings>().PlayOnPsychicRituals;

    private const bool DefaultShoebodyEnabled = true;
    private const float DefaultShoebodyVolume = 0.35f;
    private const bool DefaultDoubleSpeed = true;
    private const bool DefaultSilenceMusic = true;
    private const bool DefaultOnlyFreshCorpses = true;
    private static readonly IntRange DefaultDistRange = new(15, 30);
    private const bool DefaultPlayOnPsychicRituals = true;

    public bool ShoebodyEnabled;
    public float ShoebodyVolume;
    public bool DoubleSpeed;
    public bool SilenceMusic;
    public bool OnlyFreshCorpses;
    public IntRange DistRange;
    public bool PlayOnPsychicRituals;

    public ShoebodyModSettings() => SetDefaults();
    
    public void SetDefaults()
    {
        ShoebodyEnabled = DefaultShoebodyEnabled;
        ShoebodyVolume = DefaultShoebodyVolume;
        DoubleSpeed = DefaultDoubleSpeed;
        SilenceMusic = DefaultSilenceMusic;
        OnlyFreshCorpses = DefaultOnlyFreshCorpses;
        DistRange = DefaultDistRange;
        PlayOnPsychicRituals = DefaultPlayOnPsychicRituals;
    }
    
    public override void ExposeData()
    {
        Scribe_Values.Look(ref ShoebodyEnabled, "shoebodyEnabled", DefaultShoebodyEnabled);
        Scribe_Values.Look(ref ShoebodyVolume, "shoebodyVolume", DefaultShoebodyVolume);
        Scribe_Values.Look(ref DoubleSpeed, "shoebodyDoubleSpeed", DefaultDoubleSpeed);
        Scribe_Values.Look(ref SilenceMusic, "shoebodySilenceMusic", DefaultSilenceMusic);
        Scribe_Values.Look(ref OnlyFreshCorpses, "shoebodyOnlyFreshCorpses", DefaultOnlyFreshCorpses);
        Scribe_Values.Look(ref DistRange, "shoebodyDistRange", DefaultDistRange);
        Scribe_Values.Look(ref PlayOnPsychicRituals, "shoebodyPsychicRituals", DefaultPlayOnPsychicRituals);
    }
}