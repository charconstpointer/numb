using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Numb
{
    public class Playlist
    {
        public Playlist(uint checks = 5)
        {
            _checks = checks;
            _thread = new Thread(() => Watch(_cancellationToken))
            {
                IsBackground = true
            };
        }

        private static void Watch(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Watching ⌚");
                Thread.Sleep(1000);
            }
        }

        public Playlist(ITracksSource tracksSource, uint checks = 5)
        {
            _tracksSource = tracksSource;
            _checks = checks;
            _thread = new Thread(() => Watch(_cancellationToken))
            {
                IsBackground = true
            };
        }

        private int TrackCount => _tracks.Count;
        private readonly Thread _thread;
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly ITracksSource _tracksSource;
        private readonly uint _checks = 5;
        private readonly LinkedList<ITrack> _tracks = new LinkedList<ITrack>();
        public IEnumerable<ITrack> Tracks => _tracks.ToImmutableList();
        public event EventHandler<TrackChanged<ITrack>> TrackChanged;

        public bool IsStable { get; private set; }


        public async Task Start()
        {
#pragma warning disable 4014
            Task.Run(Stabilize);
            _thread.Start();
#pragma warning restore 4014
        }

        private async Task Stabilize()
        {
            var checks = 0;
            while (!IsStable)
            {
                var tracks = await _tracksSource.GetAsync();
                var tracksList = tracks.ToList();
                if (++checks == _checks)
                {
                    IsStable = true;
                    Console.WriteLine("Playlist is stable ✅");
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
                    TrackChanged?.Invoke(null, null);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
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

        public void OnChange()
        {
            throw new NotImplementedException();
        }
    }
}