// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using MessagePack;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    public class EditorRoomJoinedResult
    {
        [Key(0)]
        public EditorRoom Room { get; set; }

        [Key(1)]
        public SerializedEditorBeatmap Beatmap { get; set; }

        [Key(2)]
        public List<EditorCommandEvent> StagedCommands { get; set; }

        [SerializationConstructor]
        public EditorRoomJoinedResult(EditorRoom room, SerializedEditorBeatmap beatmap, List<EditorCommandEvent> stagedCommands)
        {
            Room = room;
            Beatmap = beatmap;
            StagedCommands = stagedCommands;
        }
    }
}
