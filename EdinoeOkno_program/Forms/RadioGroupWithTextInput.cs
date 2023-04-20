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
    internal class RadioGroupWithTextInput : IForms_Element
    {

        public static string noneInputValue = "radio-dop";
        public static string css_class = "form-control";
        public static string other_class = "form-other";
        private int id_question;
        private List<int> id_answers = new List<int>();

        private int maxInputLength = 20;

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
            Text = "Тип: Один из списка c текстовым вводом",
            Foreground = Brushes.Gray,
        };

        private CheckBox requiredAnswer = new CheckBox()
        {
            Content = "Обязательный вопрос?",
            HorizontalAlignment = HorizontalAlignment.Right,
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

        private StackPanel otherRow = new StackPanel()
        {
            Orientation = Orientation.Horizontal
        };

        private RadioButton otherRadioButton = new RadioButton()
        {
            Content = "",
        };

        private TextBlock otherTextBlock = new TextBlock()
        {
            Text = "Другое: ",
        };

        private TextBox otherTextBox = new TextBox()
        {
            Text = "Ввод ответа",
            FontStyle = FontStyles.Italic,
            IsReadOnly = true,
            Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
        };

        private StackPanel maxInputLengthRow = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
        };

        private TextBlock maxInputLengthTextBlock = new TextBlock()
        {
            Text = "Максимальная длина ввода:",
        };

        private TextBox maxInputLengthBox = new TextBox()
        {
            Text = "20",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
        };


        public RadioGroupWithTextInput()
        {
            miscRow.Children.Add(description);
            miscRow.Children.Add(requiredAnswer);
            otherRow.Children.Add(otherRadioButton);
            otherRow.Children.Add(otherTextBlock);
            otherRow.Children.Add(otherTextBox);
            maxInputLengthRow.Children.Add(maxInputLengthTextBlock);
            maxInputLengthRow.Children.Add(maxInputLengthBox);
            maxInputLengthBox.TextChanged += maxInputLengthBox_TextChanged;

            body.Children.Add(miscRow);
            body.Children.Add(title);
            body.Children.Add(addAnswerButton);
            addAnswerButton.Click += addAnswerButton_Click;

            AddAnswer("Вариант 1");
            AddAnswer("Вариант 2");
            AddAnswer("Вариант 3");

            body.Children.Add(answersArea);
            body.Children.Add(otherRow);
            body.Children.Add(maxInputLengthRow);
        }

        private void addAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            AddAnswer("Вариант ответа");
        }

        private void maxInputLengthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(maxInputLengthBox.Text, out _))
            {
                maxInputLength = int.Parse(maxInputLengthBox.Text);
            }
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
                    string query = $@"INSERT INTO forms.questions(id_form,name_question,type_question,is_required) VALUES ('{id_form}','{title.Text}','radio_with_text_input',{requiredAnswer.IsChecked}) RETURNING id_question;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    id_question = Convert.ToInt32(cmd.ExecuteScalar());
                    foreach (var answer in answersArea.Children)
                    {
                        query = $@"INSERT INTO forms.answers(id_question,name_answer) VALUES ('{id_question}','{((answer as StackPanel).Tag as TextBox).Text}') RETURNING id_answer;";
                        cmd = new NpgsqlCommand(query, dBconnection);
                        id_answers.Add(Convert.ToInt32(cmd.ExecuteScalar()));
                    }
                    query = $@"INSERT INTO forms.answers(id_question,name_answer,is_text_input,max_text_input) VALUES ('{id_question}','Другое:', true, {maxInputLength}) RETURNING id_answer;";
                    cmd = new NpgsqlCommand(query, dBconnection);
                    id_answers.Add(Convert.ToInt32(cmd.ExecuteScalar()));
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
                $"\t<input class = \"none\" value=\"{noneInputValue}\" name=\"{number}_radio[]\">\n" +
                $"\t<label>{title.Text}</label>\n";
            int i = 0;
            foreach (var answer in answersArea.Children)
            {
                result += $"\t\t<label><input{isRequired} value=\"{id_question}, {id_answers[i]}\" name=\"{number}_radio[]\" type=\"radio\">\n" +
                        $"\t\t{((answer as StackPanel).Tag as TextBox).Text}</label>\n";
                i++;
            }

            result += $"\t\t<div class = \"{other_class}\"><input value=\"{id_question}, {id_answers[i]},\" name=\"{number}_radio[]\" type=\"radio\">\n" +
                    $"\t\t\t<input name=\"{number}_radio[]\" maxlength=\"{maxInputLength}\" type=\"text\" placeholder=\"Ваш вариант\">\n" +
                    $"\t\t</div>\n";

            result += "</div>\n";
            return result;
        }
        public string GetPreviewHtml(int number)
        {
            string result;
            string isRequired = requiredAnswer.IsChecked == true ? " required" : "";

            result = $"<div class=\"{css_class}\">\n" +
                $"\t<input class = \"none\" value=\"{noneInputValue}\" name=\"{number}_radio[]\">\n" +
                $"\t<label>{title.Text}</label>\n";
            foreach (var answer in answersArea.Children)
            {
                result += $"\t\t<label><input{isRequired} value=\"'id_question', 'id_answer',\" name=\"{number}_radio[]\" type=\"radio\">\n" +
                        $"\t\t{((answer as StackPanel).Tag as TextBox).Text}</label>\n";
            }

            result += $"\t\t<div class = \"{other_class}\"><input value=\"'id_question', 'id_answer',\" name=\"{number}_radio[]\" type=\"radio\">\n" +
                    $"\t\t\t<input name=\"{number}_radio[]\" maxlength=\"{maxInputLength}\" type=\"text\" placeholder=\"Ваш вариант\">\n" +
                    $"\t\t</div>\n";

            result += "</div>\n";
            return result;
        }
    }
}
