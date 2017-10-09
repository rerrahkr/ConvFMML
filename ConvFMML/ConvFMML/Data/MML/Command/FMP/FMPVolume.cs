﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.FMP
{
    public class FMPVolume : Volume
    {
        public FMPVolume(int value, MMLCommandRelation relation) : base(value, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            return "v" + (Value * 15 / 127);
        }
    }
}
