using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace Velcom
{
    public partial class MainWindow : MetroWindow
    {
        private OpenFileDialog openFileDialog;
        private string pathToTemplateFile = System.Reflection.Assembly.GetExecutingAssembly().Location + "/../" + "/xmlTemplate/{0}.txt";

        private string imei = string.Empty;
        private string phoneNumber = string.Empty;
        private int typeOfTest = 0;

        public MainWindow()
        {
            InitializeComponent();

            openFileDialog = new OpenFileDialog();
        }

        private void initValues()
        {
            imei = imei_textBox.Text;
            phoneNumber = numPhone_textBox.Text;
            typeOfTest = 1;

            if (radioButton1.IsChecked == true)
                typeOfTest = 1;
            else if (radioButton2.IsChecked == true)
                typeOfTest = 2;
            else if (radioButton3.IsChecked == true)
                typeOfTest = 3;
            else if (radioButton4.IsChecked == true)
                typeOfTest = 4;
            else if (radioButton5.IsChecked == true)
                typeOfTest = 5;
            else if (radioButton6.IsChecked == true)
                typeOfTest = 6;
            else if (radioButton7.IsChecked == true)
                typeOfTest = 7;
        }

        private async void ShowMetroMessageBox(string title, string message)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                NegativeButtonText = "CANCEL",
                AnimateHide = true,
                AnimateShow = true,
                ColorScheme = MetroDialogColorScheme.Accented
            };

            await DialogManager.ShowMessageAsync(this, title, message, MessageDialogStyle.Affirmative, metroDialogSettings);
        }

        private bool isRepeatCells(string imei, string phoneNumber, int typeOfTest)
        {
            ItemTable itemForAdd = new ItemTable
            {
                IMEI = imei,
                PhoneNumber = phoneNumber,
                TypeOfTest = typeOfTest
            };

            foreach (ItemTable item in dataGridView.Items)
            {
                if (itemForAdd.IMEI.Contains(item.IMEI))
                    return true;
                else if (itemForAdd.PhoneNumber.Contains(item.PhoneNumber))
                    return true;
            }

            return false;
        }

        private bool addCellToTable(string imei, string phoneNumber, int typeOfTest)
        {
            ItemTable itemForAdd = new ItemTable
            {
                IMEI = imei,
                PhoneNumber = phoneNumber,
                TypeOfTest = typeOfTest
            };

            dataGridView.Items.Add(itemForAdd);

            return true;
        }

        private bool updateCellToTable(string imei, string phoneNumber, int typeOfTest)
        {
            ItemTable itemForAdd = new ItemTable
            {
                IMEI = imei,
                PhoneNumber = phoneNumber,
                TypeOfTest = typeOfTest
            };

            ItemTable selectedCell = (ItemTable)dataGridView.SelectedItem;

            if (selectedCell != null)
            {
                int index = dataGridView.Items.IndexOf(selectedCell);
                dataGridView.Items.RemoveAt(index);
                dataGridView.Items.Insert(index, itemForAdd);

                return true;
            }

            return false;
        }

        private bool deleteSelectedCellsFromTable()
        {
            int selectedItemsCount = dataGridView.SelectedItems.Count;
            var selectedItems = dataGridView.SelectedItems;

            if (selectedItemsCount == 0)
                return false;

            for (int i = 0; i < selectedItemsCount; i++)
                dataGridView.Items.Remove(selectedItems[0]);

            return true;
        }

        private void getChildrenFromItemTable(ref XmlDocument document, ref XmlNode uilogNode, string imei, string numPhone, int typeOfTest)
        {
            var xmlTemplateFile = File.OpenText(string.Format(pathToTemplateFile, typeOfTest));
            var textFromXmlTemplateFile = xmlTemplateFile.ReadToEnd();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textFromXmlTemplateFile);
            XmlNodeList childrenFromUilogTemplateFile = xmlDocument.LastChild.ChildNodes;
            
            int j = 0;
            XmlNodeList xmlNodesFromQueryIMEIs = xmlDocument.SelectNodes("/uilog/ClientApplicationWindow/Form/FormGroup/FormTable/FormField/inputText");
            XmlNodeList xmlNodesFromQueryNumPhones = xmlDocument.SelectNodes("/uilog/ClientApplicationWindow/Form/FormGroup/FormGroup/FormGroup/FormField/inputText");
            foreach (XmlNode xmlNode in xmlNodesFromQueryIMEIs)
            {
                XmlAttribute attributeImei = xmlNode.Attributes["text"];
                attributeImei.Value = imei;
                j++;
            }

            j = 0;
            foreach (XmlNode xmlNode in xmlNodesFromQueryNumPhones)
            {
                XmlAttribute attributeNumPhone = xmlNode.Attributes["text"];
                attributeNumPhone.Value = numPhone;
                j++;
            }

            foreach (XmlNode child in childrenFromUilogTemplateFile)
            {
                XmlNode importNode = uilogNode.OwnerDocument.ImportNode(child, true);
                uilogNode.AppendChild(importNode);
            }
        }

        private OpenFileDialog OpenFileDialog(OpenFileDialog openFileDialog)
        {
            try
            {
                openFileDialog.Filter = "Файл Microsoft Excel|*.xlsx;";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.CheckFileExists = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    return openFileDialog;
                }

                return null;
            }
            catch (Exception exception)
            {
                ShowMetroMessageBox("Error", exception.Message);
            }

            return null;
        }

        private void imei_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter.Equals(e.Key))
            {
                addButton_Click(sender, e);
            }
            else if (Key.Tab.Equals(e.Key))
            {
                numPhone_textBox.Focusable = true;
            }
            else if (imei_textBox.Text.Length < 15)
            {
                int numKey = (int)e.Key;

                if (numKey >= 34 && numKey <= 43)
                    e.Handled = false;
                else if (numKey >= 74 && numKey <= 83)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            else
                e.Handled = true;
        }

        private void numPhone_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter.Equals(e.Key))
            {
                addButton_Click(sender, e);
            }
            else if (Key.Tab.Equals(e.Key))
            {
                imei_textBox.Focusable = true;
            }
            else if (numPhone_textBox.Text.Length < 12)
            {
                int numKey = (int)e.Key;

                if (numKey >= 34 && numKey <= 43)
                    e.Handled = false;
                else if (numKey >= 74 && numKey <= 83)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            else
                e.Handled = true;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (imei_textBox.Text.Length == 15 && numPhone_textBox.Text.Length > 0)
            {
                initValues();

                if (isRepeatCells(imei, phoneNumber, typeOfTest))
                {
                    ShowMetroMessageBox("Error", "Значения, которые вы ввели уже встречаются в таблице.");
                    return;
                }

                addCellToTable(imei, phoneNumber, typeOfTest);

                imei_textBox.Text = "";
                numPhone_textBox.Text = "";

                ShowMetroMessageBox("Information", "Строка добавлена успешно.");
            }
            else
                ShowMetroMessageBox("Error", "Заполните все поля для добавления данных в таблицу.");
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            initValues();

            //if (isRepeatCells(imei, phoneNumber, typeOfTest))
            //{
            //    ShowMetroMessageBox("Error", "Значения, которые вы ввели уже встречаются в таблице.");
            //    return;
            //}
            
            bool isUpdated = updateCellToTable(imei, phoneNumber, typeOfTest);
            if (isUpdated)
            {
                ShowMetroMessageBox("Information", "Строка успешна обнавлена.");
            }
            else
                ShowMetroMessageBox("Error", "Во время выполнения произошла ошибка.");
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            bool isDeleted = deleteSelectedCellsFromTable();
            if (isDeleted)
                ShowMetroMessageBox("Information", "Операция выполнена успешно.");
            else
                ShowMetroMessageBox("Error", "Во время выполнения произошла ошибка.");
        }

        private void doButton_Click(object sender, RoutedEventArgs e)
        {
            int itemsCount = dataGridView.Items.Count;

            if (itemsCount > 0)
            {
                var resultXmlFile = File.OpenText(string.Format(pathToTemplateFile, 0));
                var textFromXmlFile = resultXmlFile.ReadToEnd();

                XmlDocument resultXmlDocument = new XmlDocument();
                resultXmlDocument.LoadXml(textFromXmlFile);

                XmlNode uilogNodeOriginal = resultXmlDocument.LastChild;
                XmlNode uilogNodeChanged = resultXmlDocument.LastChild.Clone();

                string imei;
                string numPhone;
                int typeOfTest;
                for (int i = 0; i < itemsCount; i++)
                {
                    ItemTable item = (ItemTable)dataGridView.Items[i];

                    imei = item.IMEI;
                    numPhone = item.PhoneNumber;
                    typeOfTest = item.TypeOfTest;

                    XmlComment comment = resultXmlDocument.CreateComment(i+1 + " CHILD BEGIN | IMEI: " + imei + "; Phone Number: " + numPhone + "; Type Of Test: " + typeOfTest + ";");
                    uilogNodeChanged.InsertAfter(comment, uilogNodeChanged.LastChild);

                    getChildrenFromItemTable(ref resultXmlDocument, ref uilogNodeChanged, imei, numPhone, typeOfTest);

                    comment = resultXmlDocument.CreateComment(i + 1 + " CHILD END | IMEI: " + imei + "; Phone Number: " + numPhone + "; Type Of Test: " + typeOfTest + ";");
                    uilogNodeChanged.InsertAfter(comment, uilogNodeChanged.LastChild);
                }
                
                resultXmlDocument.ReplaceChild(uilogNodeChanged, uilogNodeOriginal);
                
                string pathToResultFile = System.Reflection.Assembly.GetExecutingAssembly().Location + "/../" + "/xmlOut/" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss") + ".txt";
                string outerXml = resultXmlDocument.FirstChild.OuterXml + "\n" + XDocument.Parse(resultXmlDocument.OuterXml).ToString();

                File.WriteAllText(pathToResultFile, outerXml, Encoding.UTF8);

                ShowMetroMessageBox("Information", "Операция выполнена успешно.");
            }
            else
                ShowMetroMessageBox("Error", "Пустая таблица. Введите данные в таблицу.");
        }

        private void dataGridView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ItemTable selectedItem = (ItemTable)dataGridView.SelectedItem;

            if (selectedItem != null)
            {
                imei_textBox.Text = selectedItem.IMEI.Clone().ToString();
                numPhone_textBox.Text = selectedItem.PhoneNumber.Clone().ToString();

                int typeOfTest = selectedItem.TypeOfTest;
                if (typeOfTest == 1)
                    radioButton1.IsChecked = true;
                else if (typeOfTest == 2)
                    radioButton2.IsChecked = true;
                else if (typeOfTest == 3)
                    radioButton3.IsChecked = true;
                else if (typeOfTest == 4)
                    radioButton4.IsChecked = true;
                else if (typeOfTest == 5)
                    radioButton5.IsChecked = true;
                else if (typeOfTest == 6)
                    radioButton6.IsChecked = true;
                else if (typeOfTest == 7)
                    radioButton7.IsChecked = true;
            }
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog(openFileDialog) != null)
            {
                _Application excel = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = excel.Workbooks.Open(openFileDialog.FileName);
                Worksheet worksheet = null;
                
                try
                {
                    foreach (Worksheet item in workbook.Worksheets)
                    {
                        if (item.Name.Equals("ExportedFromDataGridView"))
                            worksheet = item;
                    }

                    if (worksheet == null)
                    {
                        ShowMetroMessageBox("Error", "Данный файл не содержит данных.");
                        return;
                    }

                    dataGridView.Items.Clear();
                    
                    for (int i = 2; i < worksheet.Rows.Count; i++)
                    {
                        string imei = worksheet.Cells[i, 1].Text;
                        string numPhone = worksheet.Cells[i, 2].Text;
                        int typeOfTest = int.Parse(worksheet.Cells[i, 3].Text);

                        addCellToTable(imei, numPhone, typeOfTest);

                        if (imei == "")
                            return;
                    }

                    ShowMetroMessageBox("Information", "Операция выполнена успешно.");
                }
                catch (Exception exception)
                {
                    //ShowMetroMessageBox("Error exception", exception.Message);
                }
                finally
                {
                    excel.Quit();
                    workbook = null;
                    excel = null;
                }
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            _Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Worksheet worksheet = null;

            try
            {
                worksheet = workbook.ActiveSheet;
                worksheet.Name = "ExportedFromDataGridView";

                int indexRow = 1;
                int indexColumn = 1;
                foreach (var column in dataGridView.Columns)
                {
                    worksheet.Cells[indexRow, indexColumn] = column.Header;
                    worksheet.Cells[indexRow, indexColumn].ColumnWidth = 20;
                    worksheet.Cells[indexRow, indexColumn].Font.Bold = true;
                    indexColumn++;
                }

                indexRow = 2;
                indexColumn = 1;
                foreach (ItemTable item in dataGridView.Items)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        worksheet.Cells[indexRow, indexColumn].EntireColumn.NumberFormat = "@";
                        worksheet.Cells[indexRow, indexColumn].NumberFormat = "@";

                        if (i == 0)
                            worksheet.Cells[indexRow, indexColumn].Value = item.IMEI;
                        else if (i == 1)
                            worksheet.Cells[indexRow, indexColumn].Value = item.PhoneNumber;
                        else if (i == 2)
                            worksheet.Cells[indexRow, indexColumn].Value = item.TypeOfTest;
                        
                        indexColumn++;
                    }
                    indexColumn = 1;
                    indexRow++;
                }
                
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Файл Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                if (saveDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveDialog.FileName);
                    ShowMetroMessageBox("Information", "Операция выполнена успешно.");
                }
            }
            catch (Exception exception)
            {
                ShowMetroMessageBox("Error exception", exception.Message);
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}