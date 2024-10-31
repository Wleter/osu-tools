// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace PerformanceCalculatorGUI.LocalCalculator
{
    public abstract class LocalSkill : Skill
    {
        public required double CountFactor { get; set; }
        public required double DiffMultiplicative { get; set; }
        public required double CountDecay { get; set; }

        private double maxHitObjectDiff = 0;
        private double effHitObjectCount = 0;
        private double countFactor => Math.Min(CountFactor, 1.0 / CountDecay);

        protected LocalSkill(Mod[] mods)
            : base(mods)
        {
        }

        /// <summary>
        /// Returns the strain value at <see cref="DifficultyHitObject"/>. This value is calculated with or without respect to previous objects.
        /// </summary>
        protected abstract double StrainValueAt(DifficultyHitObject current);

        /// <summary>
        /// Process a <see cref="DifficultyHitObject"/> and update current strain values accordingly.
        /// </summary>
        public sealed override void Process(DifficultyHitObject current)
        {
            double currentStrain = StrainValueAt(current);
            double newMaxHitObjectDiff = Math.Max(maxHitObjectDiff, currentStrain);
            if (newMaxHitObjectDiff == 0)
                return;

            if (newMaxHitObjectDiff > maxHitObjectDiff)

            {
                effHitObjectCount = effHitObjectCount * Math.Pow(maxHitObjectDiff / newMaxHitObjectDiff, CountDecay) + 1;
                maxHitObjectDiff = newMaxHitObjectDiff;
                return;
            }

            effHitObjectCount += Math.Pow(currentStrain / newMaxHitObjectDiff, CountDecay);
        }

        /// <summary>
        /// Returns the calculated difficulty value representing all <see cref="DifficultyHitObject"/>s that have been processed up to this point.
        /// </summary>
        public sealed override double DifficultyValue() => DiffMultiplicative * maxHitObjectDiff * Math.Pow(effHitObjectCount, countFactor);

        public override double EffNoteCount() => effHitObjectCount;
    }
}
