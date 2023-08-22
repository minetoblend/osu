// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using MessagePack;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    public class SerializedEditorBeatmap
    {
        [Key(0)]
        public string EncodedBeatmap { get; set; }

        [Key(1)]
        public Dictionary<string, byte[]> Files { get; set; }

        [SerializationConstructor]
        public SerializedEditorBeatmap(string encodedBeatmap, Dictionary<string, byte[]> files)
        {
            EncodedBeatmap = encodedBeatmap;
            Files = files;
        }
    }
}
