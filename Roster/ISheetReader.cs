using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roster
{
    interface ISheetReader
    {
        List<string> ReadLine(int row, int cols);
    }

    interface ISheetWriter
    {
        void WriteLine(int row, int col, string data);
    }

    interface ISheetDataSource
    {
        ISheetReader CreateReader(string workSheet);
        ISheetWriter CreateWriter(string workSheet);
    }

    public class ExcelSheetDataSource : ISheetDataSource
    {
        private readonly ExcelPackage excelPackage;

        public ExcelSheetDataSource(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        ISheetReader ISheetDataSource.CreateReader(string workSheet)
        {
            return new ExcelReader(excelPackage.Workbook.Worksheets[workSheet]);
        }

        ISheetWriter ISheetDataSource.CreateWriter(string workSheet)
        {
            return new ExcelWriter(excelPackage.Workbook.Worksheets[workSheet]);
        }
    }

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
