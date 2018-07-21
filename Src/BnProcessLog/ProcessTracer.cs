// ********************************************************************************************************************
// <summary>
// ProcessTracer.cs
// </summary>
// <history>
// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
// Ver.01.01.00.002  2013-06-12  (TSDV)hung.trinhhong  Update after change specification
// </history>
// <Copyright>
// (C) Copyright TOSHIBA Corporation 2013. All rights reserved.
// Portion (C) Copyright Microsoft Corporation.
// </Copyright>
// ********************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Toshiba.BN.DataAccess.FileAccess;

namespace Toshiba.BN.SystemBase.Log
{
    /// ----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// ProcessTracer class
    /// </summary>
    /// <remarks>
    /// Provide methods to write/delete log file
    /// </remarks>
    /// ----------------------------------------------------------------------------------------------------------------
    public class ProcessTracer
    {
        // =============================================================================================================
        // field (メンバ変数)
        // =============================================================================================================
        #region "field"

        private string m_FileName = "";

        private DateTime m_FileOpenningDate;

        private int m_KeepDays = 5;

        TextFileAccessor m_LogFile = null;
        
        private Level m_LogLevel = Level.NotWrite;
        
        private int m_MaxLength = 1024 * 4;

        private string m_SavePath = @".\";
        
        private Semaphore m_Semaphore = null;

        private Semaphore m_SemaphoreForDelete = null;
        
        private ExceptionHandling m_ExceptionHandling = ExceptionHandling.NotThrow;
                
        #endregion

        // =============================================================================================================
        // constructor (コンストラクタ)
        // =============================================================================================================
        #region "constructor"

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■ProcessTracer (constructor)
        /// </summary>
        /// <param name="savePath">         (I) folder name to save log file </param>
        /// <param name="fileName">         (I) log file name </param>
        /// <param name="logLevel">         (I) log level setting </param>
        /// <param name="maxLength">        (I) maximum length of string for each writting </param>
        /// <param name="keepDays">         (I) maximum retain log files when delete </param>
        /// <param name="exceptionHandling">(I) throw exception handling option </param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
        /// Ver.01.01.00.000  2013-06-07  (TSDV)hung.trinhhong  Update after new specification
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public ProcessTracer(string savePath, 
                             string fileName, 
                             Level logLevel,
                             int maxLength, 
                             int keepDays, 
                             ExceptionHandling exceptionHandling)
        {
            if (savePath == null)
            {
                // set default value
                m_SavePath = @".\";
            } 
            else
            {
                m_SavePath = savePath; 
            }

            if (fileName == null) 
            {
                // set default log file name
                m_FileName = "LogFile.txt";
            } 
            else 
            {
                m_FileName = fileName;
            }

            m_ExceptionHandling = exceptionHandling;
            m_LogLevel = logLevel;

            if (maxLength > 0)
            {
                m_MaxLength = maxLength;
            }
            else
            {
                // set default value 1024 * 4
            } 

            if (keepDays >= 0) 
            {
                m_KeepDays = keepDays;
            }
            else 
            {
                // set default value = 5
            } 
        }

        #endregion
                
        // =============================================================================================================
        // public method (パブリックメソッド)
        // =============================================================================================================
        #region "public method"

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■DeleteBackupOverFile (Delete log files and retain specified number of log files)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
        /// Ver.01.01.00.000  2013-06-07  (TSDV)hung.trinhhong  Update Specification change
        /// Ver.01.01.00.001  2013-06-11  (TSDV)hung.trinhhong  Update Bug when create system semaphore
        /// Ver.01.01.00.002  2013-06-12  (TSDV)hung.trinhhong  Update Confirm rule for naming semaphore
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public void DeleteBackupOverFile()
        {
            // Create semaphore for delete
           string semaphoreForDeleteString = "SemaphoreForDelete";
            
            try
            {
                // Try to reference to exist system semaphore
                m_SemaphoreForDelete = Semaphore.OpenExisting(semaphoreForDeleteString);
            }
            catch
            {
                // Create new semaphore
                m_SemaphoreForDelete = CreateSemaphore(semaphoreForDeleteString);
            }

            try
            {
                m_SemaphoreForDelete.WaitOne();

                // Get subfolder list in m_savePath
                string[] allSubFolders = DirectoryOperator.GetDirectories(m_SavePath);
                if (allSubFolders.Length > m_KeepDays)
                {
                    int numberDeleteFolders = allSubFolders.Length - m_KeepDays;
                    for (int topIndex = 0; topIndex < numberDeleteFolders; topIndex++)
                    {
                        DirectoryOperator.Delete(allSubFolders[topIndex], true);
                    }
                }
            }
            catch
            {
                if (m_ExceptionHandling == ExceptionHandling.Throw)
                {
                    throw; 
                }
                else 
                {
                    // skip exception
                } 
            }
            finally
            {
                m_SemaphoreForDelete.Release();
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■WriteLine (Write log string into log file)
        /// </summary>
        /// <param name="LogAssorment"> (I) log assorment </param>
        /// <param name="LogLevel">     (I) log level </param>
        /// <param name="LogString">    (I) log string </param>
        /// <exception> 
        /// Exception cause by System.IO.StreamWrite.WriteLine() method
        /// </exception>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
        /// Ver.01.01.00.000  2013-06-07  (TSDV)hung.trinhhong  Update Specification changed
        /// Ver.01.01.00.001  2013-06-10  (TSDV)hung.trinhhong  Fix bug when create semaphore 
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public void WriteLine(Assortment LogAssorment, Level LogLevel, string LogString)
        {
            if (LogLevel == Level.NotWrite)
            {
                return;
            }

            if (LogLevel > m_LogLevel)
            {
                return;
            }

            // Check length of LogString 
            string CheckedLogString;
            if (LogString.Length > m_MaxLength)
            {
                CheckedLogString = LogString.Substring(0, m_MaxLength);
            }
            else
            {
                CheckedLogString = LogString;
            }

            string date = DateTime.Now.ToString("yyyy/MM/dd,HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            // Create message log as pre-defined format 
            // <date>,<time>,<process name>,<level>,<assort>,<contents>
            string ALogMessage = date + "," + ((int)LogLevel).ToString() + "," + ((int)LogAssorment).ToString() + "," + CheckedLogString;
            
            string semaphoreForWriteString = "semaphoreForWrite_" + m_FileName;
            
            try
            {
                // Try to reference to exist Semaphore with name semaphoreForWriteString
                m_Semaphore = Semaphore.OpenExisting(semaphoreForWriteString);
            }
            catch
            {
                // Not exist, create new semaphore
                m_Semaphore = CreateSemaphore(semaphoreForWriteString);
            }
            
            try
            {
                // Request semaphore, block until log file is free 
                m_Semaphore.WaitOne();

                Open();
                m_LogFile.WriteLine(ALogMessage);
            }
            catch
            {
                Close();

                if (m_ExceptionHandling == ExceptionHandling.Throw)
                {
                    throw;
                }
                else
                {
                    // skip exception
                } 
            }
            finally
            {
                // Release semaphore, other process/thread can write log file 
                m_Semaphore.Release();
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Close (Close the log file, release file stream and streamwriter)
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <exception> 
        /// Exception cause by System.IO.File.Close() method
        /// </exception>
        /// <remarks>
        /// Use Close() and Flush() method of TextFileAccessor class
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
        /// Ver.01.01.00.000  2013-06-11  (TSDV)hung.trinhhong  Update Specification change
        /// Ver.01.01.00.001  2013-12-11  (TSA)Yuichi.Sekiya    Update Move a method in public
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            // Check file whether is opening
            if (m_LogFile == null)
            {
                // file is not opening
            } 
            else
            {
                try
                {
                    m_LogFile.Flush();
                    // Close FileStream, StreamWriter and release related resource
                    m_LogFile.Close(); 
                }
                catch
                {
                    if (m_ExceptionHandling == ExceptionHandling.Throw) 
                    {
                        throw; 
                    }
                    else 
                    {
                        // skip exception
                    }  
                }
            }
        }

        #endregion

        // =============================================================================================================
        // private method (プライベートメソッド)
        // =============================================================================================================
        #region "private method"

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■Open (Open log file. 1 log file per date)
        /// </summary>
        /// <exception> 
        /// </exception>
        /// <remarks>
        /// If file is not opening, m_LogFile is null -> open new log file
        /// Other hand, 
        /// if current date same m_FileOpenningDate, do nothing. 
        /// if current date different with m_FileOpenningDate, close opening log file and new log file should be created 
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
        /// Ver.01.01.00.000  2013-06-07  (TSDV)hung.trinhhong  Update  Change specification
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        private void Open()
        {
            //Get current datetime
            string currentDate = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            string filePath = m_SavePath + @"\" + currentDate + @"\" + m_FileName;

            // file is not opening
            if (m_LogFile == null) 
            {
                m_LogFile = new TextFileAccessor();

                try
                {
                    // Confirm destination subfolder
                    string subfolderPath = m_SavePath + @"\" + currentDate;
                    if (!DirectoryOperator.Exists(subfolderPath))
                    {
                        // Create folder(m_savePath)/subfolder if not exist
                        DirectoryOperator.CreateDirectory(subfolderPath);
                    }
                    
                    m_LogFile.Open(filePath, FileAccessMode.WriteMode);

                    m_LogFile.AutoFlush = true;
                    
                    m_FileOpenningDate = DateTime.Now;
                }
                catch
                {
                    if (m_ExceptionHandling == ExceptionHandling.Throw)
                    {
                        throw; 
                    }
                    else
                    {
                        // skip exception
                    } 
                }
            }
            else
            { 
                // Compare current datetime with datetime of openning log file (1 log file per day)
                string dateOfOpenningFile = m_FileOpenningDate.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                if (dateOfOpenningFile.CompareTo(currentDate) == 0) 
                {
                    // do nothing
                } 
                else
                { 
                    // Firstly, close log openning log file
                    Close();
                    m_LogFile = null;

                    // Open log file recursive
                    this.Open();
                }
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ■CreateSemaphore (Create an instance system semaphore)
        /// </summary>
        /// <param name="SemaphoreName"> (I) named semaphore </param>
        /// <returns>
        /// Refer to instance of system semaphore
        /// </returns>
        /// <exception> 
        /// Exception occur by System.Threading.Semaphore.Semaphore(int,int,string) constructor
        /// </exception>
        /// <remarks>
        /// Instance system semaphore by using constructor Semaphore().
        /// Try it in 3 times if can not create new semaphore
        /// </remarks>
        /// <history>
        /// Ver.01.00.00.000  2013-04-11  (TSDV)hung.trinhhong  Create
        /// Ver.01.01.00.000  2013-06-07  (TSDV)hung.trinhhong  Update as new specification
        /// </history>
        /// ------------------------------------------------------------------------------------------------------------
        private Semaphore CreateSemaphore(string SemaphoreName)
        {
            Semaphore semaphore = null;

            // try again if create semaphore error
            int tryBackNumber = 3; 

            // Try to create semaphore instance in 'tryBackNumber' times
            while (tryBackNumber > 0)
            {
                try
                {
                    semaphore = new Semaphore(1, 1, SemaphoreName);
                    // create semaphore successfully, finish
                    break; 
                }
                catch
                {
                    tryBackNumber--;
                    if (tryBackNumber == 0) // throw exception if can not create semaphore in 'tryBackNumber' time
                    {
                        if (m_ExceptionHandling == ExceptionHandling.Throw)
                        {
                            throw;
                        }
                        else
                        {
                            // skip exception
                        } 
                    }
                }
            }

            return semaphore;
        }

        #endregion

        //=============================================================================================================
        // For unit test only
        //=============================================================================================================
        #region "Unit test"
#if DEBUG
        /// <summary>
        /// test property
        /// </summary>
        public void _Open()
        {
            Open();
        }

        /// <summary>
        /// test property
        /// </summary>
        public void _Close()
        {
            Close();
        }

        #region Properties to access private attributes

        /// <summary>
        /// test property
        /// </summary>
        public int KeepDays
        {
            get { return m_KeepDays; }
            set { m_KeepDays = value; }
        }

        /// <summary>
        /// test property
        /// </summary>
        public Level LogLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; }
        }

        /// <summary>
        /// test property
        /// </summary>
        public int MaxLength
        {
            get { return m_MaxLength; }
            set { m_MaxLength = value; }
        }

        /// <summary>
        /// test property
        /// </summary>
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        /// <summary>
        /// test property
        /// </summary>
        public Semaphore Semaphore
        {
            get {return m_Semaphore;}
            set {m_Semaphore = value;}
        }

        /// <summary>
        /// test property
        /// </summary>
        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        #endregion
#endif
        #endregion
    }

    // =============================================================================================================
    // enumeration type (列挙型)
    // =============================================================================================================
    #region "enumeration type"

    /// <summary>
    /// Log level
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// Do not write log message to log file 
        /// </summary>
        NotWrite = 0,
        /// <summary>
        /// The information of error and important  event, must write in log file
        /// </summary>
        ErrorOrImportant,
        /// <summary>
        /// Start point of process, end point of process, start point of thread, end point of thread, 
        /// state transition, start point of important method, end point important method
        /// </summary>
        ImportantInOut,
        /// <summary>
        /// Point of "if" or "switch", start point of method, end point of method
        /// </summary>
        AttentionInOut,
        /// <summary>
        /// The information of parameter
        /// </summary>
        Parameter,
        /// <summary>
        /// The information for performance. It is unnecessary at the time of the customer use
        /// </summary>
        Performance,
        /// <summary>
        /// The information for debug. It is unnecessary at the time of the customer use
        /// </summary>
        Debug
    };

    /// <summary>
    /// Log assorment
    /// </summary>
    public enum Assortment
    {
        /// <summary>
        /// None Assortment
        /// </summary>
        None = 0,
        /// <summary>
        /// The information of user operation
        /// </summary>
        OperationLog,
        /// <summary>
        /// The information of communication
        /// </summary>
        CommunicationLog,
        /// <summary>
        /// The information of access to database
        /// </summary>
        DBAccessLog,
        /// <summary>
        /// The information of database recovery
        /// </summary>
        DBRecoveryLog,
        /// <summary>
        /// The information of error
        /// </summary>
        ErrorLog,
        /// <summary>
        /// Application Log
        /// </summary>
        ApplicationLog,
        /// <summary>
        /// The information of report process
        /// </summary>
        ReportLog
    }

    /// <summary>
    /// Throw exception option
    /// </summary>
    public enum ExceptionHandling
    {
        /// <summary>
        /// Skip exception
        /// </summary>
        NotThrow = 0,
        /// <summary>
        /// Throw exception
        /// </summary>
        Throw
    }
    
    #endregion
} 
