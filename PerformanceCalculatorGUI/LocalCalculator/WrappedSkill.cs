// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace PerformanceCalculatorGUI.LocalCalculator
{
    public class WrappedSkill : Skill
    {
        private Skill skill;

        private List<double> strainValues = new List<double>();
        public IReadOnlyList<double> StrainValues => strainValues;

        public string Name => skill.GetType().Name;

        public WrappedSkill(Skill skill, Mod[] mods)
            : base(mods)
        {
            this.skill = skill;
        }

        protected WrappedSkill(Mod[] mods)
            : base(mods)
        {
        }

        public override double DifficultyValue() => skill.DifficultyValue();

        public override double EffNoteCount() => skill.EffNoteCount();

        public override void Process(DifficultyHitObject current)
        {
            skill.Process(current);
            strainValues.Add(skill.DifficultyValue());
        }
    }
}
