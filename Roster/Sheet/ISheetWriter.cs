namespace Roster.Sheet
{
    interface ISheetWriter
    {
        void WriteLine(int row, int col, string data);
    }

}
