using UnityEngine;
using Verse;

namespace RimworldShoebody;

public static class ListingExtensions
{
    /// <summary>
    /// Labeled IntRange control,
    /// based on <see cref="Listing_Standard.IntRange"/> and <see cref="Listing_Standard.SliderLabeled"/>.
    /// </summary>
    public static Rect IntRangeLabeled(
        this Listing_Standard listing,
        string label,
        ref IntRange intRange,
        int min,
        int max,
        float labelPct = 0.5f,
        string? tooltip = null)
    {
        var rect = listing.GetRect(32f);
        Text.Anchor = TextAnchor.LowerLeft;
        Widgets.Label(rect.LeftPart(labelPct), label);
        if (tooltip != null)
        {
            TooltipHandler.TipRegion(rect, (TipSignal)tooltip);
        }

        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.IntRange(rect.RightPart(1f - labelPct), (int)listing.CurHeight, ref intRange, min, max);
        listing.Gap(listing.verticalSpacing);
        return rect;
    }
}