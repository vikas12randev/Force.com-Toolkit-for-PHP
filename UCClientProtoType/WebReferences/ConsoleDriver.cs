using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;               // Process
using System.Drawing;					// For Images
using System.IO;
using System.Runtime.InteropServices;	// For P/Invoke
using System.Text;
using System.Threading;                 // Locking


// --------------------------------------------------------------------------------
// Namespace:   Myphones.Buddies
// Author:      sgregory
// Date:        10 November 2004
// Description: Buddies namespace
// 
// --------------------------------------------------------------------------------
namespace Myphones.Buddies.WebReferences
{   
    using ICSharpCode.SharpZipLib.Zip;			// Compression Zipping

    using ICSharpCode.SharpZipLib.Checksum;

    /// <summary>Myphones.Buddies.ConsoleDriver</summary>
    /// <author>sgregory</author>
    /// <date>10 November 2004</date>
    /// <remarks>Access to standard utility functions</remarks>
    public class ConsoleDriver
    {
        // Event and delegate for logging
        // RIP Diagnostics UI - dropped out for v10
        //public static event LogEventHandler logEvent;
        //public delegate void LogEventHandler(string logText, bool isContent);


        // Interop section


        // Unmanaged code access
        // These definitions allow P/Invoke to be used to access underlying
        // raw Win32 API's.
        [DllImport("shell32.dll")]
        static private extern int ShellExecuteA(
            int hWnd,
            string lpOperation,
            string lpFile,
            string lpParameters,
            string lpDirectory,
            int nShowCmd);

        [DllImport("shell32.dll")]
        static private extern int FindExecutableA(
            string lpFile,
            string lpDirectory,
            string lpResult);


        // GDI functions (Native Calls) for Bitmap manipulation
        // These are used to capture screen to an Image
        private const int SRCCOPY = 0xCC0020; // (DWORD) dest = source

        [DllImport("gdi32.dll")]
        static extern int CreateCompatibleDC(int refDC);
        [DllImport("gdi32.dll")]
        static extern int DeleteDC(int HDC);
        [DllImport("gdi32.dll")]
        static extern int SelectObject(int HDC, int hObject);
        [DllImport("gdi32.dll")]
        static extern int DeleteObject(int hObject);
        [DllImport("gdi32.dll")]
        static extern int BitBlt(
            int hdcDest, int nXOriginDest, int nYOriginDest,
            int nWidthDest, int nHeightDest,
            int hdcSrc, int nXOriginSrc, int nYOriginSrc, int dwRop);
        [DllImport("gdi32.dll")]
        static extern int CreateCompatibleBitmap(int refDC, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        static extern int GetDC(int hWnd);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(int hWnd, int hDC);
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();


        /// <summary>Myphones.Buddies.ConsoleDriver.InitialiseLogFileLocation</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Set up the logging location etc</remarks>
        static public void InitialiseLogFileLocation()
        {
            // Create the temp folders if we need to - this includes both \Logs and \Recordings sub-folders
            ConfigHelper.FileLogLocation = CreateTemporaryFoldersIfNecessary();

            // Derive log file names
            m_fileNameApp = ConfigHelper.FileLogLocation + ConfigHelper.Buddies_DEFAULT_CAPTION.Replace(" ", "") + "-APP-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            m_fileNameSip = ConfigHelper.FileLogLocation + ConfigHelper.Buddies_DEFAULT_CAPTION.Replace(" ", "") + "-SIP-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.InitialiseDiagnostics</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Set up the logging location etc</remarks>
        /// <param name="fileNameApp" type="out string">Slot for Application file</param>
        /// <param name="fileNameSip" type="out string">Slot for SIP file</param>
        static public void InitialiseDiagnostics(
            out string fileNameApp,
            out string fileNameSip)
        {
            // Give back something
            fileNameApp = m_fileNameApp;
            fileNameSip = m_fileNameSip;

            //// Delete todays files if they exist
            //try
            //{
            //    File.Delete(fileNameApp);
            //}
            //catch (Exception)
            //{
            //    // Ignore
            //}

            //try
            //{
            //    File.Delete(fileNameSip);
            //}
            //catch (Exception)
            //{
            //    // Ignore
            //}
        }


        static public string GetMyPhonesBaseDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MyPhones";
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.CreateTemporaryFoldersIfNecessary</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Set up the logging location etc</remarks>
        static public string CreateTemporaryFoldersIfNecessary()
        {
            // Get the temp folder base path
            string fileLogLocation = GetMyPhonesBaseDirectory();
            try
            {
                if (Directory.Exists(fileLogLocation) == false)
                    Directory.CreateDirectory(fileLogLocation);
            }
            catch (Exception)
            {
                // Can't start logging!!!
            }

            // Add Application name on
            string consoleType = "Console";     // (ConfigHelper.GetXBuddiesType() == "Receptionist") ? "ConsoleR" : "Console";
            fileLogLocation += "\\" + consoleType;
            try
            {
                if (Directory.Exists(fileLogLocation) == false)
                    Directory.CreateDirectory(fileLogLocation);
            }
            catch (Exception)
            {
                // Can't start logging!!!
            }

            // Add Logs name on
            fileLogLocation += "\\Logs";
            try
            {
                if (Directory.Exists(fileLogLocation) == false)
                    Directory.CreateDirectory(fileLogLocation);
            }
            catch (Exception)
            {
                // Can't start logging!!!
            }
            fileLogLocation += "\\";

            // Do one for recordings too
            string fileRecLocation = fileLogLocation.Replace("Logs\\", "Recordings");
            try
            {
                if (Directory.Exists(fileRecLocation) == false)
                    Directory.CreateDirectory(fileRecLocation);
            }
            catch (Exception)
            {
                // Can't start logging!!!
            }

            return fileLogLocation;
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.LogEventContentToConsole</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Logs messages to diagnostics</remarks>
        static public void LogEventContentToConsole(
            string message)
        {
            LogEventContentToConsole(
                null,
                message);
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.LogEventContentToConsole</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Logs messages to diagnostics</remarks>
        /// <param name="type" type="object">Type of object doing logging</param>
        /// <param name="message" type="string">Message to log</param>
        static public void LogEventContentToConsole(
            object type,
            string message)
        {
            try
            {
                // Repair bad inputs
                string typeString = "";
                if (type != null)
                    typeString = type.GetType().ToString();
                if (message == null)
                    message = "(null)";

                // Always log message content to file
                DateTime dtNow = DateTime.Now;

                // Build line
                if (type == null)
                    message = String.Format("{0} {1}.{2}: {3}", dtNow.ToShortDateString(), dtNow.ToLongTimeString(), dtNow.TimeOfDay.Milliseconds.ToString("000"), message);
                else
                    message = String.Format("{0} {1}.{2}: [{3}] {4}", dtNow.ToShortDateString(), dtNow.ToLongTimeString(), dtNow.TimeOfDay.Milliseconds.ToString("000"), typeString, message);

                // Log it - synchronously until we figure out hwo to stop leaking / truncating messages async
                Log2FileSync(message);
                //await Log2FileAsync(message);

                // RIP Diagnostics UI - dropped out for v10
                //// Add to diagnostic log too
                //if (logEvent != null)
                //    logEvent(message, true);
            }
            catch (Exception)
            {
                // Ignore
            }
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.LogSummaryMsgToConsole</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Logs messages to diagnostics</remarks>
        /// <param name="message" type="string">Message to log</param>
        static public void LogSummaryMsgToConsole(
            string message)
        {
            LogSummaryMsgToConsole(
                null,
                message);
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.LogSummaryMsgToConsole</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Logs messages to diagnostics</remarks>
        /// <param name="type"  type="object">Type of object logging message</param>
        /// <param name="message" type="string">Message to log</param>
        static public void LogSummaryMsgToConsole(
            object type,
            string message)
        {
            try
            {
                // Repair bad inputs
                string typeString = "";
                if (type != null)
                    typeString = type.GetType().ToString();
                if (message == null)
                    message = "(null)";

                // Build line
                DateTime dtNow = DateTime.Now;
                if (type == null)
                    message = String.Format("{0} {1}.{2}: {3}", dtNow.ToShortDateString(), dtNow.ToLongTimeString(), dtNow.TimeOfDay.Milliseconds.ToString("000"), message);
                else
                    message = String.Format("{0} {1}.{2}: [{3}] {4}", dtNow.ToShortDateString(), dtNow.ToLongTimeString(), dtNow.TimeOfDay.Milliseconds.ToString("000"), typeString, message);

                // Log it - synchronously until we figure out hwo to stop leaking / truncating messages async
                Log2FileSync(message);
                //await Log2FileAsync(message);

                // RIP Diagnostics UI - dropped out for v10
                //// Add to diagnostic log too
                //if (logEvent != null)
                //    logEvent(message, false);
            }
            catch (Exception)
            {
                // Ignore
            }
        }





        /// <summary>Myphones.Buddies.ConsoleDriver.LaunchProcess</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Launches named program with optional arguments</remarks>
        /// <param name="fileName" type="string">File spec / name to start</param>
        /// <param name="workingFolder" type="string">Working folder</param>
        /// <param name="argsSDS" type="string">Arguments in a space delimited stylee</param>
        /// <param name="hidden" type="bool">true if child process window should be hidden</param>
        /// <param name="waitMs" type="int">Wait for process to complete ms</param>
        /// <returns type="string">Error message or blank</returns>
        /// <postconditions>Browser appears and redirects to relevant URL</postconditions>
        static public bool LaunchProcess(
            string fileName,
            string workingFolder,
            string argsSDS,
            bool hidden,
            int waitMs)
        {
            bool success = false;
            Process childProcess = null;

            ConsoleDriver.LogSummaryMsgToConsole("[Myphones.Buddies.ConsoleDriver] Launching program [" + fileName + "] with args [" + argsSDS + "] - waiting up to " + waitMs.ToString() + "ms...");

            try
            {
                // Create process
                childProcess = new Process();

                // Set up object
                childProcess.StartInfo.FileName = fileName;
                if (workingFolder != null && workingFolder.Length > 0)
                    childProcess.StartInfo.WorkingDirectory = workingFolder;
                if (hidden)
                {
                    childProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    childProcess.StartInfo.CreateNoWindow = true;
                }
                if (argsSDS.Length > 0)
                    childProcess.StartInfo.Arguments = argsSDS;

                // And run...waiting up to 2s
                childProcess.Start();
                switch (waitMs)
                {
                    case 0: break;                                      // Dont wait at all
                    case -1: childProcess.WaitForExit(); break;         // Wait for ever
                    default: childProcess.WaitForExit(waitMs); break;   // Wait for a time
                }

                // Mark as good
                success = true;
            }
            catch (Exception ex)
            {
                ConsoleDriver.LogSummaryMsgToConsole("[Myphones.Buddies.ConsoleDriver]   -> Exception caught while launching program - " + ex.Message);
            }

            return success;
        }


        // --------------------------------------------------------------------------------
        // Routine:     ConsoleDriver.SecsToMins
        // Author:      sgregory
        // Date:        01 Sept 2004
        // Description: Takes a string representing seconds and turns it into a formatted
        //              lump 00:00
        // --------------------------------------------------------------------------------
        static public string SecsToMins(string seconds, bool treatBlankAsStillInSession)
        {
            string secs2mins = "";

            // Preconditions. If still recording, mark with *, else if zero, mark with -
            if (seconds.Length == 0)
                seconds = (treatBlankAsStillInSession) ? "*" : "-";
            else if (seconds == "0")
                seconds = "-";

            // Set up minutes looking value
            if (false == seconds.StartsWith("-") && false == seconds.StartsWith("*"))
            {
                int secs = Convert.ToInt32(seconds);
                int hours = (int)(secs / 3600);
                secs -= (hours * 3600);
                int mins = (int)(secs / 60);
                secs2mins = mins.ToString("00") + ":" + (secs % 60).ToString("00");
                if (hours > 0)
                {
                    if (secs2mins.Substring(1) == ":")
                        secs2mins = "0" + secs2mins;
                    secs2mins = (hours % 60).ToString("00") + ":" + secs2mins;
                    if (secs2mins.StartsWith("0"))
                        secs2mins = secs2mins.Substring(1);
                }
            }
            else
                secs2mins = seconds;

            return secs2mins;
        }



        // Private helpers


        /// <summary>Myphones.Buddies.ConsoleDriver.DeleteFile</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Delete the file</remarks>
        static private void DeleteFile(
            string fileLogLocation)
        {
            try
            {
                File.Delete(fileLogLocation);
            }
            catch (Exception)
            {
                // Ignore
            }
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.Log2FileSync</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Log message to the file - sync</remarks>
        static private void Log2FileSync(
            string message)
        {
            // Massage the message
            string longText = message.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine) + Environment.NewLine;

            try
            {
                // Set lock
                m_fileLogLock.EnterWriteLock();

                // Do work
                using (FileStream fsLog = new FileStream(m_fileNameApp, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter log = new StreamWriter(fsLog))
                        log.Write(longText);
                }
            }
            catch (Exception)
            {
                // Ignore
            }
            finally
            {
                // Reelase lock
                m_fileLogLock.ExitWriteLock();
            }
        }


        /// <summary>Myphones.Buddies.ConsoleDriver.Log2FileAsync</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Log message to the file - async</remarks>
        //static private async Task Log2FileAsync(
        //    string message)
        //{
        //    // Massage the message
        //    string longText = message.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine) + Environment.NewLine;

        //    try
        //    {
        //        // Set lock
        //        m_fileLogLock.EnterWriteLock();

        //        // Do work
        //        using (FileStream fsLog = new FileStream(m_fileNameApp, FileMode.Append, FileAccess.Write, FileShare.Read, longText.Length, true))
        //        {
        //            using (StreamWriter log = new StreamWriter(fsLog))
        //                await log.WriteAsync(longText);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // Ignore
        //    }
        //    finally
        //    {
        //        // Reelase lock
        //        m_fileLogLock.ExitWriteLock();
        //    }
        //}




        // Compression routines



        /// <summary>Myphones.Buddies.ConsoleDriver.CompressIfRequired</summary>
        /// <author>Simon G</author>
        /// <date>15 September 2004</date>
        /// <remarks>Build a ZIP memory stream</remarks>
        /// <param name="entryName" type="string">Name as found in ZIP file</param>
        /// <param name="stBuffer" type="Stream">Uncompresed stream</param>
        /// <returns type="MemoryStream">Zipped stream</returns>
        static private MemoryStream CompressDiagnosticFile(
            string entryName,
            Stream stBuffer)
        {
            DateTime now = DateTime.Now;

            // Get a memory stream
            MemoryStream stZipped = new MemoryStream();

            // Read the buffer from the stream (remember to rewind the thing first)
            stBuffer.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[stBuffer.Length];
            stBuffer.Seek(0, SeekOrigin.Begin);
            stBuffer.Read(buffer, 0, buffer.Length);
            stBuffer.Close();

            Crc32 crc = new Crc32();
            MemoryStream stTarget = new MemoryStream();
            // Create a zip stream with maximum compression
            ZipOutputStream stZipTarget = new ZipOutputStream(stTarget);
            stZipTarget.SetLevel(6);
            stZipTarget.SetComment("Diagnostic file for " + ConfigHelper.Buddies_DEFAULT_CAPTION + " " + ConfigHelper.Buddies_VERSION);
            string strFileName = entryName;
            ZipEntry entry = new ZipEntry(strFileName);
            entry.DateTime = now;
            entry.Size = buffer.Length;
            crc.Update(buffer);
            entry.Crc = crc.Value;
            stZipTarget.PutNextEntry(entry);
            stZipTarget.Write(buffer, 0, buffer.Length);
            stZipTarget.Finish();

            // By now we have a new stream containing the zip

            // Write it out
            stTarget.WriteTo(stZipped);
            stZipTarget.Close();

            // Rewind it before returning it
            stZipped.Seek(0, SeekOrigin.Begin);

            // TEMPCODE
            //byte[] bytBuf = new byte[stZipped.Length];
            //stZipped.Seek(0, SeekOrigin.Begin);
            //stZipped.Read(bytBuf, 0, bytBuf.Length);
            //FileStream fs = File.Create(entryName.Replace(".log", ".zip").Replace(".jpg", ".zip").Replace(".gif", ".zip").Replace(".tif", ".zip"));
            //fs.Write(bytBuf, 0, bytBuf.Length);
            //fs.Close();
            //stZipped.Seek(0, SeekOrigin.Begin);
            // TEMPCODE

            return stZipped;
        }

        // Private constant
        private const int SW_SHOWNORMAL = 1;						// Restores Window if Minimized or Maximized

        // Log file for application stuff
        static private string m_fileNameApp = "";
        static private string m_fileNameSip = "";

        // A generic file log serialisation locker
        static private ReaderWriterLockSlim m_fileLogLock = new ReaderWriterLockSlim();
    }
}
