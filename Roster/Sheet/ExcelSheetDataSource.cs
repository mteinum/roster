using OfficeOpenXml;

namespace Roster.Sheet
{
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

}
