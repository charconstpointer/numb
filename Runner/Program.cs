using System;
using System.Threading.Tasks;
using Numb;

namespace Runner
{
    class Program
    {
        private static async Task Main()
        {
            var rks = new RkSource();
            var playlist = new PlaylistBuilder()
                .WithChecks(3)
                .WithSource(rks)
                .OnChange(e => Console.WriteLine("hi there"))
                .Build();

            await playlist.Start();
        }
    }
}