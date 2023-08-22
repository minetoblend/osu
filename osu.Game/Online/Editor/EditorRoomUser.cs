// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Text.Json.Serialization;
using MessagePack;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Online.Editor
{
    [MessagePackObject]
    [Serializable]
    public class EditorRoomUser : IEquatable<EditorRoomUser>
    {
        [Key(0)]
        public readonly int UserID;

        [IgnoreMember]
        public APIUser? User { get; set; }

        [JsonConstructor]
        public EditorRoomUser(int userId)
        {
            UserID = userId;
        }

        public bool Equals(EditorRoomUser? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UserID == other.UserID;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EditorRoomUser)obj);
        }

        public override int GetHashCode()
        {
            return UserID;
        }
    }
}
