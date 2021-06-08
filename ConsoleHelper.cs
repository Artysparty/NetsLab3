using System;
using System.Collections;

namespace NetsReal3
{
    public static class ConsoleHelper
    {
        public static object LockObject = new();

        public static void WriteToConsole(string info, string write)
        {
            lock (LockObject)
            {
                Console.WriteLine(info + " : " + write);
            }
        }

        public static void WriteToConsoleArray(string info, BitArray array)
        {
            lock (LockObject)
            {
                Console.Write(info + " : ");
                for (var i = 0; i < array.Length; i++)
                    if (array[i])
                        Console.Write("1");
                    else
                        Console.Write("0");
                Console.WriteLine();
            }
        }

        public static void WriteBitArray(BitArray array)
        {
            lock (LockObject)
            {
                for (var i = 0; i < array.Length; i++)
                    if (array[i])
                        Console.Write("1");
                    else
                        Console.Write("0");
            }
        }
    }
}