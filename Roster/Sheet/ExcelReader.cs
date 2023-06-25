using OfficeOpenXml;

namespace Roster.Sheet
{
    public class ExcelReader : ISheetReader
    {
        private readonly ExcelWorksheet worksheet;


        public ExcelReader(ExcelWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public List<string> ReadLine(int row, int cols)
        {
            var range = worksheet.Cells[row, 1, row, cols];

            return Enumerable.Range(0, cols)
                    .Select(n => range.GetCellValue<string>(n))
                    .ToList();
        }
    }

}
