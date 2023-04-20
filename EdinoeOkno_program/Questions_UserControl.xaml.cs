using Npgsql;
using System;
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

namespace EdinoeOkno_program
{
    /// <summary>
    /// Логика взаимодействия для Questions_UserControl.xaml
    /// </summary>
    public partial class Questions_UserControl : UserControl
    {
        List<Question> currentQuestionList = new List<Question>();
        NpgsqlConnection dBconnection = OurDatabase.GetConnection();
        Question selectedQuestion;
        MailSend responseMail = new MailSend();
        string view = "que_back";

        List<string> type_filter = new List<string>();
        List<string> status_filter = new List<string>();
        List<string> faculty_filter = new List<string>();
        string last_name_search;
        string subject_search;

        public Questions_UserControl()
        {
            InitializeComponent();

            GetNamesOfStatus();
            GetNamesOfFaculty();
            last_nameSearchTextBox.TextChanged += last_nameSearchTextBox_TextChanged;
            updateListBoxButton.Click += updateListBoxButton_Click;
            subjectSearchTextBox.TextChanged += subjectSearchTextBox_TextChanged;

            DefaultListBox();
            DefaultWorkingArea();
            GetQuestionList(currentQuestionList);
            FillQuestionsListBox(currentQuestionList);
        }

        private void subjectSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            subject_search = subjectSearchTextBox.Text;
        }

        private void updateListBoxButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultListBox();
            DefaultWorkingArea();
            GetQuestionList(currentQuestionList);
            FillQuestionsListBox(currentQuestionList);
        }
        private void last_nameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            last_name_search = last_nameSearchTextBox.Text;
        }

        private void filter_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.SelectedIndex = 0;
        }


        private void GetNamesOfStatus()
        {
            filter_statusComboBox.SelectionChanged += filter_ComboBox_SelectionChanged;
            foreach (string[] status_name in OurDatabase.statusNamesList)
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch
                };
                CheckBox checkBox = new CheckBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    IsChecked = true,
                    Tag = status_name[0],
                    Content = status_name[2]
                };
                checkBox.Checked += filter_statusCheckBox_Checked;
                checkBox.Unchecked += filter_statusCheckBox_Unchecked;
                item.Content = checkBox;
                filter_statusComboBox.Items.Add(item);

                status_filter.Add(status_name[0]);
            }
        }

        private void filter_statusCheckBox_Checked(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            status_filter.Add(checkBox.Tag as string);
        }

        private void filter_statusCheckBox_Unchecked(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            status_filter.Remove(checkBox.Tag as string);
        }

        private void GetNamesOfFaculty()
        {
            filter_facultyComboBox.SelectionChanged += filter_ComboBox_SelectionChanged;
            foreach (string[] fac_name in OurDatabase.facultyNamesList)
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch
                };
                CheckBox checkBox = new CheckBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    IsChecked = true,
                    Tag = fac_name[0],
                    Content = $"{fac_name[1]}"
                };
                checkBox.Checked += filter_facultyCheckBox_Checked;
                checkBox.Unchecked += filter_facultyCheckBox_Unchecked;
                item.Content = checkBox;
                filter_facultyComboBox.Items.Add(item);

                faculty_filter.Add(fac_name[0]);
            }
        }

        private void filter_facultyCheckBox_Checked(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            faculty_filter.Add(checkBox.Tag as string);
        }

        private void filter_facultyCheckBox_Unchecked(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            faculty_filter.Remove(checkBox.Tag as string);
        }


        /// <summary>
        /// Очищает requestsListBox
        /// </summary>
        private void DefaultListBox()
        {
            questionsListBox.Items.Clear();
            TextBlock loadingMessage = new TextBlock
            {
                Text = "Происходит загрузка, пожалуйста подождите...",
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontWeight = FontWeights.Bold,
            };
            questionsListBox.Items.Add(loadingMessage);
        }
        /// <summary>
        /// Очищает workingArea
        /// </summary>
        private void DefaultWorkingArea()
        {
            workingArea.Content = null;
            TextBlock defaultWorkingAreaMessage = new TextBlock
            {
                Text = "Выберите элемент из списка...",
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            workingArea.Content = defaultWorkingAreaMessage;
        }

        private void GetQuestionList(List<Question> questionsList)
        {
            //Фильтр
            string status_condition = "('" + String.Join("','", status_filter) + "')";
            string faculty_condition = "('" + String.Join("','", faculty_filter) + "')";
            string last_name_condition = $"'{last_name_search}%'";
            string subject_condition = $"'{subject_search}%'";

            //Сортировка
            string order_condition = "";
            if (sortingComboBox.SelectedIndex != 0)
                order_condition = "ORDER BY " + (sortingComboBox.SelectedItem as ComboBoxItem).Tag as string + (descOrderCheckBox.IsChecked == true ? " DESC" : "");
            else order_condition = "ORDER BY question_id" + (descOrderCheckBox.IsChecked == true ? "" : " DESC");

            if (dBconnection.State == System.Data.ConnectionState.Open)
                try
                {
                    questionsList.Clear();
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.{view} WHERE status_code IN {status_condition} AND faculty_code IN {faculty_condition} AND subject LIKE {subject_condition} AND last_name LIKE {last_name_condition} {order_condition} ", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    Question r_temp = new Question()
                                    {
                                        question_id = (int)dB["question_id"],
                                        subject = (string)dB["subject"],
                                        body = (string)dB["body"],
                                        status_code = (string)dB["status_code"],
                                        status_name = (string)dB["status_name"],
                                        status_short_name = (string)dB["status_short_name"],
                                        first_name = (string)dB["first_name"],
                                        last_name = (string)dB["last_name"],
                                        patronymic = (string)dB["patronymic"],
                                        email = (string)dB["email"],
                                        faculty_code = (string)dB["faculty_code"],
                                        faculty_name = (string)dB["faculty_name"],
                                        faculty_short_name = (string)dB["faculty_short_name"],
                                        time_when_added = (string)dB["time_when_added"],
                                        time_when_updated = (string)dB["time_when_updated"],
                                        staff_member_login = (string)dB["staff_member_login"],
                                        response_content = (string)dB["response_content"]
                                    };
                                    questionsList.Add(r_temp);
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

        /// <summary>
        /// Заполняет requestListBox исходя из переданного requestsList
        /// </summary>
        private void FillQuestionsListBox(List<Question> questionsList)
        {
            questionsListBox.Items.Clear();
            if (questionsList.Count == 0)
            {
                TextBlock noQuestionsMessage = new TextBlock
                {
                    Text = "Вопросы не найдены...",
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                questionsListBox.Items.Add(noQuestionsMessage);
                return;
            }
            Question q;
            for (int i = 0; i < questionsList.Count; i++)
            {
                q = questionsList[i];
                Button button = new Button();
                TextBlock preview = new TextBlock
                {
                    Text = $"Вопрос №{q.question_id} ({q.last_name})\n" +
                    $"Состояние: {q.status_name}\n" +
                    $"Время поступления: {q.time_when_added}\n" +
                    $"Факультет: {q.faculty_short_name}\n" +
                    $"Тема: \"{q.subject}\"",
                    TextWrapping = TextWrapping.Wrap
                };
                button.Tag = q;
                button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                button.Width = questionsListBox.Width - 35;
                button.Content = preview;
                button.Click += SelectRequest;
                questionsListBox.Items.Add(button);
            }
        }
        /// <summary>
        /// По нажатию кнопки из requestListBox выбирается соответствующий selectedRequest
        /// </summary>
        private void SelectRequest(object sender, EventArgs eventArgs)
        {
            Button selectedButton = sender as Button;
            selectedQuestion = selectedButton.Tag as Question;
            ConstructWorkingArea();
        }

        /// <summary>
        /// Возвращает новый TextBox только для чтения
        /// </summary>
        public TextBox SetReadTextBox()
        {
            return new TextBox()
            {
                Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
                IsReadOnly = true
            };
        }

        private void ConstructWorkingArea()
        {
            StackPanel st = new StackPanel();
            workingArea.Content = st;

            //Заголовок
            TextBox header = new TextBox()
            {
                IsReadOnly = true,
                Background = workingArea.Background,
                BorderBrush = null,
                Text = $"Онлайн-вопрос №{selectedQuestion.question_id} ({selectedQuestion.time_when_added})",
                FontWeight = FontWeights.Bold
            };
            st.Children.Add(header);

            //общая информация
            TextBox genInfo = new TextBox()
            {
                IsReadOnly = true,
                Background = workingArea.Background,
                BorderBrush = null,
                Text = $"Состояние: {selectedQuestion.status_name}\n" +
                        $"Время поступления: {selectedQuestion.time_when_added}\n" +
                        $"Время обновления:  {selectedQuestion.time_when_updated}\n" +
                        $"Обработано сотрудником: {selectedQuestion.staff_member_login}"
            };
            st.Children.Add(genInfo);

            //информация о заявившем
            TextBlock personInfo = new TextBlock()
            {
                FontWeight = FontWeights.Bold,
                Text = "\nИнформация о заявившем:"
            };
            st.Children.Add(personInfo);

            //фамилия
            TextBlock last_nameTextBlock = new TextBlock() { Text = "Фамилия:" };
            TextBox last_nameTextBox = SetReadTextBox();
            last_nameTextBox.Text = selectedQuestion.last_name;
            st.Children.Add(last_nameTextBlock);
            st.Children.Add(last_nameTextBox);

            //имя
            TextBlock first_nameTextBlock = new TextBlock() { Text = "Имя:" };
            TextBox first_nameTextBox = SetReadTextBox();
            first_nameTextBox.Text = selectedQuestion.first_name;
            st.Children.Add(first_nameTextBlock);
            st.Children.Add(first_nameTextBox);

            //отчество
            TextBlock patronymicTextBlock = new TextBlock() { Text = "Отчество:" };
            TextBox patronymicTextBox = SetReadTextBox();
            patronymicTextBox.Text = selectedQuestion.patronymic;
            st.Children.Add(patronymicTextBlock);
            st.Children.Add(patronymicTextBox);

            //Факультет
            TextBlock facultyTextBlock = new TextBlock() { Text = "Факультет:" };
            TextBox facultyTextBox = SetReadTextBox();
            facultyTextBox.Text = $"({selectedQuestion.faculty_short_name}) {selectedQuestion.faculty_name}";
            st.Children.Add(facultyTextBlock);
            st.Children.Add(facultyTextBox);

            //email
            TextBlock emailTextBlock = new TextBlock() { Text = "Почта:" };
            TextBox emailTextBox = SetReadTextBox();
            emailTextBox.Text = selectedQuestion.email;
            st.Children.Add(emailTextBlock);
            st.Children.Add(emailTextBox);

            //сам вопрос
            TextBlock questionInfo = new TextBlock()
            {
                FontWeight = FontWeights.Bold,
                Text = "\nИнформация о вопросе:"
            };
            st.Children.Add(questionInfo);

            //тема
            TextBlock subjectTextBlock = new TextBlock() { Text = "Тема:" };
            st.Children.Add(subjectTextBlock);
            TextBox subjectBox = new TextBox()
            {
                Text = selectedQuestion.subject,
                Width = 350,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254))
            };
            st.Children.Add(subjectBox);
            //содержание
            TextBlock bodyTextBlock = new TextBlock() { Text = "Содержание:" };
            st.Children.Add(bodyTextBlock);
            TextBox bodyBox = new TextBox()
            {
                Text = selectedQuestion.body,
                Width = 350,
                Height = 100,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254))
            };
            st.Children.Add(bodyBox);

            //предыдущий ответ
            if (selectedQuestion.staff_member_login != "student")
            {
                st.Children.Add(new TextBlock()
                {
                    Text = $"\n Предыдущий ответ от сотрудника {selectedQuestion.staff_member_login}:",
                    FontWeight = FontWeights.Bold,
                }
                );
                TextBox previous_responseTextBox = new TextBox()
                {
                    Text = selectedQuestion.response_content,
                    Width = 350,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
                };
                st.Children.Add(previous_responseTextBox);
            }

            //ответ на почту
            TextBlock responseHeader = new TextBlock()
            {
                Text = $"\nДобавить новый ответ на почту {selectedQuestion.email}:",
                FontWeight = FontWeights.Bold,
            };
            st.Children.Add(responseHeader);

            //ввод темы
            st.Children.Add(new TextBlock() { Text = "Тема:", });
            TextBox titleBox = new TextBox()
            {
                Text = $"Ответ на ваш вопрос №{selectedQuestion.question_id}: {selectedQuestion.subject}",
                Width = 350,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            st.Children.Add(titleBox);

            //ввод ответа
            st.Children.Add(new TextBlock() { Text = "Ответ:", });
            TextBox responseBox = new TextBox()
            {
                Height = 100,
                Width = 350,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                AcceptsReturn = true,
                AcceptsTab = true,
                Tag = titleBox
            };
            st.Children.Add(responseBox);

            //подтверждение отправки ответа
            CheckBox confirm_responseCheckBox = new CheckBox() { Content = "Без ответа на почту", Tag = responseBox };
            st.Children.Add(confirm_responseCheckBox);

            //перевод в другой статус
            StackPanel statusChangeRow = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock statusChangeTextBlock = new TextBlock()
            {
                Text = $"\nИзменить статус на: ",
                FontWeight = FontWeights.Bold,
            };

            ComboBox statusChangeComboBox = new ComboBox() { Tag = confirm_responseCheckBox };
            foreach (string[] statuses in OurDatabase.statusNamesList)
            {
                statusChangeComboBox.Items.Add(new ComboBoxItem()
                {
                    Tag = statuses[0],
                    Content = statuses[2]
                });
            };
            statusChangeComboBox.SelectedIndex = 1;
            statusChangeRow.Children.Add(statusChangeTextBlock);
            statusChangeRow.Children.Add(statusChangeComboBox);
            st.Children.Add(statusChangeRow);

            Button confirmStatusChangeButton = new Button()
            {
                Content = "Подтвердить перевод",
                Background = new SolidColorBrush(Color.FromArgb(255, 22, 74, 207)),
                Foreground = Brushes.White,
                Tag = statusChangeComboBox,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            confirmStatusChangeButton.Click += confirmStatusChangeButton_Click;
            st.Children.Add(confirmStatusChangeButton);
        }

        private void confirmStatusChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ComboBox comboBox = button.Tag as ComboBox;
            CheckBox checkBox = comboBox.Tag as CheckBox;
            TextBox responseBox = checkBox.Tag as TextBox;
            TextBox titleBox = responseBox.Tag as TextBox;

            DefaultListBox();
            DefaultWorkingArea();

            try
            {
                if (checkBox.IsChecked == false)
                {
                    string query = $@"WITH tmp AS
                                (INSERT INTO {OurDatabase.dBSchema}.responses (email, title, response_content, type, staff_member_login) 
                                    VALUES ('{selectedQuestion.email}', '{titleBox.Text}', '{responseBox.Text}', 'request', '{Authorization.account.login}')
                                    RETURNING response_id
                                )
                                UPDATE {OurDatabase.dBSchema}.questions SET
                                    status_code = '{(comboBox.SelectedItem as ComboBoxItem).Tag}',
                                    response_id = (SELECT * FROM tmp),
                                    time_when_updated = now(),
                                    staff_member_login = '{Authorization.account.login}'
                                WHERE question_id = '{selectedQuestion.question_id}';";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                    responseMail.Send(selectedQuestion.email, selectedQuestion.last_name, titleBox.Text, responseBox.Text);
                }
                else
                {
                    string query = $@"
                                UPDATE {OurDatabase.dBSchema}.questions SET
                                    status_code = '{(comboBox.SelectedItem as ComboBoxItem).Tag}',
                                    time_when_updated = now(),
                                    staff_member_login = '{Authorization.account.login}'
                                WHERE question_id = '{selectedQuestion.question_id}';";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            GetQuestionList(currentQuestionList);
            FillQuestionsListBox(currentQuestionList);
        }
    }
}
