using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
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
    /// Логика взаимодействия для Аdministration_UserControl.xaml
    /// </summary>
    public partial class Аdministration_UserControl : UserControl
    {
        
        NpgsqlConnection dBconnection = OurDatabase.GetConnection();
        const string view = "accounts_v";

        List<Staff_Member> accountsList = new List<Staff_Member>();
        Staff_Member selectedAccount;

        public Аdministration_UserControl()
        {
            InitializeComponent();
            GetAccountList();
            FillAccountListBox();
        }

        private void GetAccountList()
        {
            if (dBconnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    accountsList.Clear();
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.{view}", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    Staff_Member account = new Staff_Member()
                                    {
                                        login = (string)dB["login"],
                                        password = (string)dB["password"],
                                        faculty_code = (string)dB["faculty_code"],
                                        first_name = (string)dB["first_name"],
                                        last_name = (string)dB["last_name"],
                                        patronymic = (string)dB["patronymic"],
                                        is_admin = (bool)dB["is_admin"],
                                        can_view_requests = (bool)dB["can_view_requests"],
                                        can_view_questions = (bool)dB["can_view_questions"],
                                        can_view_appointments = (bool)dB["can_view_appointments"],
                                        can_view_forms = (bool)dB["can_view_forms"],
                                        request_privileges = dB.IsDBNull(dB.GetOrdinal("request_privileges")) == false ? ((string)dB["request_privileges"]).Split(',').ToList<string>() : new List<string>()
                                        //request_privileges =  ((string)dB["request_privileges"]).Split(',').ToList<string>() 
                                    };
                                    accountsList.Add(account);
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

        private void FillAccountListBox()
        {
            accountsListBox.Items.Clear();
            if (accountsList.Count == 0)
            {
                TextBlock noAccountsMessage = new TextBlock
                {
                    Text = "Аккаунты не найдены...",
                    TextWrapping = TextWrapping.Wrap,
                    Width = accountsListBox.Width,
                    FontWeight = FontWeights.Bold,
                };
                accountsListBox.Items.Add(noAccountsMessage);
                return;
            }
            Staff_Member acc;
            for (int i = 0; i < accountsList.Count; i++)
            {
                acc = accountsList[i];
                Button button = new Button();
                TextBlock preview = new TextBlock
                {
                    Text = $"{acc.login}: {acc.last_name} {acc.first_name} {acc.patronymic}",
                    TextWrapping = TextWrapping.Wrap
                };
                button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                button.Width = accountsListBox.Width - 35;
                button.Content = preview;
                button.Click += SelectAccount;
                button.Tag = acc;
                //acc.Tag = button;
                accountsListBox.Items.Add(button);
            }
        }

        private void SelectAccount(object sender, EventArgs e)
        {
            Button button = sender as Button;
            selectedAccount = button.Tag as Staff_Member;
            ConstructWorkingArea();
        }

        private void ConstructWorkingArea()
        {
            StackPanel st = new StackPanel();
            accountsWorkingArea.Content = st;

            //Заголовок
            TextBlock header = new TextBlock()
            {
                Text = "Информация об аккаунте сотрудника:\n",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(header);

            //login
            TextBlock loginTextBlock = new TextBlock()
            {
                Text = "Логин:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(loginTextBlock);

            TextBox loginTextBox = new TextBox()
            {
                Text = selectedAccount.login,
                IsReadOnly = true,
            };
            st.Children.Add(loginTextBox);

            //пароль
            TextBlock passwordTextBlock = new TextBlock()
            {
                Text = "Пароль:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(passwordTextBlock);

            TextBox passwordTextBox = new TextBox()
            {
                Text = selectedAccount.password,
            };
            passwordTextBox.TextChanged += passwordTextBox_TextChanged;
            st.Children.Add(passwordTextBox);

            //фамилия
            TextBlock last_nameTextBlock = new TextBlock()
            {
                Text = "Фамилия:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(last_nameTextBlock);

            TextBox last_nameTextBox = new TextBox()
            {
                Text = selectedAccount.last_name,
            };
            last_nameTextBox.TextChanged += last_nameTextBox_TextChanged;
            st.Children.Add(last_nameTextBox);

            //имя
            TextBlock first_nameTextBlock = new TextBlock()
            {
                Text = "Имя:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(first_nameTextBlock);

            TextBox first_nameTextBox = new TextBox()
            {
                Text = selectedAccount.first_name,
            };
            first_nameTextBox.TextChanged += first_nameTextBox_TextChanged;
            st.Children.Add(first_nameTextBox);

            //отчество
            TextBlock patronymicTextBlock = new TextBlock()
            {
                Text = "Отчество:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(patronymicTextBlock);

            TextBox patronymicTextBox = new TextBox()
            {
                Text = selectedAccount.patronymic,
            };
            patronymicTextBox.TextChanged += patronymicTextBox_TextChanged;
            st.Children.Add(patronymicTextBox);

            //факультет
            TextBlock facultyTextBlock = new TextBlock()
            {
                Text = "Факультет:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(facultyTextBlock);

            ComboBox facultyComboBox = new ComboBox();
            foreach (string[] line in OurDatabase.facultyNamesList)
            {
                ComboBoxItem facultyComboBoxItem = new ComboBoxItem()
                {
                    Content = line[1],
                    Tag = line[0]
                };
                facultyComboBox.Items.Add(facultyComboBoxItem);
                if (line[0] == selectedAccount.faculty_code)
                    facultyComboBox.SelectedItem = facultyComboBoxItem;
            }
            facultyComboBox.SelectionChanged += facultyComboBox_SelectionChanged;
            st.Children.Add(facultyComboBox);

            //разрешения
            TextBlock permissions = new TextBlock()
            {
                Text = "\nРазрешения:",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(permissions);

            CheckBox is_adminCheckBox = new CheckBox()
            {
                Content = "Является администратором?",
                IsChecked = selectedAccount.is_admin,
            };
            is_adminCheckBox.Click += is_adminCheckBox_Click;
            st.Children.Add(is_adminCheckBox);

            CheckBox can_view_requestsCheckBox = new CheckBox()
            {
                Content = "Доступ ко вкладке \"Заявки\"?",
                IsChecked = selectedAccount.can_view_requests,
            };
            can_view_requestsCheckBox.Click += can_view_requestsCheckBox_Click;
            st.Children.Add(can_view_requestsCheckBox);

            CheckBox can_view_questionsCheckBox = new CheckBox()
            {
                Content = "Доступ ко вкладке \"Вопросы\"?",
                IsChecked = selectedAccount.can_view_questions,
            };
            can_view_questionsCheckBox.Click += can_view_questionsCheckBox_Click;
            st.Children.Add(can_view_questionsCheckBox);

            CheckBox can_view_formsCheckBox = new CheckBox()
            {
                Content = "Доступ ко вкладке \"Анкетирование\"?",
                IsChecked = selectedAccount.can_view_forms,
            };
            can_view_formsCheckBox.Click += can_view_formsCheckBox_Click;
            st.Children.Add(can_view_formsCheckBox);


            //разрешения по заявкам:
            TextBlock requestsPermissions = new TextBlock()
            {
                Text = "\nРазрешения по просмотру заявлений:",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(requestsPermissions);

            foreach (string[] line in OurDatabase.requestNamesList)
            {
                CheckBox request_privilegeCheckBox = new CheckBox()
                {
                    Content = line[1],
                    Tag = line[0],
                    IsChecked = selectedAccount.request_privileges.Contains(line[0]),
                };
                request_privilegeCheckBox.Checked += request_privilegeCheckBox_Checked;
                request_privilegeCheckBox.Unchecked += request_privilegeCheckBox_Unchecked;
                st.Children.Add(request_privilegeCheckBox);
            }

            //управление аккаунтом
            TextBlock actionOnAccount = new TextBlock()
            {
                Text = "\nДействия:",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(actionOnAccount);

            //сохранение изменений
            Button saveChangesButton = new Button()
            {
                Content = "Сохранить изменения",
                Background = Brushes.Blue,
                Foreground = Brushes.White,
                HorizontalAlignment= HorizontalAlignment.Left,
            };
            saveChangesButton.Click += saveChangesButton_Click;
            st.Children.Add(saveChangesButton);

            //удалить аккаунт
            Button deleteAccountButton = new Button()
            {
                Content = "Удалить аккаунт",
                Background = Brushes.Red,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            deleteAccountButton.Click += deleteAccountButton_Click;
            st.Children.Add(deleteAccountButton);
        }

        private void deleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            accountsWorkingArea.Content = null;
            try
            {
                string query = $@"DELETE FROM {OurDatabase.dBSchema}.staff_members WHERE login = '{selectedAccount.login}'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetAccountList();
            FillAccountListBox();
        }

        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            accountsWorkingArea.Content = null;
            try
            {
                string query = $@"UPDATE {OurDatabase.dBSchema}.staff_members
                                  SET password = '{selectedAccount.password}',
                                      faculty_code = '{selectedAccount.faculty_code}',
                                      first_name = '{selectedAccount.first_name}',
                                      last_name = '{selectedAccount.last_name}',
                                      patronymic = '{selectedAccount.patronymic}',
                                      is_admin = '{selectedAccount.is_admin}',
                                      can_view_requests = {selectedAccount.can_view_requests},
                                      can_view_questions = {selectedAccount.can_view_questions},
                                      can_view_forms = {selectedAccount.can_view_forms}
                                  WHERE login = '{selectedAccount.login}'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                cmd.ExecuteNonQuery();

                query = $@"DELETE FROM {OurDatabase.dBSchema}.staff_request_privileges WHERE login = '{selectedAccount.login}'";
                cmd = new NpgsqlCommand(query, dBconnection);
                cmd.ExecuteNonQuery();
                if(selectedAccount.request_privileges.Any() == true)
                {
                    query = $@"INSERT INTO {OurDatabase.dBSchema}.staff_request_privileges (login, request_code) VALUES ('{selectedAccount.login}', '{selectedAccount.request_privileges.First()}')";
                    for (int i = 1; i < selectedAccount.request_privileges.Count; i++)
                        query += $@",('{selectedAccount.login}','{selectedAccount.request_privileges[i]}')";
                    cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetAccountList();
            FillAccountListBox();
        }

        private void can_view_formsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            selectedAccount.can_view_forms = !selectedAccount.can_view_forms;
        }

        private void can_view_questionsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            selectedAccount.can_view_questions = !selectedAccount.can_view_questions;
        }

        private void can_view_requestsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            selectedAccount.can_view_requests = !selectedAccount.can_view_requests;
        }

        private void is_adminCheckBox_Click(object sender, RoutedEventArgs e)
        {
            selectedAccount.is_admin = !selectedAccount.is_admin;
        }

        private void request_privilegeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string privilege = checkBox.Tag as string;
            selectedAccount.request_privileges.Remove(privilege);
        }

        private void request_privilegeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string privilege = checkBox.Tag as string;
            selectedAccount.request_privileges.Add(privilege);
        }

        private void facultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
            selectedAccount.faculty_code = item.Tag as string;
        }

        private void patronymicTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            selectedAccount.patronymic = txt.Text;
        }

        private void first_nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            selectedAccount.first_name = txt.Text;
        }

        private void last_nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            selectedAccount.last_name = txt.Text;
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            selectedAccount.password = txt.Text;
        }

        private void addAccountButton_Click(object sender, RoutedEventArgs e)
        {
            selectedAccount = new Staff_Member();
            StackPanel st = new StackPanel();
            accountsWorkingArea.Content = st;

            //Заголовок
            TextBlock header = new TextBlock()
            {
                Text = "Информация об аккаунте сотрудника:\n",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(header);

            //login
            TextBlock loginTextBlock = new TextBlock()
            {
                Text = "Логин:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(loginTextBlock);

            TextBox loginTextBox = new TextBox()
            {
                Text = selectedAccount.login,
                IsReadOnly = false,
            };
            loginTextBox.TextChanged += loginTextBox_TextChanged;
            st.Children.Add(loginTextBox);

            //пароль
            TextBlock passwordTextBlock = new TextBlock()
            {
                Text = "Пароль:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(passwordTextBlock);

            TextBox passwordTextBox = new TextBox()
            {
                Text = selectedAccount.password,
            };
            passwordTextBox.TextChanged += passwordTextBox_TextChanged;
            st.Children.Add(passwordTextBox);

            //фамилия
            TextBlock last_nameTextBlock = new TextBlock()
            {
                Text = "Фамилия:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(last_nameTextBlock);

            TextBox last_nameTextBox = new TextBox()
            {
                Text = selectedAccount.last_name,
            };
            last_nameTextBox.TextChanged += last_nameTextBox_TextChanged;
            st.Children.Add(last_nameTextBox);

            //имя
            TextBlock first_nameTextBlock = new TextBlock()
            {
                Text = "Имя:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(first_nameTextBlock);

            TextBox first_nameTextBox = new TextBox()
            {
                Text = selectedAccount.first_name,
            };
            first_nameTextBox.TextChanged += first_nameTextBox_TextChanged;
            st.Children.Add(first_nameTextBox);

            //отчество
            TextBlock patronymicTextBlock = new TextBlock()
            {
                Text = "Отчество:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(patronymicTextBlock);

            TextBox patronymicTextBox = new TextBox()
            {
                Text = selectedAccount.patronymic,
            };
            patronymicTextBox.TextChanged += patronymicTextBox_TextChanged;
            st.Children.Add(patronymicTextBox);

            //факультет
            TextBlock facultyTextBlock = new TextBlock()
            {
                Text = "Факультет:",
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(facultyTextBlock);

            ComboBox facultyComboBox = new ComboBox();
            foreach (string[] line in OurDatabase.facultyNamesList)
            {
                ComboBoxItem facultyComboBoxItem = new ComboBoxItem()
                {
                    Content = line[1],
                    Tag = line[0]
                };
                facultyComboBox.Items.Add(facultyComboBoxItem);

            }
            selectedAccount.faculty_code = OurDatabase.facultyNamesList[0][0];
            facultyComboBox.SelectedIndex = 0;
            facultyComboBox.SelectionChanged += facultyComboBox_SelectionChanged;
            st.Children.Add(facultyComboBox);

            //разрешения
            TextBlock permissions = new TextBlock()
            {
                Text = "\nРазрешения:",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(permissions);

            CheckBox is_adminCheckBox = new CheckBox()
            {
                Content = "Является администратором?",
                IsChecked = selectedAccount.is_admin,
            };
            is_adminCheckBox.Click += is_adminCheckBox_Click;
            st.Children.Add(is_adminCheckBox);

            CheckBox can_view_requestsCheckBox = new CheckBox()
            {
                Content = "Доступ ко вкладке \"Заявки\"?",
                IsChecked = selectedAccount.can_view_requests,
            };
            can_view_requestsCheckBox.Click += can_view_requestsCheckBox_Click;
            st.Children.Add(can_view_requestsCheckBox);

            CheckBox can_view_questionsCheckBox = new CheckBox()
            {
                Content = "Доступ ко вкладке \"Вопросы\"?",
                IsChecked = selectedAccount.can_view_questions,
            };
            can_view_questionsCheckBox.Click += can_view_questionsCheckBox_Click;
            st.Children.Add(can_view_questionsCheckBox);

            CheckBox can_view_formsCheckBox = new CheckBox()
            {
                Content = "Доступ ко вкладке \"Анкетирование\"?",
                IsChecked = selectedAccount.can_view_forms,
            };
            can_view_formsCheckBox.Click += can_view_formsCheckBox_Click;
            st.Children.Add(can_view_formsCheckBox);


            //разрешения по заявкам:
            TextBlock requestsPermissions = new TextBlock()
            {
                Text = "\nРазрешения по просмотру заявлений:",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            st.Children.Add(requestsPermissions);

            foreach (string[] line in OurDatabase.requestNamesList)
            {
                CheckBox request_privilegeCheckBox = new CheckBox()
                {
                    Content = line[1],
                    Tag = line[0],
                    IsChecked = selectedAccount.request_privileges.Contains(line[0]),
                };
                request_privilegeCheckBox.Checked += request_privilegeCheckBox_Checked;
                request_privilegeCheckBox.Unchecked += request_privilegeCheckBox_Unchecked;
                st.Children.Add(request_privilegeCheckBox);
            }

            //сохранение изменений
            Button createAccountButton = new Button()
            {
                Content = "Создать аккаунт",
                Background = Brushes.Blue,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            createAccountButton.Click += createAccountButton_Click;
            st.Children.Add(createAccountButton);
        }

        private void loginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            selectedAccount.login = textBox.Text;
        }

        private void createAccountButton_Click(object sender, RoutedEventArgs e)
        {
            accountsWorkingArea.Content = null;
            try
            {
                string query = $@"INSERT INTO {OurDatabase.dBSchema}.staff_members (login, password, faculty_code, first_name, last_name, patronymic, is_admin, can_view_requests, can_view_questions, can_view_forms)
                VALUES('{selectedAccount.login}','{selectedAccount.password}', '{selectedAccount.faculty_code}', '{selectedAccount.first_name}', '{selectedAccount.last_name}', '{selectedAccount.patronymic}', {selectedAccount.is_admin}, {selectedAccount.can_view_requests}, {selectedAccount.can_view_questions}, {selectedAccount.can_view_forms})";
                NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                cmd.ExecuteNonQuery();
                if (selectedAccount.request_privileges.Any() == true)
                {
                    query = $@"INSERT INTO {OurDatabase.dBSchema}.staff_request_privileges (login, request_code) VALUES ('{selectedAccount.login}', '{selectedAccount.request_privileges.First()}')";
                    for (int i = 1; i < selectedAccount.request_privileges.Count; i++)
                        query += $@",('{selectedAccount.login}','{selectedAccount.request_privileges[i]}')";
                    cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetAccountList();
            FillAccountListBox();
        }
    }

    
}
