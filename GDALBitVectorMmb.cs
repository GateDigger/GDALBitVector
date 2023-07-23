using System;

namespace _GDALBitVector
{
    public partial struct GDALBitVector : ICloneable
    {
        const int atomSizeLog = 5,
            atomSize = 1 << atomSizeLog,
            atomSizeMinus1 = atomSize - 1;

        UInt32[] data;
        int length;
    }
}