# Rimworld - Shoebody Bop

Rimworld mod that plays the Shoebody Bop when a pawn is attacking or downed while in water.

## Technical details

Most of the important code is in [ShoebodyMapComp.cs](RimworldShoebody/ShoebodyMapComp.cs).

You can view the `FindClosestEligibleThing()` method to see the exact logic for determining if a thing should be playing Shoebody,
but it basically finds if any of the following exists:

- A human-like corpse above water that is
  - Not older than ~3 minutes in real-time (unless the fresh corpses setting is disabled)
- A human-like pawn above water that is...
  - Attacking
  - Downed
  - Drafted while the map is in danger mode (the combat music is playing)
  - Carrying a corpse that meets the criteria above

If an eligible thing was found, `MaintainSustainer(Thing? closestThing)` will play audio at that thing's position.
Only one Shoebody plays at a time. If there are multiple pawns or corpses that it can play from, it will choose the one
closest to the center of the camera. If the thing is no longer eligible (it leaves the water or stops attacking),
the audio will continue to play for a few seconds then fade out.

`ThingPlaysShoebodyComp` is a component applied to all pawns and corpses.
This lowers the music volume whenever the camera is next to a thing playing Shoebody (if the setting is enabled).
This extends `CompPlaysMusic` so this works just like musical instruments in Royalty.

## Development

Follow [these instructions](https://ludeon.com/forums/index.php?topic=51589.0) to set up debugging for Rimworld, then update `RimworldPath` in [RimworldShoebody.csproj](RimworldShoebody/RimworldShoebody.csproj).

I have not tried debugging in 1.6 yet, but it looks like you'll need to instead download & install Unity 2022.3.35 to get the proper debug files.
It doesn't seem like you can open this install in 7-Zip like you can with 2019.2.17.
