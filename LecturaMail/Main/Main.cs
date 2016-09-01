using System;
using System.Collections.Generic;
using System.Linq;
using LecturaMail.Utils.OrdenCompra;
using LecturaMail.Utils.OrdenCompra.Integracion;
using Limilabs.Mail;
using LecturaMail.Utils;
using LecturaMail.Utils.Integracion.EMAIL.Cinemark;
using LecturaMail.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter;
using LecturaMail.Utils.Oracle.DataAccess;
using LecturaMail.Utils.Integracion.EMAIL.ArcosDorados;

namespace LecturaMail.Main
{
    public static class Main
    {

        private static void ExecuteSingleMail(IMail email)
        {
            Console.WriteLine($"=================LECTURA DE CUERPO DEL CORREO==============================");
            var option = GetOptionNumber(email);
            OrdenCompra ordenCompra = null;
            List<OrdenCompra> ordenCompraList = null;
            var ocAdapter = new OrdenCompraIntegracion();
            var ocAdapterList = new List<OrdenCompraIntegracion>();
            Console.WriteLine($"FIRST: {option}");
            switch (option)
            {
                case 0:
                    Console.WriteLine($"{email.Subject}");
                    //foreach(var b in email.GetBodyAsList())
                    //{
                    //    Console.Write($"{b}");
                    //}
                    var cinemark = new Cinemark(email);
                    ordenCompraList = cinemark.GetOrdenCompra();
                    ocAdapterList.AddRange(ordenCompraList.Select(ord => ord.TraspasoUltimateIntegracion()));


                    //foreach (var o in ordenCompraList)
                    //{
                    //    //Console.WriteLine($" {o}");
                    //    ocAdapter = o.TraspasoUltimateIntegracion();
                    //}
                    break;
                case 1:
                    var arcosDorados = new ArcosDorados(email);
                    ordenCompra = arcosDorados.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                    


            }
            ExecutePostProcess(option, email, ordenCompra, ocAdapter, ocAdapterList);
        }

        public static void ExecuteLecturaMail()
        {
            Console.WriteLine($"===============================================");
            Console.WriteLine($"            INICIO LECTURA MAIL                ");
            Console.WriteLine($"===============================================");
            var emailDictionary = EmailReader.GetAllMailLecturaMailSubject();
            var c = 0;
            foreach (var e in emailDictionary)
            {
                ExecuteSingleMail(e.Value);
                c++;
                e.Value.DeleteMail(e.Key);
            }
            FinishAnalysis(c);
        }


        #region ExecutePostProcess

        private static void ExecutePostProcess(int option, IMail email, OrdenCompra ordenCompra, OrdenCompraIntegracion ocIntegracion, List<OrdenCompraIntegracion> ocAdapterList)
        {
            var totalResult = true;
            if (InternalVariables.IsDebug())
            {
                Console.WriteLine($"PostProcess:{option} \nOC:\n" + ordenCompra);
            }
            if (option != -1)
            {
                if (ocAdapterList != null)
                {
                    foreach (var ocInt in ocAdapterList)
                    {
                        if (!OracleDataAccess.InsertOrdenCompraIntegración(ocInt))
                        {
                            totalResult = false;
                        }
                    }
                    if (!totalResult)
                    {

                    }
                    else
                    {
                        foreach (var ocI in ocAdapterList)
                        {
                            Utils.Log.AddMailUpdateTelemarketing(ocI);
                            Console.WriteLine($"ADAPTER: {ocI}");
                        }
                    }
                }


            }
        }
        /// <summary>
        /// Optiene el Numero de la Empresa a Procesar
        /// </summary>
        /// <param name="pdfReader">PDF Reader</param>
        /// <returns>Número Opción Empresa</returns>
        private static int GetOptionNumber(IMail mail)
        {
            //var mailSplit = mail.ConvertToString();
            var onlyOneLine = mail.ConvertToString();
            var first = -1;
            foreach (var form in InternalVariables.MailsFormats)
            {
                if (form.Value.Contains(";"))
                {
                    var split = form.Value.Split(';');
                    var match = split.Count(sp => onlyOneLine.Contains(sp));
                    if (match == split.Count())
                    {
                        first = form.Key;
                        break;
                    }
                    //if (onlyOneLine.Contains(split[0]) &&
                    //    onlyOneLine.Contains(split[1]))
                    //{
                    //    first = form.Key;
                    //    break;
                    //}
                }
                else if (form.Value.Contains(":"))
                {
                    var split = form.Value.Split(':');
                    if (split.Any(sp => onlyOneLine.Contains(sp)))
                    {
                        first = form.Key;
                    }
                }
                else if (onlyOneLine.Contains(form.Value))
                {
                    first = form.Key;
                    break;
                }
            }
            if (first == -1)
            {
                try
                {
                    onlyOneLine = onlyOneLine.DeleteNullHexadecimalValues();
                    foreach (var format in InternalVariables.MailsFormats.Where(format => onlyOneLine.Contains(format.Value)))
                    {
                        first = format.Key;
                        break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    first = -1;
                }
            }
            return first;
        }


        #endregion


        #region Debug

        public static void DebugAnalizar(string pdfPath)
        {
            foreach (var email in EmailReader.GetAllMailLecturaMailSubject())
            {

            }
        }
        #endregion


        #region ThrowMessages


        private static void SendAlertError()
        {
            //Utils.Email.Email.SendEmailFromProcesosXmlDimerc(
            //    InternalVariables.GetMainEmail(),
            //    null,
            //    "Alerta de Error Consecutivo",
            //    "Han ocurrido más de 3 errores consecutivos con " +
            //    $"el archivo: {InternalVariables.LastPdfPathError}. " +
            //    "Por motivos de seguridad, el sistema no seguirá procesando los ficheros.");
        }

        private static void FinishAnalysis(int count)
        {
            if (count == 0) {
                Console.WriteLine($"===============================================");
                Console.WriteLine($"            NO HAY CORREOS PARA LEER           ");
                Console.WriteLine($"===============================================");
            }
            else
            {
                UpdateTelemarketing();
                OracleDataAccess.CloseConexion();
            }
        }

        private static void UpdateTelemarketing()
        {
            Utils.Log.SendMails();
        }

        private static void ThrowFormatError(string pdfFileName, string pdfPath)
        {
            //Utils.Log.Save($"El formato del PDF: {pdfFileName}, " +
            //             "no es posible" +
            //         " reconocerlo...");
            //Email.SendEmailFromProcesosXmlDimercWithAttachments(
            //   InternalVariables.GetMainEmail(),
            //    null,
            //    "Formato de PDF Desconocido", $"El Formato del PDF: {pdfFileName}, " +
            //                           "no es posible reconocerlo, favor de Revisar...",
            //    new []
            //    {
            //        pdfPath
            //    });
        }

        private static void ThrowInsertError(string pdfFileName)
        {

            //LecturaMail.Instance.ShowBalloon("Error",
            //    "Ha ocurrido un error al momento de insertar " +
            //    $"las Ordenes de Compra del archivo {pdfFileName}",
            //    BalloonIcon.Error);
            //Utils.Log.Save("Error",
            //    "Ha ocurrido un error al momento de insertar " +
            //    $"las Ordenes de Compra del archivo {pdfFileName}");
            //Email.SendEmailFromProcesosXmlDimerc(
            //    InternalVariables.GetMainEmail(),
            //    null,
            //    "Error de Inserción de Datos",
            //    "Ha ocurrido un error al momento de insertar " +
            //    $"las Ordenes de Compra del archivo {pdfFileName}");
        }

        private static void ThrowAnalysisError(string pdfFileName, Exception e)
        {
            //Utils.Log.Save("Ha ocurrio un error en el analisis de las Ordenes de Compra...");
            //Utils.Log.Save($"Archivo de Orden de Compra: {pdfFileName}");
            //LecturaMail.Instance.ShowBalloon("Error",
            //    "Ha ocurrido un error al momento de Procesar " +
            //    $"el archivo: {pdfFileName}, " +
            //    "para mayor información, revisar el archivo de Registro de Errores.",
            //    BalloonIcon.Error);
            //Email.SendEmailFromProcesosXmlDimerc(
            //    InternalVariables.GetMainEmail(),
            //    null,
            //    "Error al momento de Procesar",
            //    "Ha ocurrido un error al momento de Procesar " +
            //    $"el archivo: {pdfFileName}, " +
            //    "para mayor información, revisar el archivo de Registro de Errores.");
            //Utils.Log.TryError(e.Message);
            //Utils.Log.TryError(e.ToString());
            //InternalVariables.AddCountError(pdfFileName);
        }

        private static void InitializerAnalysis()
        {
            //LecturaMail.Instance.State = IntegracionPdf.AppState.AnalizandoOrdenes;
            //Utils.Log.Save("Inicializando Análisis de Ordenes de Compra");
            //LecturaMail.Instance.ShowBalloon("Información",
            //    "Inicializando Análisis de Ordenes de Compra", BalloonIcon.Info);
        }

        #endregion


    }
}