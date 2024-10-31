// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Mods;

namespace PerformanceCalculatorGUI.LocalCalculator
{
    public class LocalDecaySkill : LocalSkill
    {
        /// <summary>
        /// Strain values are multiplied by this number for the given skill. Used to balance the value of different skills between each other.
        /// </summary>
        public required double SkillMultiplier { get; set; }

        /// <summary>
        /// Determines how quickly strain decays for the given skill.
        /// For example a value of 0.15 indicates that strain decays to 15% of its original value after one second.
        /// </summary>
        public required double StrainDecayBase { get; set; }

        public required Func<DifficultyHitObject, double> StrainAlgorithm { get; init; }

        /// <summary>
        /// The current strain level.
        /// </summary>
        protected double CurrentStrain { get; private set; }

        public LocalDecaySkill(Mod[] mods)
            : base(mods)
        {
        }

        private double timeStrainDecay(double ms) => 1.0 / (1.0 + Math.Exp((ms - 3000.0) / 100.0));

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            CurrentStrain *= StrainDecayBase * timeStrainDecay(current.DeltaTime);
            CurrentStrain += StrainAlgorithm.Invoke(current) * SkillMultiplier;

            return CurrentStrain;
        }
    }
}
