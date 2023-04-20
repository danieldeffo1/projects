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
    internal class Paragraph : IForms_Element
    {
        public static string css_class = "form-control";

        private StackPanel body = new StackPanel()
        {
            Background = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        private TextBlock description = new TextBlock()
        {
            Text = "Тип: Параграф",
            Foreground = Brushes.Gray,
        };

        private TextBox paragraph_text = new TextBox()
        {
            Text = "Параграф",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            TextWrapping = TextWrapping.Wrap,
            Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
        };

        public Paragraph()
        {
            body.Children.Add(description);
            body.Children.Add(paragraph_text);
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
                    string query = $@"INSERT INTO forms.questions(id_form,name_question,type_question) VALUES ('{id_form}','{paragraph_text.Text}','paragraph');";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, dBconnection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public string GetHTML(int number)
        {
            return $"<div class=\"{css_class}\">\n" +
                $"\t<label>{paragraph_text.Text}\n</label>" +
                $"</div>\n";
        }
        public string GetPreviewHtml(int number)
        {
            return $"<div class=\"{css_class}\">\n" +
                $"\t<label>{paragraph_text.Text}\n</label>" +
                $"</div>\n";
        }
    }
}
