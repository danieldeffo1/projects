using EdinoeOkno_program.Forms;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO;
using CsvHelper;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace EdinoeOkno_program
{
    /// <summary>
    /// Логика взаимодействия для Forms_UserControl.xaml
    /// </summary>
    public partial class Forms_UserControl : UserControl
    {
        NpgsqlConnection dBconnection = OurDatabase.GetConnection();
        NpgsqlConnection dBconnection1 = OurDatabase.GetConnection();

        public Forms_UserControl()
        {
            InitializeComponent();
            NewFormWorkingArea();
            InitStatsListBox();
        }

        public void NewFormWorkingArea()
        {
            StackPanel st = new StackPanel();
            formsWorkingArea.Content = st;

            StackPanel tools1 = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            StackPanel tools2 = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Button addElementButton = new Button()
            {
                Content = "Добавить элемент:",
            };
            ComboBox elementSelection = new ComboBox() { SelectedIndex = 0 };
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Заголовок" });
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Параграф" });
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Текстовый ввод" });
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Несколько из списка" });
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Несколько из списка c текстовым вводом" });
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Один из списка" });
            elementSelection.Items.Add(new ComboBoxItem() { Content = "Один из списка c текстовым вводом" });
            Button clearAllElementsButton = new Button()
            {
                Content = "Очистить список",
            };
            Button createFormButton = new Button()
            {
                Content = "Готово",
            };

            tools1.Children.Add(addElementButton);
            tools1.Children.Add(elementSelection);
            tools2.Children.Add(clearAllElementsButton);
            tools2.Children.Add(createFormButton);

            st.Children.Add(tools1);
            st.Children.Add(tools2);

            st.Children.Add(new TextBlock() { Text = "Название анкеты: " });
            TextBox formTitleBox = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 30, 0)
            };
            st.Children.Add(formTitleBox);

            st.Children.Add(new TextBlock() { Text = "Описание анкеты: " });
            TextBox descriptionTitleBox = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 30, 0)
            };
            st.Children.Add(descriptionTitleBox);

            StackPanel listPanel = new StackPanel();
            st.Children.Add(listPanel);

            void addElement_Click(object sender, EventArgs e)
            {
                int sel = elementSelection.SelectedIndex;
                StackPanel elementRow = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(5, 10, 10, 0),
                };
                switch (sel)
                {
                    case 0:
                        Header header = new Header();
                        elementRow.Children.Add(header.GetUIElement());
                        elementRow.Tag = header;
                        break;
                    case 1:
                        Forms.Paragraph paragraph = new Forms.Paragraph();
                        elementRow.Children.Add(paragraph.GetUIElement());
                        elementRow.Tag = paragraph;
                        break;
                    case 2:
                        TextInput textInput = new TextInput();
                        elementRow.Children.Add(textInput.GetUIElement());
                        elementRow.Tag = textInput;
                        break;
                    case 3:
                        CheckboxGroup checkboxGroup = new CheckboxGroup();
                        elementRow.Children.Add(checkboxGroup.GetUIElement());
                        elementRow.Tag = checkboxGroup;
                        break;
                    case 4:
                        CheckboxGroupWithTextInput checkboxGroupWithTextInput = new CheckboxGroupWithTextInput();
                        elementRow.Children.Add(checkboxGroupWithTextInput.GetUIElement());
                        elementRow.Tag = checkboxGroupWithTextInput;
                        break;
                    case 5:
                        RadioGroup radioGroup = new RadioGroup();
                        elementRow.Children.Add(radioGroup.GetUIElement());
                        elementRow.Tag = radioGroup;
                        break;
                    case 6:
                        RadioGroupWithTextInput radioGroupWithTextInput = new RadioGroupWithTextInput();
                        elementRow.Children.Add(radioGroupWithTextInput.GetUIElement());
                        elementRow.Tag = radioGroupWithTextInput;
                        break;
                }
                StackPanel elementTools = new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button upButton = new Button() { Content = "Наверх" };
                Button deleteElementButton = new Button() { Content = "Удалить" };
                Button downButton = new Button() { Content = "Вниз" };

                void upButton_Click(object sender1, EventArgs e1)
                {
                    for (int i = 1; i < listPanel.Children.Count; i++)
                    {
                        if (listPanel.Children[i] == elementRow)
                        {
                            StackPanel temp = listPanel.Children[i - 1] as StackPanel;
                            StackPanel temp1 = listPanel.Children[i] as StackPanel;
                            listPanel.Children.Remove(temp);
                            listPanel.Children.Insert(i, temp);
                            listPanel.Children.Remove(temp1);
                            listPanel.Children.Insert(i - 1, temp1);
                            break;
                        }
                    }
                }
                upButton.Click += upButton_Click;
                void deleteElementButton_Click(object sender1, EventArgs e1)
                {
                    for (int i = 0; i < listPanel.Children.Count; i++)
                    {
                        if (listPanel.Children[i] == elementRow)
                        {
                            listPanel.Children.Remove(elementRow);
                            break;
                        }
                    }
                }
                deleteElementButton.Click += deleteElementButton_Click;
                void downButton_Click(object sender1, EventArgs e1)
                {
                    for (int i = 0; i < listPanel.Children.Count - 1; i++)
                    {
                        if (listPanel.Children[i] == elementRow)
                        {
                            StackPanel temp = listPanel.Children[i] as StackPanel;
                            StackPanel temp1 = listPanel.Children[i + 1] as StackPanel;
                            listPanel.Children.Remove(temp);
                            listPanel.Children.Insert(i + 1, temp);
                            listPanel.Children.Remove(temp1);
                            listPanel.Children.Insert(i, temp1);
                            break;
                        }
                    }
                }
                downButton.Click += downButton_Click;

                elementTools.Children.Add(upButton);
                elementTools.Children.Add(deleteElementButton);
                elementTools.Children.Add(downButton);

                elementRow.Children.Add(elementTools);
                listPanel.Children.Add(elementRow);
            }
            addElementButton.Click += addElement_Click;

            void clearAllElements_Click(object sender, EventArgs e)
            {
                listPanel.Children.Clear();
            }
            clearAllElementsButton.Click += clearAllElements_Click;

            void createFormButton_Click(object sender, EventArgs e)
            {
                List<IForms_Element> forms_Elements = listPanel.Children.Cast<StackPanel>()
                    .Select(el => el.Tag as IForms_Element).ToList<IForms_Element>();
                NewFormWorkingArea();
                CreateNewForm(formTitleBox.Text, descriptionTitleBox.Text, forms_Elements);
            }
            createFormButton.Click += createFormButton_Click;
        }

        private void CreateNewForm(string form_name, string form_description, List<IForms_Element> form_questions)
        {
            if (dBconnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    string query = $@"BEGIN;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                    query = $@"INSERT INTO forms.forms(name_form, description) VALUES ('{form_name}','{form_description}') RETURNING id_form;";
                    cmd = new NpgsqlCommand(query, dBconnection);
                    int id_form = Convert.ToInt32(cmd.ExecuteScalar());
                    //TODO: написать теги для шапки анкеты
                    string html_code = "";
                    int number = 1;
                    foreach(var element in form_questions)
                    {
                        element.CreateDBElement(id_form, dBconnection);
                        html_code += element.GetHTML(number);
                        number++;
                    }
                    query = $@"UPDATE forms.forms SET html_code = '{html_code}' WHERE id_form = {id_form};";
                    cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                    query = $@"END;";
                    cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    string query = $@"END;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InitStatsListBox()
        {
            statsListBox.Items.Clear();
            if (dBconnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    void selectStatForm(object sender, EventArgs e)
                    {
                        Button button = sender as Button;
                        int id_form = Convert.ToInt32(button.Tag);
                        ConstructStatsWorkingArea(id_form);
                    }
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT id_form, name_form FROM forms.forms", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    Button button = new Button()
                                    {
                                        Content = (string)dB["name_form"],
                                        Tag = (int)dB["id_form"],
                                        HorizontalAlignment = HorizontalAlignment.Stretch,
                                        Margin = new Thickness(0, 0, 10, 0),
                                    };
                                    button.Click += selectStatForm;
                                    statsListBox.Items.Add(button);
                                }    
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        struct Stats_table
        {
            public int id_question;
            public string name_question;
            public string type_question;
            public List<KeyValuePair<string, int>> counted_answers;
            public int total;
            public List<string> text_answers;
        }

        private void ConstructStatsWorkingArea(int id_form)
        {
            List<Stats_table> stats = new List<Stats_table>();
            if (dBconnection.State == System.Data.ConnectionState.Open
                && dBconnection1.State == System.Data.ConnectionState.Open)
            {
                //try
                {
                    string temp;
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT * FROM forms.stats_v WHERE id_form = {id_form};", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    temp = null;
                                    bool is_text_input = (bool)dB["is_text_input"];
                                    if (is_text_input == true && Convert.ToInt32(dB["counted_answers"]) != 0)
                                    {
                                        NpgsqlCommand cmd1 =
                                        new NpgsqlCommand
                                        ($@"SELECT text_answers FROM forms.text_inputs_v WHERE id_answer = {(int)dB["id_answer"]};", dBconnection1);
                                        temp = cmd1.ExecuteScalar().ToString();
                                    }
                                    Stats_table row = new Stats_table
                                    {
                                        id_question = (int)dB["id_question"],
                                        name_question = (string)dB["name_question"],
                                        type_question = (string)dB["type_question"],
                                        counted_answers = new List<KeyValuePair<string, int>>()
                                        {
                                            new KeyValuePair<string, int>((string)dB["name_answer"], Convert.ToInt32(dB["counted_answers"]))
                                        },
                                        text_answers = is_text_input == true && Convert.ToInt32(dB["counted_answers"]) != 0 ? temp.Split('\t').ToList() : null
                                    };
                                    stats.Add(row);
                                }
                            }
                        }
                    }
                    stats = stats
                        .GroupBy(g => g.id_question)
                        .Select(g => new Stats_table()
                        {
                            id_question = g.First().id_question,
                            name_question = g.First().name_question,
                            type_question = g.First().type_question,
                            counted_answers = g.Select(n => n.counted_answers.First()).ToList(),
                            total = g.Select(n => n.counted_answers.First().Value).Sum(),
                            text_answers = g.Select(n => n.text_answers).SingleOrDefault(n => n != null)
                        })
                        .ToList();
                    
                    StackPanel st = new StackPanel();
                    statsWorkingArea.Content = st;

                    foreach(var row in stats)
                    {
                        if (row.type_question == "checkbox")
                        {
                            TextBlock questionTitle = new TextBlock()
                            {
                                Text = $"\"{row.name_question}\" ({row.total} отв.)",
                                FontWeight = FontWeights.Bold,
                            };
                            st.Children.Add(questionTitle);
                            if (row.total != 0)
                            {
                                BarSeries barSeries = new BarSeries()
                                {
                                    DependentValuePath = "Value",
                                    IndependentValuePath = "Key",
                                    Title = "Несколько из списка",
                                    ItemsSource = row.counted_answers.Select(p => new KeyValuePair<string, int>($"{p.Key} ({p.Value} отв., {100 * p.Value / row.total}%) ", p.Value))
                                };
                                Chart chart = new Chart()
                                {
                                    Height = 75 * row.counted_answers.Count()
                                };
                                chart.Series.Add(barSeries);
                                st.Children.Add(chart);
                            }   
                        }
                        else if (row.type_question == "checkbox_with_text_input")
                        {
                            TextBlock questionTitle = new TextBlock()
                            {
                                Text = $"\"{row.name_question}\" ({row.total} отв.)",
                                FontWeight = FontWeights.Bold,
                            };
                            st.Children.Add(questionTitle);
                            if (row.total != 0)
                            {
                                BarSeries barSeries = new BarSeries()
                                {
                                    DependentValuePath = "Value",
                                    IndependentValuePath = "Key",
                                    Title = "Несколько из списка",
                                    ItemsSource = row.counted_answers.Select(p => new KeyValuePair<string, int>($"{p.Key} ({p.Value} отв., {100 * p.Value / row.total}%) ", p.Value))
                                };
                                Chart chart = new Chart()
                                {
                                    Height = 75 * row.counted_answers.Count()
                                };
                                chart.Series.Add(barSeries);
                                st.Children.Add(chart);
                            }
                            if (row.text_answers != null)
                            {
                                TextBlock text_inputsTitle = new TextBlock()
                                {
                                    Text = $"Текстовые ответы: {row.text_answers.Count()} отв.",
                                };
                                st.Children.Add(text_inputsTitle);
                                Button reportButton = new Button()
                                {
                                    Content = "Посмотреть все ответы",
                                    Tag = row.text_answers,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                };
                                reportButton.Click += text_input_reportButton_Click;
                                st.Children.Add(reportButton);
                            }  
                        }
                        else if (row.type_question == "radio")
                        {
                            TextBlock questionTitle = new TextBlock()
                            {
                                Text = $"\"{row.name_question}\" ({row.total} отв.)",
                                FontWeight = FontWeights.Bold,
                            };
                            st.Children.Add(questionTitle);
                            if(row.total != 0)
                            {
                                PieSeries pieSeries = new PieSeries()
                                {
                                    DependentValuePath = "Value",
                                    IndependentValuePath = "Key",
                                    Title = "Один из списка",
                                    ItemsSource = row.counted_answers.Select(p => new KeyValuePair<string, int>($"{p.Key} ({p.Value} отв., {100 * p.Value / row.total}%) ", p.Value))
                                };
                                Chart chart = new Chart()
                                {
                                    Height = 75 * row.counted_answers.Count()
                                };
                                chart.Series.Add(pieSeries);
                                st.Children.Add(chart);
                            }
                            
                        }
                        else if (row.type_question == "radio_with_text_input")
                        {
                            TextBlock questionTitle = new TextBlock()
                            {
                                Text = $"\"{row.name_question}\" ({row.total} отв.)",
                                FontWeight = FontWeights.Bold,
                            };
                            st.Children.Add(questionTitle);

                            if (row.total != 0)
                            {
                                PieSeries pieSeries = new PieSeries()
                                {
                                    DependentValuePath = "Value",
                                    IndependentValuePath = "Key",
                                    Title = "Один из списка",
                                    ItemsSource = row.counted_answers.Select(p => new KeyValuePair<string, int>($"{p.Key} ({p.Value} отв., {100 * p.Value / row.total}%) ", p.Value))
                                };
                                Chart chart = new Chart()
                                {
                                    Height = 75 * row.counted_answers.Count()
                                };
                                chart.Series.Add(pieSeries);
                                st.Children.Add(chart);
                            }
                            if(row.text_answers != null)
                            {
                                TextBlock text_inputsTitle = new TextBlock()
                                {
                                    Text = $"Текстовые ответы: {row.text_answers.Count()} отв.",
                                };
                                st.Children.Add(text_inputsTitle);
                                Button reportButton = new Button()
                                {
                                    Content = "Посмотреть все ответы",
                                    Tag = row.text_answers,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                };
                                reportButton.Click += text_input_reportButton_Click;
                                st.Children.Add(reportButton);
                            }
                        }
                        else if (row.type_question == "text_input")
                        {

                            TextBlock questionTitle = new TextBlock()
                            {
                                Text = $"\"{row.name_question}\" ({row.total} отв.) - текстовый ввод",
                                FontWeight = FontWeights.Bold,
                            };
                            st.Children.Add(questionTitle);
                            if(row.text_answers != null)
                            {
                                Button reportButton = new Button()
                                {
                                    Content = "Посмотреть все ответы",
                                    Tag = row.text_answers,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                };
                                reportButton.Click += text_input_reportButton_Click;
                                st.Children.Add(reportButton);
                            }
                        }
                    }
                }
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
            }

        }
        private void text_input_reportButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            List<string> row = button.Tag as List<string>;

            Directory.CreateDirectory("FormsReports");
            string path = $"FormsReports\\{DateTime.Now:yyyy-MM-dd HH-mm-ss-ff}.csv";
            using (StreamWriter stream = new StreamWriter(path, false, Encoding.Default))
            {
                foreach (string s in row)
                {
                    stream.WriteLine(s);
                }
                stream.Close();
            }
            var sInfo = new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
