using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConvFMML.Data.Intermediate;
using ConvFMML.Data.MML;
using ConvFMML.Converter;
using ConvFMML.Modifier;
using ConvFMML.Properties;

namespace ConvFMML.Form
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private Settings settings = Settings.Load();
        private Intermediate srcMusic;
        private Intermediate modTBMusic = null;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var treeNode1 = new TreeNode(Properties.Resources.MainMenuMMLExpression1);
            var mmlPanel = new MMLExpressionPanel1(settings.mmlExpression);
            mmlPanel.TimeBaseFormButtonClick += new EventHandler(mmlPanel_TimeBaseFormButtonClick);
            mmlPanel.MMLStyleChanged += new EventHandler(mmlPanel_MMLStyleChanged);
            treeNode1.Tag = mmlPanel;
            var treeNode2 = new TreeNode(Resources.MainMenuMMLExpression2);
            treeNode2.Tag = new MMLExpressionPanel2(settings.mmlExpression);
            var treeNode3 = new TreeNode(Resources.MainMenuNoteRest1);
            treeNode3.Tag = new NoteRestPanel1(settings.noteRest);
            var treeNode4 = new TreeNode(Resources.MainMenuNoteRest2);
            treeNode4.Tag = new NoteRestPanel2(settings.noteRest, settings.mmlExpression.MMLStyle);
            var treeNode5 = new TreeNode(Resources.MainMenuGeneral);
            treeNode5.Tag = new ControlCommandGenericPanel(settings.controlCommand.generic);
            var treeNode6 = new TreeNode(Resources.MainMenuVolume);
            treeNode6.Tag = new VolumePanel(settings.controlCommand.volume, settings.mmlExpression.MMLStyle);
            var treeNode7 = new TreeNode(Resources.MainMenuPan);
            treeNode7.Tag = new PanPanel(settings.controlCommand.pan, settings.mmlExpression.MMLStyle);
            var treeNode8 = new TreeNode(Resources.MainMenuProgramChange);
            treeNode8.Tag = new ProgramChangePanel(settings.controlCommand.programChange);
            var treeNode9 = new TreeNode(Resources.MainMenuTempo);
            treeNode9.Tag = new TempoPanel(settings.controlCommand.tempo);
            var treeNode10 = new TreeNode(Resources.MainMenuControlCommands, new TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            treeNode10.Tag = treeNode5.Tag;
            var treeNode11 = new TreeNode(Resources.MainMenuPart);
            treeNode11.Tag = new OutputPartPanel(settings.outputPart, settings.mmlExpression.MMLStyle);
            treeView1.Nodes.AddRange(new TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode10,
            treeNode11});

            splitContainer4.Panel2.Controls.Add((BasePanel)treeNode1.Tag);

            string[] fileNameArray = Environment.GetCommandLineArgs();
            if (fileNameArray.Length == 2 && File.Exists(fileNameArray[1]))
            {
                string prevFileName = inputMIDITextBox.Text;
                try
                {
                    splitContainer2.Enabled = false;
                    LoadMIDI(fileNameArray[1]);    // Get 1 file only
                    toolStripStatusLabel1.Text = Resources.StatusBarReady;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    inputMIDITextBox.Text = prevFileName;
                    toolStripStatusLabel1.Text = "";
                }
                finally
                {
                    splitContainer2.Enabled = true;
                    splitContainer2.Panel2.Enabled = (modTBMusic != null);
                }
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            string[] fileNameArray = (string[])drgevent.Data.GetData(DataFormats.FileDrop, false);
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop) && fileNameArray.Length == 1 &&
                string.Equals(Path.GetExtension(fileNameArray[0]), ".mid", StringComparison.OrdinalIgnoreCase))
            {
                drgevent.Effect = DragDropEffects.All;
                return;
            }
            drgevent.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            string prevFileName = inputMIDITextBox.Text;
            try
            {
                splitContainer2.Enabled = false;
                var fileNameArray = (string[])drgevent.Data.GetData(DataFormats.FileDrop, false);
                LoadMIDI(fileNameArray[0]);    // Get 1 file only
                toolStripStatusLabel1.Text = Resources.StatusBarReady;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                inputMIDITextBox.Text = prevFileName;
                toolStripStatusLabel1.Text = (modTBMusic == null) ? "" : Resources.StatusBarReady;
            }
            finally
            {
                splitContainer2.Enabled = true;
                splitContainer2.Panel2.Enabled = (modTBMusic != null);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            settings.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void versionInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new VersionInfoForm())
            {
                form.ShowDialog();
            }
        }

        private void InputMIDIButton_Click(object sender, EventArgs e)
        {
            string prevFileName = inputMIDITextBox.Text;
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = Resources.InputMIDIDIalogTitle;
                    ofd.Filter = "MIDI File (*.mid)|*.mid|All File (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        splitContainer2.Enabled = false;
                        LoadMIDI(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                inputMIDITextBox.Text = prevFileName;
            }
            finally
            {
                toolStripStatusLabel1.Text = (modTBMusic == null) ? "" : Resources.StatusBarReady;
                splitContainer2.Enabled = true;
                splitContainer2.Panel2.Enabled = (modTBMusic != null);
            }
        }

        private void LoadMIDI(string fileName)
        {
            inputMIDITextBox.Text = fileName;

            toolStripStatusLabel1.Text = string.Format(Resources.StatusBarLoad, fileName);
            statusStrip1.Update();

            var reader = new MIDIReader();
            var converter = new MIDIToIntermediateConverter();
            srcMusic = converter.Convert(reader.Read(fileName, 1));

            UpdateMusicdata();

            AutoGenerateOutputName(fileName);
        }

        private void AutoGenerateOutputName(string inputName)
        {
            outputMMLTextBox.Text = Path.Combine(
                    Path.GetDirectoryName(inputName),
                    (Path.GetFileNameWithoutExtension(inputName) + settings.mmlExpression.Extension));
        }

        private void UpdateMusicdata()
        {
            toolStripStatusLabel1.Text = Resources.StatusBarTimeBase;
            statusStrip1.Update();
            SetModifiedMusicData();

            List<NotesStatus> list = modTBMusic.GetNotesStatusList();
            foreach (TreeNode tn in treeView1.Nodes)
            {
                ((BasePanel)tn.Tag).LoadMusicData(list);
            }
        }

        private void SetModifiedMusicData()
        {
            modTBMusic = srcMusic.Clone();
            modTBMusic.CountsPerWholeNote = (int)settings.mmlExpression.TimeBase;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = Resources.ExportAs;
                    sfd.FileName = Path.GetFileName(outputMMLTextBox.Text);
                    sfd.InitialDirectory = Path.GetDirectoryName(outputMMLTextBox.Text);
                    sfd.RestoreDirectory = true;

                    switch (settings.mmlExpression.MMLStyle)
                    {
                        case MMLStyle.Custom:
                            sfd.Filter = "All File (*.*)|*.*";
                            break;
                        case MMLStyle.FMP7:
                            sfd.Filter = "MWI File (*.mwi)|*.mwi";
                            break;
                        case MMLStyle.FMP:
                            sfd.Filter = "MPI File (*.mpi)|*.mpi|MVI File (*.mvi)|*.mvi|MZI File (*.mzi)|*.mzi";
                            sfd.FilterIndex = settings.mmlExpression.ExtensionFMP;
                            break;
                        case MMLStyle.MXDRV:
                            sfd.Filter = "MUS File (*.mus)|*.mus";
                            break;
                        case MMLStyle.PMD:
                        case MMLStyle.NRTDRV:
                            sfd.Filter = "MML File (*.mml)|*.mml";
                            break;
                        case MMLStyle.MUCOM88:
                            sfd.Filter = "MUC File (*.muc)|*.muc";
                            break;
                        case MMLStyle.Mml2vgm:
                            sfd.Filter = "GWI File (*.gwi)|*.gwi";
                            break;
                        default:
                            break;
                    }

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        outputMMLTextBox.Text = sfd.FileName;

                        if (settings.mmlExpression.MMLStyle == MMLStyle.Custom)
                        {
                            settings.mmlExpression.ExtensionCustom = Path.GetExtension(sfd.FileName);
                        }
                        else if (settings.mmlExpression.MMLStyle == MMLStyle.FMP)
                        {
                            settings.mmlExpression.ExtensionFMP = sfd.FilterIndex;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                splitContainer2.Enabled = false;

                toolStripStatusLabel1.Text = Resources.StatusBarArrange;
                statusStrip1.Update();
                var modifier = MusicDataModifier.Factory(settings.mmlExpression.MMLStyle);
                List<NotesStatus> statusList = null;
                foreach (TreeNode tn in treeView1.Nodes)
                {
                    if (tn.Tag is OutputPartPanel)
                    {
                        statusList = ((OutputPartPanel)tn.Tag).GetOutputPartSettings();
                        break;
                    }
                }
                Intermediate modMusic = modifier.Modify(modTBMusic, settings, statusList);

                IntermediateToMMLConverter converter = IntermediateToMMLConverter.Factory(settings.mmlExpression.MMLStyle);
                MML mml = converter.Convert(modMusic, settings, statusList);

                toolStripStatusLabel1.Text = Resources.StatusBarExport;
                statusStrip1.Update();
                var printer = new MMLPrinter();
                printer.Print(mml, outputMMLTextBox.Text, settings);

                toolStripStatusLabel1.Text = Resources.StatusBarComplete;
                statusStrip1.Update();
                MessageBox.Show(Resources.SuccessText, Resources.SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = Resources.StatusBarFailed;
                statusStrip1.Update();
                MessageBox.Show(ex.Message, Resources.FailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                SetModifiedMusicData();
                toolStripStatusLabel1.Text = Resources.StatusBarReady;
                splitContainer2.Enabled = true;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var panel = (BasePanel)e.Node.Tag;
            if (!splitContainer4.Panel2.Controls.Contains(panel))
            {
                splitContainer4.Panel2.Controls.Clear();
                splitContainer4.Panel2.Controls.Add(panel);
            }
        }

        private void splitContainer4_Panel2_ControlAdded(object sender, ControlEventArgs e)
        {
            var panel = (BasePanel)e.Control;
            panel.UpdateSelections(settings.mmlExpression.MMLStyle);
        }

        private void mmlPanel_TimeBaseFormButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new TimeBaseForm(settings.mmlExpression.TimeBase))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        settings.mmlExpression.TimeBase = dialog.Timebase;
                    }
                    else
                    {
                        return;
                    }
                }

                splitContainer2.Enabled = false;

                UpdateMusicdata();

                splitContainer2.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                toolStripStatusLabel1.Text = Resources.StatusBarReady;
            }
        }

        private void mmlPanel_MMLStyleChanged(object sender, EventArgs e)
        {
            foreach (TreeNode tn in treeView1.Nodes)
            {
                if (tn.Tag is OutputPartPanel)
                {
                    ((OutputPartPanel)tn.Tag).ChangeOutputPartMMLSyle(settings.mmlExpression.MMLStyle);
                }
            }

            AutoGenerateOutputName(inputMIDITextBox.Text);
        }
    }
}
