using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_analysis_player.decoder.process
{
    internal class SupportClass
    {
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        public static int URShift(int number, long bits)
        {
            return URShift(number, (int)bits);
        }

        public static long URShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2L << ~bits);
        }

        public static long URShift(long number, long bits)
        {
            return URShift(number, (int)bits);
        }

        /*******************************/


        /*******************************/

        /// <summary>
        ///     This method is used as a dummy method to simulate VJ++ behavior
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static long Identity(long literal)
        {
            return literal;
        }

        /// <summary>
        ///     This method is used as a dummy method to simulate VJ++ behavior
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static ulong Identity(ulong literal)
        {
            return literal;
        }

        /// <summary>
        ///     This method is used as a dummy method to simulate VJ++ behavior
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static float Identity(float literal)
        {
            return literal;
        }

        /// <summary>
        ///     This method is used as a dummy method to simulate VJ++ behavior
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static double Identity(double literal)
        {
            return literal;
        } 
        
        /*******************************/

        /// <summary>
        ///     Converts an array of sbytes to an array of bytes
        /// </summary>
        /// <param name="sbyteArray">The array of sbytes to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] ToByteArray(sbyte[] sbyteArray)
        {
            byte[] byteArray = new byte[sbyteArray.Length];
            for (int index = 0; index < sbyteArray.Length; index++)
                byteArray[index] = (byte)sbyteArray[index];
            return byteArray;
        }

        /// <summary>
        ///     Converts a string to an array of bytes
        /// </summary>
        /// <param name="sourceString">The string to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] ToByteArray(string sourceString)
        {
            byte[] byteArray = new byte[sourceString.Length];
            for (int index = 0; index < sourceString.Length; index++)
                byteArray[index] = (byte)sourceString[index];
            return byteArray;
        }

        /// <summary>
        ///     Method that copies an array of sbytes from a String to a received array .
        /// </summary>
        /// <param name="sourceString">The String to get the sbytes.</param>
        /// <param name="sourceStart">Position in the String to start getting sbytes.</param>
        /// <param name="sourceEnd">Position in the String to end getting sbytes.</param>
        /// <param name="destinationArray">Array to store the bytes.</param>
        /// <param name="destinationStart">Position in the destination array to start storing the sbytes.</param>
        /// <returns>An array of sbytes</returns>
        public static void GetSBytesFromString(string sourceString, int sourceStart, int sourceEnd,
            ref sbyte[] destinationArray, int destinationStart)
        {
            int sourceCounter;
            int destinationCounter;
            sourceCounter = sourceStart;
            destinationCounter = destinationStart;
            while (sourceCounter < sourceEnd)
            {
                destinationArray[destinationCounter] = (sbyte)sourceString[sourceCounter];
                sourceCounter++;
                destinationCounter++;
            }
        }
    }
}
