using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.Intermediate
{
    public class Position : IComparable<Position>, ICloneable
    {
        public uint Bar { get; }
        public uint Tick { get; }

        public Position(uint bar, uint tick)
        {
            Bar = bar;
            Tick = tick;
        }

        public Position(Position prevPosition, uint deltaTime, LinkedListNode<Event.TimeSignature> tsNode)
        {
            Bar = prevPosition.Bar;
            Tick = prevPosition.Tick + deltaTime;

            while (true)
            {
                if (tsNode.Next == null || Bar != tsNode.Next.Value.PrevSignedPosition.Bar)
                {
                    if (Tick >= tsNode.Value.TickPerBar)
                    {
                        Bar++;
                        Tick -= tsNode.Value.TickPerBar;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (CompareTo(tsNode.Next.Value.PrevSignedPosition) < 0)
                {
                    break;
                }
                else
                {
                    if (tsNode.Next.Value.PrevSignedPosition.Tick > 0)
                    {
                        Bar++;
                        Tick -= tsNode.Next.Value.PrevSignedPosition.Tick;
                    }
                    tsNode = tsNode.Next;
                }
            }
        }

        public static Position ConvertByTimeDivisionRatio(Position position, double ratio)
        {
            var newTick = (uint)(position.Tick * ratio);
            return new Position(position.Bar, newTick);
        }

        public static Position ConvertByTicksPerBar(Position position, uint prevTicksPerBar, uint ticksPerBar)
        {
            uint newBar = 0;
            uint newTick = 0;
            uint tmpBar = position.Bar;
            uint tmpTick = position.Tick;

            do
            {
                if (tmpTick == 0)
                {
                    newTick += prevTicksPerBar;
                    while (newTick >= ticksPerBar)
                    {
                        newBar++;
                        newTick -= ticksPerBar;
                    }
                    tmpBar--;
                }
                else
                {
                    newTick += tmpTick;
                    while (newTick >= ticksPerBar)
                    {
                        newBar++;
                        newTick -= ticksPerBar;
                    }
                    tmpTick = 0;
                }
            } while (tmpBar > 0);

            return new Position(newBar, newTick);
        }

        public Position Add(Position a, uint ticksPerBar)
        {
            uint newBar = Bar;
            uint newTick = Tick;

            newTick += a.Tick;
            if (newTick >= ticksPerBar)
            {
                newBar++;
                newTick -= ticksPerBar;
            }
            newBar += a.Bar;

            return new Position(newBar, newTick);
        }

        public Position Subtract(Position a, uint ticksPerWholeNote)
        {
            uint newBar = Bar;
            uint newTick = Tick;

            if (newTick < a.Tick)
            {
                newTick += ticksPerWholeNote;
                newBar--;
            }

            newBar -= a.Bar;
            newTick -= a.Tick;

            return new Position(newBar, newTick);
        }

        public int CompareTo(Position other)
        {
            if (other == null) throw new ArgumentNullException("other");

            int cmp = Bar.CompareTo(other.Bar);
            if (cmp == 0)
            {
                return Tick.CompareTo(other.Tick);
            }
            else
            {
                return cmp;
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Position Clone()
        {
            return (Position)MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{Bar,3}:{Tick,4}";
        }
    }
}
