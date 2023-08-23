// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using MessagePack;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    public class EditorCommandEvent
    {
        [Key(0)]
        public byte[] SerializedCommands { get; set; }

        [Key(1)]
        public long StateVersion { get; set; }

        [Key(2)]
        public long UserId { get; set; }

        [SerializationConstructor]
        public EditorCommandEvent(byte[] commands, long stateVersion, long userId)
        {
            SerializedCommands = commands;
            StateVersion = stateVersion;
            UserId = userId;
        }
    }
}
