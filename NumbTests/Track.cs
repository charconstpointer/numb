using System;
using Numb;

namespace NumbTests
{
    public class Track : ITrack
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