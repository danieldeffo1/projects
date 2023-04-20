using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdinoeOkno_program
{
    internal class Question
    {
        public int question_id;
        public string subject;
        public string body;
        public string status_code;
        public string status_name;
        public string status_short_name;
        public string first_name;
        public string last_name;
        public string patronymic;
        public string email;
        public string faculty_code;
        public string faculty_name;
        public string faculty_short_name;
        public string time_when_added;
        public string time_when_updated = "-";
        public string staff_member_login = "-";
        public string response_content;
    }
}
