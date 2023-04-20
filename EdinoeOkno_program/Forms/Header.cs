using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EdinoeOkno_program.Forms
{
    internal class Header : IForms_Element
    {
        public static string css_class = "form-control-text";

        private StackPanel body = new StackPanel()
        {
            Background = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        private TextBlock description = new TextBlock()
        {
            Text = "Тип: Заголовок",
            Foreground = Brushes.Gray,
        };

        private TextBox title = new TextBox()
        {
            Text = "Заголовок",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            TextWrapping = TextWrapping.Wrap,
            Background = new SolidColorBrush(Color.FromArgb(255, 232, 240, 254)),
        };

        public Header()
        {
            body.Children.Add(description);
            body.Children.Add(title);
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
                    string query = $@"INSERT INTO forms.questions(id_form,name_question,type_question) VALUES ('{id_form}','{title.Text}','header');";
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
            return $"<div class=\"{css_class}\">\n\t<h1>{title.Text}</h1>\n</div>\n"; ; 
        }
        public string GetPreviewHtml(int number)
        {
            return $"<div class=\"{css_class}\">\n\t<h1>{title.Text}</h1>\n</div>\n";
        }
    }
}
