
using Roster.Sheet;

namespace Roster
{
    class SpreadsheetWriter
    {
        public static void WriteBackToSheet(List<Duty> duties, ISheetDataSource service)
        {
            var writer = service.CreateWriter("Aktiviteter");

            foreach (var duty in duties)
            {
                int row = duty.Row - 1;

                writer.WriteLine(row, 7, duty.Person?.Name ?? String.Empty);
                writer.WriteLine(row, 8, duty.Person?.Mobile ?? String.Empty);
                writer.WriteLine(row, 9, duty.Person2?.Name ?? String.Empty);
                writer.WriteLine(row, 10, duty.Person2?.Mobile ?? String.Empty);
            }
        }
    }
}