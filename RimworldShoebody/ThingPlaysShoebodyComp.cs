using RimWorld;
using Verse;

namespace RimworldShoebody;

/// <summary>
/// A <see cref="CompPlaysMusic"/> applied to human-like pawns and corpses (things eligible for the Shoebody sound).
///
/// This is used to tell the music manager that this thing is emitting Shoebody, and that the game music should fade out
/// while near this thing, just like musical instruments.
///
/// See <see cref="MusicManagerPlay.UpdateMusicFadeout"/> for the fade out code.
/// </summary>
public class ThingPlaysShoebodyComp : CompPlaysMusic
{
    /// <summary>
    /// This is static so we can change the sound range to match the sub sound's DistRange setting
    /// (though this doesn't seem to actually change anything...)
    /// </summary>
    internal static FloatRange StaticSoundRange;

    static ThingPlaysShoebodyComp()
    {
        StaticSoundRange = SoundDef.Named("Shoebody_Sound").subSounds[0].distRange;
    }
    
    public override bool Playing
    {
        get
        {
            if (ShoebodyModSettings.CurrentSilenceMusicSetting == false)
            {
                return false;
            }

            return Find.CurrentMap.GetComponent<ShoebodyMapComp>()?.CurrentShoebodyThing == parent;
        }
    }

    public override FloatRange SoundRange => StaticSoundRange;
}