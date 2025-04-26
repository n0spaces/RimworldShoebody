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

        if (_settings.ShoebodyEnabled)
        {
            _settings.ShoebodyVolume = listing.SliderLabeled($"Volume: {_settings.ShoebodyVolume * 100:f0}%",
                _settings.ShoebodyVolume, 0f, 1f, 0.3f, "Volume of the Shoebody Bop. Default is 35%");

            listing.CheckboxLabeled("Use 2x speed version", ref _settings.DoubleSpeed,
                "Whether to play the double-speed version of the song.");

            listing.CheckboxLabeled("Silence game music when audible", ref _settings.SilenceMusic,
                "Whether to silence the game music whenever the Shoebody Bop can be heard. " +
                "Similar to how other musical instruments work in this game.");
            
            listing.CheckboxLabeled("Only trigger for fresh corpses", ref _settings.OnlyFreshCorpses,
                "If checked, Shoebody will play on human-like corpses only if they are fresh (about 3 minutes old in real-time).\n\n" +
                "If unchecked, Shoebody will play on all human-like corpses, regardless of how old or decomposing they are.");
        }
        
        listing.Label("(These settings might take a few seconds to apply if the Shoebody is currently playing.)");
        
        listing.End();
    }

    public override string SettingsCategory() => "Shoebody Bop";
}