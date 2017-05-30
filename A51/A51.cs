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

namespace A51
{
    public partial class A51 : Form
    {

        FileSystemWatcher fsw;
        string lastAddedFile = "";
        bool keyStatus = false;
        int programLogCounter = 0;
        public ProgramState.ProgramStateHelper programStateHelper;
        public Label status,processedFile;
        Cypher cyper;
        public A51()
        {
            InitializeComponent();
            status = statusLbl;
            this.processedFile = fileNameLbl;
        }

        private void inputLocationBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                inputLbl.Text = fbd.SelectedPath;

                fsw.Path = fbd.SelectedPath;
                programStateHelper.directoryPathChange();
                programStateHelper.setLastUsedInputPath(fbd.SelectedPath);
            }
        }
        private void outputLocationBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                outputLbl.Text = fbd.SelectedPath;
                programStateHelper.setLastUsedOutputPath(fbd.SelectedPath);
            }
        }
        private void decryptBtn_Click(object sender, EventArgs e)
        {
            if (!keyStatus)
            {
                MessageBox.Show("Key is empty or not valid!");
                return;
            }
            //fsw.EnableRaisingEvents = false;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            DialogResult dResult = openFileDialog1.ShowDialog();
            if ( dResult == DialogResult.OK)
            {
                string filePath = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                cyper.addFileForEncryption(new FilePathAndKey(openFileDialog1.FileName, filePath, textBoxKey.Text, false));
            }
        }
        private void encryptBtn_Click(object sender, EventArgs e)
        {
            if (!keyStatus)
            {
                MessageBox.Show("Key is empty or not valid!");
                return;
            }

            //fsw.EnableRaisingEvents = false;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                cyper.addFileForEncryption(new FilePathAndKey(openFileDialog1.FileName, filePath, textBoxKey.Text, true));
            }
        }
        private void fileSystemWatcherCB_CheckedChanged(object sender, EventArgs e)
        {
            if (fileSystemWatcherCB.Checked)
            {
                fsw.EnableRaisingEvents = true;
                programStateHelper.setFileSystemWathcer(true);
            }
            else
            {
                fsw.EnableRaisingEvents = false;
                programStateHelper.setFileSystemWathcer(false);
            }
        }
        private void setupFileSystemWatcher(string folderPath)
        {
            fsw = new FileSystemWatcher();
            fsw.Path = folderPath;

            fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            fsw.Filter = "*.*";

            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.Created += new FileSystemEventHandler(OnChanged);
            fsw.Deleted += new FileSystemEventHandler(OnDelete);
            fsw.Renamed += new RenamedEventHandler(OnRenamed);
        }
        private void OnDelete(object source, FileSystemEventArgs e)
        {
            programStateHelper.fileDeleted(e.Name);
            lastAddedFile = "";
            this.Invoke((MethodInvoker)delegate
            {
                newProgramLog(e.Name, "Deleted");
            });

        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (!keyStatus)
            {
                MessageBox.Show("File with name: \"" + e.Name + "\" has changed externaly, but key is not valid so encryption is aborted!");
                return;
            }
            if(lastAddedFile.Equals(e.Name))
            {
                return;
            }
            lastAddedFile = e.Name;
            //only procceed plaintext files
            if (e.Name.Contains("-encrypted") || e.Name.Contains("-decrypted"))
                return;

            bool ignore;

            cyper.addFileForEncryption(new FilePathAndKey(e.FullPath,programStateHelper.getLastUsedOutputPath(out ignore),textBoxKey.Text,true));
           
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            programStateHelper.fileNameChanged(e.OldName, e.Name);
            this.Invoke((MethodInvoker)delegate
            {
                newProgramLog(e.Name + " (" + e.OldName + ")", "Renamed");
            });
        }
        private void A51_FormClosing(object sender, FormClosingEventArgs e)
        {
            cyper.stopWatching();
            programStateHelper.saveProgramState();
        }
        private void A51_Load(object sender, EventArgs e)
        {
            cyper = new Cypher(this);
            cyper.startWatching();
            textBoxKey.Text = "1111000010001000001000110110100100010100101000010000010010001001";
            this.FormClosing += new FormClosingEventHandler(A51_FormClosing);
            this.Shown += new EventHandler(A51_Shown);
        }
        //called just after the A51 is shown
        private void A51_Shown(object sender, EventArgs e)
        {
            programStateHelper = new ProgramState.ProgramStateHelper();

            bool inputPathIsChanged, outputPathIsChanged;

            string inputPath = programStateHelper.getLastUsedInputPath(out inputPathIsChanged);
            string outputPath = programStateHelper.getLastUsedOutputPath(out outputPathIsChanged);

            inputLbl.Text = inputPath;
            outputLbl.Text = outputPath;
            setupFileSystemWatcher(inputPath);
            fileSystemWatcherCB.Checked = programStateHelper.getFileSystemWatcher();

            if (programStateHelper.getFileSystemWatcher() && !inputPathIsChanged)//if is fileSystemWatcher checked then check all files and encrypt new files
            {
                encryptMultypleFiles();
            }

        }
        private void encryptMultypleFiles()
        {
            bool inputPathIsChanged, outputPathIsChanged;

            string inputPath = programStateHelper.getLastUsedInputPath(out inputPathIsChanged);
            string outputPath = programStateHelper.getLastUsedOutputPath(out outputPathIsChanged);

            if (inputPathIsChanged)
            {
                MessageBox.Show("Input path is not valid anymore!\nInput path is changed to default value (aplication base directory).");
                inputLbl.Text = inputPath;
                return;
            }
            if (outputPathIsChanged)
            {
                MessageBox.Show("Output path is not valid anymore!\nOutput path is changed to Input path (if is valid) or default value (aplication base directory).");
                outputLbl.Text = outputPath;
            }
            DirectoryInfo dinfo = new DirectoryInfo(inputPath);

            FileInfo[] files = dinfo.GetFiles("*.*");

            foreach (FileInfo file in files)
            {
                //if file is't early encrypted or if does't contains string -encrypted or -decrypted then this file is new file
                if (!(programStateHelper.checkFileName(file.Name) || file.Name.Contains("-encrypted") || file.Name.Contains("-decrypted")))
                {
                    KeyForm keyForm = new KeyForm(file.Name);

                    DialogResult dialogResult = keyForm.ShowDialog();
                    if (dialogResult == DialogResult.Ignore)
                    {
                        continue;
                    }
                    else if (dialogResult == DialogResult.OK)
                    {
                        cyper.addFileForEncryption(new FilePathAndKey(file.FullName, outputPath, keyForm.Key,true));
                    }
                    else
                    {
                        break;
                    }

                }
            }
            
        }
        private void newProgramLog(string fileName, string action)
        {
            programLogCounter++;
            ListViewItem item = new ListViewItem(programLogCounter.ToString());
            item.SubItems.Add(fileName);
            item.SubItems.Add(action);

            programLogLv.Items.Add(item);
        }
        public void newProgramLog(string fileName, string action, long time)
        {
            programLogCounter++;
            ListViewItem item = new ListViewItem(programLogCounter.ToString());
            item.SubItems.Add(fileName);
            item.SubItems.Add(action);
            string timeString;
            if (time < 1000)
                timeString = time.ToString() + "ms";
            else
            {
                time /= 1000;
                timeString = time.ToString() + "s";
            }    
            item.SubItems.Add(timeString);
            programLogLv.Items.Add(item);
        }        
        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
            if (textBoxKey.Text.All(chr => chr.Equals('0')|| chr.Equals('1')) && !string.IsNullOrEmpty(textBoxKey.Text) && textBoxKey.Text.Length==64)
            {
                keyStatusLbl.ForeColor = Color.Green;
                keyStatusLbl.Text = "Valid.";
                keyStatus = true;
            }
            else
            {
                keyStatusLbl.ForeColor = Color.Red;
                keyStatusLbl.Text = "Invalid!";
                keyStatus = false;
            }
        }
        private void encryptAllBtn_Click(object sender, EventArgs e)
        {
            encryptMultypleFiles();
        }
    }
}
