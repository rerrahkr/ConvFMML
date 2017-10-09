using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.FMP
{
    public class FMPNote : Note
    {
        public FMPNote(int octave, string name, List<int> length, MMLCommandRelation relation) : base(octave, name, length, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            int len = Length[0];
            string str = String.Empty;
            Settings.NoteRest nrSettings = settings.noteRest;

            if (nrSettings.TieStyle == 0 ||
                (!CommandRelation.HasFlag(MMLCommandRelation.TieBefore) || CommandRelation.HasFlag(MMLCommandRelation.PrevControl)))
            {
                str += Name;
            }
            str += len.ToString();

            int dotlen = 0;
            for (int i = 1; i < Length.Count; i++)
            {
                if (len * 2 == Length[i] && nrSettings.DotEnable &&
                    (nrSettings.DotLength == 0 || dotlen < nrSettings.DotLength))
                {
                    str += ".";
                    dotlen++;
                }
                else
                {
                    if (nrSettings.TieStyle == 0)       // Tie and Name
                    {
                        str = str + "&" + Name + Length[i];
                    }
                    else       // Tie only
                    {
                        str = str + "&" + Length[i];
                    }
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
