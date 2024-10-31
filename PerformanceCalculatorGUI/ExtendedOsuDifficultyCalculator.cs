// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Difficulty.Skills;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Scoring;
using PerformanceCalculatorGUI.LocalCalculator;

namespace PerformanceCalculatorGUI;

public class ExtendedOsuDifficultyCalculator : OsuDifficultyCalculator, IExtendedDifficultyCalculator
{
    private const double difficulty_multiplier = 0.0675;


    public bool WithLocalSkills { get; set; } = false;
    public SkillParams[] LocalSkillParams { get; set; } = new SkillParams[]
    {
        new SkillParams(80.0, 0.8, 6, 0.4, 1.2),
        new SkillParams(1375, 0.95, 3, 0.25, 1.2),
        new SkillParams(0.052, 0.15, 3, 0.5, 0.75),
    };

    private Skill[] skills;

    public ExtendedOsuDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
        : base(ruleset, beatmap)
    {
    }

    public Skill[] GetSkills() => skills;
    public DifficultyHitObject[] GetDifficultyHitObjects(IBeatmap beatmap, double clockRate) => CreateDifficultyHitObjects(beatmap, clockRate).ToArray();

    protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
    {
        this.skills = skills;
        if (WithLocalSkills)
        {
            return localDifficultyAttributes(beatmap, mods, skills, clockRate);
        }
        return base.CreateDifficultyAttributes(beatmap, mods, skills, clockRate);
    }

    protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
    {
        if (!WithLocalSkills)
        {
            return new Skill[]
            {
                new WrappedSkill(new Aim(mods, true), mods, "Aim"),
                new WrappedSkill(new Aim(mods, false), mods, "Aim Slider-less"),
                new WrappedSkill(new Speed(mods), mods, "Speed"),
                new WrappedSkill(new Flashlight(mods), mods, "Flashlight")
            };
        }

        return new Skill[]
        {
            new WrappedSkill(OsuSkillFactory.CreateCustom(mods, SkillType.Aiming, LocalSkillParams[0]), mods, "Aiming"),
            new WrappedSkill(OsuSkillFactory.CreateCustom(mods, SkillType.Tapping, LocalSkillParams[1]), mods, "Tapping"),
            new WrappedSkill(OsuSkillFactory.CreateCustom(mods, SkillType.Reading, LocalSkillParams[2]), mods, "Reading"),
        };
    }

    private DifficultyAttributes localDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
    {
        if (beatmap.HitObjects.Count == 0)
            return new LocalOsuDifficultyAttributes { Mods = mods };

        double aimingRating = Math.Sqrt(skills[0].DifficultyValue()) * difficulty_multiplier;
        double tappingRating = Math.Sqrt(skills[1].DifficultyValue()) * difficulty_multiplier;
        double readingRating = Math.Sqrt(skills[2].DifficultyValue()) * difficulty_multiplier;

        double missPenalty = 0.8;
        double accuracyPenalty = 0.7;

        double baseAimingPerformance = Math.Pow(5 * Math.Max(1, aimingRating / 0.0675) - 4, 3) / 100000;
        double baseTappingPerformance = Math.Pow(5 * Math.Max(1, tappingRating / 0.0675) - 4, 3) / 100000;
        double baseReadingPerformance = Math.Pow(5 * Math.Max(1, readingRating / 0.0675) - 4, 3) / 100000;

        double basePerformance =
            Math.Pow(
                Math.Pow(baseAimingPerformance, 1.1) +
                Math.Pow(baseTappingPerformance, 1.1) +
                Math.Pow(baseReadingPerformance, 1.1), 1.0 / 1.1
            );

        double starRating = basePerformance > 0.00001
            ? Math.Cbrt(OsuPerformanceCalculator.PERFORMANCE_BASE_MULTIPLIER) * 0.027 * (Math.Cbrt(100000 / Math.Pow(2, 1 / 1.1) * basePerformance) + 4)
            : 0;

        double preempt = IBeatmapDifficultyInfo.DifficultyRange(beatmap.Difficulty.ApproachRate, 1800, 1200, 450) / clockRate;
        double drainRate = beatmap.Difficulty.DrainRate;
        int maxCombo = beatmap.GetMaxCombo();

        int hitCirclesCount = beatmap.HitObjects.Count(h => h is HitCircle);
        int sliderCount = beatmap.HitObjects.Count(h => h is Slider);
        int spinnerCount = beatmap.HitObjects.Count(h => h is Spinner);

        HitWindows hitWindows = new OsuHitWindows();
        hitWindows.SetDifficulty(beatmap.Difficulty.OverallDifficulty);

        double hitWindowGreat = hitWindows.WindowFor(HitResult.Great) / clockRate;

        return new LocalOsuDifficultyAttributes
        {
            StarRating = starRating,

            AimingDifficulty = aimingRating,
            TappingDifficulty = tappingRating,
            ReadingDifficulty = readingRating,

            MissPenalty = missPenalty,
            AccuracyPenalty = accuracyPenalty,

            Mods = mods,
            DrainRate = drainRate,
            MaxCombo = maxCombo,
            HitCircleCount = hitCirclesCount,
            SliderCount = sliderCount,
            SpinnerCount = spinnerCount,
        };
    }
}
