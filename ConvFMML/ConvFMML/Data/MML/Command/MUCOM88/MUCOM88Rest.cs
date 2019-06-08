using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML.Command.MUCOM88
{
    public class MUCOM88Rest : Rest
    {
        public MUCOM88Rest(List<int> length, MMLCommandRelation relation) : base(length, relation) { }

        protected override string GenerateString(Settings settings, SoundModule module)
        {
            int len = Length[0];
            string str = "r" + len.ToString();
            Settings.NoteRest nrSettings = settings.noteRest;

            int dotlen = 0;
            for (int i = 1; i < Length.Count; i++)
            {
                if (len * 2 == Length[i] && nrSettings.DotEnable && dotlen < 1)
                {
                    str += ".";
                    dotlen++;
                }
                else
                {
                    str = str + "r" + Length[i];
                    dotlen = 0;
                }
                len = Length[i];
            }

            if (!nrSettings.UnuseTiedRest && CommandRelation.HasFlag(MMLCommandRelation.TieAfter) && !CommandRelation.HasFlag(MMLCommandRelation.NextControl))
            {
                str += "&";
            }

            return str;
        }
    }
}
