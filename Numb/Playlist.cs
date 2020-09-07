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

        private Playlist(IEnumerable<ITrack> tracks)
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

            _current = _tracks.First;
        }

        public static async Task<Playlist> CreateWith(ITracksSource source)
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

            var playlist = new Playlist(tracksList);
            return playlist;
        }

        public ITrack Current()
        {
            return _current?.Value;
        }

        public ITrack Next()
        {
            //XD
            return _current?.Next?.Value;
        }

        public void MoveNext()
        {
            _current = _current?.Next;
        }

        public ITrack Previous()
        {
            //XD
            return _current?.Previous?.Value;
        }
    }
}