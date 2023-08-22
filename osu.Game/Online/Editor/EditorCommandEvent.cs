// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using MessagePack;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    public class EditorCommandEvent
    {
        [Key(0)]
        public long StateVersion { get; set; }

        [SerializationConstructor]
        public EditorCommandEvent(long stateVersion)
        {
            StateVersion = stateVersion;
        }
    }
}
