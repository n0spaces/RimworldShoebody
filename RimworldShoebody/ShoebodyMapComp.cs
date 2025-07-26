using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimworldShoebody;

/// <summary>
/// Main component that manages the Shoebody audio in the map.
/// </summary>
[UsedImplicitly]
public class ShoebodyMapComp(Map map) : MapComponent(map)
{
    private const int MaxCorpseAgeTicks = 10800; // 3 minutes in real time

    private static readonly ThingRequest CorpseThingRequest = ThingRequest.ForGroup(ThingRequestGroup.Corpse);

    private Sustainer? _sustainer;

    public Thing? CurrentShoebodyThing => _sustainer?.info.Maker.Thing;

    public override void MapComponentTick()
    {
        // Don't do anything shoebody related if this isn't the current map
        if (Find.CurrentMap != map)
        {
            if (!(_sustainer == null || _sustainer.Ended))
            {
                _sustainer.End();
                _sustainer = null;
            }

            return;
        }

        if (ShoebodyModSettings.CurrentEnabledSetting)
        {
            var closestThing = FindClosestEligibleThing();
            MaintainSustainer(closestThing);
        }
        else
        {
            // Shoebody is disabled, kill any existing sustainers
            if (!(_sustainer == null || _sustainer.Ended))
            {
                _sustainer.End();
            }

            _sustainer = null;
        }
    }

    private Thing? FindClosestEligibleThing()
    {
        // Find human-like corpses above water that are not too old
        var corpseInnerPawns = map.listerThings.ThingsMatching(CorpseThingRequest)
            .Where(thing => IsEligibleCorpse((Corpse)thing))
            .Where(IsOverWater);

        // Find human-like pawns above water where any of the following are true:
        //  - Pawn is drafted while map is in danger mode
        //  - Pawn is attacking
        //  - Pawn is downed
        //  - Pawn is carrying a corpse that meets the corpse eligibility
        var pawnThings = map.mapPawns.AllHumanlikeSpawned
            .Where(IsOverWater)
            .Where(pawn => (map.dangerWatcher.DangerRating == StoryDanger.High && pawn.Drafted)
                           || pawn.IsAttacking()
                           || pawn.Downed
                           || (pawn.carryTracker.CarriedThing is Corpse corpse && IsEligibleCorpse(corpse)));

        // If Anomaly expansion is active and the setting is enabled, also find pawns invoking a psychic ritual
        var shouldFindInvokingPawns = ExpansionDefOf.Anomaly.Status == ExpansionStatus.Active &&
                                      ShoebodyModSettings.CurrentPsychicRitualSetting;
        
        var invokingPawns = shouldFindInvokingPawns
            ? map.mapPawns.AllHumanlikeSpawned.Where(pawn => pawn.mindState?.duty?.def == DutyDefOf.Invoke)
            : [];

        var eligibleThings = corpseInnerPawns.Concat(pawnThings).Concat(invokingPawns);

        var cameraPos = Find.CameraDriver.MapPosition;

        var closestThing = eligibleThings
            .OrderBy(thing => IntVec3Utility.ManhattanDistanceFlat(thing.Position, cameraPos))
            .FirstOrDefault();

        return closestThing;
    }

    private int _ticksSinceLastEligibleThing;

    // Wait longer for the sustainer to end if the map is in danger mode
    private int MaxTicksSinceLastEligibleThing => map.dangerWatcher.DangerRating == StoryDanger.High ? 500 : 200;

    private void MaintainSustainer(Thing? closestThing)
    {
        // Check if 2x setting changed, and stop this sustainer if it did
        var shouldBeDoubleSpeed = ShoebodyModSettings.CurrentDoubleSpeedSetting;
        if (_sustainer?.def.defName == (shouldBeDoubleSpeed ? "Shoebody_Sound" : "Shoebody_Sound2x"))
        {
            _sustainer.End();
            _sustainer = null;
        }

        // Sustainer not running
        if (_sustainer == null)
        {
            // No eligible thing, do nothing
            if (closestThing == null)
            {
                return;
            }

            // There is an eligible thing, start sustainer
            _ticksSinceLastEligibleThing = 0;
            _sustainer = ShoebodySustainerHelpers.Create(closestThing);
            return;
        }

        // Update volume in case setting changed
        _sustainer.externalParams["ShoebodySound_VolumeParam"] = ShoebodyModSettings.CurrentVolumeSetting;

        // prevClosestThing is the closest eligible thing from the last time SoundInfo was updated
        var prevClosestThing = _sustainer.info.Maker.Thing;

        if (prevClosestThing != closestThing)
        {
            if (closestThing != null && _ticksSinceLastEligibleThing <= MaxTicksSinceLastEligibleThing)
            {
                // If prevClosestThing is still on-camera and new closestThing is off-camera,
                // treat it as if there is no new eligible thing and keep sound on prev thing
                var rect = Find.CameraDriver.CurrentViewRect;
                var newPos = closestThing.Position;
                var newVisible = rect.minX <= newPos.x
                                 && newPos.x <= rect.maxX
                                 && rect.minZ <= newPos.z
                                 && newPos.z <= rect.maxZ;
                var prevPos = prevClosestThing.Position;
                var prevVisible = rect.minX <= prevPos.x
                                  && prevPos.x <= rect.maxX
                                  && rect.minZ <= prevPos.z
                                  && prevPos.z <= rect.maxZ;
                if (prevVisible && !newVisible)
                {
                    closestThing = null;
                }
            }

            // If new closestThing is null (or off-camera while prev is on-camera),
            // increment tick counter and end if it's been long enough
            if (closestThing == null)
            {
                if (++_ticksSinceLastEligibleThing > MaxTicksSinceLastEligibleThing)
                {
                    _sustainer.End();
                    _sustainer = null;
                }

                return;
            }

            // Otherwise replace SoundInfo with the new closest thing
            _sustainer.info = SoundInfo.InMap(closestThing);
        }
    }

    private bool IsOverWater(Thing thing) => map.terrainGrid.TerrainAt(thing.Position).IsWater;

    private static bool IsEligibleCorpse(Corpse corpse) =>
        (corpse.InnerPawn?.RaceProps.Humanlike ?? false)
        && (!ShoebodyModSettings.CurrentFreshCorpsesSetting || corpse.Age < MaxCorpseAgeTicks);
}