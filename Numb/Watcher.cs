using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Numb
{
    public class Watcher
    {
        private readonly ConcurrentDictionary<string, Playlist> _playlists =
            new ConcurrentDictionary<string, Playlist>();

        private readonly Thread _watcher;
        private readonly Thread _stabilizer;
        private readonly TimeSpan _tickDelay;
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        public CancellationToken CancellationToken => _cancellationToken;
        public event EventHandler<TrackChanged<ITrack>> TrackChanged;

        public Watcher()
        {
            _watcher = new Thread(OnTick)
            {
                IsBackground = true
            };
            _stabilizer = new Thread(async () => await Stabilize())
            {
                IsBackground = true
            };
            _tickDelay = TimeSpan.FromSeconds(1);
        }

        private async Task Stabilize()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                foreach (var playlistsKey in _playlists.Keys)
                {
                    if (!_playlists.TryGetValue(playlistsKey, out var playlist)) continue;
                    if (playlist.IsStable) continue;
                    await playlist.Stabilize();
                }

                Console.WriteLine("Stabilizer");
                Thread.Sleep(10000);
            }
        }

        public void Start()
        {
            _watcher.Start();
            _stabilizer.Start();
        }

        private void OnTick()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                foreach (var playlistsKey in _playlists.Keys)
                {
                    if (!_playlists.TryGetValue(playlistsKey, out var channel)) continue;
                    var isOutdated = channel.Current()?.Stop < DateTime.UtcNow.AddHours(2);
                    if (!isOutdated) continue;
                    channel.MoveNext();
                    var current = channel.Current();
                    var next = channel.Next();
                    OnTrackChanged(channel.Name, current, next);
                }

                Console.WriteLine("Ticker");
                Thread.Sleep(1000);
            }
        }

        public void AddTarget(string key, Playlist playlist)
        {
            ValidateArguments();
            if (!_playlists.TryAdd(key, playlist))
            {
                throw new ApplicationException("Could not add playlist under provided key");
            }


            void ValidateArguments()
            {
                if (playlist == null) throw new ArgumentNullException(nameof(playlist));
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("Value cannot be null or empty.", nameof(key));
                if (_playlists.ContainsKey(key))
                {
                    throw new ApplicationException("Provided key is not empty");
                }
            }
        }

        private void OnTrackChanged(string channel, ITrack current, ITrack next)
        {
            TrackChanged?.Invoke(this, new TrackChanged<ITrack>
            {
                Channel = channel,
                Current = current,
                Next = next
            });
        }
    }
}