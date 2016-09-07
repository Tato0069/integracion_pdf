using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace LecturaMail.Utils.Email
{
    public static class Email
    {
        public static void SendEmailFromGmail(string to, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("apardo@ofimarket.cl") // mzapata@ofimarket.cl
            };
            mail.To.Add(to);
            mail.To.Add("apardo@ofimarket.cl"); // mzapata@ofimarket.cl
            mail.Subject = subject;
            mail.Body = body;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 25,//465; //587
                Credentials = new NetworkCredential("apardo@ofimarket.cl", "APARDO"),
                EnableSsl = true
            };
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                smtp.Dispose();
            }

        }

        public static void SendEmailFromProcesosXmlDimerc(string[] to, string[] cc, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(InternalVariables.GetEmailFrom())
            };
            foreach (var t in to)
                mail.To.Add(t);
            if (cc != null)
                foreach (var c in cc)
                    mail.CC.Add(c);
            mail.Subject = subject;
            mail.Body = body;
            var smtp = new SmtpClient
            {
                Host = InternalVariables.GetHostEmailFrom(),
                Port = InternalVariables.GetPortEmailFrom(),
                Credentials = new NetworkCredential(
                    InternalVariables.GetEmailFrom()
                    , InternalVariables.GetPasswordEmailFrom()),
                EnableSsl = true
            };
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                smtp.Dispose();
            }
        }

        public static void SendEmailFromProcesosXmlDimercWithAttachments(string[] to, string[] cc, string subject, string body, string[] attachmentPaths)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(InternalVariables.GetEmailFrom())
            };
            foreach (var t in to)
                mail.To.Add(t);
            if (cc != null)
                foreach (var c in cc)
                    mail.CC.Add(c);
            if (attachmentPaths != null)
                foreach (var a in attachmentPaths)
                    mail.Attachments.Add(new Attachment(a));
            mail.Priority = MailPriority.High;
            mail.Subject = subject;
            mail.Body = body;
            var smtp = new SmtpClient
            {
                Host = InternalVariables.GetHostEmailFrom(),
                Port = InternalVariables.GetPortEmailFrom(),
                Credentials = new NetworkCredential(
                    InternalVariables.GetEmailFrom()
                    , InternalVariables.GetPasswordEmailFrom()),
                EnableSsl = true
            };
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                smtp.Dispose();
            }
        }
    }

    public class MailCencosFaltantes
    {
        public MailCencosFaltantes(string rutCli, string razonSocial, string mailEjecutivo, string asunto)
        {
            Body = new List<string> { asunto };
            RutCliente = rutCli;
            RazonSocial = razonSocial;
            MailEjecutivo = mailEjecutivo;
            Subject = $"Falta CC para {razonSocial}";
        }

        public string Subject { get; set; }
        public string RutCliente { get; set; }
        public string RazonSocial { get; set; }
        public string MailEjecutivo { get; set; }

        public List<string> Body { get; set; }

        public string MailBody
        {
            get
            {
                var str = $"Para la Empresa: {RazonSocial}, Rut: {RutCliente}\nEjecutiva(o): {MailEjecutivo}\n";
                str = Body.Aggregate(str, (current, s) => current + (s + "\n"));
                return str;
            }
        }
    }

    public class MailProblemaConversionUnidades
    {
        public MailProblemaConversionUnidades(string rutCli, string razonSocial, string mailEjecutivo, string asunto, string query)
        {
            Body = new List<string> { asunto };
            Querys = new List<string> { query };
            RutCliente = rutCli;
            RazonSocial = razonSocial;
            MailEjecutivo = mailEjecutivo;
            Subject = $"Problema de Conversión de Unidades para {razonSocial}";
        }

        public string Subject { get; set; }

        public string RutCliente { get; set; }
        public string RazonSocial { get; set; }
        public string MailEjecutivo { get; set; }

        public List<string> Body { get; set; }

        public List<string> Querys { get; set; }

        public string MailBody
        {
            get
            {
                var str = $"Para la Empresa: {RazonSocial}\n" +
                          $"Ejecutiva(o): {MailEjecutivo}\n" +
                          "Existen los siguientes problemas de Conversión de Unidades:\n";
                var c = 1;
                str = Body.Aggregate(str, (current, body) => current + $"\t{c++}.-\t{body}\n");
                str += $"\n\nSi los Datos son Correctos, utilizar los siguientes comandos SQL:\n";
                c = 1;
                str = Querys.Aggregate(str, (current, query) => current + $"\t{c++}.-\t{query}\n");
                return str;
            }
        }
    }

    public class MailSkuFaltante
    {
        public MailSkuFaltante(string rutCli, string razonSocial, string mailEjecutivo, string asunto)
        {
            Body = new List<string> { asunto };
            RutCliente = rutCli;
            RazonSocial = razonSocial;
            MailEjecutivo = mailEjecutivo;
            Subject = $"SKU Faltantes para {razonSocial}";
        }

        public string Subject { get; set; }

        public string RutCliente { get; set; }
        public string RazonSocial { get; set; }
        public string MailEjecutivo { get; set; }

        public List<string> Body { get; set; }


        public string MailBody
        {
            get
            {
                var str = $"Para la Empresa: {RazonSocial},  Rut: {RutCliente}\n" +
                          $"Ejecutiva(o): {MailEjecutivo}\n" +
                          "Existen los siguientes productos no poseen pareo de Sku Dimerc:\n";
                var c = 1;
                str = Body.Aggregate(str, (current, body) => current + $"\t{c++}.-\t{body}\n");
                return str;
            }
        }
    }


}