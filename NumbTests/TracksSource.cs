using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Numb;

namespace NumbTests
{
    public class TracksSource : ITracksSource
    {
        public async Task<IEnumerable<ITrack>> GetAsync()
        {
            var now = new DateTime(2020, 1, 1, 1, 1, 1);
            return await Task.FromResult(new List<ITrack> {new Track(now, now.AddMilliseconds(3))});
        }
    }
}