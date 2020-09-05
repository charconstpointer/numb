using System;

namespace Numb
{
    public interface ITrack
    {
        DateTime Start { get; }
        DateTime Stop { get; }
    }
}