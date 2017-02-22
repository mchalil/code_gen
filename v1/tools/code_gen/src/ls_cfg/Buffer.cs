using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ls_code_gen
{
    class Buffer
    {
        public Buffer(int id, int size)
        {
            nSize = size;
            nId = id;
        }
        public int nId;
        int nSize;
    }
    class BufferManager
    {
        int nCount;
        int nMax;
        public BufferManager(int maxBuffers)
        {
            nMax = maxBuffers;
            nCount = 0;
        }
        public Buffer Alloc(int size)
        {
            Buffer buffer = new Buffer(nCount, size);
            nCount++;
            return buffer;
        }


    }
}
