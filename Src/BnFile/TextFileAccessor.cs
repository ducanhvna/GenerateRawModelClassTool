// ********************************************************************************************************************
// <summary>
// TextFileAccessor.cs
// </summary>
// <history>
// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
// Ver.01.00.01.000  2013-06-12  (TSDV)hung.trinhhong  Update Add AutoFlush property
// Ver.01.00.00.001  2013-12-03  (TSA)Yuichi.Sekiya    Update Add RandomWriteMode
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
    /// TextFileAccessor class
    /// </summary>
    /// <remarks>
    /// Provide methods to access text file
    /// </remarks>
    /// ----------------------------------------------------------------------------------------------------------------
    public class TextFileAccessor
    {
        // =============================================================================================================
        // field (メンバ変数)
        // =============================================================================================================
        #region "field"

        // private -----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Indicates whether the current stream position is at the end of the stream.
        /// </summary>
        private bool m_Eof;
        /// <summary>
        /// Reference to an instance of FileStream
        /// </summary>
        private FileStream m_TextRW;
        /// <summary>
        /// Reference to an instance StreamReader
        /// </summary>
        private StreamReader m_TextReader;
        /// <summary>
        /// Reference to an instance StreamWriter
        /// </summary>
        private StreamWriter m_TextWriter;

        #endregion

        // =============================================================================================================
        // property (プロパティ)
        // =============================================================================================================
        #region "property"
        
        /// <summary>
        /// Get value indicates whether the current stream position is at the end of the stream.
        /// </summary>
        public bool EOF
        {
            get
            {
                if (m_TextReader != null)
                {
                    m_Eof = m_TextReader.EndOfStream;
                }
                return m_Eof;
            }
        }

        // Ver.01.00.01.000 2013-06-12 Insert Start Add this property
        /// <summary>
        /// AutoFlush option for StreamWriter
        /// </summary>
        public bool AutoFlush
        {
            set { m_TextWriter.AutoFlush = value; }
        }
        // Ver.01.00.01.000 2013-06-12 Insert End
        #endregion

        // =============================================================================================================
        // constructor (コンストラクタ)
        // =============================================================================================================
        #region "constructor"

        /// <summary>
        ///  TextFileAccessor() constructor
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <remarks>
        /// This function do nothing, instance an object with default attributes
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public TextFileAccessor()
        { 
        }

        #endregion

        // =============================================================================================================
        // public method (パブリックメソッド)
        // =============================================================================================================
        #region "public method"

        /// <summary>
        /// Close file.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <exception></exception>
        /// <remarks>
        /// Close the FileStream, StreamReader and StreamWriter objects and 
        /// release any system resource associated with.
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public void Close()
        {
            if (m_TextReader != null) { m_TextReader.Close(); }
            if (m_TextWriter != null) { m_TextWriter.Close(); }
            if (m_TextRW != null) { m_TextRW.Close(); }
        }

        /// <summary>
        /// Clear StreamWriter buffer.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <exception>
        /// Exception of System.IO.StreamWriter.Flush()
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.StreamWriter.Flush(). Clear all buffer of current writer.
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public void Flush()
        {
            if (m_TextWriter != null) { m_TextWriter.Flush(); }
        }

        /// <summary>
        /// Open file.
        /// </summary>
        /// <param name="path"> (I) path file name string </param>
        /// <param name="mode"> (I) file access mode </param>
        /// <returns></returns>
        /// <exception>
        /// Exception of System.IO.FileStream(string, FileMode)
        /// Exception of System.IO.StreamReader(FileStream)
        /// Exception of System.IO.StreamWriter(FileStream)
        /// </exception>
        /// <remarks>
        /// Instance FileStream
        /// Instance StreamReader for reading if mode = ReadMode
        /// Instance StreamWriter for writing if mode = WriteMode
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// Ver.01.00.00.001  2013-06-17  (TSDV)binh.nguyenthanh  Modify to encoding Japanese in ".csv" file
        /// </history>
        public void Open(string path, FileAccessMode mode)
        {
            if (mode == FileAccessMode.ReadMode)
            {
                // Open exist file
                m_TextRW = new FileStream(path, FileMode.Open);
                m_TextReader = new StreamReader(m_TextRW, new UTF8Encoding(true, true));
            }
            else if (mode == FileAccessMode.WriteMode)
            {
                // Open exist file to write append. If file not exist, create new
                m_TextRW = new FileStream(path, FileMode.Append);
                m_TextWriter = new StreamWriter(m_TextRW, new UTF8Encoding(true, true));
            }
            else
            { 
            }
        }

        /// <summary>
        /// Write string into end of text file.
        /// </summary>
        /// <param name="value"> (I) input string </param>
        /// <returns></returns>
        /// <exception>
        /// Exception of System.IO.StreamWrite.WriteLine(string)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.StreamWriter.WriteLine(). 
        /// Writes a string followed by a line terminator to the text string or stream. 
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public void WriteLine(string value)
        {
            if (m_TextWriter != null) { m_TextWriter.WriteLine(value); }
        }

        /// <summary>
        /// Read a line at current from text file.
        /// </summary>
        /// <param></param>
        /// <returns>
        /// /// return  (O) read string
        /// </returns>
        /// <exception>
        /// Exception of System.IO.StreamReader.ReadLine()
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.StreamReader.ReadLine(). 
        /// Reads a line of characters from the current stream and returns the data as a string. 
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        public string ReadLine()
        {
            if (m_TextReader != null) { return m_TextReader.ReadLine(); }
            else return null;
        }

        #endregion
    }

    // =============================================================================================================
    // enumeration type (列挙型)
    // =============================================================================================================
    #region "enumeration type"

    /// <summary>
    /// File access mode enumeration
    /// </summary>
    /// Ver.01.00.00.001  2013-12-03  (TSA)Yuichi.Sekiya    Update Add RandomWriteMode
    public enum FileAccessMode
    {
        /// <summary>
        /// Read access to file. Data can be read from the file.
        /// </summary>
        ReadMode,
        /// <summary>
        /// Write access to the file. Data can be written to the file.
        /// </summary>
        WriteMode,
        /// <summary>
        /// Write access to the file. Data can be random written to the file.
        /// This mode is BinaryAccess only.
        /// </summary>
        RandomWriteMode
    }

    #endregion
}
