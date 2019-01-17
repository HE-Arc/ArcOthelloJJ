using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OthelloJJ
{
    /// <summary>
    /// Class that manage serialization for the game
    /// <source>https://stackoverflow.com/a/22417240</source>
    /// </summary>
    class BinarySerialization
    {
        /// <summary>
        /// serialized a class into a file
        /// </summary>
        /// <typeparam name="T">type of class to serialized</typeparam>
        /// <param name="filePath">path of the file</param>
        /// <param name="objectToWrite">object to serialized and write in the file</param>
        /// <param name="append">if we append on the file or rewrite it</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {

            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
            

        }
        /// <summary>
        /// deserialize a binary file into class
        /// </summary>
        /// <typeparam name="T">Type of the class to deserialize</typeparam>
        /// <param name="filePath">path of the file</param>
        /// <returns></returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
        
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
           
        }
    }
}
