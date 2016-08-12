using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using IntegracionPDF.Integracion_PDF.Utils.Email;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion;
using IntegracionPDF.Integracion_PDF.Utils.Popup;

namespace IntegracionPDF.Integracion_PDF.Utils
{
    public static class Log
    {

        #region VARIABLES
        public static List<MailCencosFaltantes> CencosFaltantes { private get; set; }

        public static List<MailProblemaConversionUnidades> ProblemaConversionUnidades { private get; set; }

        public static List<MailSkuFaltante> SkuFaltantes { private get; set; }

        public static List<OrdenCompraIntegracion> OrdenesProcesadas { get; set; }

        public static List<Popup.Popup> PopupsEjecutivos { get; set; }

        #endregion

        public static void Save(string title, string save)
        {
            try
            {
                var writer = new StreamWriter(InternalVariables.GetLogFolder() + @"\Integración_Log.txt", true);
                writer.WriteLine($"{DateTime.Now} - {title} - {save}");
                writer.Close();
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                Save(title, save);
            }

        }

        public static void Save(string save)
        {
            try
            {
                var writer = new StreamWriter(InternalVariables.GetLogFolder() + @"\Integración_Log.txt", true);
                writer.WriteLine($"{DateTime.Now} - {save}");
                writer.Close();
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                Save(save);
            }

        }

        public static void SaveItemFaltantes(string rutCli,string cab, string save)
        {
            try
            {
                var writer = new StreamWriter(InternalVariables.GetLogFolder() + @"\FaltaPareoSKU_Cliente.txt", true);
                writer.WriteLine($"{DateTime.Now} - {cab} {save}");
                writer.Close();
                AddMailSkuFaltantes(rutCli, save);
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                SaveItemFaltantes(rutCli,cab, save);
            }

        }

        public static void TryError(string save)
        {
            try
            {
                var writer = new StreamWriter(InternalVariables.GetLogFolder() + @"\Error_Log.txt", true);
                writer.WriteLine($"{DateTime.Now} - {save}");
                writer.Close();
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                TryError(save);
            }
        }

        public static void StartApp()
        {
            var fecha = DateTime.Now;
            Save("Inicio",
                $"Aplicación Iniciada el {fecha.ToShortDateString()}" + $" a las {fecha.ToLongTimeString()} Hrs.");
        }

        public static void FinalizeApp()
        {
            var fecha = DateTime.Now;
            Save("Fin",
                $"Aplicación Finalizada el {fecha.ToShortDateString()} a las {fecha.ToLongTimeString()} Hrs.");
        }

        public static void SaveCentroCostoFaltantes(string rutCli, string ccosto)
        {
            try
            {

                var writer = new StreamWriter(InternalVariables.GetLogFolder() + @"\FaltaPareoCC_Cliente.txt", true);
                writer.WriteLine(
                    $"{DateTime.Now} - No existe Centro de Costo para el Cliente rut: {rutCli}, con Descripción: {ccosto}");
                writer.Close();
                AddMailCencosFaltantes(rutCli, ccosto);
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                SaveCentroCostoFaltantes(rutCli, ccosto);
            }
        }

        public static void SaveProblemaConversionUnidades(string ocNumber, string rutCli,string codProCarozzi, string codPro, string subPdf, int subMult)
        {
            try
            {

                var writer = new StreamWriter(InternalVariables.GetLogFolder() + @"\ProblemaUnidades.txt", true);
                var subP = subMult/int.Parse(subPdf);
                var asunto =
                    $"Orden de Compra: {ocNumber}, producto: {codProCarozzi} / {codPro}, el Sub Total del PDF es: {subPdf}, el Sub Total de Tabla de Conversión es: {subMult}";
                var query = $"update re_codcli set multiplo = {subP} where rutcli = {rutCli} and codpro = '{codPro}'";
                writer.WriteLine(
                    $"{DateTime.Now}\t - Problema Subtotal para el Cliente: {rutCli}.\n" +
                    $"\t\t\t\t\t - {asunto}\n" +
                    "\t\t\t\t\t - Revisar Tabla de Conversión de Unidades.\n" +
                    $"\t\t\t\t\t - Si el sistema está en los correcto, el multiplo debe ser: {subP},\n" +
                    "\t\t\t\t\t - Favor de Actualizar la Tabla de Conversiones con el siguiente Comando SQL:\n" +
                    $"\t\t\t\t\t - {query}");
                writer.Close();
                asunto += $". El multiplo debe ser: {subP}.";
                AddMailProblemaConversionUnidades(rutCli, asunto, query);
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                SaveProblemaConversionUnidades(ocNumber, rutCli, codProCarozzi, codPro, subPdf, subMult);
            }
        }


        #region Mail's

        public static void AddMailUpdateTelemarketing(OrdenCompraIntegracion oc)
        {
            OrdenesProcesadas.Add(oc);
        }

        private static void AddPopupTelemarketing(Popup.Popup p)
        {
            PopupsEjecutivos.Add(p);
            //var exist = false;
            //foreach (var pop in PopupsEjecutivos)
            //{
            //    if (pop.RutUsuario.Equals(p.RutUsuario))
            //    {
            //        Console.WriteLine("EXISTE RUT: "+p.RutUsuario);
            //        foreach(var d in p.Detalles)
            //            p.AddDetallePopup(d);
            //        exist = true;
            //    }
            //}
            //if(!exist)
            //    PopupsEjecutivos.Add(p);
        }

        private static void AddDetallePopupToRutUsuario(string rutUsuario, DetallePopup det)
        {
            foreach (var pop in from pop in PopupsEjecutivos.Where(pop => pop.RutUsuario.Equals(rutUsuario)) from d in pop.Detalles.Where(d => d.RutCli.Equals(det.RutCli)) select pop)
            {
                pop.AddDetallePopup(det);
            }
        }

        private static void AddMailCencosFaltantes(string rutCli, string ccosto)
        {
            var m = CencosFaltantes.Find(mail => mail.RutCliente.Equals(rutCli));
            if (m != null)
            {
                m.Body.Add($"No existe Centro de Costo con Descripción: {ccosto}");
            }
            else
            {
                var razon = OracleDataAccess.GetRazonSocial(rutCli);
                var email = OracleDataAccess.GetEmailFromRutCliente(rutCli);
                CencosFaltantes.Add(new MailCencosFaltantes(rutCli, 
                    razon,
                    email,
                    $"No existe Centro de Costo con Descripción: {ccosto}"));
            }
        }
        
        private static void AddMailProblemaConversionUnidades(string rutCli, string asunto, string query)
        {
            var m = ProblemaConversionUnidades.Find(mail => mail.RutCliente.Equals(rutCli));
            if (m != null)
            {
                m.Body.Add(asunto);
                m.Querys.Add(query);
            }
            else
            {
                var razon = OracleDataAccess.GetRazonSocial(rutCli);
                var email = OracleDataAccess.GetEmailFromRutCliente(rutCli);
                ProblemaConversionUnidades.Add(new MailProblemaConversionUnidades(rutCli,
                    razon,
                    email,
                    asunto, query));
            }
        }

        private static void AddMailSkuFaltantes(string rutCli, string asunto)
        {
            var m = SkuFaltantes.Find(mail => mail.RutCliente.Equals(rutCli));
            if (m != null)
            {
                m.Body.Add(asunto);
            }
            else
            {
                var razon = OracleDataAccess.GetRazonSocial(rutCli);
                var email = OracleDataAccess.GetEmailFromRutCliente(rutCli);
                SkuFaltantes.Add(new MailSkuFaltante(rutCli,
                    razon,
                    email,
                    asunto));
            }
        }

        private static void SendMailCencosFaltantes()
        {
            if (CencosFaltantes.Count == 0) return;
            foreach (var mail in CencosFaltantes)
            {
                Email.Email.
                    SendEmailFromProcesosXmlDimerc(
                        InternalVariables.GetMainEmail()
                        , null
                        , mail.Subject
                        , mail.MailBody);
            }
            CencosFaltantes = new List<MailCencosFaltantes>();
        }


        private static void SendMailProblemasConversion()
        {
            if (ProblemaConversionUnidades.Count == 0) return;
            foreach (var mail in ProblemaConversionUnidades)
            {
                Email.Email.
                    SendEmailFromProcesosXmlDimerc(
                        InternalVariables.GetMainEmail(),
                        null,
                        InternalVariables.IsDebug()
                            ? $"DEBUG {InternalVariables.GetSubjectDebug()}: {mail.Subject}"
                            : mail.Subject,
                        mail.MailBody);
            }
            ProblemaConversionUnidades = new List<MailProblemaConversionUnidades>();
        }

        private static void SendMailSkuFaltantes()
        {
            if (SkuFaltantes.Count == 0) return;
            foreach (var mail in SkuFaltantes)
            {
                Email.Email.
                    SendEmailFromProcesosXmlDimerc(
                        InternalVariables.GetMainEmail(),
                        null,
                        InternalVariables.IsDebug()
                            ? $"DEBUG {InternalVariables.GetSubjectDebug()}: {mail.Subject}"
                            : mail.Subject,
                        mail.MailBody);
            }
            SkuFaltantes = new List<MailSkuFaltante>();
        }

        private static void SendMailOrdenesProcesadas()
        {
            if (OrdenesProcesadas.Count == 0) return;
            var lastRut = 0;
            var subject = $"Fecha: {DateTime.Now} - Ordenes Procesadas: {OrdenesProcesadas.Count}";
            var body = $"Fecha: {DateTime.Now} - Ordenes Procesadas: {OrdenesProcesadas.Count}\n";
            var first = true;
            foreach (var oc in OrdenesProcesadas.OrderBy(o => o.RutCli))
            {
                oc.EmailEjecutivo = OracleDataAccess.GetEmailFromRutCliente(oc.RutCli.ToString());
                oc.RutUsuario = OracleDataAccess.GetRutUsuarioFromRutCliente(oc.RutCli.ToString());
                oc.Razon = OracleDataAccess.GetRazonSocial(oc.RutCli.ToString());
                if (!lastRut.Equals(oc.RutCli))
                {
                    lastRut = oc.RutCli;
                    var pop = new Popup.Popup
                    {
                        RutUsuario = oc.RutUsuario
                    };
                    var d = new DetallePopup
                    {
                        RutCli = oc.RutCli.ToString(),
                        Razon = oc.Razon,
                        CantidadOrdenes = 1
                    };
                    //Console.WriteLine($"DETALLE POPUP: {d.ToString()}");
                    pop.AddDetallePopup(d);
                    AddPopupTelemarketing(pop);
                    body +=
                        $"{(first ? "\n\n" : "\n\n\n")}Empresa: {oc.Razon}, Rut: {oc.RutCli}, Mail ejecutivo: {oc.EmailEjecutivo}\n\n" +
                        $"N° Pedido: {oc.NumPed}, N° OC Cliente: {oc.OcCliente}, Centro de Costo: {oc.CenCos}, Numero de Items: {oc.DetallesCompra.Count}, Subtotal: " +
                        $"{(from det in oc.DetallesCompra select det.SubTotal).Sum():C0}\n";
                    first = false;
                }
                else
                {
                    var d = new DetallePopup
                    {
                        RutCli = oc.RutCli.ToString(),
                        Razon = oc.Razon,
                        CantidadOrdenes = 1
                    };
                    AddDetallePopupToRutUsuario(oc.RutUsuario, d);
                    body +=
                        $"N° Pedido: {oc.NumPed}, N° OC Cliente: {oc.OcCliente}, Centro de Costo: {oc.CenCos}, Numero de Items: {oc.DetallesCompra.Count}, Subtotal: " +
                        $"{(from det in oc.DetallesCompra select det.SubTotal).Sum():C0}\n";
                }
            }
            Email.Email.
                SendEmailFromProcesosXmlDimerc(
                    InternalVariables.GetMainEmail(),
                    InternalVariables.IsDebug()
                        ? null
                        : InternalVariables.GetEmailCc(),
                    InternalVariables.IsDebug()
                        ? $"DEBUG {InternalVariables.GetSubjectDebug()}: {subject}"
                        : subject,
                    body);
            OrdenesProcesadas = new List<OrdenCompraIntegracion>();
        }

        public static void SendMails()
        {
            SendMailCencosFaltantes();
            SendMailProblemasConversion();
            SendMailSkuFaltantes();
            SendMailOrdenesProcesadas();
            SendPopupTelemarketing();
        }

        private static void SendPopupTelemarketing()
        {
            if (InternalVariables.SendPopup())
            {
                //TODO ENVIAR POPUP'S
                foreach (var pop in PopupsEjecutivos)
                {
                    Console.WriteLine($"POPUP: {pop.ToString()}");
                    OracleDataAccess.InsertPopupTelemarketing(pop);
                }
            }
            PopupsEjecutivos = new List<Popup.Popup>();
        }

        #endregion

        public static void InitializeVariables()
        {
            CencosFaltantes = new List<MailCencosFaltantes>();
            ProblemaConversionUnidades = new List<MailProblemaConversionUnidades>();
            SkuFaltantes = new List<MailSkuFaltante>();
            OrdenesProcesadas = new List<OrdenCompraIntegracion>();
            PopupsEjecutivos = new List<Popup.Popup>();
        }
    }
}
