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
        const float labelPct = 0.3f;

        var listing = new Listing_Standard();

        listing.Begin(inRect);

        listing.CheckboxLabeled("Enabled", ref _settings.ShoebodyEnabled,
            "If checked, the Shoebody Bop will occasionally play near water, " +
            "generally when pawns are attacking or downed.\n\n" +
            "Uncheck if this shit gets too annoying.");

        if (_settings.ShoebodyEnabled)
        {
            _settings.ShoebodyVolume = listing.SliderLabeled($"Volume: {_settings.ShoebodyVolume * 100:f0}%",
                _settings.ShoebodyVolume, 0f, 1f, labelPct, "Volume of the Shoebody Bop. Default is 35%");

            listing.CheckboxLabeled("Use 2x speed version", ref _settings.DoubleSpeed,
                "Whether to play the double-speed version of the song.");

            listing.CheckboxLabeled("Silence game music when audible", ref _settings.SilenceMusic,
                "Whether to silence the game music whenever the Shoebody Bop can be heard. " +
                "Similar to how other musical instruments work in this game.");

            listing.CheckboxLabeled("Only trigger for fresh corpses", ref _settings.OnlyFreshCorpses,
                "If checked, Shoebody will play on human-like corpses only if they are fresh " +
                "(about 3 minutes old in real-time).\n\n" +
                "If unchecked, Shoebody will play on all human-like corpses, regardless of how old or decomposing they are.");

            listing.CheckboxLabeled("Play during psychic rituals", ref _settings.PlayOnPsychicRituals,
                "Whether to play the Shoebody on pawns invoking a psychic ritual. Requires Anomaly expansion.");

            listing.Gap();

            const string distRangeTooltip =
                "The distance that the camera must be from the Shoebody pawn for it to be audible.\n\n" +
                "This works like musical instruments. At the minimum distance, " +
                "the Shoebody can be heard at its full volume. At the maximum, it can't be heard.\n\n" +
                "Default is 15~30. Increase the maximum if you want to hear the Shoebody from further away.\n\n" +
                "Warning: this is a bit finicky. Keep in mind if you change this value:\n" +
                " - This setting is applied the next time Shoebody plays from the start.\n" +
                " - This behavior might be buggy if the range is too high or low, or if the min and max are equal.\n" +
                " - The \"Silence game music when audible\" option may not accurately reflect this range.";

            var oldDistRange = _settings.DistRange;
            listing.IntRangeLabeled("Camera distance range", ref _settings.DistRange, 0, 200, labelPct,
                distRangeTooltip);

            // Update SubSoundDefs if DistRange was modified
            if (oldDistRange != _settings.DistRange)
            {
                ShoebodyDistRangeHelper.UpdateSubSoundDefDistRange();
            }
        }

        listing.Gap();

        listing.Label("(These settings might take a few seconds to apply if the Shoebody is currently playing.)");

        listing.Gap(24);

        if (listing.ButtonText("Reset to default", null, labelPct))
        {
            _settings.SetDefaults();
        }

        listing.End();
    }

    public override string SettingsCategory() => "Shoebody Bop";
}