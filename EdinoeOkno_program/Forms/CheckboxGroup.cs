using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EdinoeOkno_program.Forms
{
    internal class CheckboxGroup : IForms_Element
    {
        public static string noneInputValue = "check-box";
        public static string css_class = "form-control";
        private int id_question;
        private List<int> id_answers = new List<int>();


        private StackPanel body = new StackPanel()
        {
            Background = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        private TextBlock description = new TextBlock()
        {
            Text = "Тип: Несколько из списка ",
            Foreground = Brushes.Gray,
        };

        private TextBox title = new TextBox()
        {
            Text = "Вопрос",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            TextWrapping = TextWrapping.Wrap,
            Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
        };

        private Button addAnswerButton = new Button()
        {
            Content = "Добавить ответ",
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        private StackPanel answersArea = new StackPanel();

        public void AddAnswer(string ans)
        {
            StackPanel answer = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            CheckBox checkBox = new CheckBox()
            {
                Content = "",
                //IsEnabled = false,
            };
            TextBox textBox = new TextBox()
            {
                Text = ans,
                TextWrapping = TextWrapping.Wrap,
                Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
            };
            Button deleteButton = new Button()
            {
                Content = "Удалить",
                Tag = answer,
            };
            deleteButton.Click += DeleteAnswer;

            answer.Tag = textBox;

            answer.Children.Add(checkBox);
            answer.Children.Add(textBox);
            answer.Children.Add(deleteButton);

            answersArea.Children.Add(answer);
        }

        private void DeleteAnswer(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            StackPanel answer = deleteButton.Tag as StackPanel;
            answersArea.Children.Remove(answer);
        }

        public CheckboxGroup()
        {
            body.Children.Add(description);
            body.Children.Add(title);
            body.Children.Add(addAnswerButton);
            addAnswerButton.Click += addAnswerButton_Click;

            AddAnswer("Вариант 1");
            AddAnswer("Вариант 2");
            AddAnswer("Вариант 3");

            body.Children.Add(answersArea);
        }

        private void addAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            AddAnswer("Вариант ответа");
        }

        public StackPanel GetUIElement()
        {
            return body;
        }
        public void CreateDBElement(int id_form, NpgsqlConnection dBconnection)
        {
            if (dBconnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    string query = $@"INSERT INTO forms.questions(id_form,name_question,type_question) VALUES ('{id_form}','{title.Text}','checkbox') RETURNING id_question;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    id_question = Convert.ToInt32(cmd.ExecuteScalar());
                    foreach (var answer in answersArea.Children)
                    {
                        query = $@"INSERT INTO forms.answers(id_question,name_answer) VALUES ('{id_question}','{((answer as StackPanel).Tag as TextBox).Text}') RETURNING id_answer;";
                        cmd = new NpgsqlCommand(query, dBconnection);
                        id_answers.Add(Convert.ToInt32(cmd.ExecuteScalar()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public string GetHTML(int number)
        {
            string result;
            result = $"<div class=\"{css_class}\">\n" +
                $"\t<input class = \"none\" value=\"{noneInputValue}\" name=\"{number}_checkbox[]\">\n" +
                $"\t<label>{title.Text}</label>\n";
            int i = 0;
            foreach (var answer in answersArea.Children)
            {
                result += $"\t\t<label><input value=\"{id_question}, {id_answers[i]}\" name=\"{number}_checkbox[]\" type=\"checkbox\">\n" +
                        $"\t\t{((answer as StackPanel).Tag as TextBox).Text}</label>\n";
                i++;
            }
            result += "</div>\n";
            return result;
        }
        public string GetPreviewHtml(int number)
        {
            string result;
            result = $"<div class=\"{css_class}\">\n" +
                $"\t<input class = \"none\" value=\"{noneInputValue}\" name=\"{number}_checkbox[]\">\n" +
                $"\t<label>{title.Text}</label>\n";
            foreach (var answer in answersArea.Children)
            {
                result += $"\t\t<label><input value=\"'id_question', 'id_answer',\" name=\"{number}_checkbox[]\" type=\"checkbox\">\n" +
                        $"\t\t{((answer as StackPanel).Tag as TextBox).Text}</label>\n";
            }

            result += "</div>\n";
            return result;
        }
    }
}
