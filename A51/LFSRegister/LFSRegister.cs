using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A51.LFSRegister
{
    public abstract class LFSRegister
    {
        protected  uint register = 0;

        public static bool XOR(bool a, bool b)
        {
            if (a == b)
                return false;
            else
                return true;
        }
        public abstract bool getClockingBit();
        public abstract void shift();
        //public abstract void shift(bool keyBit);

        public abstract bool getLastBit();
        public uint getInt()
        {
            return register;
        }
    }
}
