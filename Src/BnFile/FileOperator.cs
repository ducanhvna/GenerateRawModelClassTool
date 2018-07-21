// ********************************************************************************************************************
// <summary>
// FileOperator.cs
// </summary>
// <history>
// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
// </history>
// <Copyright>
// (C) Copyright TOSHIBA Corporation 2013. All rights reserved.
// Portion (C) Copyright Microsoft Corporation.
// </Copyright>
// ********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Toshiba.BN.DataAccess.FileAccess
{
    /// ----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// FileOperator class
    /// </summary>
    /// <remarks>
    /// Provide methods to operate file
    /// </remarks>
    /// ----------------------------------------------------------------------------------------------------------------
    public class FileOperator
    {
        // =============================================================================================================
        // public method (パブリックメソッド)
        // =============================================================================================================
        #region "public method"

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
        /// </summary>
        /// <param name="sourceFileName"> (I) The file to copy </param>
        /// <param name="destFileName">   (I) The name of the destination file. This cannot be a directory </param>
        /// <param name="overwrite">      (I) true if the destination file can be overwritten; otherwise, false </param>
        /// <returns></returns>
        /// <exception> 
        /// Exception of System.IO.File.Copy();
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.File.Copy(string,string,bool)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        /// <summary>
        /// Deletes the specified file. 
        /// </summary>
        /// <param name="path"> (I) The name of the file to be deleted. Wildcard characters are not supported </param>
        /// <returns></returns>
        /// <exception> 
        /// Exception of System.IO.File.Delete(string);
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.File.Delete(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static void Delete(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path"> (I) The file to check </param>
        /// <returns></returns>
        /// <exception> </exception>
        /// <remarks>
        /// Wrapper function System.IO.File.Exists(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Returns the date and time the specified file or directory was last written to.
        /// </summary>
        /// <param name="path"> (I) The file or directory for which to obtain write date and time information </param>
        /// <returns></returns>
        /// <exception> 
        /// Exception of System.IO.File.GetLastWriteTime(string);
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.File.GetLastWriteTime(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        /// <summary>
        /// Moves a specified file to a new location.
        /// </summary>
        /// <param name="sourceFileName"> (I) The name of the file to move </param>
        /// <param name="destFileName">   (I) The new path for the file </param>
        /// <param name="overwrite">      (I) true if the destination file exist; otherwise, false </param>
        /// <returns></returns>
        /// <exception> 
        /// Exception of System.IO.File.Move(string,string) and Replace(string,string,string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.File.Move(string,string) and Replace(string,string,string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            if (overwrite == false)
            {
                // move from source -> destination, delete source file. If destination file exist, exception
                File.Move(sourceFileName, destFileName);
            }
            else
            {
                // replace content of destination file by source file, delete source file. Destination file must be exist.
                File.Replace(sourceFileName, destFileName, null);
            }
        }

        #endregion
    }
}
