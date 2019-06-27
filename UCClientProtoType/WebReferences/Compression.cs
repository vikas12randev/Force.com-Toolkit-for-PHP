using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebReferences
{
    public class Compression
    {
        //Compression

        /// <summary>Myphones.Buddies.Comms.Decompress</summary>
        /// <author>sgregory</author>
        /// <date>10 November 2004</date>
        /// <remarks>Given a base64 compressed string, decompresses and returns inflated string</remarks>
        /// <param name="inputBytes" type="byte[]">Input string as byte array</param>
        /// <returns type="string">A base64 version of the string </returns>
        /// <postconditions>Decompression taken place or exception thrown into callers catch - we purposefully do not try to catch at this level</postconditions>
        static internal string Decompress(
            byte[] inputBytes)
        {
            MemoryStream stZipped = null;
            MemoryStream stUnZipped = null;
            ZipInputStream stZipSource = null;

            try
            {
                // Create the streams
                stZipped = new MemoryStream();
                stUnZipped = new MemoryStream();

                // Write bytes into stream
                stZipped.Write(inputBytes, 0, inputBytes.Length);
                stZipped.Seek(0, SeekOrigin.Begin);

                // Load the compressed stream into zip input stream (which will automatically inflate it)
                stZipSource = new ZipInputStream(stZipped);
                ZipEntry theEntry;
                if ((theEntry = stZipSource.GetNextEntry()) != null)
                {
                    int intBufSize = 32768;
                    byte[] buffer = new byte[32768];
                    while (true)
                    {
                        intBufSize = stZipSource.Read(buffer, 0, buffer.Length);
                        if (intBufSize > 0)
                            stUnZipped.Write(buffer, 0, intBufSize);
                        else
                            break;
                    }
                }

                // Tidy input streams
                stZipped.Close();
                stZipSource.Close();

                // Rewind output stream for use...and read into byte array and return that
                stUnZipped.Seek(0, SeekOrigin.Begin);
                byte[] newBuffer = new byte[stUnZipped.Length];
                stUnZipped.Read(newBuffer, 0, (int)stUnZipped.Length);
                return new UTF8Encoding().GetString(newBuffer);
            }
            finally
            {
                // Tidy real carefully
                if (stZipSource != null)
                    stZipSource.Close();
                if (stUnZipped != null)
                    stUnZipped.Close();
                if (stZipped != null)
                    stZipped.Close();
            }
        }
    }
}
