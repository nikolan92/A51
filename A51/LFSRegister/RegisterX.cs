using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A51.LFSRegister
{
    class RegisterX : LFSRegister
    {
        //key must have 64 characters and charaters are '0' or '1'
        public RegisterX(string key)
        {
            for(int i=0; i < 19; i++)//first 19bits 
            {
                if(key[i].Equals('0'))
                {
                    register = register << 1;
                    //register = register & 1048575;//mask 19bits// there is no need for mask because i shift 19 times and overflow is not possible
                }
                else
                {
                    register = register << 1; //shift 
                    register = register | 1;
                    //register = register & 1048575;//mask 19bits
                }
            }
        }
        public override bool getClockingBit()
        {
            uint tmpReg = register;
            tmpReg = register & 256;//bit 8
            if (tmpReg > 0)
                return true;
            else
                return false;
        }

        public override bool getLastBit()
        {
            register = register & 262144;// bitwise AND with bit 18
            if (register > 0)
                return true;
            else
                return false;
        }
        public override void shift()
        {
            bool t = false;
  
            uint tmpReg = register;

            tmpReg = register & 262144;//bit 18
            if (tmpReg > 0)
                t = true;

            tmpReg = register & 131072 ;//bit 17
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            tmpReg = register & 65536;//bit 16
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            tmpReg = register & 8192;//bit 13 
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            register = register << 1; //shift 
            register = register & 1048575;//mask 19bits
            if (t)
                register = register | 262144; //write bit on position 18
        }
        //public override void shift(bool keyBit)
        //{
        //    bool t = false;

        //    uint tmpReg = register;

        //    tmpReg = register & 262144;//bit 18
        //    if (tmpReg > 0)
        //        t = true;

        //    tmpReg = register & 131072;//bit 17
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    tmpReg = register & 65536;//bit 16
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    tmpReg = register & 8192;//bit 13 
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    t = XOR(t, keyBit);

        //    register = register << 1; //shift 
        //    register = register & 1048575;//mask 19bits
        //    if (t)
        //        register = register | 1; //write bit on position 18
        //}
    }
}
