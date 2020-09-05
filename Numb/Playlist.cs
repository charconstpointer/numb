using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Numb
{
    public class Playlist
    {
        private readonly LinkedList<ITrack> _tracks = new LinkedList<ITrack>();
        private bool _isSteady = false;
        public IEnumerable<ITrack> Tracks => _tracks.ToImmutableList();

        private void OnTick()
        {
            while (!_isSteady)
            {
                _isSteady = true;
            }
        }
        
        public void AddTrack(ITrack track)
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

            _tracks.AddFirst(track);

        }
    }
}