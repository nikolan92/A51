using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A51.LFSRegister
{
    class RegisterZ : LFSRegister
    {
        //key must have 64 characters and charaters are '0' or '1'
        public RegisterZ(string key)
        {
            for (int i = 41; i < 64; i++)
            {
                if (key[i].Equals('0'))
                {
                    register = register << 1;
                    //register = register & 16777215;//mask 23bits 
                }
                else
                {
                    register = register << 1; //shift 
                    register = register | 1;
                    //register = register & 16777215;//mask 23bits 
                }
            }
        }
        public override bool getClockingBit()
        {
            register = register & 1024;// bitwise AND with bit 10
            if (register > 0)
                return true;
            else
                return false;
        }

        public override bool getLastBit()
        {
            uint tmpReg = register;
            tmpReg = register & 4194304;//bit 22
            if (tmpReg > 0)
                return true;
            else
                return false;
        }

        public override void shift()
        {
            bool t = false;

            uint tmpReg = register;

            tmpReg = register & 4194304;//bit 22
            if (tmpReg > 0)
                t = true;

            tmpReg = register & 2097152;//bit 21
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            tmpReg = register & 1048576;//bit 20
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            tmpReg = register & 128;//bit 7
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            register = register << 1; //shift 
            register = register & 16777215;//mask 23bits 
            if (t)
                register = register | 4194304; //write bit on position 22
        }

        //public override void shift(bool keyBit)
        //{
        //    bool t = false;

        //    uint tmpReg = register;

        //    tmpReg = register & 4194304;//bit 22
        //    if (tmpReg > 0)
        //        t = true;

        //    tmpReg = register & 2097152;//bit 21
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    tmpReg = register & 1048576;//bit 20
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    tmpReg = register & 128;//bit 7
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    t = XOR(t, keyBit);

        //    register = register << 1; //shift
        //    register = register & 16777215;//mask 23bits 

        //    if (t)
        //        register = register | 4194304; //write bit on position 22
            
        //}
    }
}
