﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using System.IO.Compression;

namespace Randomizer
{
    class Program
    {
        static void Main(string[] args)
        {
            DetectReplace();
        }

        private static byte[] randomizeString =
        {
          0x53, 0x00, 0x75, 0x00, 0x70, 0x00, 0x65, 0x00, 0x72, 0x00, 0x55, 0x00, 0x6E, 0x00, 0x69, 0x00, 0x71, 0x00, 0x75, 0x00, 0x65, 0x00,
          0x53, 0x00, 0x74, 0x00, 0x72, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x67, 0x00, 0x45, 0x00, 0x61, 0x00, 0x73, 0x00, 0x69, 0x00, 0x6C, 0x00,
          0x79, 0x00, 0x44, 0x00, 0x65, 0x00, 0x74, 0x00, 0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x61, 0x00, 0x62, 0x00, 0x6C, 0x00, 0x65, 0x00,
          0x54, 0x00, 0x6F, 0x00, 0x42, 0x00, 0x65, 0x00, 0x41, 0x00, 0x62, 0x00, 0x6C, 0x00, 0x65, 0x00, 0x54, 0x00, 0x6F, 0x00, 0x52, 0x00,
          0x61, 0x00, 0x6E, 0x00, 0x64, 0x00, 0x6F, 0x00, 0x6D, 0x00, 0x69, 0x00, 0x7A, 0x00, 0x65, 0x00, 0x54, 0x00, 0x68, 0x00, 0x65, 0x00,
          0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x67, 0x00, 0x72, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x41, 0x00, 0x6E, 0x00, 0x64, 0x00, 0x42, 0x00,
          0x79, 0x00, 0x70, 0x00, 0x61, 0x00, 0x73, 0x00, 0x73, 0x00, 0x53, 0x00, 0x69, 0x00, 0x67, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x74, 0x00,
          0x75, 0x00, 0x72, 0x00, 0x65, 0x00, 0x42, 0x00, 0x61, 0x00, 0x73, 0x00, 0x65, 0x00, 0x64, 0x00, 0x42, 0x00, 0x6C, 0x00, 0x6F, 0x00,
          0x63, 0x00, 0x6B, 0x00
        };

        private static char RandomChar()
        {
            char[] chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&".ToCharArray();
            Random r = new Random();
            return chars[r.Next(chars.Length)];
        }

        private static long RandomLong(long min, long max)
        {
            Random rand = new Random();
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }

        public static void DetectReplace()
        {

            using (var stream = new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"\ShinraMeter.exe", FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                KeyValuePair<long, long>? detected = Detect(stream);
                if(detected == null)
                {
                    Console.WriteLine("The file as already been randomized or randomization impossible. Exiting");
                    return;
                }
                Randomize(stream, detected.Value);

            }
            return;
        }

        public static void Randomize(FileStream stream, KeyValuePair<long, long> positions)
        {
            long size = positions.Value - positions.Key;
            long beginRandomize = RandomLong(positions.Value, positions.Key-1);
            long sizeRandomize = RandomLong(2, positions.Key - beginRandomize);

            stream.Position = beginRandomize;
            if(stream.ReadByte() != 0)
            {
                stream.Position--;
            }
            while(stream.Position < beginRandomize + sizeRandomize)
            {
                stream.WriteByte(Convert.ToByte(RandomChar()));
                stream.Position = stream.Position+2;
            }
        }

        public static KeyValuePair<long, long>? Detect(FileStream stream)
        {
            int byteCheckPosition = 0;
            long beginPosition = 0;
            while(stream.Position < stream.Length)
            {
                if (byteCheckPosition == 0)
                {
                    beginPosition = stream.Position;
                }

                if (stream.ReadByte() == randomizeString[byteCheckPosition])
                {
                    byteCheckPosition++;
                }
                else
                {
                    byteCheckPosition = 0;
                }
                if(byteCheckPosition == randomizeString.Length)
                {
                    return new KeyValuePair<long, long>(beginPosition, stream.Position-1);
                }

            }
            return null;
        }
    }
}