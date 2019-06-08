using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.FMP
{
    public class FMPRest : Rest
    {
        public FMPRest(List<int> length, MMLCommandRelation relation) : base(length, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            int len = Length[0];
            string str = string.Empty;
            Settings.NoteRest nrSettings = settings.noteRest;

            if (nrSettings.TieStyle == 0 || !CommandRelation.HasFlag(MMLCommandRelation.TieBefore) || CommandRelation.HasFlag(MMLCommandRelation.PrevControl))
            {
                str += "r";
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
                    if (nrSettings.TieStyle == 0)       // No Tie
                    {
                        str = str + "r" + Length[i];
                    }
                    else       // Tie only
                    {
                        str = str + "&" + Length[i];
                    }
                    dotlen = 0;
                }
                len = Length[i];
            }

            if (nrSettings.TieStyle == 1 && CommandRelation.HasFlag(MMLCommandRelation.TieAfter) && !CommandRelation.HasFlag(MMLCommandRelation.NextControl))
            {
                str += "&";
            }

            return str;
        }
    }
}
