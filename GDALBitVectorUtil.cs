using System;
using System.Runtime.CompilerServices;

using static InlineIL.IL.Emit;

namespace _GDALBitVector
{
    public partial struct GDALBitVector : ICloneable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BitLengthToArrayLength(int bitLength)
        {
            return ((bitLength - 1) >> atomSizeLog) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BitLengthToArrayMaxIndex(int bitLength)
        {
            return (bitLength - 1) >> atomSizeLog;
        }
    }
}