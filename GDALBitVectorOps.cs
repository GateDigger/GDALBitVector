using InlineIL;
using System;
using System.Runtime.CompilerServices;
using static InlineIL.IL.Emit;

namespace _GDALBitVector
{
    public partial struct GDALBitVector : ICloneable
    {
        /// <summary>
        /// Resize a GDALBitVector to a desired length
        /// </summary>
        /// <param name="vector">The original GDALBitVector</param>
        /// <param name="newLength">The length to resize the GDALBitVector to</param>
        /// <returns>A new GDALBitVector instance v; v.Length == newLength</returns>
        public static GDALBitVector Resize(GDALBitVector vector, int newLength)
        {
            int oldLength = vector.Length,
                index = BitLengthToArrayLength(newLength);
            UInt32[] array = vector.data,
                resultArray = new UInt32[index];
            if (newLength > oldLength)
            {
                Array.Copy(array, resultArray, index = BitLengthToArrayLength(oldLength));
                if (index > 0)
                    resultArray[index - 1] &= ~0U >> (atomSize - (oldLength & atomSizeMinus1));
            }
            else
            {
                Array.Copy(array, resultArray, index);
            }
            return new GDALBitVector(newLength, resultArray);
        }

        /// <summary>
        /// Mass-set bits onwards from an index.
        /// Bound checks are left to the user.
        /// </summary>
        /// <param name="vector">The original GDALBitVector</param>
        /// <param name="index">The starting index</param>
        /// <param name="value">Bits to be mass-set</param>
        /// <returns>A new GDALBitVector instance</returns>
        public static GDALBitVector Write(GDALBitVector vector, int index, UInt32 value)
        {
            int lowIndex = index >> atomSizeLog,
                lowShift = index & atomSizeMinus1;
            UInt32[] array = vector.data,
                resultArray = (UInt32[])array.Clone();

            if (lowShift == 0)
                resultArray[lowIndex] = value;
            else
                WriteBody(resultArray, value, lowIndex, lowShift);

            return new GDALBitVector(vector.length, resultArray);
        }

        //Nothing to see here, move on
        static void WriteBody(UInt32[] how, UInt32 did, int weGet, int here)
        {
            //Split Arg#2 in two parts, blit the lower part onto the lower array index and the upper part onto the upper array index
            IL.DeclareLocals(new LocalVar(typeof(UInt32)), new LocalVar(typeof(Int32)));

            Ldarg_S(2);
            {
                Ldc_I4_S(1);
                {
                    Add();
                }
                Stloc_S(0);
            }

            Ldc_I4_S(atomSize);
            {
                Ldarg_S(3);
                {
                    Sub();
                }
                Stloc_S(1);
            }

            Ldarg_S(0);
            {
                Ldarg_S(2);
                {
                    Ldarg_S(1);
                    {
                        Ldarg_S(3);
                        {
                            Shl();
                        }
                        Ldc_I4_S(-1);
                        {
                            Ldloc_S(1);
                            {
                                Shr_Un();
                            }
                            Ldarg_S(0);
                            {
                                Ldarg_S(2);
                                {
                                    Ldelem_U4();
                                }
                                And();
                            }
                            Or();
                        }
                        Stelem_I4();
                    }
                }
            }
            Ldarg_S(0);
            {
                Ldloc_S(0);
                {
                    Ldarg_S(1);
                    {
                        Ldloc_S(1);
                        {
                            Shr_Un();
                        }
                        Ldc_I4_S(-1);
                        {
                            Ldarg_S(3);
                            {
                                Shl();
                            }
                            Ldarg_S(0);
                            {
                                Ldloc_S(0);
                                {
                                    Ldelem_U4();
                                }
                                And();
                            }
                            Or();
                        }
                        Stelem_I4();
                    }
                }
            }
            Ret();
        }


        /// <summary>
        /// Computes _ ∙ _ (concatenation) of two GDALBitVectors.
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">Also a GDALBitVector</param>
        /// <returns>A new GDALBitVector instance v; v == left ∙ right</returns>
        public static GDALBitVector Concat(GDALBitVector left, GDALBitVector right)
        {
            // This method can be optimized further, but it will become completely unreadable... Yes, this is readable.
            int index1,
                index2,
                leftLength = left.length,
                rightLength = right.length,
                length,
                leftArrayLength,
                rightArrayLength,
                arrayLength,
                bitShift,
                complBitShift;

            UInt32[] currentArray,
                resultArray;
            UInt32 tmp;

            if (leftLength == 0)
                return right.TheRealCloneMethod();
            if (rightLength == 0)
                return left.TheRealCloneMethod();

            length = leftLength + rightLength;
            leftArrayLength = BitLengthToArrayLength(leftLength);
            rightArrayLength = BitLengthToArrayLength(rightLength);
            arrayLength = BitLengthToArrayLength(length);
            bitShift = leftLength & atomSizeMinus1;

            currentArray = left.data;
            resultArray = new UInt32[arrayLength];

            if (bitShift == 0)
            {
                Array.Copy(currentArray, resultArray, leftArrayLength);
                Array.Copy(right.data, 0, resultArray, leftArrayLength, rightArrayLength);
            }
            else
            {
                complBitShift = atomSize - bitShift;

                Array.Copy(currentArray, resultArray, index1 = leftArrayLength - 1);

                index2 = 0;
                resultArray[index1] = (currentArray[index1++] & (~0U >> complBitShift)) | ((tmp = (currentArray = right.data)[index2++]) << bitShift);
                while (index2 < rightArrayLength)
                    resultArray[index1++] = (tmp >> complBitShift) | ((tmp = currentArray[index2++]) << bitShift);
                if (leftArrayLength + rightArrayLength == arrayLength)
                    resultArray[index1] = tmp >> complBitShift;
            }

            return new GDALBitVector(length, resultArray);
        }

        /// <summary>
        /// Computes ~ _ (bitwise-not) of a GDALBitVector
        /// </summary>
        /// <param name="value">The GDALBitVector to negate</param>
        /// <returns>A new GDALBitVector instance v; v == ~value</returns>
        public static GDALBitVector operator ~(GDALBitVector value)
        {
            int resultLength = value.length,
                index = BitLengthToArrayLength(resultLength);
            UInt32[] array = value.data,
                resultArray = new UInt32[index];
            while (--index > -1)
                resultArray[index] = ~array[index];
            return new GDALBitVector(resultLength, resultArray);
        }

        /// <summary>
        /// Computes ! _ (bitwise-not) of a GDALBitVector
        /// </summary>
        /// <param name="value">The GDALBitVector to negate</param>
        /// <returns>A new GDALBitVector instance v; v == !value</returns>
        public static GDALBitVector operator !(GDALBitVector value)
        {
            return ~value;
        }

        /// <summary>
        /// Computes _ & _ (bitwise-and) of two GDALBitVectors
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">Also a GDALBitVector</param>
        /// <returns>A new GDALBitVector instance v; v == left & right</returns>
        public static GDALBitVector operator &(GDALBitVector left, GDALBitVector right)
        {
            int leftLength = left.length,
                rightLength = right.length,
                index;
            UInt32[] leftArray = left.data,
                rightArray = right.data,
                resultArray;

            if (leftLength > rightLength)
            {
                resultArray = new UInt32[BitLengthToArrayLength(leftLength)];
                index = BitLengthToArrayMaxIndex(rightLength);
                if (index < 0)
                    return new GDALBitVector(leftLength, resultArray);
                resultArray[index] = leftArray[index] & rightArray[index] & (~0U >> (atomSize - rightLength));
                for (index--; index > -1; index--)
                    resultArray[index] = leftArray[index] & rightArray[index];
                return new GDALBitVector(leftLength, resultArray);
            }
            else
            {
                resultArray = new UInt32[BitLengthToArrayLength(rightLength)];
                index = BitLengthToArrayMaxIndex(leftLength);
                if (index < 0)
                    return new GDALBitVector(rightLength, resultArray);
                resultArray[index] = leftArray[index] & rightArray[index] & (~0U >> (atomSize - leftLength));
                for (index--; index > -1; index--)
                    resultArray[index] = leftArray[index] & rightArray[index];
                return new GDALBitVector(rightLength, resultArray);
            }
        }

        /// <summary>
        /// Computes _ | _ (bitwise-or) of two GDALBitVectors
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">Also a GDALBitVector</param>
        /// <returns>A new GDALBitVector instance v; v == left | right</returns>
        public static GDALBitVector operator |(GDALBitVector left, GDALBitVector right)
        {
            int leftLength = left.length,
                rightLength = right.length,
                index,
                floor;
            UInt32[] leftArray = left.data,
                rightArray = right.data,
                resultArray;

            if (leftLength > rightLength)
            {
                floor = BitLengthToArrayMaxIndex(rightLength);
                index = BitLengthToArrayLength(leftLength);
                resultArray = new UInt32[index];
                if (floor > -1)
                {
                    Array.Copy(leftArray, floor, resultArray, floor, index - (index = floor));
                    resultArray[index] = leftArray[index] | (rightArray[index] & (~0U >> (atomSize - rightLength)));
                    for (index--; index > -1; index--)
                        resultArray[index] = leftArray[index] | rightArray[index];
                }
                else
                {
                    Array.Copy(leftArray, resultArray, index);
                }

                return new GDALBitVector(leftLength, resultArray);
            }
            else
            {
                floor = BitLengthToArrayMaxIndex(leftLength);
                index = BitLengthToArrayLength(rightLength);
                resultArray = new UInt32[index];
                if (floor > -1)
                {
                    Array.Copy(rightArray, floor, resultArray, floor, index - (index = floor));
                    resultArray[index] = rightArray[index] | (leftArray[index] & (~0U >> (atomSize - leftLength)));
                    for (index--; index > -1; index--)
                        resultArray[index] = leftArray[index] | rightArray[index];
                }
                else
                {
                    Array.Copy(rightArray, resultArray, index);
                }

                return new GDALBitVector(rightLength, resultArray);
            }
        }

        /// <summary>
        /// Computes _ ^ _ (bitwise-xor) of two GDALBitVectors
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">Also a GDALBitVector</param>
        /// <returns>A new GDALBitVector instance v; v == left ^ right</returns>
        public static GDALBitVector operator ^(GDALBitVector left, GDALBitVector right)
        {
            int leftLength = left.length,
                rightLength = right.length,
                index,
                floor;
            UInt32[] leftArray = left.data,
                rightArray = right.data,
                resultArray;
            if (leftLength > rightLength)
            {
                floor = BitLengthToArrayMaxIndex(rightLength);
                index = BitLengthToArrayLength(leftLength);
                resultArray = new UInt32[index];
                if (floor > -1)
                {
                    Array.Copy(leftArray, floor, resultArray, floor, index - (index = floor));
                    resultArray[index] = leftArray[index] ^ (rightArray[index] & (~0U >> (atomSize - rightLength)));
                    for (index--; index > -1; index--)
                        resultArray[index] = leftArray[index] ^ rightArray[index];
                }
                else
                {
                    Array.Copy(leftArray, resultArray, index);
                }

                return new GDALBitVector(leftLength, resultArray);
            }
            else
            {
                floor = BitLengthToArrayMaxIndex(leftLength);
                index = BitLengthToArrayLength(rightLength);
                resultArray = new UInt32[index];
                if (floor > -1)
                {
                    Array.Copy(rightArray, floor, resultArray, floor, index - (index = floor));
                    resultArray[index] = rightArray[index] ^ (leftArray[index] & (~0U >> (atomSize - leftLength)));
                    for (index--; index > -1; index--)
                        resultArray[index] = leftArray[index] ^ rightArray[index];
                }
                else
                {
                    Array.Copy(rightArray, resultArray, index);
                }

                return new GDALBitVector(rightLength, resultArray);
            }
        }

        /// <summary>
        /// Determines whether two GDALBitVectors carry identical data
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">Also a GDALBitVector</param>
        /// <returns>true if identical</returns>
        public static bool operator ==(GDALBitVector left, GDALBitVector right)
        {
            int length = left.length,
                index;
            UInt32[] leftArray = left.data,
                rightArray = right.data;

            if (length != right.length)
                return false;
            if (length == 0)
                return true;

            index = BitLengthToArrayMaxIndex(length);
            if (((leftArray[index] ^ rightArray[index]) << length) != 0U)
                return false;
            for (index--; index > -1; index--)
                if (leftArray[index] != rightArray[index])
                    return false;
            return true;
        }

        /// <summary>
        /// Determines whether two GDALBitVectors carry non-identical data
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">Also a GDALBitVector</param>
        /// <returns>true if non-identical</returns>
        public static bool operator !=(GDALBitVector left, GDALBitVector right)
        {
            int length = left.length,
                index;
            UInt32[] leftArray = left.data,
                rightArray = right.data;

            if (length != right.length)
                return true;
            if (length == 0)
                return false;

            index = BitLengthToArrayMaxIndex(length);
            if (((leftArray[index] ^ rightArray[index]) << length) != 0U)
                return true;
            for (index--; index > -1; index--)
                if (leftArray[index] != rightArray[index])
                    return true;
            return false;
        }

        /// <summary>
        /// Computes _ << _ (left bit-shift) of a GDALBitVector
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">An integer</param>
        /// <returns>A new GDALBitVector instance v; v == left << right</returns>
        public static GDALBitVector operator <<(GDALBitVector left, int right)
        {
            // Also optimizable at the expense of readability.
            int index1,
                index2,
                leftLength,
                leftArrayLength,
                length,
                arrayLength,
                arrayShift,
                bitShift,
                complBitShift;

            UInt32[] leftArray,
                resultArray;
            UInt32 tmp;

            if (right < 0)
                return left >> -right;
            leftLength = left.length;
            if (leftLength == 0)
                return new GDALBitVector(right);

            length = leftLength + right;

            leftArrayLength = BitLengthToArrayLength(leftLength);
            leftArray = left.data;

            arrayLength = BitLengthToArrayLength(length);
            resultArray = new UInt32[arrayLength];

            arrayShift = right >> atomSizeLog;
            bitShift = right & atomSizeMinus1;
            complBitShift = atomSize - bitShift;

            if (bitShift == 0)
            {
                Array.Copy(leftArray, 0, resultArray, arrayShift, arrayLength - arrayShift);
            }
            else
            {
                tmp = 0U;
                index1 = arrayShift;
                index2 = 0;
                while (index2 < leftArrayLength)
                    resultArray[index1++] = (tmp >> complBitShift) | ((tmp = leftArray[index2++]) << bitShift);
                if (index1 < arrayLength)
                    resultArray[index1] = tmp >> complBitShift;
            }

            return new GDALBitVector(length, resultArray);
        }

        /// <summary>
        /// Computes _ >> _ (right bit-shift) of a GDALBitVector
        /// </summary>
        /// <param name="left">A GDALBitVector</param>
        /// <param name="right">An integer</param>
        /// <returns>A new GDALBitVector instance v; v == left >> right</returns>
        public static GDALBitVector operator >>(GDALBitVector left, int right)
        {
            // Also optimizable at the expense of readability.
            int index1,
                index2,
                leftLength,
                leftArrayLength,
                length,
                arrayLength,
                arrayShift,
                bitShift,
                complBitShift;

            UInt32[] leftArray,
                resultArray;
            UInt32 tmp;

            if (right < 0)
                return left << -right;

            leftLength = left.length;
            length = leftLength - right;

            if (length <= 0)
                return Empty;

            leftArrayLength = BitLengthToArrayLength(leftLength);
            leftArray = left.data;

            arrayLength = BitLengthToArrayLength(length);
            resultArray = new UInt32[arrayLength];

            arrayShift = right >> atomSizeLog;
            bitShift = right & atomSizeMinus1;
            complBitShift = atomSize - bitShift;

            if (bitShift == 0)
            {
                Array.Copy(leftArray, arrayShift, resultArray, 0, arrayLength);
            }
            else
            {
                index1 = 0;
                index2 = arrayShift;
                tmp = leftArray[index2++];
                while (index2 < leftArrayLength)
                    resultArray[index1++] = (tmp >> bitShift) | ((tmp = leftArray[index2++]) << complBitShift);
                if (index1 < arrayLength)
                    resultArray[index1] = (tmp >> bitShift);
            }

            return new GDALBitVector(length, resultArray);
        }

        public static GDALBitVector Random(int length, Random rand)
        {
            int arrayIndex = BitLengthToArrayLength(length);
            UInt32[] resultArray = new UInt32[arrayIndex];
            while (--arrayIndex > -1)
                resultArray[arrayIndex] = RandomNextUInt32(rand);
            return new GDALBitVector(length, resultArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RandomNextUInt32(Random rand)
        {
            return (UInt32)(rand.Next() & 0xFFFF) | ((UInt32)(rand.Next() & 0xFFFF) << (atomSize >> 1));
        }

        public object Clone()
        {
            return TheRealCloneMethod();
        }

        public GDALBitVector TheRealCloneMethod()
        {
            UInt32[] array = this.data,
                resultArray;

            int length = this.length,
                resultArrayLength = BitLengthToArrayLength(length);

            resultArray = new UInt32[resultArrayLength];
            Array.Copy(array, resultArray, resultArrayLength);

            return new GDALBitVector(length, resultArray);
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is GDALBitVector v)
                return v == this;
            return false;
        }
    }
}