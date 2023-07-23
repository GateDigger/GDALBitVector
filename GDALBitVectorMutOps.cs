using System;

namespace _GDALBitVector
{
    public partial struct GDALBitVector : ICloneable
    {
        /// <summary>
        /// Mass-set bits onwards from an index
        /// </summary>
        /// <param name="index">The starting index</param>
        /// <param name="value">Bits to be mass-set</param>
        /// <returns>The mutated GDALBitVector instance</returns>
        public GDALBitVector Write(int index, UInt32 value)
        {
            int lowIndex = index >> atomSizeLog,
                lowShift = index & atomSizeMinus1;
            UInt32[] array = this.data;

            if (lowShift == 0)
                array[lowIndex] = value;
            else
                WriteBody(array, value, lowIndex, lowShift);

            return this;

            /*
            int highIndex = lowIndex + 1,
                highShift = atomSize - lowShift;

            data[lowIndex] = (content << lowShift) | (data[lowIndex] & (~0U >> highShift));
            data[highIndex] = (content >> highShift)  | (data[highIndex] & (~0U << lowShift));
            */
        }

        /// <summary>
        /// Performs ~ _ (bitwise-not) on a GDALBitVector
        /// </summary>
        /// <returns>The mutated GDALBitVector instance</returns>
        public GDALBitVector ApplyNot()
        {
            int index = BitLengthToArrayMaxIndex(length);
            UInt32[] array = this.data;
            while (index > -1)
                array[index] = ~array[index--];
            return this;
        }

        /// <summary>
        /// Performs _ & value (bitwise-and) on a GDALBitVector
        /// </summary>
        /// <param name="value">A GDALBitVector</param>
        /// <returns>The mutated GDALBitVector instance</returns>
        public GDALBitVector ApplyAnd(GDALBitVector value)
        {
            int leftLength = this.length,
                rightLength = value.length,
                index,
                floor;
            UInt32[] leftArray = this.data,
                rightArray = value.data;
            if (leftLength > rightLength)
            {
                floor = BitLengthToArrayMaxIndex(rightLength);
                index = BitLengthToArrayMaxIndex(leftLength);
                for (; index > floor; index--)
                    leftArray[index] = 0U;
                if (index < 0)
                    return this;
                leftArray[index] &= rightArray[index] & (~0U >> (atomSize - rightLength));
                for (index--; index > -1; index--)
                    leftArray[index] &= rightArray[index];
                return this;
            }
            else
            {
                index = BitLengthToArrayMaxIndex(leftLength);
                for (; index > -1; index--)
                    leftArray[index] &= rightArray[index];
                return this;
            }
        }

        /// <summary>
        /// Performs _ | value (bitwise-or) on a GDALBitVector
        /// </summary>
        /// <param name="value">A GDALBitVector</param>
        /// <returns>The mutated GDALBitVector instance</returns>
        public GDALBitVector ApplyOr(GDALBitVector value)
        {
            int leftLength = this.length,
                rightLength = value.length,
                index;
            UInt32[] leftArray = this.data,
                rightArray = value.data;
            if (leftLength > rightLength)
            {
                index = BitLengthToArrayMaxIndex(rightLength);
                if (index < 0)
                    return this;
                leftArray[index] |= rightArray[index] & (~0U >> (atomSize - rightLength));
                for (index--; index > -1; index--)
                    leftArray[index] |= rightArray[index];
                return this;
            }
            else
            {
                index = BitLengthToArrayMaxIndex(leftLength);
                for (; index > -1; index--)
                    leftArray[index] |= rightArray[index];
                return this;
            }
        }

        /// <summary>
        /// Performs _ ^ value (bitwise-xor) on a GDALBitVector
        /// </summary>
        /// <param name="value">A GDALBitVector</param>
        /// <returns>The mutated GDALBitVector instance</returns>
        public GDALBitVector ApplyXor(GDALBitVector value)
        {
            int leftLength = this.length,
                rightLength = value.length,
                index;
            UInt32[] leftArray = this.data,
                rightArray = value.data;
            if (leftLength > rightLength)
            {
                index = BitLengthToArrayMaxIndex(rightLength);
                if (index < 0)
                    return this;
                leftArray[index] ^= rightArray[index] & (~0U >> (atomSize - rightLength));
                for (index--; index > -1; index--)
                    leftArray[index] ^= rightArray[index];
                return this;
            }
            else
            {
                index = BitLengthToArrayMaxIndex(leftLength);
                for (; index > -1; index--)
                    leftArray[index] ^= rightArray[index];
                return this;
            }
        }
    }
}