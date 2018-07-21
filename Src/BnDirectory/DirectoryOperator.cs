// ********************************************************************************************************************
// <summary>
// DirectoryOperator.cs
// </summary>
// <history>
// Ver.01.00.00.000  2013-06-03  (TSDV)binh.nguyenthanh  Create
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
    /// Directory operator class
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// ----------------------------------------------------------------------------------------------------------------
    public class DirectoryOperator
    {
        #region "public method"
        /// <summary>
        /// Creates all directories and subdirectories in the input path
        /// </summary>
        /// <param name="path">   (I)   Directory path</param>
        /// <history>
        /// Ver.01.00.00.000  2013-06-03  (TSDV)binh.nguyenthanh  Create
        /// </history>
        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);

        }

        /// <summary>
        /// Deletes the specified directory and, if indicated, any subdirectories and files in the directory
        /// </summary>
        /// <param name="path">
        ///   (I)   Directory path</param>
        /// <param name="recuresive">
        ///   (I)   True to remove directories, subdirectories, and files in paths; otherwise, false
        /// </param>
        /// <history>
        /// Ver.01.00.00.000  2013-06-03  (TSDV)binh.nguyenthanh  Create
        /// </history>
        public static void Delete(string path, bool recuresive)
        {
            Directory.Delete(path, recuresive);
        }

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path">   (I)   Directory path</param>
        /// <returns>   (O)   Return true if path refers to an existing directory; otherwise return false</returns>
        public static bool Exists(string path)
        {
            return Directory.Exists(path);

        }

        /// <summary>
        /// Gets the names of subdirectories (including their paths) in the specified directory
        /// </summary>
        /// <param name="path">   (I)   Directory path</param>
        /// <returns>   (O)   Return array of the full names (including paths) of subdirectories in the specified path</returns>
        public static string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }


        /// <summary>
        /// Returns the names of files (including their paths) in the specified directory
        /// </summary>
        /// <param name="path">   (I)   Directory path</param>
        /// <returns>   (O)   Return array of the full names (including paths) for the files in the specified directory</returns>
        public static string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
        #endregion
    }
}
