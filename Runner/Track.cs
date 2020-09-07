using System;
using Numb;

namespace Runner
{
    public sealed class Track : ITrack
    {
        public Track(int id, string title, string artist, DateTime start, DateTime stop)
        {
            var image = "https://i.imgur.com/aj6sB86.png";
            Id = id;
            Title = title;
            Artist = artist;
            Image = image;
            Start = start;
            Stop = stop;
        }

        private Track()
        {
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Image { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }

        public override string ToString()
        {
            return $"{Title}" +
                   $", {Artist}";
        }
    }
}