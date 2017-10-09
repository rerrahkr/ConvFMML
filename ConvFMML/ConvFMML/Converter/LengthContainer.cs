using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Converter
{
    public class LengthContainer : ICloneable
    {
        private List<LengthElement> length = new List<LengthElement>();

        public int Count
        {
            get
            {
                return length.Count;
            }
        }

        private int _gate = 0;
        public int Gate
        {
            get
            {
                return _gate;
            }
        }

        public int TripletCount
        {
            get
            {
                int cnt = 0;
                foreach (LengthElement e in length)
                {
                    if (e.TripletFlag)
                    {
                        cnt++;
                    }
                }
                return cnt;
            }
        }

        public void AddLengthElement(LengthElement e)
        {
            length.Add(e);
            length = length.OrderBy(x => x.Length).ToList();
            _gate += e.Gate;
        }

        public int[] GetLength(Settings.NoteRest settings)
        {
            var list = new List<LengthElement>();

            if (settings.DotEnable)
            {
                var temp = new LinkedList<LengthElement>(length);
                list.Add(temp.First.Value);
                temp.RemoveFirst();

                while (temp.Count > 0)
                {
                    int prevlen = list[list.Count - 1].Length;
                    LinkedListNode<LengthElement> n = temp.First;
                    while (true)
                    {
                        if (n.Value.Length == prevlen / 2)
                        {
                            list.Add(n.Value);
                            temp.Remove(n);
                            break;
                        }

                        if (n.Next == null)
                        {
                            list.Add(temp.First.Value);
                            temp.RemoveFirst();
                            break;
                        }
                        else
                        {
                            n = n.Next;
                        }
                    }
                }
            }
            else
            {
                list = length;
            }

            return list.Select(x => x.Length).ToArray();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public LengthContainer Clone()
        {
            var clone = new LengthContainer();
            length.ForEach(x => clone.AddLengthElement(x));
            return clone;
        }
    }
}
