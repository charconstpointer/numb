using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Numb
{
    public class Playlist
    {
        public Playlist(uint checks = 5)
        {
            _checks = checks;
        }

        public Playlist(ITracksSource tracksSource, uint checks = 5)
        {
            _tracksSource = tracksSource;
            _checks = checks;
        }

        private int TrackCount => _tracks.Count;
        private readonly ITracksSource _tracksSource;
        private readonly uint _checks = 5;
        private readonly LinkedList<ITrack> _tracks = new LinkedList<ITrack>();
        public IEnumerable<ITrack> Tracks => _tracks.ToImmutableList();
        public event EventHandler<TrackChanged<ITrack>> TrackChanged;

        public bool IsSteady { get; private set; }


        public async Task Start()
        {
            var checks = 0;
            while (!IsSteady)
            {
                try
                {
                    var tracks = await _tracksSource.GetAsync();
                    var tracksList = tracks.ToList();
                    if (++checks == _checks)
                    {
                        IsSteady = true;
                        Console.WriteLine("Steady✅");
                        return;
                    }

                    if (tracksList.Count != TrackCount)
                    {
                        foreach (var track in tracksList)
                        {
                            AddTrack(track);
                        }

                        checks = 0;

                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }
                    else
                    {
                        TrackChanged.Invoke(null, null);
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
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

            var first = _tracks.First;
            if (first?.Value.Start == track.Start && first.Value.Stop == track.Stop)
            {
                return;
            }

            _tracks.AddFirst(track);
        }

        public void OnChange()
        {
            throw new NotImplementedException();
        }
    }
}