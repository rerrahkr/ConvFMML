using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML
{
    [Serializable]
    public class Settings
    {
        public MMLExpression mmlExpression { get; set; } = new MMLExpression();
        public NoteRest noteRest { get; set; } = new NoteRest();
        public ControlCommand controlCommand { get; set; } = new ControlCommand();
        public OutputPart outputPart { get; set; } = new OutputPart();

        [Serializable]
        public class MMLExpression
        {
            public MMLStyle MMLStyle { set; get; } = MMLStyle.FMP7;
            public string ExtensionCustom { set; get; } = ".mml";
            public int ExtensionFMP { set; get; } = 1;
            public decimal TimeBase { set; get; } = 192;
            public int PrintTimeBase { set; get; } = 1;
            public int PrintTimeBasePMD { set; get; } = 1;
            public int NewBlockByBar { set; get; } = 1;
            public decimal NewLineBarCount { set; get; } = 2;
            public int NewLineByTimeSignature { set; get; } = 1;
            public int TitleEnable { set; get; } = 1;

            public string Extension
            {
                get
                {
                    switch (MMLStyle)
                    {
                        case MMLStyle.Custom:
                            return ExtensionCustom;
                        case MMLStyle.FMP7:
                            return ".mwi";
                        case MMLStyle.FMP:
                            switch (ExtensionFMP)
                            {
                                case 1: return ".mpi";
                                case 2: return ".mvi";
                                case 3: return ".mzi";
                                default: return null;
                            }
                        case MMLStyle.MXDRV:
                            return ".mus";
                        case MMLStyle.PMD:
                        case MMLStyle.NRTDRV:
                            return ".mml";
                        case MMLStyle.MUCOM88:
                            return ".muc";
                        default:
                            return null;
                    }
                }
            }
        }

        [Serializable]
        public class NoteRest
        {
            public int OctaveInNewLine { set; get; } = 0;
            public string OctaveCommandCustom { set; get; } = "";
            public int OctaveDirection { set; get; } = 0;
            public int LengthStyle { set; get; } = 0;
            public bool DotEnable { set; get; } = true;
            public decimal DotLength { set; get; } = 0;
            public int CutByBar { set; get; } = 1;
            public bool NewBlockInCutted { set; get; } = true;
            public string TieCommandCustom { set; get; } = "";
            public int TieStyle { set; get; } = 0;
            public bool UnuseTiedRest { set; get; } = false;
        }

        [Serializable]
        public class ControlCommand
        {
            public Generic generic { set; get; } = new Generic();
            public Volume volume { set; get; } = new Volume();
            public Pan pan { set; get; } = new Pan();
            public ProgramChange programChange { set; get; } = new ProgramChange();
            public Tempo tempo { set; get; } = new Tempo();

            [Serializable]
            public class Generic
            {
                public int Invalid { set; get; } = 1;
                public int SamePosition { set; get; } = 1;
                public int Predeclared { set; get; } = 1;
            }

            [Serializable]
            public class Volume
            {
                public bool Enable { set; get; } = true;
                public int CommandPMD { set; get; } = 0;
                public int CommandMXDRV { set; get; } = 0;
                public int CommandNRTDRV { set; get; } = 0;
                public string CommandCustom { set; get; } = "";
                public decimal RangeCustom { set; get; } = 0;
                public decimal VStep { set; get; } = 15;
            }

            [Serializable]
            public class Pan
            {
                public bool Enable { set; get; } = true;
                public int CommandCustom { set; get; } = 1;
                public int CommandFMP7 { set; get; } = 1;
                public string MIDICommandCustom { set; get; } = "";
                public string LeftCommandCustom { set; get; } = "";
                public string CenterCommandCustom { set; get; } = "";
                public string RightCommandCustom { set; get; } = "";
                public decimal BorderLeft { set; get; } = 32;
                public decimal BorderRight { set; get; } = 96;
                public bool BorderUsingNegative { set; get; } = false;
            }

            [Serializable]
            public class ProgramChange
            {
                public bool Enable { set; get; } = true;
                public string CommandCustom { set; get; } = "";
            }

            [Serializable]
            public class Tempo
            {
                public bool Enable { set; get; } = true;
                public string CommandCustom { set; get; } = "";
            }
        }

        [Serializable]
        public class OutputPart
        {
            public int PrintStyle { set; get; } = 2;
            public int PrintStyleCustom { set; get; } = 1;
            public int AutoNameFMP7 { set; get; } = 0;
            public int AutoNameFMP { set; get; } = 0;
            public int AutoNamePMD { set; get; } = 2;
            public int AutoNameNRTDRV { set; get; } = 1;
            public bool RemoveEmptyParts { set; get; } = true;
        }

        public static Settings Load()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                path = Path.Combine(path, Common.AssemblyCompany, Common.AssemblyTitle);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, "Settings.xml");

                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                using (var sr = new StreamReader(path, new UTF8Encoding(false)))
                {
                    return (Settings)serializer.Deserialize(sr);
                }
            }
            catch
            {
                return new Settings();  // Set default settings in first execution or read failure
            }
        }

        public void Save()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                path = Path.Combine(path, Common.AssemblyCompany, Common.AssemblyTitle);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, "Settings.xml");

                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
                {
                    serializer.Serialize(sw, this);
                }
            }
            catch
            {
                // Fail to save setings
            }
        }
    }
}
