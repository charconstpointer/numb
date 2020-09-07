namespace Numb
{
    public interface IPlaylist
    {
        ITrack Current();
        ITrack Next();
        void MoveNext();
        ITrack Previous();
    }
}