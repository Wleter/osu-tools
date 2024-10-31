using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using PerformanceCalculatorGUI.LocalCalculator;

namespace PerformanceCalculatorGUI;

public class ExtendedOsuPerformanceCalculator: OsuPerformanceCalculator
{
    public bool WithLocalSkills { get; set; } = false;

    private double accuracy;
    private int countMiss;
    private double missPenalty;
    private double accuracyPenalty;

    protected override PerformanceAttributes CreatePerformanceAttributes(ScoreInfo score, DifficultyAttributes attributes)
    {
        if (!WithLocalSkills)
            return base.CreatePerformanceAttributes(score, attributes);

        var osuAttributes = (LocalOsuDifficultyAttributes)attributes;

        accuracy = score.Accuracy;
        countMiss = score.Statistics.GetValueOrDefault(HitResult.Miss);

        missPenalty = osuAttributes.MissPenalty;
        accuracyPenalty = osuAttributes.AccuracyPenalty;

        double multiplier = PERFORMANCE_BASE_MULTIPLIER;

        double aimingValue = computeAimingValue(score, osuAttributes);
        double tappingValue = computeTappingValue(score, osuAttributes);
        double readingValue = computeReadingValue(score, osuAttributes);
        double totalValue =
            Math.Pow(
                Math.Pow(aimingValue, 1.1) +
                Math.Pow(tappingValue, 1.1) +
                Math.Pow(readingValue, 1.1)
            , 1.0 / 1.1) * multiplier;

        return new LocalOsuPerformanceAttributes
        {
            Aiming = aimingValue,
            Tapping = tappingValue,
            Reading = readingValue,
            Total = totalValue
        };
    }

    private double computeAimingValue(ScoreInfo score, LocalOsuDifficultyAttributes attributes)
    {
        double aimValue = Math.Pow(5.0 * Math.Max(1.0, attributes.AimingDifficulty / 0.0675) - 4.0, 3.0) / 100000.0;
        aimValue *= Math.Pow(accuracyPenalty, accuracy) * Math.Pow(missPenalty, countMiss);

        return aimValue;
    }

    private double computeTappingValue(ScoreInfo score, LocalOsuDifficultyAttributes attributes)
    {
        if (score.Mods.Any(h => h is OsuModRelax))
            return 0.0;

        double tappingValue = Math.Pow(5.0 * Math.Max(1.0, attributes.TappingDifficulty / 0.0675) - 4.0, 3.0) / 100000.0;
        tappingValue *= Math.Pow(accuracyPenalty, accuracy) * Math.Pow(missPenalty, countMiss);

        return tappingValue;
    }

    private double computeReadingValue(ScoreInfo score, LocalOsuDifficultyAttributes attributes)
    {
        double readingValue = Math.Pow(attributes.ReadingDifficulty, 2.0) * 25.0;
        readingValue *= Math.Pow(accuracyPenalty, accuracy) * Math.Pow(missPenalty, countMiss);

        return readingValue;
    }
}
