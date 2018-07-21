using System.Collections.Generic;

namespace BnExcel
{
    public class ExcelPoint
    {
        public string SheetName { get; set; }
        public List<string> ListCells { get; set; }
    }
    public class ExLocation
    {
        public ExLocation() {
            this.Row = 0;
            this.Column = 0;
       }
        public ExLocation(uint row, uint col)
        {
            this.Row = row;
            this.Column = col;
        }

        public uint Row { get; set; }
        public uint Column { get; set; }
    }
}