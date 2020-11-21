using JsonGridEditor.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace JsonGridEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Page _editorPage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonOpenCsv_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.Filter = "Csv檔案(*.csv)|*.csv|其他檔案|*.*";
                dialog.FilterIndex = 1;
                dialog.Title = "開啟檔案";
                dialog.RestoreDirectory = true;
                dialog.Multiselect = false;

                var result = dialog.ShowDialog();

                var fileName1 = string.Empty;
                if (result.HasValue && result.Value)
                {
                    fileName1 = dialog.FileName;
                }

                var fileName = fileName1;

                if (string.IsNullOrEmpty(fileName) == false)
                {
                    _editorPage = new CsvEditorPage();
                    var editor = (IEditor)_editorPage;

                    await editor.LoadAsync(fileName);
                    ContentFrame.Content = _editorPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonOpenJson_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.Filter = "Json file(*.json)|*.json|Csv file(*.csv)|*.csv|other files|*.*";
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

                if (string.IsNullOrEmpty(fileName) == false)
                {
                    _editorPage = new JsonEditorPage();
                    var editor = (IEditor)_editorPage;

                    editor.LoadAsync(fileName);
                    ContentFrame.Content = _editorPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_editorPage is IEditor editor)
                {
                    await editor.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ButtonSaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_editorPage is IEditor editor)
                {
                    await editor.SaveAsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}