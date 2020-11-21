using System;
using Microsoft.Win32;

namespace JsonGridEditor.Extensions
{
    public static class FileDialog
    {
        public static string GetOpenFileName()
        {
            var dialog = new OpenFileDialog();
            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.Filter = "Csv檔案(*.csv)|*.csv|其他檔案|*.*";
            dialog.FilterIndex = 1;
            dialog.Title = "開啟檔案";
            dialog.RestoreDirectory = true;
            dialog.Multiselect = false;

            var result = dialog.ShowDialog();

            var fileName = string.Empty;
            if (result.HasValue && result.Value)
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }

        public static string GetSaveFileName()
        {
            var fileName = string.Empty;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Csv檔案(*.csv)|*.csv";
            //saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.FileName = $"{DateTime.Now.Date:yyyyMMdd}";
            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                fileName = saveFileDialog.FileName;
            }

            return fileName;
        }
    }
}