using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A51.LFSRegister
{
    class RegisterY : LFSRegister
    {
        //key must have 64 characters and charaters are '0' or '1'
        public RegisterY(string key)
        {
            for (int i = 19; i < 41; i++)
            {
                if (key[i].Equals('0'))
                {
                    register = register << 1;
                    //register = register & 1048575;//mask 22bits 
                }
                else
                {
                    register = register << 1; //shift 
                    register = register | 1;
                    //register = register & 1048575;//mask 22bits 
                }
            }
        }
        public override bool getClockingBit()
        {
            register = register & 1024; // bitwise AND with bit 10
            if (register > 0)
                return true;
            else
                return false;
        }

        public override bool getLastBit()
        {
            uint tmpReg = register;
            tmpReg = register & 2097152;//bit 21
            if (tmpReg > 0)
                return true;
            else
                return false;
        }

        public override void shift()
        {
            bool t = false;

            uint tmpReg = register;

            tmpReg = register & 2097152;//bit 21
            if (tmpReg > 0)
                t = true;

            tmpReg = register & 1048576;//bit 20
            if (tmpReg > 0)
                t = XOR(t, true);
            else
                t = XOR(t, false);

            register = register << 1; //shift 
            register = register & 1048575;//mask 22bits 
            if (t)
                register = register | 2097152; //write bit on position 21
        }

        //public override void shift(bool keyBit)
        //{
        //    bool t = false;

        //    uint tmpReg = register;

        //    tmpReg = register & 2097152;//bit 21
        //    if (tmpReg > 0)
        //        t = true;

        //    tmpReg = register & 1048576;//bit 20
        //    if (tmpReg > 0)
        //        t = XOR(t, true);
        //    else
        //        t = XOR(t, false);

        //    t = XOR(t, keyBit);

        //    register = register << 1; //shift
        //    register = register & 1048575;//mask 22bits 
        //    if (t)
        //        register = register | 2097152; //write bit on position 21
            
        //}
    }
}
