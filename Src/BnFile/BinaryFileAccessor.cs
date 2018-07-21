// ********************************************************************************************************************
// <summary>
// BinaryFileAccessor.cs
// </summary>
// <history>
// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
// Ver.01.00.00.001  2013-12-03  (TSA)Yuichi.Sekiya    Update Add RandomWriteMode
// Ver.01.00.00.002  2014-04-18  (TJ)Yoshinori.Ando    Update prismy:92 ReadModeに、読取アクセスを指定
// Ver.01.00.00.003  2014-04-18  (TJ)Takayuki.Okabe    Insert prismy:121 非同期ファイルアクセス処理(ReadAsync, WriteAsync)を追加   
// Ver.01.00.00.004  2014-05-07  (TJ)Yoshinori.Ando    Update prismy:92 ReadModeに、読取用または書込用でのオープン許可を指定　
// </history>
// <Copyright>
// (C) Copyright TOSHIBA Corporation 2013-2014. All rights reserved.
// Portion (C) Copyright Microsoft Corporation.
// </Copyright>
// ********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toshiba.BN.DataAccess.FileAccess
{
    /// ----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// BinaryFileAccessor class
    /// </summary>
    /// <remarks>
    /// Provide methods to access binary file
    /// </remarks>
    /// ----------------------------------------------------------------------------------------------------------------
    public class BinaryFileAccessor
    {
        // =============================================================================================================
        // field (メンバ変数)
        // =============================================================================================================
        #region "field"

        // private -----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Reference to FileStream instance
        /// </summary>
        private FileStream m_BinaryRW;
        /// <summary>
        /// Size of binary file
        /// </summary>
        private long m_length;

        #endregion

        // =============================================================================================================
        // property (プロパティ)
        // =============================================================================================================
        #region "property"
        /// <summary>
        /// Get length of FileStream property
        /// </summary>
        public long Length
        {
            get 
            {
                if (m_BinaryRW != null) 
                {
                    m_length = m_BinaryRW.Length; 
                }
                return m_length;
            }
        }
        #endregion

        // =============================================================================================================
        // constructor (コンストラクタ)
        // =============================================================================================================
        #region "constructor"

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■BinaryFileAccessor(constructor)
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <remarks>
        /// This function do nothing, instance an object with default attributes
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public BinaryFileAccessor()
        { 
        }

        #endregion

        // =============================================================================================================
        // public method (パブリックメソッド)
        // =============================================================================================================
        #region "public method"

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Close (file close)
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <exception></exception>
        /// <remarks>
        /// Wrapper function System.IO.FileStream.Close()
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            if (m_BinaryRW != null) { m_BinaryRW.Close(); }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Open (file open)
        /// </summary>
        /// <param name="path"> (I) path file string </param>
        /// <param name="mode"> (I) file access mode </param>
        /// <returns> </returns>
        /// <exception> 
        /// Exception of System.IO.FileStream(string, FileMode)
        /// </exception>
        /// <remarks>
        /// Open exist file for reading or append writing. 
        /// If file is not exist when writing, new file is created.
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// Ver.01.00.00.001  2013-12-03  (TSA)Yuichi.Sekiya    Update Add RandomWriteMode
        /// Ver.01.00.00.002  2014-04-18  (TJ)Yoshinori.Ando    Update prismy:92 ReadModeに、読取アクセスを指定　
        /// Ver.01.00.00.004  2014-05-07  (TJ)Yoshinori.Ando    Update prismy:92 ReadModeに、読取用または書込用でのオープン許可を指定　
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public void Open(string path, FileAccessMode mode)
        {
            if (mode == FileAccessMode.ReadMode)
            {
                //m_BinaryRW = new FileStream(path, FileMode.Open);

                // 読取アクセスを指定、後続のファイルアクセスに読取用または書込用でのファイルオープンを許可　
                m_BinaryRW = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            }
            else if (mode == FileAccessMode.WriteMode)
            {
                m_BinaryRW = new FileStream(path, FileMode.Append);
            }
            else if (mode == FileAccessMode.RandomWriteMode)
            {
                m_BinaryRW = new FileStream(path, FileMode.Open, System.IO.FileAccess.Write);
            }
            else 
            { 
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Read (specified bytes from binary file)
        /// </summary>
        /// <param name="array"> (O) array to read data </param>
        /// <param name="count"> (I) number bytes request to read </param>
        /// <returns> 
        /// Return               (O) actual read bytes
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.FileStream.Read(byte[], int, int)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.FileStream.Read(array,offset,count) with offset = 0.
        /// Read count bytes to array buffer. Return actual read bytes
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public int Read(byte[] array, int count)
        {
            int actualBytes = 0;
            if (m_BinaryRW != null)
            {
                actualBytes = m_BinaryRW.Read(array, 0, count);
            }
            return actualBytes;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Seek (Sets the current position of this stream to the given value.)
        /// </summary>
        /// <param name="offset"> (I) The point relative to origin from which to begin seeking </param>
        /// <param name="origin"> (I) Specifies the beginning, the end, or the current position as a reference point for origin </param>
        /// <returns> 
        /// Return                (O) the new position in the stream
        /// </returns>
        /// <exception> 
        /// Exception of System.IO.FileStream.Seek(long, SeekOrigin)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.FileStream.Seek(long, SeekOrigin)
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public long Seek(int offset, SeekPoint origin)
        { 
            long position = 0;
            if (m_BinaryRW != null)
            {
                // mapping Toshiba.BN.DataAccess.FileAccess.SeekPoint with Sytem.IO.SeekOrigin
                SeekOrigin seekOrigin;
                switch (origin)
                { 
                    case SeekPoint.Begin:
                        seekOrigin = SeekOrigin.Begin;
                        break;
                    case SeekPoint.End:
                        seekOrigin = SeekOrigin.End;
                        break;
                    default:
                        seekOrigin = SeekOrigin.Current;
                        break;
                }
                position = m_BinaryRW.Seek(offset, seekOrigin);
            }
            return position;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Write (specified bytes to binary file)
        /// </summary>
        /// <param name="array"> (I) array to write data </param>
        /// <param name="count"> (I) number bytes request to write </param>
        /// <returns> </returns>
        /// <exception> 
        /// Exception of System.IO.FileStream.Write(byte[], int, int)
        /// </exception>
        /// <remarks>
        /// Wrapper function System.IO.FileStream.Write(array,offset,count) with offset = 0.
        /// Write count bytes from current FileStream position.
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-06-05  (TSDV)hung.trinhhong  Create
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public void Write(byte[] array, int count)
        {
            if (m_BinaryRW != null)
            {
                m_BinaryRW.Write(array, 0, count); 
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■ReadAsync(非同期読込処理)
        /// </summary>
        /// <param name="array">             (I) 読込データ</param>
        /// <param name="offset"> 　         (I) 読込開始位置 </param>
        /// <param name="count">             (I) 読込バイト数 </param>
        /// <param name="cancellationToken"> (I) キャンセル要求鵜監視トークン </param>
        /// <returns> </returns>
        /// <exception> 
        /// ArgumentNullException        bufferが null
        /// ArgumentOutOfRangeException　offset または count が負の値の場合
        /// ArgumentException          　offset と count の合計値が、バッファー長より大きい場合
        /// NotSupportedException　　　　ストリームが読み取りをサポートしていない場合
        /// ObjectDisposedException      ストリームが破棄されている場合。
        /// InvalidOperationException    ストリームが現在、前の読み取り操作で使用中の場合
        /// OperationCanceledException   キャンセル要求が発生した場合
        /// </exception>
        /// <remarks>
        /// 読込処理を非同期で実行する
        /// </remarks>
        /// <history>        
        /// Ver.01.00.00.003  2014-04-18  (TJ)Takayuki.Okabe    Create
		/// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            // FileStream.ReadAsyncを実行
            return m_BinaryRW.ReadAsync(array, offset, count, cancellationToken);
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■WriteAsync(非同期書込み処理)
        /// </summary>
        /// <param name="array">             (I) 書込データ</param>
        /// <param name="offset"> 　         (I) 書込開始位置 </param>
        /// <param name="count">             (I) 書込バイト数 </param>
        /// <param name="cancellationToken"> (I) キャンセル要求鵜監視トークン </param>
        /// <returns> </returns>
        /// <exception> 
        /// ArgumentNullException        bufferが null
        /// ArgumentOutOfRangeException　offset または count が負の値の場合
        /// ArgumentException          　offset と count の合計値が、バッファー長より大きい場合
        /// NotSupportedException　　　　ストリームが書き込みをサポートしていない場合(OPEN時のモード異常)
        /// ObjectDisposedException      ストリームが破棄されている場合。
        /// InvalidOperationException    ストリームが現在、前の書き込み操作で使用中の場合
        /// OperationCanceledException   キャンセル要求が発生した場合
        /// </exception>
        /// <remarks>
        /// 書込み処理を非同期で実行する
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.003  2014-04-18  (TJ)Takayuki.Okabe    Create
		/// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            // FileStream.WriteAsyncを実行
            return m_BinaryRW.WriteAsync(array, offset, count, cancellationToken); 
        }

        #endregion
    }

    // =================================================================================================================
    // enumeration type (列挙型)
    // =================================================================================================================
    #region "enumeration type"

    /// <summary>
    /// SeekPoint enumeration base on System.IO.SeekOrigin
    /// </summary>
    public enum SeekPoint
    {
        /// <summary>
        /// Specifies the beginning of a stream.
        /// </summary>
        Begin,
        /// <summary>
        /// Specifies the current position within a stream.
        /// </summary>
        Current,
        /// <summary>
        /// Specifies the end of a stream.
        /// </summary>
        End
    }

    #endregion
}
