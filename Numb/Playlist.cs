using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numb
{
    public class Playlist : IPlaylist
    {
        public string Name { get; }
        private readonly LinkedList<ITrack> _tracks = new LinkedList<ITrack>();
        private LinkedListNode<ITrack> _current;
        private readonly ITracksSource _tracksSource;
        private readonly uint _requiredChecks = 3;
        private uint _checks = 0;
        public bool IsStable { get; private set; }
        public int TracksCount => _tracks.Count;

        private Playlist(IEnumerable<ITrack> tracks, ITracksSource source)
        {
            var notYetPlayed = tracks.Where(track =>
            {
                var now = DateTime.UtcNow.AddHours(2);
                return track.Stop >= now;
            });
            foreach (var track in notYetPlayed)
            {
                _tracks.AddLast(track);
            }

            _tracksSource = source;
            _current = _tracks.First;
        }

        public static async Task<Playlist> CreateFrom(ITracksSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var tracks = await source.GetAsync();
            var tracksList = tracks.ToList();
            if (!tracksList.Any())
            {
                throw new ApplicationException("Tracks source has not returned any tracks");
            }

            var playlist = new Playlist(tracksList, source);
            return playlist;
        }

        //TODO Should also check for tracks to remove
        //
        public async Task Stabilize()
        {
            var tracks = await _tracksSource.GetAsync();
            var tracksList = tracks.ToList();
            if (++_checks == _requiredChecks)
            {
                IsStable = true;
                return;
            }
            //TODO I could recreate LL everytime playlist changes instead of maintaining old one, TBD
            if (tracksList.Count > TracksCount)
            {
                foreach (var track in tracksList)
                {
                    AddTrack(track);
                }

                _checks = 0;
            }
        }

        //TODO Clean this shit up 🎃
        private void AddTrack(ITrack track)
        {
            lock (track)
            {
                if (!_tracks.Any())
                {
                    _tracks.AddFirst(track);
                    return;
                }

                var current = _tracks.Last;
                if (current == null)
                {
                    _tracks.AddFirst(track);
                    return;
                }

                while (current != null)
                {
                    if (current.Value.Stop < track.Start)
                    {
                        _tracks.AddAfter(current, track);
                        return;
                    }

                    current = current.Next;
                }

                var first = _tracks.First;
                if (first?.Value.Start == track.Start && first.Value.Stop == track.Stop)
                {
                    return;
                }

                _tracks.AddFirst(track);
            }
        }

        public ITrack Current()
        {
            return _current?.Value;
        }

        public ITrack Next()
        {
            //Whenever Next is null consider raising an event about possible playlist end?
            return _current?.Next?.Value;
        }

        public void MoveNext()
        {
            _current = _current?.Next;
        }

        public ITrack Previous()
        {
            return _current?.Previous?.Value;
        }
    }
}