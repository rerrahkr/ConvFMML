﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvFMML.Data.MML;
using ConvFMML.Data.MML.Command;
using System.IO;
using ConvFMML.Properties;

namespace ConvFMML
{
    public class MMLPrinter
    {
        public void Print(MML mml, string path, Settings settings)
        {
            try
            {
                using (var sw = new StreamWriter(path, false, Encoding.GetEncoding(
                    (mml.Style == MMLStyle.Mml2vgm) ? "utf-8" : "shift_jis")))
                {
                    string str = GenerateHeader(mml, settings);
                    sw.Write(str);


                    foreach (Part part in mml.PartList)
                    {
                        Note prevNote = null;
                        bool isNewLineNote = false;
                        bool needPartName = true;


                        for (int i = 0; i < part.Length; i++)
                        {
                            Bar bar = part.BarList[i];
                            str = string.Empty;


                            if (bar.CommandList.Count > 0)
                            {
                                // Print part name
                                if (needPartName)
                                {
                                    if (settings.outputPart.PrintStyle != 0)
                                    {
                                        str = part.Name + (settings.mmlExpression.UseTabAfterPartName ? "\t" : " ");
                                    }
                                    needPartName = false;

                                    if (i == 0 &&
                                        ((mml.PartList.IndexOf(part) == 0 &&
                                        (mml.Style == MMLStyle.FMP && settings.mmlExpression.PrintTimeBase == 1) ||
                                        (mml.Style == MMLStyle.PMD && settings.mmlExpression.PrintTimeBasePMD == 2))
                                        || (mml.Style == MMLStyle.MUCOM88 && settings.mmlExpression.PrintTimeBase == 1)))
                                    {
                                        str += "C" + mml.CountsPerWholeNote + " ";
                                    }
                                }


                                // Print commands a bar
                                foreach (Command c in bar.CommandList)
                                {
                                    if (c is Note)
                                    {
                                        var note = (Note)c;
                                        if (prevNote == null)
                                        {
                                            if (mml.Style == MMLStyle.Custom)
                                            {
                                                str += settings.noteRest.OctaveCommandCustom + note.Octave + " ";
                                            }
                                            else
                                            {
                                                str += "o" + note.Octave + " ";
                                            }
                                        }
                                        else
                                        {
                                            if (!isNewLineNote && settings.noteRest.OctaveInNewLine == 1)
                                            {
                                                if (mml.Style == MMLStyle.Custom)
                                                {
                                                    str += settings.noteRest.OctaveCommandCustom + note.Octave + " ";
                                                }
                                                else
                                                {
                                                    str += "o" + note.Octave + " ";
                                                }
                                            }
                                            else
                                            {
                                                int dif = note.Octave - prevNote.Octave;
                                                if (dif > 0)
                                                {
                                                    for (int j = 0; j < dif; j++)
                                                    {
                                                        switch (mml.Style)
                                                        {
                                                            case MMLStyle.Custom:
                                                            case MMLStyle.MXDRV:
                                                            case MMLStyle.NRTDRV:
                                                            case MMLStyle.PMD:
                                                            case MMLStyle.Mml2vgm:
                                                                str += ((settings.noteRest.OctaveDirection == 0) ? ">" : "<");
                                                                break;
                                                            default:
                                                                str += ">";
                                                                break;
                                                        }
                                                    }
                                                }
                                                else if (dif < 0)
                                                {
                                                    for (int j = 0; j > dif; j--)
                                                    {
                                                        switch (mml.Style)
                                                        {
                                                            case MMLStyle.Custom:
                                                            case MMLStyle.MXDRV:
                                                            case MMLStyle.NRTDRV:
                                                            case MMLStyle.PMD:
                                                            case MMLStyle.Mml2vgm:
                                                                str += ((settings.noteRest.OctaveDirection == 0) ? "<" : ">");
                                                                break;
                                                            default:
                                                                str += "<";
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                            isNewLineNote = true;
                                        }
                                        prevNote = note;
                                    }


                                    str += c.ToString(settings, part.SoundModule);


                                    if (bar.CommandList.Last.Value != c)
                                    {
                                        if (c.CommandRelation.HasFlag(MMLCommandRelation.NextControl))
                                        {
                                            str += " ";
                                        }
                                        else if (c is ControlCommand && !c.CommandRelation.HasFlag(MMLCommandRelation.NextControl))
                                        {
                                            str += " ";
                                        }
                                    }
                                }
                            }


                            if (bar == part.BarList.Last())
                            {
                                if (!needPartName)
                                {
                                    str += Environment.NewLine;
                                }
                                str += Environment.NewLine;
                            }
                            else
                            {
                                if (!needPartName)
                                {
                                    str += bar.SeperateSign;
                                    if (bar.SeperateSign == Environment.NewLine)
                                    {
                                        isNewLineNote = false;
                                        needPartName = true;
                                    }
                                }
                            }


                            sw.Write(str);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.ErrorMMLFailed, path), ex);
            }
        }

        private string GenerateHeader(MML mml, Settings settings)
        {
            string str = string.Empty;
            bool flag = false;

            switch (mml.Style)
            {
                case MMLStyle.FMP7:
                    if (settings.mmlExpression.TitleEnable == 1 && mml.Title.Length > 0)
                    {
                        str += " Title=" + mml.Title + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.mmlExpression.PrintTimeBase == 1)
                    {
                        str += " ClockCount=" + mml.CountsPerWholeNote + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.outputPart.PrintStyle != 0)
                    {
                        var lookUp = mml.PartList.ToLookup(x => x.SoundModule, x => x.Name);
                        if (lookUp[SoundModule.FM].Count() > 0)
                        {
                            string stemp = string.Empty;
                            foreach (string s in lookUp[SoundModule.FM])
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    stemp += s.Substring(1);
                                }
                            }
                            if (stemp.Count() > 0)
                            {
                                str = str + " PartOPNA=" + stemp + Environment.NewLine;
                                flag = true;
                            }
                        }
                        if (lookUp[SoundModule.SSG].Count() > 0)
                        {
                            string stemp = string.Empty;
                            foreach (string s in lookUp[SoundModule.SSG])
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    stemp += s.Substring(1);
                                }
                            }
                            if (stemp.Count() > 0)
                            {
                                str = str + " PartSSG=" + stemp + Environment.NewLine;
                                flag = true;
                            }
                        }
                    }
                    if (flag)
                    {
                        str = "\'{" + Environment.NewLine +
                            str +
                            "}" + Environment.NewLine;
                    }
                    break;
                case MMLStyle.MXDRV:
                    if (settings.mmlExpression.TitleEnable == 1 && mml.Title.Length > 0)
                    {
                        str += "#title\t\"" + mml.Title + "\"" + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.noteRest.OctaveDirection == 1)
                    {
                        str = "#OCTAVE-REV" + Environment.NewLine;
                        flag = true;
                    }
                    break;
                case MMLStyle.NRTDRV:
                    if (settings.mmlExpression.TitleEnable == 1 && mml.Title.Length > 0)
                    {
                        str += "#TITLE\t" + mml.Title + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.mmlExpression.PrintTimeBase == 1)
                    {
                        str += "#COUNT\t" + mml.CountsPerWholeNote + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.noteRest.OctaveDirection == 1)
                    {
                        str = "#OCTAVE_REV" + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.controlCommand.volume.Enable && settings.controlCommand.volume.CommandNRTDRV == 0)
                    {
                        str += "#V_STEP\t" + settings.controlCommand.volume.VStep + Environment.NewLine;
                        flag = true;
                    }
                    break;
                case MMLStyle.PMD:
                    if (settings.mmlExpression.TitleEnable == 1 && mml.Title.Length > 0)
                    {
                        str += "#Title\t" + mml.Title + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.mmlExpression.PrintTimeBasePMD == 1)
                    {
                        str += "#Zenlen\t" + mml.CountsPerWholeNote + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.outputPart.PrintStyle == 1 ||
                        (settings.outputPart.PrintStyle == 2 && settings.outputPart.AutoNamePMD == 2))
                    {
                        var lookUp = mml.PartList.ToLookup(x => x.SoundModule, x => x.Name);
                        if (lookUp[SoundModule.FM3ch].Count() > 0)
                        {
                            string stemp = string.Empty;
                            lookUp[SoundModule.FM3ch].ToList().ForEach(x => stemp += x);
                            if (stemp.Count() > 0)
                            {
                                str = str + "#FM3Extend\t" + stemp + Environment.NewLine;
                                flag = true;
                            }
                        }
                    }
                    if (settings.noteRest.OctaveDirection == 1)
                    {
                        str += "#Octave\tReverse" + Environment.NewLine;
                        flag = true;
                    }
                    break;
                case MMLStyle.MUCOM88:
                    if (settings.mmlExpression.TitleEnable == 1 && mml.Title.Length > 0)
                    {
                        str += "#title\t" + mml.Title + Environment.NewLine;
                        flag = true;
                    }
                    break;
                case MMLStyle.Mml2vgm:
                    if (settings.mmlExpression.TitleEnable == 1 && mml.Title.Length > 0)
                    {
                        str += " TitleName=" + mml.Title + Environment.NewLine;
                        flag = true;
                    }
                    if (settings.mmlExpression.PrintTimeBase == 1)
                    {
                        str += " ClockCount=" + mml.CountsPerWholeNote + Environment.NewLine;
                        flag = true;
                    }
                    if (flag)
                    {
                        str = "\'{" + Environment.NewLine +
                            str +
                            "}" + Environment.NewLine;
                    }
                    break;
                default:
                    break;
            }

            if (flag)
            {
                str += Environment.NewLine;
            }

            return str;
        }
    }
}
