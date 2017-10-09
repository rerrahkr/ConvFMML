using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML
{
    public static class Common
    {
        public static string AssemblyTitle
        {
            get
            {
                var ata = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false);
                return ata.Title;
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                var ada = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false);
                return ada.Description;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                var aca = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false);
                return aca.Company;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                var aca = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false);
                return aca.Copyright;
            }
        }

        public static string AssemblyFileVersion
        {
            get
            {
                var afva = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyFileVersionAttribute), false);
                return afva.Version;
            }
        }

        public static System.Drawing.Icon Icon
        {
            get
            {
                string path = Assembly.GetEntryAssembly().Location;
                return System.Drawing.Icon.ExtractAssociatedIcon(path);
            }
        }
    }

    public enum MMLStyle : int
    {
        FMP7 = 0,
        FMP = 1,
        PMD = 2,
        MXDRV = 3,
        NRTDRV = 4,
        Custom = 5
    }

    public enum Key : int
    {
        CMaj, GMaj, DMaj, AMaj, EMaj, BMaj, FsMaj, CsMaj, FMaj, BfMaj, EfMaj, AfMaj, DfMaj, GfMaj, CfMaj,
        Amin, Emin, Bmin, Fsmin, Csmin, Gsmin, Dsmin, Asmin, Dmin, Gmin, Cmin, Fmin, Bfmin, Efmin, Afmin
    }

    public enum SoundModule
    {
        FM,
        SSG,
        FM3ch,
        Others
    }

    public enum MMLCommandRelation
    {
        Clear = 0x00,
        TieBefore = 0x01,
        TieAfter = 0x02,
        PrevControl = 0x04,
        NextControl = 0x08
    }
}
