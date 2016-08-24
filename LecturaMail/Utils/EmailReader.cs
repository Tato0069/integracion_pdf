using System;
using System.Collections.Generic;
using System.Text;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
namespace LecturaMail.Utils
{
    public static class EmailReader
    {
        public static Dictionary<string, IMail> GetAllMailLecturaMailSubject()
        {
            using (Pop3 pop3 = new Pop3())
            {
                pop3.Connect("mail.dimerc.cl");
                pop3.Login("procesosxml@dimerc.cl", "password_123");
                //Console.WriteLine($"===============CORREOS POP3===============");
                List<string> uidList = pop3.GetAll();
                Dictionary<string, IMail> mails = new Dictionary<string, IMail>();
                foreach (string uid in uidList)
                {
                    IMail email = new MailBuilder().CreateFromEml(
                        pop3.GetMessageByUID(uid));
                    if (email.Subject.Equals("LECTURA MAIL"))
                    {
                        mails.Add(uid, email);
                        //ProcessMessage(email);
                    }
                }
                pop3.Close();
                return mails;
            }
        }

        public static void DeleteMail(this IMail email, string uid)
        {
            if (InternalVariables.IsDebug()) return;
            using (Pop3 pop3 = new Pop3())
            {
                pop3.Connect("mail.dimerc.cl");
                pop3.Login("procesosxml@dimerc.cl", "password_123");
                pop3.DeleteMessageByUID(uid);
                pop3.Close();
            }
        }

        public static List<string> GetBodyAsList(this IMail email)
        {
            var splitText = email.Text.Split('\n').BorrarLineasBlancas();
            var ret = new List<string>();
            foreach(var s in splitText)
            {
                ret.Add(s);
            }
            return ret;
        }

        private static void ProcessMessage(IMail email)
        {
            Console.WriteLine("Subject: " + email.Subject);
            Console.WriteLine("From: " + JoinAddresses(email.From));
            Console.WriteLine("To: " + JoinAddresses(email.To));
            Console.WriteLine("Cc: " + JoinAddresses(email.Cc));
            Console.WriteLine("Bcc: " + JoinAddresses(email.Bcc));
            var text = email.Text.Split('\n').BorrarLineasBlancas();
            //Console.WriteLine("HTML: " + email.Html);

            //Console.WriteLine("Attachments: ");
            //foreach (MimeData attachment in email.Attachments)
            //{
            //    Console.WriteLine(attachment.FileName);
            //    attachment.Save(@"c:\" + attachment.SafeFileName);
            //}
        }

        public static string ConvertToString(this IMail email)
        {
            var splitText = email.Text.Split('\n').BorrarLineasBlancas();
            var ret = $"{email.Subject} {JoinAddresses(email.From)} {JoinAddresses(email.To)} {JoinAddresses(email.Cc)} {JoinAddresses(email.Bcc)} {splitText.ArrayToString(0, splitText.Length - 1)}";
            return ret;
        }


        private static string JoinAddresses(IList<MailAddress> addresses)
        {
            StringBuilder builder = new StringBuilder();

            foreach (MailAddress address in addresses)
            {
                if (address is MailGroup)
                {
                    MailGroup group = (MailGroup)address;
                    builder.AppendFormat("{0}: {1};, ", group.Name, JoinAddresses(group.Addresses));
                }
                if (address is MailBox)
                {
                    MailBox mailbox = (MailBox)address;
                    builder.AppendFormat("{0} <{1}>, ", mailbox.Name, mailbox.Address);
                }
            }
            return builder.ToString();
        }

        private static string JoinAddresses(IList<MailBox> mailboxes)
        {
            return string.Join(",",
                new List<MailBox>(mailboxes).ConvertAll(m => string.Format("{0} <{1}>", m.Name, m.Address))
                .ToArray());
        }
    }
}
