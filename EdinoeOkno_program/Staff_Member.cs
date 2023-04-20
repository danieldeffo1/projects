using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdinoeOkno_program
{
    internal class Staff_Member
    {
        public string login = "";
        public string password = "";
        public string faculty_code = "";
        public string first_name = "";
        public string last_name = "";
        public string patronymic = "";
        public bool is_admin = false;
        public bool can_view_requests = false;
        public bool can_view_questions = false;
        public bool can_view_appointments = false;
        public bool can_view_forms = false;

        public List<string> request_privileges = new List<string>();
    }
}
