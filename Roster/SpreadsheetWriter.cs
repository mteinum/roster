﻿using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Roster
{
    class SpreadsheetWriter
    {
        public static void WriteBackToSheet(List<Duty> duties, SheetsService service)
        {
            var values = new List<ValueRange>();

            static ValueRange CreateValueRange(int row, string column, string content)
            {
                var oblist = new List<object>() { content };

                return new ValueRange
                {
                    MajorDimension = "COLUMNS",
                    Range = $"Aktiviteter!{column}{row}",
                    Values = new List<IList<object>> { oblist }
                };
            }

            foreach (var duty in duties)
            {
                values.Add(CreateValueRange(duty.Row, "H", duty.Person?.Name ?? String.Empty));
                values.Add(CreateValueRange(duty.Row, "I", duty.Person?.Mobile ?? String.Empty));

                values.Add(CreateValueRange(duty.Row, "J", duty.Person2?.Name ?? String.Empty));
                values.Add(CreateValueRange(duty.Row, "K", duty.Person2?.Mobile ?? String.Empty));
            }

            BatchUpdateValuesRequest body = new()
            {
                Data = values,
                ValueInputOption = "RAW"
            };

            var update2 = service.Spreadsheets.Values.BatchUpdate(
                body, Config.SpreadsheetId);

            var response = update2.Execute();

        }
    }
}