using OfficeOpenXml;

namespace Roster.Sheet
{
    class ExcelWriter : ISheetWriter
    {
        private readonly ExcelWorksheet worksheet;

        public ExcelWriter(ExcelWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public void WriteLine(int row, int col, string data)
        {
            worksheet.Cells.SetCellValue(row, col, data);
        }
    }

}
