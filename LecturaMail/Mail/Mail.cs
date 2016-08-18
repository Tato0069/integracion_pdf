using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Limilabs.Mail.MIME;
using System.Text;
using Limilabs.Mail.Headers;

namespace LecturaMail.Mail
{
    class Mail
    {
        public static void TestLecturaMailDLL()
        {
            using (Pop3 pop3 = new Pop3())
            {
                pop3.Connect("mail.dimerc.cl");                      // Use overloads or ConnectSSL if you need to specify different port or SSL.
                pop3.Login("procesosxml@dimerc.cl", "password_123");               // You can also use: LoginAPOP, LoginPLAIN, LoginCRAM, LoginDIGEST methods,
                                                                                   // or use UseBestLogin method if you want Mail.dll to choose for you.

                List<string> uidList = pop3.GetAll();       // Get unique-ids of all messages.
                Console.WriteLine($"===============CORREOS POP3=============== {uidList[0]}");
                foreach (string uid in uidList)
                {
                    IMail email = new MailBuilder().CreateFromEml(  // Download and parse each message.
                        pop3.GetMessageByUID(uid));

                    ProcessMessage(email);                          // Display email data, save attachments.
                }
                pop3.Close();
            }
        }
        private static void ProcessMessage(IMail email)
        {
            Console.WriteLine("Subject: " + email.Subject);
            Console.WriteLine("From: " + JoinAddresses(email.From));
            Console.WriteLine("To: " + JoinAddresses(email.To));
            Console.WriteLine("Cc: " + JoinAddresses(email.Cc));
            Console.WriteLine("Bcc: " + JoinAddresses(email.Bcc));

            Console.WriteLine("Text: " + email.Text);
            Console.WriteLine("HTML: " + email.Html);

            Console.WriteLine("Attachments: ");
            foreach (MimeData attachment in email.Attachments)
            {
                Console.WriteLine(attachment.FileName);
                attachment.Save(@"c:\" + attachment.SafeFileName);
            }
        }
        private static string JoinAddresses(IList<Limilabs.Mail.Headers.MailAddress> addresses)
        {
            StringBuilder builder = new StringBuilder();

            foreach (Limilabs.Mail.Headers.MailAddress address in addresses)
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
