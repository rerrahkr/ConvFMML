using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MIDI
{
    public class Track
    {
        public LinkedList<Event.Event> EventList { get; }

        public Track(LinkedList<Event.Event> eventList)
        {
            EventList = eventList;
        }
    }
}
