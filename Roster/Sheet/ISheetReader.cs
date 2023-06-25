namespace Roster.Sheet
{
    interface ISheetReader
    {
        List<string> ReadLine(int row, int cols);
    }
}
