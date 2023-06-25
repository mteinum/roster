namespace Roster.Sheet
{
    interface ISheetDataSource
    {
        ISheetReader CreateReader(string workSheet);
        ISheetWriter CreateWriter(string workSheet);
    }

}
