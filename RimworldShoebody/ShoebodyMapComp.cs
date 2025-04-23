using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimworldShoebody;

/// <summary>
///     Plays the “shoebody ambiance” near fresh human corpses or combat pawns standing on water.
/// </summary>
[UsedImplicitly]
public sealed class ShoebodyMapComp(Map map) : MapComponent(map)
{
    // ───────────────────────────────────────────── constants
    private const int MaxCorpseAgeTicks = 60 * 60; // 1 real-time minute

    private static readonly ThingRequest CorpseThingRequest = ThingRequest.ForGroup(ThingRequestGroup.Corpse);

    // ───────────────────────────────────────────── state
    private Sustainer? _sustainer;
    private int        _ticksSinceLastEligibleThing;

    // Wait longer while the map is in danger mode so music isn’t stop-start mid-raid.
    private int MaxTicksSinceLastEligibleThing =>
        map.dangerWatcher.DangerRating == StoryDanger.High ? 500 : 200;

    public Thing? CurrentShoebodyThing => _sustainer?.info.Maker.Thing;

    // ───────────────────────────────────────────── tick loop
    public override void MapComponentTick()
    {
        if (!ShoebodyModSettings.CurrentEnabledSetting)
        {
            KillSustainer();
            return;
        }

        var cameraPos   = Find.CameraDriver.MapPosition;
        var viewRect    = Find.CameraDriver.CurrentViewRect;
        var closestThing = FindClosestEligibleThing(cameraPos);

        MaintainSustainer(closestThing, viewRect);
    }

    // ───────────────────────────────────────────── core logic
    private Thing? FindClosestEligibleThing(IntVec3 cameraPos)
    {
        var candidates   = ListPool<Thing>.Get();
        AddEligibleCorpses(candidates);
        AddEligiblePawns(candidates);

        var closest = candidates
            .OrderBy(t => IntVec3Utility.ManhattanDistanceFlat(t.Position, cameraPos))
            .FirstOrDefault();

        ListPool<Thing>.Return(candidates);
        return closest;
    }

    private void MaintainSustainer(Thing? closestThing, CellRect viewRect)
    {
        // no sustainer yet → try to start one when we have a target
        if (_sustainer is null || _sustainer.Ended)
        {
            if (closestThing == null) return;

            TryStartSustainer(closestThing);
            return;
        }

        _sustainer.externalParams["ShoebodySound_VolumeParam"] = ShoebodyModSettings.CurrentVolumeSetting;

        var prevThing = _sustainer.info.Maker.Thing;
        if (prevThing == closestThing) return;

        // decide whether to keep, switch, or stop
        if (closestThing is null || ShouldPreferOldTarget(prevThing, closestThing, viewRect))
        {
            if (++_ticksSinceLastEligibleThing <= MaxTicksSinceLastEligibleThing) return;
            KillSustainer();
            return;
        }

        // switch to the new target
        _ticksSinceLastEligibleThing = 0;
        _sustainer.info              = SoundInfo.InMap(closestThing);
    }

    // ───────────────────────────────────────────── helpers
    private void TryStartSustainer(Thing thing)
    {
        try
        {
            _ticksSinceLastEligibleThing = 0;
            _sustainer                   = ShoebodySustainerHelpers.Create(thing);
        }
        catch (Exception ex)
        {
            Log.Error($"[Shoebody] Failed to create sustainer: {ex}");
        }
    }

    private void KillSustainer()
    {
        _sustainer?.End();
        _sustainer = null;
        _ticksSinceLastEligibleThing = 0;
    }

    private static bool ShouldPreferOldTarget(Thing oldT, Thing newT, CellRect viewRect)
    {
        // keep the old one if it’s on-camera and the new one isn’t
        bool OldVisible() => viewRect.Contains(oldT.Position);
        bool NewVisible() => viewRect.Contains(newT.Position);
        return OldVisible() && !NewVisible();
    }

    private void AddEligibleCorpses(List<Thing> list)
    {
        foreach (var thing in map.listerThings.ThingsMatching(CorpseThingRequest))
        {
            if (thing is Corpse corpse && IsEligibleCorpse(corpse) && IsOverWater(corpse))
                list.Add(corpse);
        }
    }

    private void AddEligiblePawns(List<Thing> list)
    {
        bool raidOngoing = map.dangerWatcher.DangerRating == StoryDanger.High;

        foreach (var pawn in map.mapPawns.AllHumanlikeSpawned)
        {
            if (!IsOverWater(pawn)) continue;

            bool isCombatPawn = raidOngoing && pawn.Drafted || pawn.IsAttacking() || pawn.Downed;
            bool carryingCorpse = pawn.carryTracker.CarriedThing is Corpse c && IsEligibleCorpse(c);

            if (isCombatPawn || carryingCorpse)
                list.Add(pawn);
        }
    }

    private bool IsOverWater(Thing t)    => map.terrainGrid.TerrainAt(t.Position).IsWater;
    private static bool IsEligibleCorpse(Corpse c) => c.InnerPawn?.RaceProps.Humanlike is true && c.Age < MaxCorpseAgeTicks;
}
