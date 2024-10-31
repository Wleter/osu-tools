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
        public Skill Skill { get; }

        private List<double> strainValues = new List<double>();
        public IReadOnlyList<double> StrainValues => strainValues;
        public string Name { get; }

        public WrappedSkill(Skill skill, Mod[] mods, string name)
            : base(mods)
        {
            Skill = skill;
            Name = name;
        }

        protected WrappedSkill(Mod[] mods)
            : base(mods)
        {
        }

        public override double DifficultyValue() => Skill.DifficultyValue();

        public override double EffNoteCount() => Skill.EffNoteCount();

        public override void Process(DifficultyHitObject current)
        {
            Skill.Process(current);
            strainValues.Add(Skill.DifficultyValue());
        }
    }
}
