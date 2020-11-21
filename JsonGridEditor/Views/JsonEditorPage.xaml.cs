using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
        private JToken _selectedJToken;

        public JsonEditorPage()
        {
            InitializeComponent();
        }

        public async Task LoadAsync(string fileName)
        {
            try
            {
                _fileName = fileName;

                using (var reader = new StreamReader(fileName))
                {
                    var rawData = await reader.ReadToEndAsync();
                    var token = JToken.Parse(rawData);

                    var children = new List<JToken>();
                    if (token != null)
                    {
                        children.Add(token);
                    }

                    TreeView1.ItemsSource = null;
                    TreeView1.Items.Clear();
                    TreeView1.ItemsSource = children;
                    foreach (object item in TreeView1.Items)
                    {
                        var treeItem = TreeView1.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                        if (treeItem != null)
                        {
                            ExpandAll(treeItem, true);
                            treeItem.IsExpanded = true;
                        }
                    }
                }
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
                var fileName1 = string.Empty;
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".json";
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "Json file(*.json)|*.json";
                //saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.FileName = $"{DateTime.Now.Date:yyyyMMdd}";
                var result = saveFileDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    fileName1 = saveFileDialog.FileName;
                }

                var fileName = fileName1;
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
                if (_fileName == null || TreeView1.ItemsSource == null) return;

                using var file = new FileStream(_fileName + "_1", FileMode.Create);
                using (var stream = new StreamWriter(file, Encoding.UTF8))
                {
                    var root = TreeView1.ItemsSource as List<JToken>;
                    await stream.WriteAsync(root[0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectedJToken = TreeView1.SelectedItem as JToken;
                if (_selectedJToken == null) return;

                var tableColumn = new Dictionary<string, object>();

                foreach (var jToken in _selectedJToken.Children())
                {
                    var jProperty = jToken.ToObject<JProperty>();
                    tableColumn.Add(jProperty.Name, jProperty.Value);
                }
                var dataTable = new DataTable();
                foreach (var s in tableColumn)
                {
                    dataTable.Columns.Add(s.Key);
                }

                var dataRow = dataTable.NewRow();
                foreach (var s in tableColumn)
                {
                    dataRow[s.Key] = s.Value;
                }
                dataTable.Rows.Add(dataRow);
                DataGrid1.ItemsSource = dataTable.AsDataView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonSetData_OnClick(object sender, RoutedEventArgs e)
        {
            return;
            if (_selectedJToken == null) return;

            var dataView = DataGrid1.ItemsSource as DataView;
            var dataTable = dataView?.ToTable();
            if (dataTable == null) return;

            if (_selectedJToken.Type == JTokenType.Object && dataTable.Rows[0] != null)
            {
                var children = _selectedJToken.Children().Select(x => x.ToObject<JProperty>()).ToList();

                foreach (var jProperty in children)
                {
                    var json = dataTable.Rows[0][jProperty.Name];
                    jProperty.Value = JToken.FromObject(json);

                    //var jObject = JObject.Parse(json.ToString());
                    //var jObject = JObject.Parse(_selectedJToken.SelectToken(jProperty.Name).ToString());
                    //jObject.Replace(json.ToString());
                    _selectedJToken.SelectToken(jProperty.Name).Replace(jProperty);
                }
            }

            //var token = TreeView1.SelectedItem as List<JToken>;
            //var jEnumerable = token.Children();

            //var root = TreeView1.ItemsSource as List<JToken>;
            //var jToken = JToken.Parse(_selectedItem.ToString());
            //root.Select(jToken);
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }

        private void JsonEditorPage_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}