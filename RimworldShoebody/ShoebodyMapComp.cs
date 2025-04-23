using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimworldShoebody;

[UsedImplicitly]
public class ShoebodyMapComp(Map map) : MapComponent(map)
{
    private const int MaxCorpseAgeTicks = 3600; // 1 minute in real time

    private static readonly ThingRequest CorpseThingRequest = ThingRequest.ForGroup(ThingRequestGroup.Corpse);

    private Sustainer? _sustainer;

    public Thing? CurrentShoebodyThing => _sustainer?.info.Maker.Thing;

    public override void FinalizeInit()
    {
    }

    public override void MapComponentTick()
    {
        if (ShoebodyModSettings.CurrentEnabledSetting)
        {
            // TODO: If a sustainer exists in two maps, only the latest one will play
            // but the music will still fade out in the earlier map as if its sustainer is still playing.
            // Only really a problem if base gets raided while fighting in another map.
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

        var eligibleThings = corpseInnerPawns.Concat(pawnThings);

        var cameraPos = Find.CameraDriver.MapPosition;

        var closestThing = eligibleThings
            .OrderBy(thing => IntVec3Utility.ManhattanDistanceFlat(thing.Position, cameraPos))
            .FirstOrDefault();

        return closestThing;
    }

    private int _ticksSinceLastEligibleThing;

    // Wait longer for the sustainer to end if the map is in danger mode
    private int MaxTicksSinceLastEligibleThing => map.dangerWatcher.DangerRating == StoryDanger.High ? 1000 : 200;

    private void MaintainSustainer(Thing? closestThing)
    {
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
        // TODO: trigger with event instead
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
        (corpse.InnerPawn?.RaceProps.Humanlike ?? false) && corpse.Age < MaxCorpseAgeTicks;
}