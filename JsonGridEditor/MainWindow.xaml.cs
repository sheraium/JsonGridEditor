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

        private void ButtonOpenCsv_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = FileDialog.GetOpenFileName();

                if (string.IsNullOrEmpty(fileName) == false)
                {
                    _editorPage = new CsvEditorPage();
                    var editor = (IEditor)_editorPage;

                    editor.LoadFile(fileName);
                    ContentFrame.Content = _editorPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_editorPage is IEditor editor)
                {
                    editor.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonSaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_editorPage is IEditor editor)
                {
                    editor.SaveAs();
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

        private void ButtonOpenJson_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = FileDialog.GetOpenFileName();

                if (string.IsNullOrEmpty(fileName) == false)
                {
                    _editorPage = new JsonEditorPage();
                    var editor = (IEditor)_editorPage;

                    editor.LoadFile(fileName);
                    ContentFrame.Content = _editorPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}