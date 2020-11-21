using JsonGridEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JsonGridEditor.Views
{
    /// <summary>
    /// Interaction logic for JsonEditorPage.xaml
    /// </summary>
    public partial class JsonEditorPage : Page, IEditor
    {
        private string _fileName;

        public JsonEditorPage()
        {
            InitializeComponent();
        }

        public async Task LoadAsync(string fileName)
        {
            try
            {
                _fileName = fileName;
                var dataTable = new DataTable();
                var tableColumn = new List<string>();

                using var reader = new StreamReader(fileName);
                var head = await reader.ReadLineAsync();
                tableColumn.AddRange(head.Split(','));
                foreach (var s in tableColumn)
                {
                    dataTable.Columns.Add(s);
                }

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',').ToList();

                    var dataRow = dataTable.NewRow();
                    var columnEnumerator = tableColumn.GetEnumerator();
                    var valueEnumerator = values.GetEnumerator();
                    while (columnEnumerator.MoveNext() && valueEnumerator.MoveNext())
                    {
                        var column = columnEnumerator.Current;
                        var value = valueEnumerator.Current;

                        dataRow[column] = value;
                    }
                    dataTable.Rows.Add(dataRow);
                }

                DataGrid1.ItemsSource = dataTable.AsDataView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Task SaveAsAsync()
        {
            try
            {
                var fileName = FileDialog.GetSaveFileName();
                if (string.IsNullOrEmpty(fileName) == false)
                {
                    _fileName = fileName;
                    return SaveAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            try
            {
                var dataView = DataGrid1.ItemsSource as DataView;
                var dataTable = dataView?.ToTable();
                if (_fileName == null || dataTable == null) return;

                using (var stream = new StreamWriter(_fileName))
                {
                    var columns = new List<string>();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        var caption = column.Caption;
                        columns.Add(caption);
                    }

                    await stream.WriteLineAsync(string.Join(",", columns));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var values = new List<string>();
                        foreach (var column in columns)
                        {
                            values.Add(row[column].ToString());
                        }
                        await stream.WriteLineAsync(string.Join(",", values));
                        values.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void JsonEditorPage_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}