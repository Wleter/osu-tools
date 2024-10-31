

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty.Evaluators;

namespace PerformanceCalculatorGUI.LocalCalculator;
public static class OsuSkillFactory
{
    public static Skill CreateAiming(Mod[] mods)
    {
        return new LocalDecaySkill(mods)
        {
            SkillMultiplier = 80.0,
            StrainDecayBase = 0.8,
            CountDecay = 6,
            CountFactor = 0.4,
            DiffMultiplicative = 1.2,
            StrainAlgorithm = ob => AimEvaluator.EvaluateDifficultyOf(ob, true),
        };
    }

    public static Skill CreateTapping(Mod[] mods)
    {
        return new LocalDecaySkill(mods)
        {
            SkillMultiplier = 1375,
            StrainDecayBase = 0.95,
            StrainAlgorithm = SpeedEvaluator.EvaluateDifficultyOf,
            CountDecay = 3,
            CountFactor = 0.25,
            DiffMultiplicative = 1.2,
        };
    }

    public static Skill CreateReading(Mod[] mods)
    {
        return new LocalDecaySkill(mods)
        {
            SkillMultiplier = 1,
            StrainDecayBase = 0.15,
            StrainAlgorithm = ob => FlashlightEvaluator.EvaluateDifficultyOf(ob, false),
            CountDecay = 0.75,
            CountFactor = 0.75,
            DiffMultiplicative = 0.75,
        };
    }

    public static Skill CreateCustom(Mod[] mods, SkillType skillType, SkillParams skillParams)
    {
        Func<DifficultyHitObject, double> algorithm = skillType switch
        {
            SkillType.Aiming => ob => AimEvaluator.EvaluateDifficultyOf(ob, true),
            SkillType.Tapping => SpeedEvaluator.EvaluateDifficultyOf,
            SkillType.Reading => ob => FlashlightEvaluator.EvaluateDifficultyOf(ob, false),
            _ => throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null)
        };

        return new LocalDecaySkill(mods)
        {
            SkillMultiplier = skillParams.SkillMultiplier,
            StrainDecayBase = skillParams.StrainDecayBase,
            CountDecay = skillParams.CountDecay,
            CountFactor = skillParams.CountFactor,
            DiffMultiplicative = skillParams.DiffMultiplicative,
            StrainAlgorithm = algorithm,
        };
    }
}

public enum SkillType
{
    Aiming = 0,
    Tapping = 1,
    Reading = 2
}

public class SkillParams
{
    public double SkillMultiplier;
    public double StrainDecayBase;
    public double CountDecay;
    public double CountFactor;
    public double DiffMultiplicative;

    public SkillParams(double skillMultiplier, double strainDecayBase, double countDecay, double countFactor, double diffMultiplicative)
    {
        SkillMultiplier = skillMultiplier;
        StrainDecayBase = strainDecayBase;
        CountDecay = countDecay;
        CountFactor = countFactor;
        DiffMultiplicative = diffMultiplicative;
    }
}
