using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace EdinoeOkno_program
{
    /// <summary>
    /// Логика взаимодействия для Requests_UserConrol.xaml
    /// </summary>
    public partial class Requests_UserConrol : UserControl
    {
        List<Request> currentRequestList = new List<Request>();
        NpgsqlConnection dBconnection = OurDatabase.GetConnection();
        Request selectedRequest;
        MailSend responseMail = new MailSend();
        string view = "req_back";

        List<string> type_filter = new List<string>();
        List<string> status_filter = new List<string>();
        List<string> faculty_filter = new List<string>();
        string last_name_search;

        public Requests_UserConrol()
        {
            InitializeComponent();

            GetNamesOfRequests();
            GetNamesOfStatus();
            GetNamesOfFaculty();
            last_nameSearchTextBox.TextChanged += last_nameSearchTextBox_TextChanged;
            updateListBoxButton.Click += updateListBoxButton_Click;

            DefaultListBox();
            DefaultWorkingArea();
            GetRequestList(currentRequestList);
            FillRequestsListBox(currentRequestList);
        }

        private void updateListBoxButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultListBox();
            DefaultWorkingArea();
            GetRequestList(currentRequestList);
            FillRequestsListBox(currentRequestList);
        }

        private void last_nameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            last_name_search = last_nameSearchTextBox.Text;
        }

        private void GetNamesOfRequests()
        {
            filter_requestComboBox.SelectionChanged += filter_ComboBox_SelectionChanged;
            foreach(string[] req_name in OurDatabase.requestNamesList)
            {
                if (Authorization.account.request_privileges.Contains(req_name[0]))
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
                        Tag = req_name[0],
                        Content = req_name[1]
                    };
                    checkBox.Checked += filter_requestCheckBox_Checked;
                    checkBox.Unchecked += filter_requestCheckBox_Unchecked;
                    item.Content = checkBox;
                    filter_requestComboBox.Items.Add(item);

                    type_filter.Add(req_name[0]);
                }   
            }
        }

        private void filter_requestCheckBox_Checked(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            type_filter.Add(checkBox.Tag as string);
        }

        private void filter_requestCheckBox_Unchecked(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            type_filter.Remove(checkBox.Tag as string);
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
            requestsListBox.Items.Clear();
            TextBlock loadingMessage = new TextBlock
            {
                Text = "Происходит загрузка, пожалуйста подождите...",
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontWeight = FontWeights.Bold,
            };
            requestsListBox.Items.Add(loadingMessage);
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
        /// <summary>
        /// Заполняет заданный requestsList со статусом status_code с помощью запроса к БД
        /// </summary>
        private void GetRequestList(List<Request> requestsList)
        {
            //Фильтр
            string status_condition = "('" + String.Join("','", status_filter) + "')";
            string faculty_condition = "('" + String.Join("','", faculty_filter) + "')";
            string type_condition = "('" + String.Join("','", type_filter) + "')";
            string last_name_condition = $"'{last_name_search}%'";

            //Сортировка
            string order_condition = "";
            if (sortingComboBox.SelectedIndex != 0)
                order_condition = "ORDER BY " + (sortingComboBox.SelectedItem as ComboBoxItem).Tag as string + (descOrderCheckBox.IsChecked == true ? " DESC" : "");
            else order_condition = "ORDER BY request_id" + (descOrderCheckBox.IsChecked == true ? "" : " DESC");

            if (dBconnection.State == System.Data.ConnectionState.Open)
            try
            {
                requestsList.Clear();
                using (NpgsqlCommand cmd =
                new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.{view} WHERE status_code IN {status_condition} AND faculty_code IN {faculty_condition} AND request_code IN {type_condition} AND last_name LIKE {last_name_condition} {order_condition} ", dBconnection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            foreach (DbDataRecord dB in reader)
                            {
                                Request r_temp = new Request()
                                {
                                    request_id = (int)dB["request_id"],
                                    request_code = (string)dB["request_code"],
                                    request_name = (string)dB["request_name"],
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
                                    student_group = (string)dB["student_group"],
                                    doc_storage_id = (int)dB["doc_storage_id"],
                                    doc_amount = (int)dB["doc_amount"],
                                    public_url = (string)dB["public_url"],
                                    time_when_added = (string)dB["time_when_added"],
                                    time_when_updated = (string)dB["time_when_updated"],
                                    staff_member_login = (string)dB["staff_member_login"],
                                    response_content = (string)dB["response_content"]
                                };
                                requestsList.Add(r_temp);
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
        private void FillRequestsListBox(List<Request> requestsList)
        {
            requestsListBox.Items.Clear();
            if (requestsList.Count == 0)
            {
                TextBlock noRequestsMessage = new TextBlock
                {
                    Text = "Заявления не найдены...",
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                requestsListBox.Items.Add(noRequestsMessage);
                return;
            }
            Request r;
            for (int i = 0; i < requestsList.Count; i++)
            {
                r = requestsList[i];
                Button button = new Button();
                TextBlock preview = new TextBlock
                {
                    Text = $"Заявка №{r.request_id} ({r.last_name})\n" +
                    $"Состояние: {r.status_name}\n" +
                    $"Время поступления: {r.time_when_added}\n" +
                    $"Факультет: {r.faculty_short_name}\n" +
                    $"Тип: {r.request_name}",
                    TextWrapping = TextWrapping.Wrap
                };
                button.Tag = r;
                button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                button.Width = requestsListBox.Width - 35;
                button.Content = preview;
                button.Click += SelectRequest;
                requestsListBox.Items.Add(button);
            }
        }
        /// <summary>
        /// По нажатию кнопки из requestListBox выбирается соответствующий selectedRequest
        /// </summary>
        private void SelectRequest(object sender, EventArgs eventArgs)
        {
            Button selectedButton = sender as Button;
            selectedRequest = selectedButton.Tag as Request;
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

        /// <summary>
        /// Строит workingArea исходя из текущего selectedRequest
        /// </summary>
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
                Text = $"Заявка №{selectedRequest.request_id} ({selectedRequest.time_when_added})",
                FontWeight = FontWeights.Bold
            };
            st.Children.Add(header);

            //общая информация
            TextBox genInfo = new TextBox()
            {
                IsReadOnly = true,
                Background = workingArea.Background,
                BorderBrush = null,
                Text = $"Состояние: {selectedRequest.status_name}\n" +
                        $"Время поступления: {selectedRequest.time_when_added}\n" +
                        $"Время обновления:  {selectedRequest.time_when_updated}\n" +
                        $"Обработано сотрудником: {selectedRequest.staff_member_login}\n" +
                        $"Тип: {selectedRequest.request_name}",
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
            last_nameTextBox.Text = selectedRequest.last_name;
            st.Children.Add(last_nameTextBlock);
            st.Children.Add(last_nameTextBox);

            //имя
            TextBlock first_nameTextBlock = new TextBlock() { Text = "Имя:" };
            TextBox first_nameTextBox = SetReadTextBox();
            first_nameTextBox.Text = selectedRequest.first_name;
            st.Children.Add(first_nameTextBlock);
            st.Children.Add(first_nameTextBox);

            //отчество
            TextBlock patronymicTextBlock = new TextBlock() { Text = "Отчество:" };
            TextBox patronymicTextBox = SetReadTextBox();
            patronymicTextBox.Text = selectedRequest.patronymic;
            st.Children.Add(patronymicTextBlock);
            st.Children.Add(patronymicTextBox);

            //Факультет
            TextBlock facultyTextBlock = new TextBlock() { Text = "Факультет:" };
            TextBox facultyTextBox = SetReadTextBox();
            facultyTextBox.Text = $"({selectedRequest.faculty_short_name}) {selectedRequest.faculty_name}";
            st.Children.Add(facultyTextBlock);
            st.Children.Add(facultyTextBox);

            //группа
            TextBlock groupTextBlock = new TextBlock() { Text = "Группа:" };
            TextBox groupTextBox = SetReadTextBox();
            groupTextBox.Text = selectedRequest.student_group;
            st.Children.Add(groupTextBlock);
            st.Children.Add(groupTextBox);

            //email
            TextBlock emailTextBlock = new TextBlock() { Text = "Почта:" };
            TextBox emailTextBox = SetReadTextBox();
            emailTextBox.Text = selectedRequest.email;
            st.Children.Add(emailTextBlock);
            st.Children.Add(emailTextBox);

            //кол-во файлов
            TextBlock filesInfo = new TextBlock()
            {
                FontWeight = FontWeights.Bold,
                Text = $"\nПрикреплённые документы: {selectedRequest.doc_amount} шт.",
            };
            st.Children.Add(filesInfo);

            //файлы
            if (selectedRequest.doc_amount != 0)
            {
                TextBlock linkTextBlock = new TextBlock() { Text = "Ссылка:" };
                TextBox linkTextBox = SetReadTextBox();
                linkTextBox.Text = selectedRequest.public_url;
                Button linkCopyButton = new Button()
                {
                    Content = "Перейти по ссылке",
                    Tag = selectedRequest.public_url,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush(Color.FromArgb(255, 22, 74, 207)),
                    Foreground = Brushes.White,
                };
                linkCopyButton.Click += linkBox_Click;
                st.Children.Add(linkTextBlock);
                st.Children.Add(linkTextBox);
                st.Children.Add(linkCopyButton);
            }

            //предыдущий ответ
            if (selectedRequest.staff_member_login != "student")
            {
                st.Children.Add(new TextBlock()
                {
                    Text = $"\n Предыдущий ответ от сотрудника {selectedRequest.staff_member_login}:",
                    FontWeight = FontWeights.Bold,
                }
                );
                TextBox previous_responseTextBox = new TextBox()
                {
                    Text = selectedRequest.response_content,
                    Width = 350,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment= HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
                };
                st.Children.Add(previous_responseTextBox);
            }

            //ответ на почту
            TextBlock responseHeader = new TextBlock()
            {
                Text = $"\nДобавить новый ответ на почту {selectedRequest.email}:",
                FontWeight = FontWeights.Bold,
            };
            st.Children.Add(responseHeader);

            //ввод темы
            st.Children.Add(new TextBlock() { Text = "Тема:", });
            TextBox titleBox = new TextBox()
            {
                Text = $"Ваша заявка №{selectedRequest.request_id}: {selectedRequest.request_name}",
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
                                    VALUES ('{selectedRequest.email}', '{titleBox.Text}', '{responseBox.Text}', 'request', '{Authorization.account.login}')
                                    RETURNING response_id
                                )
                                UPDATE {OurDatabase.dBSchema}.requests SET
                                    status_code = '{(comboBox.SelectedItem as ComboBoxItem).Tag}',
                                    response_id = (SELECT * FROM tmp),
                                    time_when_updated = now(),
                                    staff_member_login = '{Authorization.account.login}'
                                WHERE request_id = '{selectedRequest.request_id}';";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                    responseMail.Send(selectedRequest.email, selectedRequest.last_name, titleBox.Text, responseBox.Text);
                }
                else
                {
                    string query = $@"
                                UPDATE {OurDatabase.dBSchema}.requests SET
                                    status_code = '{(comboBox.SelectedItem as ComboBoxItem).Tag}',
                                    time_when_updated = now(),
                                    staff_member_login = '{Authorization.account.login}'
                                WHERE request_id = '{selectedRequest.request_id}';";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            GetRequestList(currentRequestList);
            FillRequestsListBox(currentRequestList);
        }

        private void linkBox_Click(object sender, EventArgs e)
        {
            Button textBox = sender as Button;
            var sInfo = new System.Diagnostics.ProcessStartInfo((string)(textBox.Tag)) { UseShellExecute = true };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
