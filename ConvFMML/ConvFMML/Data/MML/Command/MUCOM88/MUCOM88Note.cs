using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.MUCOM88
{
    public class MUCOM88Note : Note
    {
        public MUCOM88Note(int octave, string name, List<int> length, MMLCommandRelation relation) : base(octave, name, length, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            int len = Length[0];
            string str = Name + len.ToString();
            Settings.NoteRest nrSettings = settings.noteRest;

            int dotlen = 0;
            for (int i = 1; i < Length.Count; i++)
            {
                if (len * 2 == Length[i] && nrSettings.DotEnable &&
                (nrSettings.DotLength == 0 || dotlen < nrSettings.DotLength))
                {
                    str += ".";
                    dotlen++;
                }
                else       // Tie and Name
                {
                    str = str + "&" + Name + Length[i];
                    dotlen = 0;
                }
                len = Length[i];
            }

            if (CommandRelation.HasFlag(MMLCommandRelation.TieAfter))
            {
                str += "&";
            }

            return str;
        }
    }
}
