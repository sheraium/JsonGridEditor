using System.Threading.Tasks;

namespace JsonGridEditor.Views
{
    public interface IEditor
    {
        Task LoadAsync(string fileName);

        Task SaveAsAsync();

        Task SaveAsync();
    }
}