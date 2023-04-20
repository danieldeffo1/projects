using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EdinoeOkno_program
{
    internal static class Authorization
    {
        static public Staff_Member account;
        static public bool logged = false;
        static public Grid requestGrid;
        static public Grid questionsGrid;
        static public Grid adminGrid;
        static public Grid formsGrid;

        static public void LogIn(Staff_Member acc)
        {
            account = acc;
            logged = true;

            if(account.can_view_requests == true || account.is_admin == true)
            {
                requestGrid.Children.Clear();
                requestGrid.Children.Add(new Requests_UserConrol());
            }
            else
            {
                requestGrid.Children.Clear();
                requestGrid.Children.Add(DefaultMessage());
            }

            if(account.can_view_questions == true || account.is_admin == true)
            {
                questionsGrid.Children.Clear();
                questionsGrid.Children.Add(new Questions_UserControl());
            }
            else
            {
                questionsGrid.Children.Clear();
                questionsGrid.Children.Add(DefaultMessage());
            }

            if (account.can_view_forms == true || account.is_admin == true)
            {
                formsGrid.Children.Clear();
                formsGrid.Children.Add(new Forms_UserControl());
            }
            else
            {
                formsGrid.Children.Clear();
                formsGrid.Children.Add(DefaultMessage());
            }

            if(account.is_admin == true)
            {
                adminGrid.Children.Clear();
                adminGrid.Children.Add(new Аdministration_UserControl());
            }
            else
            {
                adminGrid.Children.Clear();
                adminGrid.Children.Add(DefaultMessage());
            }
        }

        static private TextBlock DefaultMessage()
        {
            return new TextBlock()
            {
                Text = "У этого аккаунта нет прав для просмотра этой страницы",
                FontSize = 35,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                TextAlignment = System.Windows.TextAlignment.Center,
                TextWrapping = System.Windows.TextWrapping.Wrap
            };
        }

        static public void LogOut()
        {
            logged = false;

            requestGrid.Children.Clear();
            questionsGrid.Children.Clear();
            formsGrid.Children.Clear();
            adminGrid.Children.Clear();

            requestGrid.Children.Add(DefaultMessage());
            questionsGrid.Children.Add(DefaultMessage());
            formsGrid.Children.Add(DefaultMessage());
            adminGrid.Children.Add(DefaultMessage());
        }
    }
}
