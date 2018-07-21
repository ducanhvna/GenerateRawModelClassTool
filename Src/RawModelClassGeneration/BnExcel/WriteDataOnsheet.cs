using System.Collections.Generic;

namespace BnExcel
{
    public class WriteDataOnsheet
    {
        public string SheetName { get; set; }
        public List<WriteData> CellsData { get; set; }
        public WriteDataOnsheet()
        {
            CellsData = new List<WriteData>();
        }
    }
}