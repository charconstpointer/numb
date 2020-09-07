using System;
using System.Threading.Tasks;
using Numb;

namespace Runner
{
    class Program
    {
        private static async Task Main()
        {
            var source = new RkSource();
            var playlist = await Playlist.CreateFrom(source);
            var watcher = new Watcher();
            watcher.AddTarget("keyName", playlist);
            watcher.Start();
            watcher.TrackChanged += (sender, changed) => Console.WriteLine($"{changed}");
            Console.ReadKey();
        }
    }
}