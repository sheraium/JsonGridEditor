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
        private JToken _rootJToken;
        private string _selectedTokenPath;

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

                    _rootJToken = JToken.Parse(rawData);
                    ResetTreeView(_rootJToken);
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
                var fileName = string.Empty;
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
                    fileName = saveFileDialog.FileName;
                }

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
                    await stream.WriteAsync(_rootJToken.ToString());
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
                var selectedToken = TreeView1.SelectedItem as JToken;
                if (selectedToken == null) return;
                _selectedTokenPath = selectedToken.Path;

                var tableColumn = new Dictionary<string, object>();
                foreach (var jToken in selectedToken.Children())
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
            try
            {
                if (_rootJToken == null || string.IsNullOrEmpty(_selectedTokenPath)) return;
                var selectToken = _rootJToken.SelectToken(_selectedTokenPath);
                if (selectToken == null) return;

                var dataView = DataGrid1.ItemsSource as DataView;
                var dataTable = dataView?.ToTable();
                if (dataTable == null) return;

                if (selectToken.Type == JTokenType.Object)
                {
                    var children = selectToken.Children().Select(x => x.ToObject<JProperty>()).ToList();

                    foreach (var jProperty in children)
                    {
                        var jToken = selectToken.SelectToken(jProperty.Name);
                        var newValue = dataTable.Rows[0][jProperty.Name].ToString();
                        if (jToken == null || newValue == null) continue;
                        SetJToken(jToken, newValue);
                    }
                }

                ResetTreeView(_rootJToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void SetJToken(JToken jToken, string newValue)
        {
            switch (jToken.Type)
            {
                case JTokenType.None:
                    break;

                case JTokenType.Object:
                    break;

                case JTokenType.Array:
                    break;

                case JTokenType.Constructor:
                    break;

                case JTokenType.Property:
                    break;

                case JTokenType.Comment:
                    break;

                case JTokenType.Integer:
                    jToken.Replace(new JValue(Convert.ToInt32(newValue)));
                    break;

                case JTokenType.Float:
                    jToken.Replace(new JValue(Convert.ToSingle(newValue)));
                    break;

                case JTokenType.String:
                    jToken.Replace(new JValue(newValue));
                    break;

                case JTokenType.Boolean:
                    jToken.Replace(new JValue(Convert.ToBoolean(newValue)));
                    break;

                case JTokenType.Null:
                    break;

                case JTokenType.Undefined:
                    break;

                case JTokenType.Date:
                    jToken.Replace(new JValue(Convert.ToDateTime(newValue)));
                    break;

                case JTokenType.Raw:
                    break;

                case JTokenType.Bytes:
                    break;

                case JTokenType.Guid:
                    jToken.Replace(new JValue(Guid.Parse(newValue)));
                    break;

                case JTokenType.Uri:
                    break;

                case JTokenType.TimeSpan:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        private void ResetTreeView(JToken token)
        {
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
}