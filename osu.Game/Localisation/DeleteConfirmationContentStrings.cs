// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Scoring;

namespace osu.Game.Localisation
{
    public static class DeleteConfirmationContentStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.DeleteConfirmationContent";

        /// <summary>
        /// "Are you sure you want to delete all beatmaps?"
        /// </summary>
        public static LocalisableString Beatmaps => new TranslatableString(getKey(@"beatmaps"), @"Are you sure you want to delete all beatmaps?");

        /// <summary>
        /// "Are you sure you want to delete {0} - {1}?"
        /// </summary>
        public static LocalisableString Beatmap(string artist, string title) => new TranslatableString(getKey(@"beatmap"), @"Are you sure you want to delete {0} - {1}?", artist, title);

        /// <summary>
        /// "Are you sure you want to delete all beatmaps videos? This cannot be undone!"
        /// </summary>
        public static LocalisableString BeatmapVideos => new TranslatableString(getKey(@"beatmap_videos"), @"Are you sure you want to delete all beatmaps videos? This cannot be undone!");

        /// <summary>
        /// "Are you sure you want to delete all skins? This cannot be undone!"
        /// </summary>
        public static LocalisableString Skins => new TranslatableString(getKey(@"skins"), @"Are you sure you want to delete all skins? This cannot be undone!");

        /// <summary>
        /// "Are you sure you want to delete all collections? This cannot be undone!"
        /// </summary>
        public static LocalisableString Collections => new TranslatableString(getKey(@"collections"), @"Are you sure you want to delete all collections? This cannot be undone!");

        /// <summary>
        /// "Are you sure you want to delete {0} ({1} beatmap(s))? This cannot be undone!"
        /// </summary>
        public static LocalisableString Collection(string name, int beatmapCount) =>
            new TranslatableString(getKey(@"collection"), @"Are you sure you want to delete {0} ({1} beatmap(s))? This cannot be undone!", name, beatmapCount);

        /// <summary>
        /// "Are you sure you want to delete all scores? This cannot be undone!"
        /// </summary>
        public static LocalisableString Scores => new TranslatableString(getKey(@"scores"), @"Are you sure you want to delete all scores? This cannot be undone!");

        /// <summary>
        /// "Are you sure you want to delete all scores on {0} - {1}? This cannot be undone!"
        /// </summary>
        public static LocalisableString BeatmapScores(string artist, string title) =>
            new TranslatableString(getKey(@"beatmap_scores"), @"Are you sure you want to delete all scores on {0} - {1}? This cannot be undone!", artist, title);

        /// <summary>
        /// "Are you sure you want to delete the score by {0} ({1}, {2})? This cannot be undone!"
        /// </summary>
        public static LocalisableString Score(string username, LocalisableString accuracy, ScoreRank rank) => new TranslatableString(getKey(@"score"),
            @"Are you sure you want to delete the score by {0} ({1}, {2})? This cannot be undone!", username, accuracy, rank);

        /// <summary>
        /// "Are you sure you want to delete the difficulty &quot;{0}&quot;? This cannot be undone!"
        /// </summary>
        public static LocalisableString BeatmapDifficulty(string difficultyName) =>
            new TranslatableString(getKey(@"beatmap_difficulty"), @"Are you sure you want to delete the difficulty ""{0}""? This cannot be undone!", difficultyName);

        /// <summary>
        /// "Are you sure you want to delete all mod presets?"
        /// </summary>
        public static LocalisableString ModPresets => new TranslatableString(getKey(@"mod_presets"), @"Are you sure you want to delete all mod presets?");

        /// <summary>
        /// "Are you sure you want to the mod preset &quot;{0}&quot;?"
        /// </summary>
        public static LocalisableString ModPreset(string name) => new TranslatableString(getKey(@"mod_preset"), @"Are you sure you want to the mod preset ""{0}""?", name);

        /// <summary>
        /// "Delete team &quot;{0}&quot;?"
        /// </summary>
        public static LocalisableString Team(string name) => new TranslatableString(getKey(@"team"), @"Delete team ""{0}""?", name);

        /// <summary>
        /// "Delete unnamed team?"
        /// </summary>
        public static LocalisableString TeamUnnamed => new TranslatableString(getKey(@"team_unnamed"), @"Delete unnamed team?");

        /// <summary>
        /// "Are you sure you want to delete the skin {0}? This cannot be undone!"
        /// </summary>
        public static LocalisableString Skin(string name) => new TranslatableString(getKey(@"skin"), @"Are you sure you want to delete the skin {0}? This cannot be undone!", name);

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
