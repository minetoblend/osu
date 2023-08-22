// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;

namespace osu.Game.Online.Editor
{
    public interface IEditorClient
    {
        Task UserJoined(EditorRoomUser user);

        Task UserLeft(EditorRoomUser user);

        Task RoomClosed();

        Task UserStateChanged(int userId, EditorUserState state);

        Task CommandsSubmitted(EditorCommandEvent commands);
    }
}
