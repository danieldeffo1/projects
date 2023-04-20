using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace EdinoeOkno_program.Forms
{
    internal class RadioGroup : IForms_Element
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

        private StackPanel miscRow = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
        };

        private TextBlock description = new TextBlock()
        {
            Text = "Тип: Один из списка   ",
            Foreground = Brushes.Gray,
        };

        private CheckBox requiredAnswer = new CheckBox()
        {
            Content = "Обязательный вопрос?"
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
            RadioButton radioButton = new RadioButton()
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

            answer.Children.Add(radioButton);
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

        public RadioGroup()
        {
            miscRow.Children.Add(description);
            miscRow.Children.Add(requiredAnswer);

            body.Children.Add(miscRow);
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
                    string query = $@"INSERT INTO forms.questions(id_form,name_question,type_question,is_required) VALUES ('{id_form}','{title.Text}','radio',{requiredAnswer.IsChecked}) RETURNING id_question;";
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
            string isRequired = requiredAnswer.IsChecked == true ? " required" : "";

            result = $"<div class=\"{css_class}\">\n" +
                $"\t<label>{title.Text}</label>\n";
            int i = 0;
            foreach (var answer in answersArea.Children)
            {
                result += $"\t\t<label><input{isRequired} value=\"{id_question},{id_answers[i]}\" name=\"{number}_radio\" type=\"radio\">\n" +
                        $"\t\t{((answer as StackPanel).Tag as TextBox).Text}</label>\n";
                i++;
            }

            result += "</div>\n";
            return result;
        }
        public string GetPreviewHtml(int number)
        {
            string result;
            string isRequired = requiredAnswer.IsChecked == true ? " required" : "";

            result = $"<div class=\"{css_class}\">\n" +
                $"\t<label>{title.Text}</label>\n";
            foreach (var answer in answersArea.Children)
            {
                result += $"\t\t<label><input{isRequired} value=\"'id_question', 'id_answer',\" name=\"{number}_radio\" type=\"radio\">\n" +
                        $"\t\t{((answer as StackPanel).Tag as TextBox).Text}</label>\n";
            }

            result += "</div>\n";
            return result;
        }
    }
}
