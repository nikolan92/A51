using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A51
{
    public class A51Algorithm
    {
        private LFSRegister.LFSRegister X,Y,Z;
        public A51Algorithm(string key)
        {
            X = new LFSRegister.RegisterX(key);
            Y = new LFSRegister.RegisterY(key);
            Z = new LFSRegister.RegisterZ(key);
        }
        public byte[] encrypt_decrypt(byte[] bytesForEncryptOrDecrypt)
        {
            if (bytesForEncryptOrDecrypt == null || bytesForEncryptOrDecrypt.Length == 0)
                return null;

            byte[] encryptedBytes = new byte[bytesForEncryptOrDecrypt.Length];

            uint tmp,finalByte;
            bool xoredBit,s;
            for(int i=0; i<bytesForEncryptOrDecrypt.Length; i++)
            {
                finalByte = 0;
                //encryption all bits in one byte bit by bit
                for (uint j = 1; j <= 255; j = j << 1)
                {
                    //shifting register
                    bool m = mayorityVote(X.getClockingBit(), Y.getClockingBit(), Z.getClockingBit());
                    if (X.getClockingBit() == m)
                        X.shift();
                    if (Y.getClockingBit() == m)
                        Y.shift();
                    if (Z.getClockingBit() == m)
                        Z.shift();

                    s = LFSRegister.LFSRegister.XOR(LFSRegister.LFSRegister.XOR(X.getClockingBit(), Y.getClockingBit()), Z.getClockingBit());
                    //encryption bit by bit with s
                    tmp = bytesForEncryptOrDecrypt[i] & j;//bit 0,bit 2 ... bit 7
                    if (tmp > 0)
                    {
                        xoredBit = LFSRegister.LFSRegister.XOR(s, true);

                        finalByte = finalByte << 1;
                        if (xoredBit)
                            finalByte = finalByte | 1;
                    }
                    else
                    {
                        xoredBit = LFSRegister.LFSRegister.XOR(s, false);

                        finalByte = finalByte << 1;
                        if (xoredBit)
                            finalByte = finalByte | 1;
   
                    }
                }//end for
                encryptedBytes[i] = (byte)finalByte;
            }

            return encryptedBytes;
        }
        public byte[] encrypt_decrypt(byte[] bytesForEncryptOrDecrypt, A51 form, out long time)
        {
            if (bytesForEncryptOrDecrypt == null || bytesForEncryptOrDecrypt.Length == 0)
            {
                time = 0;
                return null;
            }
            Stopwatch sw = new Stopwatch();
            byte[] encryptedBytes = new byte[bytesForEncryptOrDecrypt.Length];

            int step = bytesForEncryptOrDecrypt.Length / 100;
            int finish = 0;
            if(step == 0)
            {
                step = bytesForEncryptOrDecrypt.Length;
                finish = 100;
            }
            uint tmp, finalByte;
            bool xoredBit, s;
            sw.Start();
            for (int i = 0; i < bytesForEncryptOrDecrypt.Length; i++)
            {
                finalByte = 0;
                //encryption all bits in one byte bit by bit
                for (uint j = 1; j <= 255; j = j << 1)
                {
                    //shifting register
                    bool m = mayorityVote(X.getClockingBit(), Y.getClockingBit(), Z.getClockingBit());
                    if (X.getClockingBit() == m)
                        X.shift();
                    if (Y.getClockingBit() == m)
                        Y.shift();
                    if (Z.getClockingBit() == m)
                        Z.shift();

                    s = LFSRegister.LFSRegister.XOR(LFSRegister.LFSRegister.XOR(X.getClockingBit(), Y.getClockingBit()), Z.getClockingBit());
                    //encryption bit by bit with s
                    tmp = bytesForEncryptOrDecrypt[i] & j;//bit 1
                    if (tmp > 0)
                    {
                        xoredBit = LFSRegister.LFSRegister.XOR(s, true);

                        finalByte = finalByte << 1;
                        if (xoredBit)
                            finalByte = finalByte | 1;

                    }
                    else
                    {
                        xoredBit = LFSRegister.LFSRegister.XOR(s, false);

                        finalByte = finalByte << 1;
                        if (xoredBit)
                            finalByte = finalByte | 1;

                    }
                }//end for
                if ((i % step) == 0)
                {
                    //Change UI
                    form.BeginInvoke((MethodInvoker)delegate
                    {
                        form.status.Text = finish.ToString() + "%";
                        finish++;
                    });
                }
                encryptedBytes[i] = (byte)finalByte;
            }
            sw.Stop();
            //Change UI
            form.BeginInvoke((MethodInvoker)delegate
            {
                form.status.Text = "Idle";
                finish++;
            });
            time = sw.ElapsedMilliseconds;
            return encryptedBytes;
        }
        public bool mayorityVote(bool X,bool Y,bool Z)
        {
            short sum = 0;
            if (X)
                sum++;
            if (Y)
                sum++;
            if (Z)
                sum++;

            if (sum >= 2)
                return true;
            else
                return false;
        }

        //
        //public byte[] encrypt_decrypt(byte[] bytesForEncryptOrDecrypt, ProgressBar progressBar, out long time)
        //{
        //    if (bytesForEncryptOrDecrypt == null || bytesForEncryptOrDecrypt.Length == 0)
        //    {
        //        time = 0;
        //        return null;
        //    }
        //    Stopwatch sw = new Stopwatch();

        //    progressBar.Minimum = 0;
        //    progressBar.Maximum = bytesForEncryptOrDecrypt.Length;
        //    progressBar.Step = 1;
        //    progressBar.Value = 0;
        //    byte[] encryptedBytes = new byte[bytesForEncryptOrDecrypt.Length];

        //    uint tmp, finalByte;
        //    bool xoredBit, s;
        //    sw.Start();
        //    for (int i = 0; i < bytesForEncryptOrDecrypt.Length; i++)
        //    {
        //        finalByte = 0;
        //        //encryption all bits in one byte bit by bit
        //        for (uint j = 1; j <= 255; j = j << 1)
        //        {
        //            //shifting register
        //            bool m = mayorityVote(X.getClockingBit(), Y.getClockingBit(), Z.getClockingBit());
        //            if (X.getClockingBit() == m)
        //                X.shift();
        //            if (Y.getClockingBit() == m)
        //                Y.shift();
        //            if (Z.getClockingBit() == m)
        //                Z.shift();

        //            s = LFSRegister.LFSRegister.XOR(LFSRegister.LFSRegister.XOR(X.getClockingBit(), Y.getClockingBit()), Z.getClockingBit());
        //            //encryption bit by bit with s
        //            tmp = bytesForEncryptOrDecrypt[i] & j;//bit 1
        //            if (tmp > 0)
        //            {
        //                xoredBit = LFSRegister.LFSRegister.XOR(s, true);

        //                finalByte = finalByte << 1;
        //                if (xoredBit)
        //                    finalByte = finalByte | 1;

        //            }
        //            else
        //            {
        //                xoredBit = LFSRegister.LFSRegister.XOR(s, false);

        //                finalByte = finalByte << 1;
        //                if (xoredBit)
        //                    finalByte = finalByte | 1;

        //            }
        //        }//end for
        //        progressBar.PerformStep();
        //        encryptedBytes[i] = (byte)finalByte;
        //    }
        //    sw.Stop();
        //    time = sw.ElapsedMilliseconds;
        //    return encryptedBytes;
        //}
    }
}
