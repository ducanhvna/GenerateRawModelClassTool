// ********************************************************************************************************************
// <summary>
// PathEditor.cs
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
    /// PathEditor class
    /// </summary>
    /// <remarks>
    /// Provide methods to get/set path file name
    /// </remarks>
    /// ----------------------------------------------------------------------------------------------------------------
    public class PathEditor
    {
        // =============================================================================================================
        // public method (パブリックメソッド)
        // =============================================================================================================
        #region "public method"

        /// <summary>
        /// Changes the extension of a path string.
        /// </summary>
        /// <param name="path">      (I) The path information to modify </param>
        /// <param name="extension"> (I) The new extension.Specify null to remove an existing extension from path </param>
        /// <returns>
        /// return                   (O) The modified path information.
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.Path.ChangeExtension(string,string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.Path.ChangeExtension(string,string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static string ChangeExtension(string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }

        /// <summary>
        /// Combines two strings into a path.
        /// </summary>
        /// <param name="path1"> (I) The first path to combine  </param>
        /// <param name="path2"> (I) The second path to combine </param>
        /// <returns>
        /// return                   (O) The combined paths.
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.Path.Combine(string,string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.Path.Combine(string,string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// Returns the directory information for the specified path string.
        /// </summary>
        /// <param name="path">     (I) The path of a file or directory </param>
        /// <returns>
        /// return                  (O) Directory information for path.
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.Path.GetDirectoryName(string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.Path.GetDirectoryName(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Returns the extension of the specified path string.
        /// </summary>
        /// <param name="path"> (I) The path string from which to get the extension </param>
        /// <returns>
        /// return              (O) The extension of the specified path (including the period "."), or null, or String.Empty
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.Path.GetExtension(string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.Path.GetExtension(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// Returns the file name and extension of the specified path string.
        /// </summary>
        /// <param name="path">     (I) The path string from which to obtain the file name and extension.</param>
        /// <returns>
        /// return                  (O) The characters after the last directory character in path.
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.Path.GetFileName(string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.Path.GetFileName(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Returns the file name of the specified path string without the extension.
        /// </summary>
        /// <param name="path"> (I) The path of the file </param>
        /// <returns>
        /// return    (O) The string returned by GetFileName, minus the last period (.) and all characters following it.
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.Path.GetFileNameWithoutExtension(string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.Path.GetFileNameWithoutExtension(string)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        #endregion
    }
}
