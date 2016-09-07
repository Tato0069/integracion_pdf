using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;

namespace LecturaMail.Utils
{
    public static class InternalVariables
    {
        public static int CountCriticalError = 0;
        public static int CountSendErrorAlert = 0;
        public static string LastPdfPathError = "null";
        #region Variables

        public static readonly Dictionary<int, string> MailsFormats = new Dictionary<int, string>
        {

            // : DELIMITADOR PARA 'OR' LÓGICO (||)
            // ; DELIMITADOR PARA 'AND' LÓGICO (&&) === SOLO PUEDE HABER UNO POR TOKEN
            {0,"@cinemark.cl:CineMark.:96659800:96.659.800" },
            {1, "ARCOS DORADOS RESTAURANTES DE CHILE LTDA" }, //APARDO
            {2,"96856780" },
        };

        public static readonly Dictionary<int, string> XlsFormat = new Dictionary<int, string>
        {
            // ; DELIMITADOR PARA 'AND' LÓGICO (&&)
            // * DELIMITADOR PARA 'OR' LÓGICO (||)
            //   SOLO PUEDE HABER UNO POR TOKEN
            {0, "Tienda : Flores*Tienda : Tienda Flores"},//MZAPATA
            {2, "76008982*76021762*77335750*76.008.982-6*77.457.040-3"},//MZAPATA
            {1, "PLANILLA_ESTANDAR" }//MZAPATA
            //76021762-K
        };

        internal static object GetSubjectMail()
        {
            return ConfigurationManager.AppSettings.Get("SubjectMail");
        }

        #endregion


        #region Funciones


        #region Email

        public static int GetPortEmailFrom()
        {
            return int.Parse(ConfigurationManager.AppSettings.Get("PortEmailFrom"));
        }

        public static string GetHostEmailFrom()
        {
            return ConfigurationManager.AppSettings.Get("HostEmailFrom");
        }

        public static string GetEmailFrom()
        {
            return ConfigurationManager.AppSettings.Get("SendEmailFrom");
        }

        public static string GetPasswordEmailFrom()
        {
            return ConfigurationManager.AppSettings.Get("PasswordEmailFrom");
        }

        public static string GetSubjectDebug()
        {
            try
            {
                var sMacAddress = string.Empty;
                var nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)// La primera Mac de la Tarjeta
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                return ConfigurationManager.AppSettings.Get(sMacAddress).Split('@')[0].ToUpper();
            }
            catch
            {
                return "";
            }
        }

        public static string[] GetMainEmail()
        {
            var sMacAddress = string.Empty;
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            //var cc = 0;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// La primera Mac de la Tarjeta
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
                //Console.WriteLine($"{++cc}.-{adapter.GetPhysicalAddress().ToString()}");
            }
            if (IsDebug()) //Si es Debug solo enviar email a responsable (definida por su respectiva Mac)
            {
                Console.WriteLine("MACCCC: " + sMacAddress);
                try
                {
                    //Console.WriteLine($"MACCC: {ConfigurationManager.AppSettings.Get(sMacAddress)}");
                    return new string[] { ConfigurationManager.AppSettings.Get(sMacAddress) == null? ConfigurationManager.AppSettings.Get("1002B5C4275F") : ConfigurationManager.AppSettings.Get(sMacAddress) };
                }
                catch { };
            }
            return ConfigurationManager.AppSettings.Get("MainEmail").Split(';').ToArray();
        }

        public static string[] GetEmailCc()
        {
            return ConfigurationManager.AppSettings.Get("EmailCC").Split(';').ToArray();
        }

        #endregion

      

        #region CARPETAS
        public static string GetOcAProcesarFolder()
        {
            return ConfigurationManager.AppSettings.Get("CarpetaOrdenesProcesar");
        }

        public static string GetOcDavilaAnexoAProcesarFolder()
        {
            return ConfigurationManager.AppSettings.Get("CarpetaOrdenesProcesarDavilaAnexo");
        }

        public static string GetOcExcelAProcesarFolder()
        {
            return ConfigurationManager.AppSettings.Get("CarpetaOrdenesProcesarExcel");
        }

        public static string GetOcProcesadasFolder()
        {
            return ConfigurationManager.AppSettings.Get("CarpetaOrdenesProcesadas");
        }

        public static string GetLogFolder()
        {
            return ConfigurationManager.AppSettings.Get("CarpetaLog");
        }

        public static void ChangeLogFolder(string newValue)
        {
            Save("CarpetaLog", newValue);
            Log.Save("Información", "La Ruta del Archivo Log ha sido Cambiada por: " + newValue);
        }

        public static void ChangeOCaProcesarFolder(string newValue)
        {
            Save("CarpetaOrdenesProcesar", newValue);
            Log.Save("Información", "La Ruta de las Ordenes a Procesar ha sido Cambiada por: " + newValue);
        }

        public static void ChangeOcProcesadasFolder(string newValue)
        {
            Save("CarpetaOrdenesProcesadas", newValue);
            Log.Save("Información", "La Ruta de Ordenes Procesadas ha sido Cambiada por: " + newValue);
        }

        #endregion


        #region SISTEMA


        public static void AddCountError(string pdfPath)
        {
            if (!LastPdfPathError.Equals(pdfPath))
            {
                LastPdfPathError = pdfPath;
                CountCriticalError = 0;
            }
            CountCriticalError++;
        }

        public static bool SendPopup()
        {
            return bool.Parse(ConfigurationManager.AppSettings.Get("SendPopupTlmk"));
        }

        public static bool SendMailEjecutivos()
        {
            return bool.Parse(ConfigurationManager.AppSettings.Get("SendEmailEjecutivo"));
        }
        public static bool IsDebug()
        {
            return bool.Parse(ConfigurationManager.AppSettings.Get("Debug"));
        }

        public static Visibility ShowDebug()
        {
            return ConfigurationManager.AppSettings.Get("ShowDebug").Equals("true") ? Visibility.Visible : Visibility.Collapsed;
        }

        public static int GetTiempoHorasCiclo()
        {
            return int.Parse(ConfigurationManager.AppSettings.Get("TiempoCicloHoras"));
        }

        public static int GetTiempoMinutosCiclo()
        {
            return int.Parse(ConfigurationManager.AppSettings.Get("TiempoCicloMinutos"));
        }

        private static void Save(string nameKey, string newValue)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(nameKey);
            config.AppSettings.Settings.Add(nameKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void InitializeVariables()
        {
            Log.InitializeVariables();
        }
        #endregion

        #endregion
    }

}