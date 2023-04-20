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
using Npgsql;

namespace EdinoeOkno_program
{
    /// <summary>
    /// Логика взаимодействия для Authorization_UserControl.xaml
    /// </summary>
    public partial class Authorization_UserControl : UserControl
    {
        string login = "";
        string password = "";
        bool logged = false;

        NpgsqlConnection dBconnection = OurDatabase.GetConnection();
        const string view = "accounts_v";

        public Authorization_UserControl()
        {
            InitializeComponent();
        }

        private void loginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            login = loginTextBox.Text;
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            password = passwordTextBox.Text;
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            authButton.IsEnabled = false;
            if (logged == false)
            {
                if (dBconnection.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        using (NpgsqlCommand cmd =
                        new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.{view} WHERE login = '{login}' AND password = '{password}'", dBconnection))
                        {
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    password = "";
                                    passwordTextBox.Text = "";
                                    loginTextBox.IsEnabled = false;
                                    passwordTextBox.IsEnabled = false;
                                    feedTextBlock.Foreground = Brushes.Lime;
                                    feedTextBlock.Text = $"Выполнен вход в аккаунт {login}\nДля выхода нажмите кнопку \"Выйти\"";
                                    authButton.Content = "Выйти";
                                    logged = true;
                                    authButton.IsEnabled = true;
                                    TabItem tabItem = new TabItem();
                                    Staff_Member account;
                                    foreach (DbDataRecord dB in reader)
                                    {
                                        account = new Staff_Member()
                                        {
                                            login = (string)dB["login"],
                                            first_name = (string)dB["first_name"],
                                            last_name = (string)dB["last_name"],
                                            patronymic = (string)dB["patronymic"],
                                            is_admin = (bool)dB["is_admin"],
                                            can_view_requests = (bool)dB["can_view_requests"],
                                            can_view_questions = (bool)dB["can_view_questions"],
                                            can_view_appointments = (bool)dB["can_view_appointments"],
                                            can_view_forms = (bool)dB["can_view_forms"],
                                            request_privileges = ((string)dB["request_privileges"]).Split(',').ToList<string>()
                                        };
                                        Authorization.LogIn(account);
                                    }
                                }
                                else
                                {
                                    feedTextBlock.Foreground = Brushes.Red;
                                    feedTextBlock.Text = "Неверный логин или пароль";
                                    authButton.IsEnabled = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        authButton.IsEnabled = true;
                    }
                } 
            }
            else
            {
                loginTextBox.IsEnabled = true;
                passwordTextBox.IsEnabled = true;
                feedTextBlock.Foreground = Brushes.Black;
                feedTextBlock.Text = "Пожалуйста, введите ваш логин и пароль сотрудника";
                authButton.Content = "Войти";
                logged = false;
                authButton.IsEnabled = true;

                Authorization.LogOut();
            }
        }
    }
}
