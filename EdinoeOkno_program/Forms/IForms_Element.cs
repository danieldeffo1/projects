using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EdinoeOkno_program.Forms
{
    internal interface IForms_Element
    {
        string GetPreviewHtml(int number);
        string GetHTML(int number);
        StackPanel GetUIElement();
        void CreateDBElement(int id_form, NpgsqlConnection dBconnection);

    }
}
