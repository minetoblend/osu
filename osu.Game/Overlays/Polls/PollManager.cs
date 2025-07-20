// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Overlays.Polls
{
    public partial class PollManager : Component
    {
        public IBindable<APIPoll?> ActivePoll => activePoll;

        public bool HasActiveRequest => activeRequest != null;

        private readonly Bindable<APIPoll?> activePoll = new Bindable<APIPoll?>();

        private IBindable<APIState> apiState = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            apiState = api.State.GetBoundCopy();
        }

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            apiState.BindValueChanged(state =>
            {
                if (state.NewValue == APIState.Online)
                    fetchPoll();
            }, true);
        }

        private void fetchPoll() => queueRequest(new GetActivePollRequest());

        public Task Vote(APIPoll poll, APIPollOption option)
        {
            var t = new TaskCompletionSource();

            var request = new PollVoteRequest(poll.Id, option.Id);

            request.Success += _ => t.TrySetResult();
            request.Failure += e =>
            {
                Logger.Error(e, "Failed to vote for poll");
                t.TrySetException(e);
            };

            queueRequest(request);

            return t.Task;
        }

        private APIRequest<APIPoll>? activeRequest;

        private void queueRequest(APIRequest<APIPoll> request)
        {
            activeRequest?.Cancel();

            request.Success += poll =>
            {
                if (activeRequest == request)
                    activeRequest = null;

                activePoll.Value = poll;
            };

            request.Failure += _ =>
            {
                if (activeRequest == request)
                    activeRequest = null;
            };

            api.Queue(activeRequest = request);
        }
    }
}
