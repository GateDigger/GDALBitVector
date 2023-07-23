//#define BLOCK_HEADERS

using System;

namespace _GDALBitVector
{
    public partial struct GDALBitVector : ICloneable
    {
        const char YES = '1',
                NO = '0';
        /// <summary>
        /// Returns a 0-1 string representing the current state of the bit vector
        /// </summary>
        /// <returns>The resulting 0-1 string representation s; GDALBitVector.Parse(s) == this</returns>
        public override string ToString()
        {
            UInt32[] data = this.data;
            UInt32 tmp;
            char[] result;
            int dataLength = this.length,
                dataIndex,
                arrayIndex,
                subIndex;

            if (dataLength == 0)
                return "_";

            result = new char[dataLength--];

            tmp = data[arrayIndex = dataLength >> atomSizeLog];
            for (subIndex = dataLength & atomSizeMinus1, dataIndex = arrayIndex << atomSizeLog; subIndex > -1; subIndex--, dataIndex++, tmp >>= 1)
#if BLOCK_HEADERS
                result[dataIndex] = (tmp & 1U) == 1U ? (dataIndex & 31) == 0 ? 'Y' : YES : (dataIndex & 31) == 0 ? 'N' : NO;
#else
                result[dataIndex] = (tmp & 1U) == 1U ? YES : NO;
#endif

            while (arrayIndex-- > 0)
            {
                tmp = data[arrayIndex];
                for (subIndex = atomSizeMinus1, dataIndex = arrayIndex << atomSizeLog; subIndex > -1; subIndex--, dataIndex++, tmp >>= 1)
#if BLOCK_HEADERS
                    result[dataIndex] = (tmp & 1U) == 1U ? (dataIndex & 31) == 0 ? 'Y' : YES : (dataIndex & 31) == 0 ? 'N' : NO;
#else
                    result[dataIndex] = (tmp & 1U) == 1U ? YES : NO;
#endif
            }

            return new string(result);
        }

        /// <summary>
        /// Returns a GDALBitVector representing the 0-x string passed as an input
        /// </summary>
        /// <param name="input">A string to parse</param>
        /// <returns>A new GDALBitVector instance v; GDALBitVector.Parse(v.ToString()) == GDALBitVector.Parse(input)</returns>
        public static GDALBitVector Parse(string input)
        {
            UInt32[] resultArray;
            UInt32 tmp;
            int inputLength = input.Length,
                inputIndex,
                arrayIndex,
                subIndex;

            if (inputLength == 0)
                return GDALBitVector.Empty;

            inputIndex = inputLength - 1;

            resultArray = new UInt32[(arrayIndex = inputIndex >> atomSizeLog) + 1];

            tmp = 0U;
            for (subIndex = inputIndex & atomSizeMinus1; subIndex > -1; subIndex--, inputIndex--)
            {
                tmp <<= 1;
                if (input[inputIndex] != NO)
                    tmp += 1;
            }
            resultArray[arrayIndex] = tmp;

            while (arrayIndex-- > 0)
            {
                tmp = 0U;
                for (subIndex = atomSizeMinus1; subIndex > -1; subIndex--, inputIndex--)
                {
                    tmp <<= 1;
                    if (input[inputIndex] != NO)
                        tmp += 1;
                }
                resultArray[arrayIndex] = tmp;
            }

            return new GDALBitVector(inputLength, resultArray);
        }
    }
}