using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A51
{
    public class Cypher
    {
        private Queue<FilePathAndKey> filesForEncryption;
        private Thread thread;
        private bool doWork = true;
        private A51 form;
        
        public Cypher(A51 form)
        {
            this.form = form;
        }
        public void addFileForEncryption(FilePathAndKey fileName)
        {
            lock (filesForEncryption)
            {
                filesForEncryption.Enqueue(fileName);
                Monitor.Pulse(filesForEncryption);
            }
        }
        public void startWatching()
        {
            filesForEncryption = new Queue<FilePathAndKey>(50);
            thread = new Thread(new ThreadStart(encryptFiles));
            doWork = true;
            //try
            //{
            thread.Start();
            //}
            //catch (ThreadInterruptedException) { }
        }
        public void stopWatching()
        {
            doWork = false;
            if (thread.ThreadState == ThreadState.Running)
                thread.Join();
            else
            {
                thread.Interrupt();
                thread.Join();
            }
        }
        private void encryptFiles()
        {
            FilePathAndKey fileForProceed;
            while (doWork)
            {
                lock (filesForEncryption)
                {
                    if (filesForEncryption.Count == 0)
                    {
                        try
                        {
                            Monitor.Wait(filesForEncryption);
                        }
                        catch (ThreadInterruptedException) { }

                        if (!doWork)
                            break;
                    }
                    fileForProceed = filesForEncryption.Dequeue();
                }//lock end
                //Encrypting file
                A51Algorithm a51 = new A51Algorithm(fileForProceed.Key);
                FileSystemHelper fsh = new FileSystemHelper();

                byte[] proceesedBytes;
                long time;
                //Change UI
                form.BeginInvoke((MethodInvoker)delegate
                {
                    form.processedFile.Text = fileForProceed.FileInputPath;
                });

                proceesedBytes = a51.encrypt_decrypt(fsh.readBytesFromFile(fileForProceed.FileInputPath), form, out time);
                string newFileName;

                if (fileForProceed.Encrypt)
                {
                    newFileName = fileForProceed.FileOutputPath +
                        "\\" + Path.GetFileNameWithoutExtension(fileForProceed.FileInputPath) + "-encrypted.bin";
                }
                else
                {
                    newFileName = fileForProceed.FileOutputPath +
                        "\\" + Path.GetFileNameWithoutExtension(fileForProceed.FileInputPath) + "-decrypted.bin";
                }
                if (proceesedBytes != null)
                {
                    fsh.writeBtytesToFile(proceesedBytes, proceesedBytes.Length, newFileName);

                    string oldFileName = Path.GetFileName(fileForProceed.FileInputPath);
                    //Change UI
                    form.Invoke((MethodInvoker)delegate
                    {
                        if (fileForProceed.Encrypt)
                        {
                            form.programStateHelper.addEncryptedFile(oldFileName);
                            form.newProgramLog(oldFileName, "Encrypted", time);
                        }
                        else
                        {
                            form.newProgramLog(oldFileName, "Decrypted", time);
                        }
                        form.processedFile.Text = "-";
                    });
                }
            }
        }
    }

    public class FilePathAndKey
    {
        private string key;
        private string fileInputPath;
        private string fileOutputPath;
        private bool encrypt;
        public FilePathAndKey(string fileInputPath,string fileOutputPath,string key,bool encrypt)
        {
            Key = key;
            FileInputPath = fileInputPath;
            FileOutputPath = fileOutputPath;
            Encrypt = encrypt;
        }
        public string FileInputPath
        {
            get { return fileInputPath; }
            set { fileInputPath = value; }
        }
        public string Key
        {
            get { return key; }
            set { key = value; }
        }
        public string FileOutputPath
        {
            get { return fileOutputPath; }
            set { fileOutputPath = value; }
        }
        public bool Encrypt
        {
            get { return encrypt; }
            set { encrypt = value; }
        }

    }
}
