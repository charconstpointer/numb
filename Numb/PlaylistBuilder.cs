using System;
using System.Collections.Generic;

namespace Numb
{
    public class PlaylistBuilder
    {
        private uint _checks = 3;
        private ITracksSource _source;
        private readonly ICollection<Action<TrackChanged<ITrack>>> _actions = new List<Action<TrackChanged<ITrack>>>();


        public PlaylistBuilder WithChecks(uint checks)
        {
            _checks = checks;
            return this;
        }

        public PlaylistBuilder WithSource(ITracksSource source)
        {
            _source = source;
            return this;
        }

        public PlaylistBuilder OnChange(Action<TrackChanged<ITrack>> action)
        {
            _actions.Add(action);
            return this;
        }

        public Playlist Build()
        {
            var playlist = new Playlist(_source, _checks);
            playlist.TrackChanged += (sender, changed) =>
            {
                foreach (var action in _actions)
                {
                    action(changed);
                }
            };
            return playlist;
        }
    }
}