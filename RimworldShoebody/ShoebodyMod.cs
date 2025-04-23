using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace RimworldShoebody;

[UsedImplicitly]
public class ShoebodyMod : Mod
{
    private readonly ShoebodyModSettings _settings;

    public ShoebodyMod(ModContentPack content) : base(content)
    {
        _settings = GetSettings<ShoebodyModSettings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();
        listing.Begin(inRect);
        listing.CheckboxLabeled("Enabled", ref _settings.ShoebodyEnabled,
            "If checked, the Shoebody Bop will occasionally play near water, " +
            "generally when pawns are attacking or downed.\n\n" +
            "Uncheck if this shit gets too annoying.");
        _settings.ShoebodyVolume = listing.SliderLabeled($"Volume: {_settings.ShoebodyVolume * 100:f0}%",
            _settings.ShoebodyVolume, 0f, 1f, 0.3f, "Volume of the Shoebody Bop. Default is 35%");
        
        listing.End();
    }

    public override string SettingsCategory() => "Shoebody Bop";
}