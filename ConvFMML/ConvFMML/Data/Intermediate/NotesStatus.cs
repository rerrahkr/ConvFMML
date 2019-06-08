using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate
{
    public class NotesStatus : ICloneable
    {
        public string Name { get; set; } = string.Empty;
        public int TrackNumber { get; }
        public string TrackName { get; }
        public int NumberInTrack { get; }
        public bool Printable { get; set; } = true;
        public bool IsEmpty { get; }
        public SoundModule SoundModule { get; set; } = SoundModule.FM;

        public NotesStatus(int trackNumber, string trackName, int numberInTrack, bool isEmpty)
        {
            TrackNumber = trackNumber;
            TrackName = trackName;
            NumberInTrack = numberInTrack;
            IsEmpty = isEmpty;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public NotesStatus Clone()
        {
            return (NotesStatus)MemberwiseClone();
        }
    }
}
