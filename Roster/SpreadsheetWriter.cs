﻿using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Roster
{
    class SpreadsheetWriter
    {
        public static void WriteBackToSheet(List<Duty> duties, SheetsService service, string spreadsheetId)
        {
            var values = new List<ValueRange>();

            static ValueRange CreateValueRange(int row, string column, string content)
            {
                var oblist = new List<object>() { content };

                var valueRange = new ValueRange
                {
                    MajorDimension = "COLUMNS",
                    Range = $"Høst 2022!{column}{row}",
                    Values = new List<IList<object>> { oblist }
                };

                return valueRange;
            }

            foreach (var duty in duties)
            {
                values.Add(CreateValueRange(duty.Row, "H", duty.Person.Name));

                if (duty.Person2 != null)
                {
                    values.Add(CreateValueRange(duty.Row, "J", duty.Person2.Name));
                }

            }

            BatchUpdateValuesRequest body = new()
            {
                Data = values,
                ValueInputOption = "RAW"
            };


            SpreadsheetsResource.ValuesResource.BatchUpdateRequest update2 = service.Spreadsheets.Values.BatchUpdate(
                body, spreadsheetId);

            var response = update2.Execute();

        }
    }
}