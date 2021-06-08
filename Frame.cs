using System;
using System.Collections;

namespace NetsReal3
{
    public class Frame
    {
        private const int ControlSize = 16;

        private const int ChecksumSize = 8;
        private static readonly Random rand = new();

        public BitArray Control { get; set; }

        public BitArray Data { get; set; }

        public BitArray Checksum { get; set; }

        public BitArray ToBitArray()
        {
            var result = new BitArray(ControlSize + ChecksumSize + Data.Count);

            result.Write(0, Control);
            result.Write(ControlSize, Data);
            result.Write(ControlSize + Data.Count, Checksum);

            return result;
        }

        public static Frame Parse(BitArray bitArray)
        {
            var frame = new Frame();

            frame.Control = bitArray.Subsequence(0, ControlSize);
            frame.Checksum = bitArray.Subsequence(bitArray.Length - ChecksumSize, ChecksumSize);
            frame.Data = bitArray.Subsequence(ControlSize, bitArray.Length - ControlSize - ChecksumSize);

            return frame;
        }

        public static BitArray CreateFrameBitArray()
        {
            var frame = new Frame();

            frame.Control = new BitArray(ControlSize);

            frame.Data = new BitArray(Utils.DecimalToBinary(rand.Next(10000)));

            frame.Checksum = Utils.DecimalToBinary(Utils.CheckSum(frame.Data));


            return frame.ToBitArray();
        }
    }
}