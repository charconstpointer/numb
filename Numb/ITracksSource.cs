using System.Collections.Generic;
using System.Threading.Tasks;

namespace Numb
{
    public interface ITracksSource
    {
        Task<IEnumerable<ITrack>> GetAsync();
    }
}