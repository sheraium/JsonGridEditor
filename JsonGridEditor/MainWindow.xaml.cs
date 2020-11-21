using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows;

namespace JsonGridEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _fileName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
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
                if (result.HasValue && result.Value)
                {
                    _fileName = dialog.FileName;
                    var dataTable = new DataTable();
                    var tableColumn = new List<string>();

                    using var reader = new StreamReader(_fileName);
                    var head = reader.ReadLine();
                    tableColumn.AddRange(head.Split(','));
                    foreach (var s in tableColumn)
                    {
                        dataTable.Columns.Add(s);
                    }

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
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
                if (_fileName == null)
                {
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
                        _fileName = saveFileDialog.FileName;
                    }
                }

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

                    stream.WriteLine(string.Join(",", columns));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var values = new List<string>();
                        foreach (var column in columns)
                        {
                            values.Add(row[column].ToString());
                        }
                        stream.WriteLine(string.Join(",", values));
                        values.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            return;
            //_dtos = new ObservableCollection<object>();
            //_dtos.Add(new Dto() { Value = 1, Data = "1" });
            //_dtos.Add(new Dto() { Value = 2, Data = "2" });
            //_dtos.Add(new Dto() { Value = 3, Data = "3" });
            //DataGrid1.ItemsSource = _dtos;

            var table = new List<Dictionary<string, string>>();
            var tableColumn = new List<string>();

            using (var reader = new StreamReader("param.csv"))
            {
                var head = reader.ReadLine();
                tableColumn.AddRange(head.Split(','));

                var line = reader.ReadLine();
                var values = line.Split(',').ToList();

                var headEnumerator = tableColumn.GetEnumerator();
                var valueEnumerator = values.GetEnumerator();

                var row = new Dictionary<string, string>();
                while (headEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    var column = headEnumerator.Current;
                    var value = valueEnumerator.Current;

                    row.Add(column, value);
                }
                table.Add(row);
            }

            var list = new List<dynamic>();

            foreach (var dictionary in table)
            {
                var d = new ExpandoObject() as IDictionary<string, object>;
                foreach (var c in tableColumn)
                {
                    d.Add(c, dictionary[c]);
                }

                list.Add(d);
            }

            var dataTable = new DataTable();

            foreach (var s in tableColumn)
            {
                //DataGrid1.Columns.Add(new DataGridTextColumn()
                //{
                //    Header = s,
                //});
                dataTable.Columns.Add(s);
            }

            foreach (var d in table)
            {
                var dataRow = dataTable.NewRow();
                foreach (var c in tableColumn)
                {
                    dataRow[c] = d[c];
                }
                dataTable.Rows.Add(dataRow);
            }
            DataGrid1.ItemsSource = dataTable.AsDataView();
        }
    }
}