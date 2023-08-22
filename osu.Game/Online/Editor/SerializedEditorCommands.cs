// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using MessagePack;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    public class SerializedEditorCommands
    {
        public byte[] SerializedCommands { get; set; }

        [SerializationConstructor]
        public SerializedEditorCommands(byte[] serializedCommands)
        {
            SerializedCommands = serializedCommands;
        }
    }
}
