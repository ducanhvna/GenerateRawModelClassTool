using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace BnExcel
{
    public class MyExcel
    {
        public static string DB_PATH = @"";
        private static Microsoft.Office.Interop.Excel.Workbook MyBook = null;
        private static Excel.Application MyApp = null;
        private static Excel.Worksheet MySheet = null;
        private static int lastRow = 0;
        public static void InitializeExcel()
        {
            MyApp = new Excel.Application();
            MyApp.Visible = true;
            MyBook = MyApp.Workbooks.Open(DB_PATH);
            MySheet = (Excel.Worksheet)MyBook.Sheets[1]; // Explict cast is not required here
            lastRow = MySheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
        }
        public static List<string> ReadMyExcel(ExcelPoint postion)
        {
            MySheet = (Excel.Worksheet)MyBook.Sheets[postion.SheetName];
            List<string> result = new List<string>();
            foreach (var range in postion.ListCells)
            {
                string MyValue = (string)MySheet.get_Range(range).Cells.Value;
                result.Add(MyValue);
            }

            return result;
        }
        public static List<string> ReadMyExcel(Excel.Worksheet MySheet, ExcelPoint postion)
        {
            List<string> result = new List<string>();
            foreach (var range in postion.ListCells)
            {
                string MyValue = (string)MySheet.get_Range(range).Cells.Value;
                result.Add(MyValue);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeData"></param>
        public static void ReadMyExcel(WriteDataOnsheet writeData)
        {
            var xlsSheet = (Excel.Worksheet)MyBook.Sheets[writeData.SheetName];
            foreach (var cell in writeData.CellsData)
            {
                cell.data = xlsSheet.Cells[cell.row, cell.column].Value;
            }
            MyBook.Save();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="ListColumnToRead"></param>
        /// <returns></returns>
        public static List<List<string>> ReadDataByColum(string sheetName,int startRow, int startColumn, List<int> ListColumnToRead)
        {
            var xlsSheet = (Excel.Worksheet)MyBook.Sheets[sheetName];
            List<List<string>> result = new List<List<string>>();
            int row = startRow;
            while(row<10000&&xlsSheet.Cells[row, startColumn].Value !=null&&(string)xlsSheet.Cells[row, startColumn].Value != "")
            {
                var resultItem = new List<string>();
                foreach(var column in ListColumnToRead)
                {
                    resultItem.Add((string)xlsSheet.Cells[row, column].Value);
                }
                result.Add(resultItem);
                row++;
            }
            return result;
        }
        /// <summary>
        /// WriteToExcel
        /// </summary>
        /// <param name="writeData"></param>
        public static void WriteToExcel(WriteDataOnsheet writeData)
        {
            var xlsSheet = (Excel.Worksheet)MyBook.Sheets[writeData.SheetName];
            foreach (var cell in writeData.CellsData)
            {
                xlsSheet.Cells[cell.row, cell.column] = cell.data;
            }
            MyBook.Save();
        }
        /// <summary>
        /// CloseExcel
        /// </summary>
        public static void CloseExcel()
        {
            MyBook.Saved = true;
            MyApp.Quit();
        }

    }
}
