using JsonGridEditor.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using FileDialog = JsonGridEditor.Extensions.FileDialog;

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
                var fileName = FileDialog.GetOpenFileName();

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
                var fileName = FileDialog.GetOpenFileName();

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