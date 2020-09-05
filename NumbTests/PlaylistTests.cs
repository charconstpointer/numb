using System;
using System.Linq;
using FluentAssertions;
using Numb;
using Xunit;

namespace NumbTests
{
    public class PlaylistTests
    {
        [Fact]
        public void Playlist_AddTrack_ShouldInsertBeforeExistingTrack()
        {
            var playlist = new Playlist();
            var now = DateTime.UtcNow;
            var track = new Track(now.AddMilliseconds(2), now.AddMilliseconds(3));
            playlist.AddTrack(track);
            var track2 = new Track(now, now.AddMilliseconds(1));
            playlist.AddTrack(track2);
            playlist.Tracks.ElementAt(0).Should().Be(track2);
            playlist.Tracks.ElementAt(1).Should().Be(track);
        }
        
        [Fact]
        public void Playlist_AddTrack_ShouldInsertAfterExistingTrack()
        {
            var playlist = new Playlist();
            var now = DateTime.UtcNow;
            var track = new Track(now.AddMilliseconds(2), now.AddMilliseconds(3));
            playlist.AddTrack(track);
            var track2 = new Track(now.AddMinutes(2), now.AddMinutes(13));
            playlist.AddTrack(track2);
            playlist.Tracks.ElementAt(0).Should().Be(track);
            playlist.Tracks.ElementAt(1).Should().Be(track2);
        }
    }
}