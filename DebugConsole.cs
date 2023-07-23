//#define BLOCK_HEADERS

using _GDALBitVector;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace GDALBitVectorDebugConsole
{
    internal static class DebugConsole
    {
        static void Main(string[] args)
        {
            //PerfTest(5000000);
            //NBVRandTest(800);
            ManualDebug();
        }

        static void PerfTest(int iterations)
        {
            int lengthFloor = 32, lengthCeiling = 83, i1, counter;
            Random rand = new Random();
            Stopwatch sw = new Stopwatch();

            GDALBitVector jitDummy;
            GDALBitVector[] mGDALBitVectorArray1 = new GDALBitVector[iterations],
                iGDALBitVectorArray1 = new GDALBitVector[iterations],
                _GDALBitVectorArray2 = new GDALBitVector[iterations],
                iGDALBitVectorArrayRes = new GDALBitVector[iterations];
            int[] resizeArgs = new int[iterations],
                blitIndexArgs = new int[iterations];
            UInt32[] blitContentArgs = new UInt32[iterations];

            for (counter = 0; counter < iterations; counter++)
            {
                resizeArgs[counter] = rand.Next(lengthFloor, lengthCeiling);
                blitContentArgs[counter] = GDALBitVector.RandomNextUInt32(rand);
                mGDALBitVectorArray1[counter] = iGDALBitVectorArray1[counter] = GDALBitVector.Random(rand.Next(lengthFloor, lengthCeiling), rand);
                _GDALBitVectorArray2[counter] = GDALBitVector.Random(rand.Next(lengthFloor, lengthCeiling), rand);
                blitIndexArgs[counter] = rand.Next(mGDALBitVectorArray1[counter].Length - 31);
            }

            Console.WriteLine("APPLY_BLIT");
            jitDummy = GDALBitVector.Random(67, rand).Write(10, 651076541U);
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                mGDALBitVectorArray1[counter].Write(blitIndexArgs[counter], blitContentArgs[counter]);
            sw.Stop();
            Console.WriteLine(mGDALBitVectorArray1[rand.Next(counter)]);
            Console.WriteLine("APPLY_BLIT: " + sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("CALC_BLIT");
            jitDummy = GDALBitVector.Write(GDALBitVector.Random(67, rand), 10, 651076541U);
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                iGDALBitVectorArrayRes[counter] = GDALBitVector.Write(iGDALBitVectorArray1[counter], blitIndexArgs[counter], blitContentArgs[counter]);
            sw.Stop();
            Console.WriteLine(iGDALBitVectorArrayRes[rand.Next(counter)]);
            Console.WriteLine("CALC_BLIT: " + sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();


            Console.WriteLine("APPLY_NOT");
            jitDummy = GDALBitVector.Random(17, rand).ApplyNot();
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                mGDALBitVectorArray1[counter].ApplyNot();
            sw.Stop();
            Console.WriteLine(mGDALBitVectorArray1[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("CALC_NOT");
            jitDummy = !GDALBitVector.Random(17, rand);
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                iGDALBitVectorArrayRes[counter] = !iGDALBitVectorArray1[counter];
            sw.Stop();
            Console.WriteLine(iGDALBitVectorArrayRes[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("APPLY_AND");
            jitDummy = GDALBitVector.Random(17, rand).ApplyAnd(GDALBitVector.Random(16, rand));
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                mGDALBitVectorArray1[counter].ApplyAnd(_GDALBitVectorArray2[counter]);
            sw.Stop();
            Console.WriteLine(mGDALBitVectorArray1[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("CALC_AND");
            jitDummy = GDALBitVector.Random(17, rand) & GDALBitVector.Random(17, rand);
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                iGDALBitVectorArrayRes[counter] = iGDALBitVectorArray1[counter] & _GDALBitVectorArray2[counter];
            sw.Stop();
            Console.WriteLine(iGDALBitVectorArrayRes[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("APPLY_OR");
            jitDummy = GDALBitVector.Random(17, rand).ApplyOr(GDALBitVector.Random(16, rand));
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                mGDALBitVectorArray1[counter].ApplyOr(_GDALBitVectorArray2[counter]);
            sw.Stop();
            Console.WriteLine(mGDALBitVectorArray1[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("CALC_OR");
            jitDummy = GDALBitVector.Random(17, rand) | GDALBitVector.Random(17, rand);
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                iGDALBitVectorArrayRes[counter] = iGDALBitVectorArray1[counter] | _GDALBitVectorArray2[counter];
            sw.Stop();
            Console.WriteLine(iGDALBitVectorArrayRes[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("APPLY_XOR");
            jitDummy = GDALBitVector.Random(17, rand).ApplyXor(GDALBitVector.Random(16, rand));
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                mGDALBitVectorArray1[counter].ApplyXor(_GDALBitVectorArray2[counter]);
            sw.Stop();
            Console.WriteLine(mGDALBitVectorArray1[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("CALC_XOR");
            jitDummy = GDALBitVector.Random(17, rand) ^ GDALBitVector.Random(17, rand);
            Console.WriteLine(jitDummy);
            sw.Start();
            for (counter = 0; counter < iterations; counter++)
                iGDALBitVectorArrayRes[counter] = iGDALBitVectorArray1[counter] ^ _GDALBitVectorArray2[counter];
            sw.Stop();
            Console.WriteLine(iGDALBitVectorArrayRes[rand.Next(counter)]);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            sw.Reset();

            Console.WriteLine("END");
        }

        static void ManualDebug()
        {
            GDALBitVector v1 = GDALBitVector.Empty,
                v2 = GDALBitVector.Empty;

            NaiveBitVector nv1 = NaiveBitVector.Empty,
                nv2 = NaiveBitVector.Empty;

            Random rand = new Random();

            string input;
            int arg;
            while (true)
            {
                input = Console.ReadLine();
                if (input.StartsWith("create "))
                {
                    input = input.Substring(7);
                    arg = int.Parse(input);
                    v1 = CreateVector(arg);
                    nv1 = CreateNVector(arg);
                    continue;
                }
                if (input.StartsWith("parse rand "))
                {
                    input = input.Substring(11);
                    input = Generate01String(int.Parse(input), rand);
                    Console.WriteLine(input);

                    v1 = ParseVector(input);
                    nv1 = ParseNVector(input);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    continue;
                }
                if (input.StartsWith("parse "))
                {
                    input = input.Substring(6);
                    Console.WriteLine(input);
                    v1 = ParseVector(input);
                    nv1 = ParseNVector(input);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input == "seed0")
                {
                    SeedVectors0(v1, nv1);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input == "seed1")
                {
                    SeedVectors1(v1, nv1);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("seed"))
                {
                    SeedVectors(v1, nv1, rand);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("resize "))
                {
                    input = input.Substring(7);
                    arg = int.Parse(input);
                    v1 = GDALBitVector.Resize(v1, arg);
                    nv1 = NaiveBitVector.Resize(nv1, arg);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("get "))
                {
                    input = input.Substring(4);
                    arg = int.Parse(input);
                    Console.WriteLine(GetBit(v1, arg));
                    Console.WriteLine(GetBit(nv1, arg));
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("set 0 "))
                {
                    input = input.Substring(6);
                    arg = int.Parse(input);
                    SetBit(v1, arg, false);
                    SetBit(nv1, arg, false);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("set 1 "))
                {
                    input = input.Substring(6);
                    arg = int.Parse(input);
                    SetBit(v1, arg, true);
                    SetBit(nv1, arg, true);
                    Console.WriteLine(v1);
                    Console.WriteLine(nv1);
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("not"))
                {
                    Console.WriteLine("IN  GDAL: " + v1);
                    Console.WriteLine("IN  NBV: " + nv1);
                    Console.WriteLine("OUT GDAL: " + !v1);
                    Console.WriteLine("OUT NBV: " + !nv1);
                    Console.WriteLine("INW GDAL: " + v1.ApplyNot());
                    Console.WriteLine("INW NBV: " + nv1.ApplyNot());
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("and "))
                {
                    input = input.Substring(4);
                    v2 = ParseVector(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());
                    Console.WriteLine("IN2 GDAL: " + v2.ToString());

                    nv2 = ParseNVector(input);
                    Console.WriteLine("IN1 NBV: " + nv1.ToString());
                    Console.WriteLine("IN2 NBV: " + nv2.ToString());

                    Console.WriteLine("OUT GDAL: " + (v1 & v2).ToString());
                    Console.WriteLine("OUT NBV: " + (nv1 & nv2).ToString());
                    Console.WriteLine("INW GDAL: " + (v1.ApplyAnd(v2)).ToString());
                    Console.WriteLine("INW NBV: " + (nv1.ApplyAnd(nv2)).ToString());

                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("or "))
                {
                    input = input.Substring(3);
                    v2 = ParseVector(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());
                    Console.WriteLine("IN2 GDAL: " + v2.ToString());

                    nv2 = ParseNVector(input);
                    Console.WriteLine("IN1 NBV: " + nv1.ToString());
                    Console.WriteLine("IN2 NBV:" + nv2.ToString());

                    Console.WriteLine("OUT GDAL: " + (v1 | v2).ToString());
                    Console.WriteLine("OUT NBV: " + (nv1 | nv2).ToString());
                    Console.WriteLine("INW GDAL: " + (v1.ApplyOr(v2)).ToString());
                    Console.WriteLine("INW NBV: " + (nv1.ApplyOr(nv2)).ToString());

                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("xor "))
                {
                    input = input.Substring(4);
                    v2 = ParseVector(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());
                    Console.WriteLine("IN2 GDAL: " + v2.ToString());

                    nv2 = ParseNVector(input);
                    Console.WriteLine("IN1 NBV: " + nv1.ToString());
                    Console.WriteLine("IN2 NBV: " + nv2.ToString());

                    Console.WriteLine("OUT GDAL: " + (v1 ^ v2).ToString());
                    Console.WriteLine("OUT NBV: " + (nv1 ^ nv2).ToString());
                    Console.WriteLine("INW GDAL: " + (v1.ApplyXor(v2)).ToString());
                    Console.WriteLine("INW NBV: " + (nv1.ApplyXor(nv2)).ToString());
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("cmp "))
                {
                    input = input.Substring(4);
                    v2 = ParseVector(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());
                    Console.WriteLine("IN2 GDAL: " + v2.ToString());

                    nv2 = ParseNVector(input);
                    Console.WriteLine("IN1 NBV: " + nv1.ToString());
                    Console.WriteLine("IN2 NBV: " + nv2.ToString());

                    Console.WriteLine("OUT GDAL: " + (v1 == v2).ToString());
                    Console.WriteLine("OUT NBV: " + (nv1 == nv2).ToString());
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("concat rand "))
                {
                    input = input.Substring(12);
                    arg = int.Parse(input);
                    input = Generate01String(arg, rand);

                    v2 = ParseVector(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());
                    Console.WriteLine("IN2 GDAL: " + new string(' ', v1.Length) + v2.ToString());

                    nv2 = ParseNVector(input);
                    Console.WriteLine("IN1 NBV: " + nv1.ToString());
                    Console.WriteLine("IN2 NBV: " + new string(' ', nv1.Length) + nv2.ToString());

                    Console.WriteLine("OUT GDAL: " + ((v1 = GDALBitVector.Concat(v1, v2)).ToString()));
                    Console.WriteLine("OUT NBV: " + ((nv1 = NaiveBitVector.Concat(nv1, nv2)).ToString()));
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("concat "))
                {
                    input = input.Substring(7);

                    v2 = ParseVector(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());
                    Console.WriteLine("IN2 GDAL: " + v2.ToString());

                    nv2 = ParseNVector(input);
                    Console.WriteLine("IN1 NBV: " + nv1.ToString());
                    Console.WriteLine("IN2 NBV: " + nv2.ToString());

                    Console.WriteLine("OUT GDAL: " + (GDALBitVector.Concat(v1, v2).ToString()));
                    Console.WriteLine("OUT NBV: " + (NaiveBitVector.Concat(nv1, nv2).ToString()));
                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("shl "))
                {
                    input = input.Substring(4);
                    arg = int.Parse(input);
                    Console.WriteLine("IN1 GDAL: " + new string(' ', arg) + v1.ToString());

                    Console.WriteLine("IN1 NBV: " + new string(' ', arg) + nv1.ToString());

                    Console.WriteLine("OUT GDAL: " + (v1 << arg).ToString());
                    Console.WriteLine("OUT NBV: " + (nv1 << arg).ToString());

                    Console.WriteLine();
                    continue;
                }
                if (input.StartsWith("shr "))
                {
                    input = input.Substring(4);
                    arg = int.Parse(input);
                    Console.WriteLine("IN1 GDAL: " + v1.ToString());

                    Console.WriteLine("IN1 NBV: " + nv1.ToString());

                    Console.WriteLine("OUT GDAL: " + new string(' ', arg) + (v1 >> arg).ToString());
                    Console.WriteLine("OUT NBV: " + new string(' ', arg) + (nv1 >> arg).ToString());

                    Console.WriteLine();
                    continue;
                }
            }
        }

        static void NBVRandTest(int iterations)
        {
            int lengthFloor = 0, lengthCeiling = 83, i1, counter;

            Random rand = new Random();

            string s1, s2, svRes, snvRes;
            bool b1;
            GDALBitVector v1, v2, vRes;
            NaiveBitVector nv1, nv2, nvRes;
            UInt32 i2;

        //goto CALC_RESIZE;

        APPLY_BLIT:
            counter = iterations * iterations;
            Console.WriteLine("APPLY_BLIT");
            while ((counter -= 5) > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor + 32, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                i1 = rand.Next(lengthFloor, s1.Length - 32);
                i2 = (UInt32)rand.Next() | (rand.Next(2) == 1 ? 1U << 31 : 0U);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v1.Write(i1, i2);
                nv1.Blit(i1, i2);

                svRes = v1.ToString();
                snvRes = nv1.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    //Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("APPLY_BLIT FINISHED");

        APPLY_NOT:
            counter = iterations;
            Console.WriteLine("APPLY_NOT");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v1.ApplyNot();
                nv1.ApplyNot();

                svRes = v1.ToString();
                snvRes = nv1.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    //Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("APPLY_NOT FINISHED");

        APPLY_AND:
            counter = iterations * iterations;
            Console.WriteLine("APPLY_AND");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                v1.ApplyAnd(v2);
                nv1.ApplyAnd(nv2);

                svRes = v1.ToString();
                snvRes = nv1.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("APPLY_AND FINISHED");

        APPLY_OR:
            counter = iterations * iterations;
            Console.WriteLine("APPLY_OR");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                v1.ApplyOr(v2);
                nv1.ApplyOr(nv2);

                svRes = v1.ToString();
                snvRes = nv1.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("APPLY_OR FINISHED");

        APPLY_XOR:
            counter = iterations * iterations;
            Console.WriteLine("APPLY_XOR");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                v1.ApplyXor(v2);
                nv1.ApplyXor(nv2);

                svRes = v1.ToString();
                snvRes = nv1.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("APPLY_XOR FINISHED");

        CALC_RESIZE:
            counter = iterations;
            Console.WriteLine("CALC_RESIZE");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                i1 = rand.Next(lengthFloor, lengthCeiling);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                vRes = GDALBitVector.Resize(v1, i1);
                nvRes = NaiveBitVector.Resize(nv1, i1);

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + i1);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_RESIZE FINISHED");

        CALC_BLIT:
            counter = iterations * iterations;
            Console.WriteLine("CALC_BLIT");
            while ((counter -= 5) > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor + 32, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                i1 = rand.Next(lengthFloor, s1.Length - 32);
                i2 = (UInt32)rand.Next() | (rand.Next(2) == 1 ? 1U << 31 : 0U);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                vRes = GDALBitVector.Write(v1, i1, i2);
                nvRes = NaiveBitVector.Blit(nv1, i1, i2);

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    //Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_BLIT FINISHED");

        CALC_CONCAT:
            counter = iterations * iterations;
            Console.WriteLine("CALC_CONCAT");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                vRes = GDALBitVector.Concat(v1, v2);
                nvRes = NaiveBitVector.Concat(nv1, nv2);

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_CONCAT FINISHED");

        CALC_NOT:
            counter = iterations;
            Console.WriteLine("CALC_NOT");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                vRes = ~v1;
                nvRes = ~nv1;

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    //Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_NOT FINISHED");

        CALC_AND:
            counter = iterations * iterations;
            Console.WriteLine("CALC_AND");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                vRes = v1 & v2;
                nvRes = nv1 & nv2;

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_AND FINISHED");

        CALC_OR:
            counter = iterations * iterations;
            Console.WriteLine("CALC_OR");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                vRes = v1 | v2;
                nvRes = nv1 | nv2;

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_OR FINISHED");

        CALC_XOR:
            counter = iterations * iterations;
            Console.WriteLine("CALC_XOR");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                v2 = ParseVector(s2);
                nv2 = ParseNVector(s2);

                vRes = v1 ^ v2;
                nvRes = nv1 ^ nv2;

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_XOR FINISHED");

        CALC_SHL:
            counter = iterations * iterations;
            Console.WriteLine("CALC_SHL");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                i1 = rand.Next(-lengthCeiling, lengthCeiling);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                vRes = v1 << i1;
                nvRes = nv1 << i1;

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + i1);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_SHL FINISHED");

        CALC_SHR:
            counter = iterations * iterations;
            Console.WriteLine("CALC_SHR");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);
                i1 = rand.Next(-lengthCeiling, lengthCeiling);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                vRes = v1 >> i1;
                nvRes = nv1 >> i1;

                svRes = vRes.ToString();
                snvRes = nvRes.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    Console.WriteLine("IN2: " + i1);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("CALC_SHR FINISHED");

        RANDOM_ACCESS_READ:
            counter = iterations;
            Console.WriteLine("RANDOM_ACCESS_READ");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor + 1, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                i2 = (uint)Math.Sqrt(iterations);
                while (--i2 > 0)
                {
                    i1 = rand.Next(s1.Length);
                    if (v1[i1] != nv1[i1])
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine("IN1: " + s1);
                        Console.WriteLine("IN2: " + i1.ToString());
                        Console.WriteLine("OUT GDAL: " + v1[i1].ToString());
                        Console.WriteLine("OUT NBV: " + nv1[i1].ToString());
                    }
                }
            }
            Console.WriteLine("RANDOM_ACCESS_READ FINISHED");

        RANDOM_ACCESS_WRITE:
            counter = iterations;
            Console.WriteLine("RANDOM_ACCESS_WRITE");
            while (--counter > -1)
            {
                s1 = Generate01String(rand.Next(lengthFloor + 1, lengthCeiling), rand);
                //s2 = Generate01String(rand.Next(lengthFloor, lengthCeiling), rand);

                v1 = ParseVector(s1);
                nv1 = ParseNVector(s1);

                i2 = (uint)iterations;
                while (i2-- > 0)
                {
                    i1 = rand.Next(s1.Length);
                    b1 = rand.Next(2) == 1;
                    v1[i1] = b1;
                    nv1[i1] = b1;
                }

                svRes = v1.ToString();
                snvRes = nv1.ToString();

                if (svRes != snvRes)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("IN1: " + s1);
                    //Console.WriteLine("IN2: " + s2);
                    Console.WriteLine("OUT GDAL: " + svRes);
                    Console.WriteLine("OUT NBV: " + snvRes);
                }
            }
            Console.WriteLine("RANDOM_ACCESS_WRITE FINISHED");
        }

        static GDALBitVector CreateVector(int length)
        {
            return new GDALBitVector(length);
        }

        static NaiveBitVector CreateNVector(int length)
        {
            return new NaiveBitVector(length);
        }

        static GDALBitVector ParseVector(string input)
        {
            return GDALBitVector.Parse(input);
        }

        static NaiveBitVector ParseNVector(string input)
        {
            return NaiveBitVector.Parse(input);
        }

        static string Generate01String(int length, Random rand)
        {
            char[] result = new char[length];
            for (length--; length > -1; length--)
                result[length] = rand.Next(2) == 1 ? '1' : '0';
            return new string(result);
        }

        static (GDALBitVector v, NaiveBitVector nv) SeedVectors(GDALBitVector vector, NaiveBitVector nVector, Random rand)
        {
            for (int index = 0, ceil = vector.Length; index < ceil; index++)
                nVector[index] = vector[index] = rand.Next(2) == 1;
            return (vector, nVector);
        }

        static void SeedVectors0(GDALBitVector vector, NaiveBitVector nVector)
        {
            for (int index = 0, ceil = vector.Length; index < ceil; index++)
                nVector[index] = vector[index] = false;
        }

        static void SeedVectors1(GDALBitVector vector, NaiveBitVector nVector)
        {
            for (int index = 0, ceil = vector.Length; index < ceil; index++)
                nVector[index] = vector[index] = true;
        }

        static bool GetBit(GDALBitVector vector, int index)
        {
            return vector[index];
        }

        static void SetBit(GDALBitVector vector, int index, bool value)
        {
            vector[index] = value;
        }

        static bool GetBit(NaiveBitVector vector, int index)
        {
            return vector[index];
        }

        static void SetBit(NaiveBitVector vector, int index, bool value)
        {
            vector[index] = value;
        }
    }

    /// <summary>
    /// Naive BitVector implementation designed to test GDALBitVector against
    /// </summary>
    public struct NaiveBitVector
    {
        public static readonly NaiveBitVector Empty = new NaiveBitVector(0);

        bool[] data;
        int length;

        public NaiveBitVector(int length)
        {
            data = new bool[length];
            this.length = length;
        }

        public NaiveBitVector(params bool[] data)
        {
            this.data = (bool[])data.Clone();
            this.length = data.Length;
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public bool this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {

                data[index] = value;
            }
        }

        public static NaiveBitVector Resize(NaiveBitVector vector, int newLength)
        {
            bool[] data = vector.data,
                newData = new bool[newLength];
            Array.Copy(data, newData, Math.Min(data.Length, newData.Length));
            return new NaiveBitVector(newData);
        }

        public NaiveBitVector Blit(int index, UInt32 value)
        {
            bool[] data = this.data;
            for (int ceil = index + 32; index < ceil; index++)
            {
                data[index] = (value & 1U) == 1;
                value >>= 1;
            }
            return this;
        }

        public static NaiveBitVector Blit(NaiveBitVector vector, int index, UInt32 value)
        {
            bool[] data = vector.data,
                newData = (bool[])data.Clone();

            for (int ceil = index + 32; index < ceil; index++)
            {
                newData[index] = (value & 1U) == 1;
                value >>= 1;
            }

            return new NaiveBitVector(newData);
        }

        public NaiveBitVector ApplyNot()
        {
            int index = length;
            bool[] data = this.data;
            while (--index > -1)
                data[index] = !data[index];
            return this;
        }

        public NaiveBitVector ApplyAnd(NaiveBitVector value)
        {
            int leftLength = this.length, rightLength = value.length, arrayIndex, floor;
            bool[] leftArray = this.data, rightArray = value.data;
            if (leftLength > rightLength)
            {
                floor = rightLength - 1;
                arrayIndex = leftLength;
                for (arrayIndex--; arrayIndex > floor; arrayIndex--)
                    leftArray[arrayIndex] = false;
                for (; arrayIndex > -1; arrayIndex--)
                    leftArray[arrayIndex] &= rightArray[arrayIndex];
                return this;
            }
            else
            {
                arrayIndex = leftLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    leftArray[arrayIndex] &= rightArray[arrayIndex];
                return this;
            }
        }

        public NaiveBitVector ApplyOr(NaiveBitVector value)
        {
            int leftLength = this.length, rightLength = value.length, arrayIndex;
            bool[] leftArray = this.data, rightArray = value.data;
            if (leftLength > rightLength)
            {
                arrayIndex = rightLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    leftArray[arrayIndex] |= rightArray[arrayIndex];
                return this;
            }
            else
            {
                arrayIndex = leftLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    leftArray[arrayIndex] |= rightArray[arrayIndex];
                return this;
            }
        }

        public NaiveBitVector ApplyXor(NaiveBitVector value)
        {
            int leftLength = this.length, rightLength = value.length, arrayIndex;
            bool[] leftArray = this.data, rightArray = value.data;
            if (leftLength > rightLength)
            {
                arrayIndex = rightLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    leftArray[arrayIndex] ^= rightArray[arrayIndex];
                return this;
            }
            else
            {
                arrayIndex = leftLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    leftArray[arrayIndex] ^= rightArray[arrayIndex];
                return this;
            }
        }

        public static NaiveBitVector Concat(NaiveBitVector left, NaiveBitVector right)
        {
            int ceil = left.length, index1, index2;
            bool[] currentArray = left.data, result = new bool[ceil + right.length];
            for (index1 = 0; index1 < ceil; index1++)
                result[index1] = currentArray[index1];
            currentArray = right.data;
            for (index2 = 0, ceil = right.length; index2 < ceil; index1++, index2++)
                result[index1] = currentArray[index2];
            return new NaiveBitVector(result);
        }

        public static NaiveBitVector operator ~(NaiveBitVector value)
        {
            int index;
            bool[] data = value.data, result = new bool[index = data.Length];
            while (--index > -1)
                result[index] = !data[index];
            return new NaiveBitVector(result);
        }

        public static NaiveBitVector operator !(NaiveBitVector value)
        {
            int index;
            bool[] data = value.data, result = new bool[index = data.Length];
            while (--index > -1)
                result[index] = !data[index];
            return new NaiveBitVector(result);
        }

        public static NaiveBitVector operator &(NaiveBitVector left, NaiveBitVector right)
        {
            int leftLength = left.length, rightLength = right.length, arrayIndex;
            bool[] leftArray = left.data, rightArray = right.data, resultArray;
            if (leftLength > rightLength)
            {
                resultArray = new bool[leftLength];
                arrayIndex = rightLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex] & rightArray[arrayIndex];
                return new NaiveBitVector(resultArray);
            }
            else
            {
                resultArray = new bool[rightLength];
                arrayIndex = leftLength;
                for (arrayIndex--; arrayIndex > -1; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex] & rightArray[arrayIndex];
                return new NaiveBitVector(resultArray);
            }
        }

        public static NaiveBitVector operator |(NaiveBitVector left, NaiveBitVector right)
        {
            int leftLength = left.length, rightLength = right.length, arrayIndex, floor;
            bool[] leftArray = left.data, rightArray = right.data, resultArray;
            if (leftLength > rightLength)
            {
                floor = rightLength - 1;
                arrayIndex = leftLength;
                resultArray = new bool[arrayIndex];
                for (arrayIndex--; arrayIndex > floor; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex];
                for (; arrayIndex > -1; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex] | rightArray[arrayIndex];
                return new NaiveBitVector(resultArray);
            }
            else
            {
                floor = leftLength - 1;
                arrayIndex = rightLength;
                resultArray = new bool[arrayIndex];
                for (arrayIndex--; arrayIndex > floor; arrayIndex--)
                    resultArray[arrayIndex] = rightArray[arrayIndex];
                for (; arrayIndex > -1; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex] | rightArray[arrayIndex];
                return new NaiveBitVector(resultArray);
            }
        }

        public static NaiveBitVector operator ^(NaiveBitVector left, NaiveBitVector right)
        {
            int leftLength = left.length, rightLength = right.length, arrayIndex, floor;
            bool[] leftArray = left.data, rightArray = right.data, resultArray;
            if (leftLength > rightLength)
            {
                floor = rightLength - 1;
                arrayIndex = leftLength;
                resultArray = new bool[arrayIndex];
                for (arrayIndex--; arrayIndex > floor; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex];
                for (; arrayIndex > -1; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex] ^ rightArray[arrayIndex];
                return new NaiveBitVector(resultArray);
            }
            else
            {
                floor = leftLength - 1;
                arrayIndex = rightLength;
                resultArray = new bool[arrayIndex];
                for (arrayIndex--; arrayIndex > floor; arrayIndex--)
                    resultArray[arrayIndex] = rightArray[arrayIndex];
                for (; arrayIndex > -1; arrayIndex--)
                    resultArray[arrayIndex] = leftArray[arrayIndex] ^ rightArray[arrayIndex];
                return new NaiveBitVector(resultArray);
            }
        }

        public static bool operator ==(NaiveBitVector left, NaiveBitVector right)
        {
            int length = left.length, arrayIndex;
            bool[] leftArray = left.data, rightArray = right.data;
            if (length != right.length)
                return false;
            if (length == 0)
                return true;
            arrayIndex = length;
            for (; arrayIndex > -1; arrayIndex--)
                if (leftArray[arrayIndex] != rightArray[arrayIndex])
                    return false;
            return true;
        }

        public static bool operator !=(NaiveBitVector left, NaiveBitVector right)
        {
            int length = left.length, arrayIndex;
            bool[] leftArray = left.data, rightArray = right.data;
            if (length != right.length)
                return true;
            if (length == 0)
                return false;
            arrayIndex = length;
            for (; arrayIndex > -1; arrayIndex--)
                if (leftArray[arrayIndex] != rightArray[arrayIndex])
                    return true;
            return false;
        }

        public static NaiveBitVector operator >>(NaiveBitVector left, int right)
        {
            if (right < 0)
                return left << -right;
            int index = 0, shiftIndex = right, ceil = left.length - right;
            if (ceil < 1)
                return new NaiveBitVector(0);
            bool[] leftArray = left.data,
                resultArray = new bool[ceil];
            for (; index < ceil; index++, shiftIndex++)
            {
                resultArray[index] = leftArray[shiftIndex];
            }
            return new NaiveBitVector(resultArray);
        }

        public static NaiveBitVector operator <<(NaiveBitVector left, int right)
        {
            if (right < 0)
                return left >> -right;
            int index = right, shiftIndex = 0, ceil;
            bool[] leftArray = left.data,
                resultArray = new bool[ceil = left.length + right];
            for (; index < ceil; index++, shiftIndex++)
            {
                resultArray[index] = leftArray[shiftIndex];
            }
            return new NaiveBitVector(resultArray);
        }

        public object Clone()
        {
            bool[] data = this.data,
                newData;
            int length = this.length, arrayLength = data.Length;
            newData = new bool[arrayLength];
            Array.Copy(data, newData, arrayLength);
            return new NaiveBitVector(newData);
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NaiveBitVector v)
                return v == this;
            return false;
        }

        const char YES = '1',
                NO = '0';
        public override string ToString()
        {
            bool[] data = this.data;
            char[] result;
            int dataLength = this.length,
                arrayIndex = dataLength;

            if (dataLength == 0)
                return "_";

            result = new char[dataLength--];

            while (--arrayIndex > -1)
#if BLOCK_HEADERS
                result[arrayIndex] = data[arrayIndex] ? (arrayIndex & 31) == 0 ? 'Y' : YES : (arrayIndex & 31) == 0 ? 'N' : NO;
#else
                result[arrayIndex] = data[arrayIndex] ? YES : NO;
#endif

            return new string(result);
        }

        public static NaiveBitVector Parse(string input)
        {
            bool[] resultArray;
            int inputLength = input.Length,
                inputIndex;

            if (inputLength == 0)
                return NaiveBitVector.Empty;

            inputIndex = inputLength - 1;

            resultArray = new bool[inputLength];

            for (; inputIndex > -1; inputIndex--)
            {
                resultArray[inputIndex] = input[inputIndex] != NO;
            }
            return new NaiveBitVector(resultArray);
        }
    }
}