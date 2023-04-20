using System;
using System.Collections.Generic;
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
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EdinoeOkno_program
{
    public class MailSend
    {
        public SmtpClient mySmtpClient = new SmtpClient("smtp.mail.ru");
        public string ourAddress = "edinoeokno@internet.ru";
        public string password = "Y23xgi7F4zFZLMvbBxVz";
        public string ourName = "МФЦ \"Единое Окно\"";

        public MailSend()
        {
            //password = "";
            mySmtpClient.UseDefaultCredentials = true;
            mySmtpClient.EnableSsl = true; // использование шифрования по SSL
            System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(ourAddress, password);
            mySmtpClient.Credentials = basicAuthenticationInfo;
        }

        public MailSend(string smptServer, string usingMail, string password)
        {
            this.mySmtpClient = new SmtpClient(smptServer); //Сервер исходящей почты (SMTP-сервер)
            // set smtp-client with basicAuthentication
            mySmtpClient.UseDefaultCredentials = true;
            mySmtpClient.EnableSsl = true; // использование шифрования по SSL
            System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(usingMail, password);
            mySmtpClient.Credentials = basicAuthenticationInfo;
        }


        public bool Send(string addressee, string name, string subject, string messageText)
        {
            try
            {   
                // add from,to mailaddresses
                MailAddress from = new MailAddress(ourAddress, ourName);// от кого с именем отправителя 
                MailAddress to = new MailAddress(addressee, name); // кому
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // add ReplyTo
                MailAddress replyTo = new MailAddress(ourAddress); // этот емаил будет подставлятся при нажатии кнопки "ответить на сообщение"
                myMail.ReplyToList.Add(replyTo);

                // set subject and encoding
                myMail.Subject = subject;    // тема сообшения
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = messageText;  // текст сообщения в формате text or html (если html, то нужно сделать myMail.IsBodyHtml = true)
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = false;

                mySmtpClient.Send(myMail);

                return true;
            }

            catch (SmtpException ex)
            {
                MessageBox.Show(ex.Message);
                //throw new ApplicationException
                //("SmtpException has occured: " + ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw ex;
                return false;
            }

            //MessageBox.Show("Отправлено");
        }

    }
}