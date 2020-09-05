using System;
using Numb;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var playlist = new Playlist();
            var now = DateTime.UtcNow;
            var track = new Track(now, now.AddSeconds(1));
            var track2 = new Track(now.AddSeconds(5), now.AddSeconds(9));
            var track3 = new Track(now.AddSeconds(2), now.AddSeconds(3));
            playlist.AddTrack(track3);
            playlist.AddTrack(track);
            playlist.AddTrack(track2);
            foreach (var playlistTrack in playlist.Tracks)
            {
                Console.WriteLine(playlistTrack);
            }
        }
    }

    internal class Track : ITrack
    {
        public DateTime Start { get; }
        public DateTime Stop { get; }

        public Track(DateTime start, DateTime stop)
        {
            Start = start;
            Stop = stop;
        }

        public override string ToString()
        {
            return $"< {Start} : {Stop} >";
        }
    }
}