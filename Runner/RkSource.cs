using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Numb;

namespace Runner
{
    internal class RkSource : ITracksSource
    {
        public async Task<IEnumerable<ITrack>> GetAsync()
        {
            var client = new HttpClient();
            var mojePolskieResponse = await client.GetFromJsonAsync<MojepolskieResponse>(
                $"https://moje.polskieradio.pl/api/?mobilestationid={121}&key=d590cafd-31c0-4eef-b102-d88ee2341b1a");
            mojePolskieResponse.Id = 121;
            for (var i = 0; i < mojePolskieResponse.Songs.Count() - 1; i++)
            {
                var current = mojePolskieResponse.Songs.ElementAt(i);
                var next = mojePolskieResponse.Songs.ElementAt(i + 1);
                current.Stop = next.ScheduleTime;
            }

            var tracks =
                mojePolskieResponse.Songs.Select(s => new Track(s.Id, s.Title, s.Artist, s.ScheduleTime, s.Stop));
            return tracks;
        }
    }
}