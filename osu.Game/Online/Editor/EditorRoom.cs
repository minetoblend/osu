// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using MessagePack;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    [Serializable]
    public class EditorRoom
    {
        /// <summary>
        /// The ID of the room, used for database persistence.
        /// </summary>
        [Key(0)]
        public readonly long RoomID;

        [Key(1)]
        public long StateVersion { get; set; }

        [Key(2)]
        public IList<EditorRoomUser> Users { get; set; } = new List<EditorRoomUser>();

        [SerializationConstructor]
        public EditorRoom(long roomId)
        {
            RoomID = roomId;
        }

        public void AddUser(EditorRoomUser user)
        {
            Users.Add(user);
        }
    }
}
