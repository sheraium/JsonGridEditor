namespace JsonGridEditor.Views
{
    public interface IEditor
    {
        void LoadFile(string fileName);

        void Save();

        void SaveAs();
    }
}