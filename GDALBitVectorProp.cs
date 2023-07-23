using InlineIL;
using System;

using static InlineIL.IL.Emit;

namespace _GDALBitVector
{
    public partial struct GDALBitVector : ICloneable
    {
        public static GDALBitVector Empty
        {
            get
            {
                return new GDALBitVector(0);
            }
        }

        /// <summary>
        /// The number of bits the GDALBitVector can currently store
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// Access to individual bits
        /// </summary>
        /// <param name="index">The index of the desired bit</param>
        /// <returns>true if 1, false if 0</returns>
        public bool this[int index]
        {
            get
            {
                return (data[index >> atomSizeLog] & (1U << index)) != 0U;
            }
            set
            {
                //word ^= (1U << (index & atomSizeModConst)) & ((val << (index & atomSizeModConst)) ^ word);
                IL.DeclareLocals(new LocalVar(typeof(UInt32)), new LocalVar(typeof(UInt32)), new LocalVar(typeof(Int32)));

                Ldarg_S(0);
                {
                    Ldfld(FieldRef.Field(typeof(GDALBitVector), "data"));
                    Dup();
                    {
                        Ldarg_S(1);
                        {
                            Ldc_I4_S(atomSizeLog);
                            {
                                Shr();
                            }
                            Dup();
                            {
                                Stloc_S(1);
                            }
                            Ldelem_U4();
                        }
                        Stloc_S(0);
                    }
                    Ldloc_S(1);
                    {
                        Ldloc_S(0);
                        {
                            Ldc_I4_S(1);
                            {
                                Ldarg_S(1);
                                {
                                    Ldc_I4_S(atomSizeMinus1);
                                    {
                                        And();
                                    }
                                    Dup();
                                    {
                                        Stloc_S(2);
                                    }
                                    Shl();
                                }
                                Ldarg_S(2);
                                {
                                    Ldloc_S(2);
                                    {
                                        Shl();
                                    }
                                    Ldloc_S(0);
                                    {
                                        Xor();
                                    }
                                    And();
                                }
                                Xor();
                            }
                            Stelem_I4();
                        }
                    }
                }
                Ret();

                //The slow version
                //if (value)
                //    data[index >> atomSizeLog] |= 1U << (index /* & atomSizeModConst */);
                //else
                //    data[index >> atomSizeLog] &= ~(1U << (index /* & atomSizeModConst */));

            }
        }
    }
}