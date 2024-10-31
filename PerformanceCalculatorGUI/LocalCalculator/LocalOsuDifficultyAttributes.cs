using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;

namespace PerformanceCalculatorGUI.LocalCalculator;

public class LocalOsuDifficultyAttributes : DifficultyAttributes
    {
        /// <summary>
        /// The difficulty corresponding to the aiming.
        /// </summary>
        [JsonProperty("aiming_difficulty")]
        public double AimingDifficulty { get; set; }

        /// <summary>
        /// The difficulty corresponding to the tapping.
        /// </summary>
        [JsonProperty("tapping_difficulty")]
        public double TappingDifficulty { get; set; }

        /// <summary>
        /// The difficulty corresponding to the reading.
        /// </summary>
        [JsonProperty("reading_difficulty")]
        public double ReadingDifficulty { get; set; }

        [JsonProperty("miss_penalty")]
        public double MissPenalty { get; set; }

        [JsonProperty("accuracy_penalty")]
        public double AccuracyPenalty { get; set; }

        /// <summary>
        /// The beatmap's drain rate. This doesn't scale with rate-adjusting mods.
        /// </summary>
        public double DrainRate { get; set; }

        /// <summary>
        /// The number of hitcircles in the beatmap.
        /// </summary>
        public int HitCircleCount { get; set; }

        /// <summary>
        /// The number of sliders in the beatmap.
        /// </summary>
        public int SliderCount { get; set; }

        /// <summary>
        /// The number of spinners in the beatmap.
        /// </summary>
        public int SpinnerCount { get; set; }

        public override IEnumerable<(int attributeId, object value)> ToDatabaseAttributes()
        {
            throw new NotImplementedException("Database retrieving/saving not implemented for LocalOsuDifficultyAttributes.");
        }

        public override void FromDatabaseAttributes(IReadOnlyDictionary<int, double> values, IBeatmapOnlineInfo onlineInfo)
        {
            throw new NotImplementedException("Database retrieving/saving not implemented for LocalOsuDifficultyAttributes.");
        }

        #region Newtonsoft.Json implicit ShouldSerialize() methods

        // The properties in this region are used implicitly by Newtonsoft.Json to not serialise certain fields in some cases.
        // They rely on being named exactly the same as the corresponding fields (casing included) and as such should NOT be renamed
        // unless the fields are also renamed.

        [UsedImplicitly]
        public bool ShouldSerializeFlashlightDifficulty() => Mods.Any(m => m is ModFlashlight);

        #endregion
    }
