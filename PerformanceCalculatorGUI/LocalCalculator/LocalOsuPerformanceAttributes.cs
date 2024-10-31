// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Game.Rulesets.Difficulty;

namespace PerformanceCalculatorGUI.LocalCalculator;

public class LocalOsuPerformanceAttributes : PerformanceAttributes
{
    [JsonProperty("aiming")]
    public double Aiming { get; set; }

    [JsonProperty("tapping")]
    public double Tapping { get; set; }

    [JsonProperty("reading")]
    public double Reading { get; set; }

    public override IEnumerable<PerformanceDisplayAttribute> GetAttributesForDisplay()
    {
        throw new NotImplementedException("Database retrieving/saving not implemented for LocalOsuDifficultyAttributes.");

    }
}
