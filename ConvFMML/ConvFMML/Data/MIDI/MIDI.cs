using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI
{
    public class MIDI
    {
        public List<Track> TrackList { get; }
        public int TrackSize { get; }
        public int TimeDivision { get; }
        public int Format { get; }

        public MIDI(List<Track> trackList, int format, int trackSize, int timeDivision)
        {
            TrackList = trackList;
            Format = format;
            TrackSize = trackSize;
            TimeDivision = timeDivision;
        }
    }
}
