using System;

namespace _GDALBitVector
{
    /// <summary>
    /// GateDigger's arbitrary length BitVector
    /// </summary>
    public partial struct GDALBitVector : ICloneable
    {
        /// <summary>
        /// Initialize a GDALBitVector capable of storing the specified number of bits
        /// </summary>
        /// <param name="length">The number of bits</param>
        public GDALBitVector(int length)
        {
            data = new UInt32[BitLengthToArrayLength(length)];
            this.length = length;
        }

        /// <summary>
        /// Initialize a GDALBitVector v; v[index] = data[index]
        /// The constructor is algorithmically equivalent to GDALBitVector.Parse(string)
        /// </summary>
        /// <param name="data">The values to fill the GDALBitVector</param>
        public GDALBitVector(params bool[] data)
        {
            UInt32[] resultArray;
            UInt32 tmp;
            int inputLength = data.Length,
                inputIndex,
                arrayIndex,
                subIndex;

            if (inputLength == 0)
            {
                this.data = new UInt32[0];
                this.length = 0;
                return;
            }

            inputIndex = inputLength - 1;

            resultArray = new UInt32[(arrayIndex = inputIndex >> atomSizeLog) + 1];

            tmp = 0U;
            for (subIndex = inputIndex & atomSizeMinus1; subIndex > -1; subIndex--, inputIndex--)
            {
                tmp <<= 1;
                if (data[inputIndex])
                    tmp += 1;
            }
            resultArray[arrayIndex] = tmp;

            while (arrayIndex-- > 0)
            {
                tmp = 0U;
                for (subIndex = atomSizeMinus1; subIndex > -1; subIndex--, inputIndex--)
                {
                    tmp <<= 1;
                    if (data[inputIndex])
                        tmp += 1;
                }
                resultArray[arrayIndex] = tmp;
            }

            this.data = resultArray;
            this.length = inputLength;

        }

        /// <summary>
        /// Initialize a GDALBitVector to wrap the supplied array
        /// </summary>
        /// <param name="data">The array to wrap</param>
        public GDALBitVector(params UInt32[] data)
        {
            this.data = data;
            this.length = data.Length << atomSizeLog;
        }

        /// <summary>
        /// Initialize a GDALBitVector to wrap the supplied array, with a specified length
        /// </summary>
        /// <param name="length">The number of significant bits</param>
        /// <param name="data">The array to wrap</param>
        public GDALBitVector(int length, params UInt32[] data)
        {
            this.data = data;
            this.length = length;
        }
    }
}