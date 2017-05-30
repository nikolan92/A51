using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A51
{
    public class FileSystemHelper
    {

        public byte[] readBytesFromFile(string filePath)
        {
            try
            {
                using (FileStream fsSource = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] bytes = new byte[fsSource.Length];

                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;

                    while (numBytesToRead > 0)
                    {
                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    fsSource.Close();
                    return bytes;
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }

        }

        public void writeBtytesToFile(byte[] bytes, int bytesLength, string filePath)
        {
            if (bytes == null)
                return;
            using (FileStream fsNew = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fsNew.Write(bytes, 0, bytesLength);
                fsNew.Close();
            }
        }
    }
}
