using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML
{
    public static class Tables
    {
        public static readonly Key[,] KeyTable = new Key[,]
        {
            { Key.CfMaj, Key.GfMaj, Key.DfMaj, Key.AfMaj, Key.EfMaj, Key.BfMaj, Key.FMaj, Key.CMaj, Key.GMaj, Key.DMaj, Key.AMaj, Key.EMaj, Key.BMaj, Key.FsMaj, Key.CsMaj },
            { Key.Afmin, Key.Efmin, Key.Bfmin, Key.Fmin, Key.Cmin, Key.Gmin, Key.Dmin, Key.Amin, Key.Emin, Key.Bmin, Key.Fsmin, Key.Csmin, Key.Gsmin, Key.Dsmin, Key.Asmin}
        };

        public static readonly Dictionary<Key, string[]> NoteNameDictionary = new Dictionary<Key, string[]>
        {
            { Key.CMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.GMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.DMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.AMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.EMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.BMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.FsMaj, new string[] { "c",  "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b" } },
            { Key.CsMaj, new string[] { "b+", "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b" } },
            { Key.FMaj,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "b-", "b" } },
            { Key.BfMaj, new string[] { "c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "g+", "a", "b-", "b" } },
            { Key.EfMaj, new string[] { "c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "b-", "b" } },
            { Key.AfMaj, new string[] { "c",  "d-", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "b-", "b" } },
            { Key.DfMaj, new string[] { "c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "b-", "b" } },
            { Key.GfMaj, new string[] { "c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "b-", "c-" } },
            { Key.CfMaj, new string[] { "c",  "d-", "d", "e-", "f-", "f",  "g-", "g", "a-", "a", "b-", "c-" } },
            { Key.Amin,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Emin,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Bmin,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Fsmin, new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Csmin, new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Gsmin, new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Dsmin, new string[] { "c",  "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Asmin, new string[] { "b+", "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Dmin,  new string[] { "c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "b-", "b" } },
            { Key.Gmin,  new string[] { "c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "g+", "a", "a+", "b" } },
            { Key.Cmin,  new string[] { "c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "a+", "b" } },
            { Key.Fmin,  new string[] { "c",  "d-", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "a+", "b" } },
            { Key.Bfmin, new string[] { "c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "a+", "b" } },
            { Key.Efmin, new string[] { "c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "a+", "c-" } },
            { Key.Afmin, new string[] { "c",  "d-", "d", "e-", "f-", "f",  "g-", "g", "a-", "a", "a+", "c-" } }
        };
    }
}
