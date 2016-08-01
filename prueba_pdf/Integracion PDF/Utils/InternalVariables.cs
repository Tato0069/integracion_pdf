using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace IntegracionPDF.Integracion_PDF.Utils
{
    public static class InternalVariables
    {
        public static int CountCriticalError = 0;
        public static int CountSendErrorAlert = 0;
        public static string LastPdfPathError = "null";
        #region Variables

        public static readonly Dictionary<int, string> PdfFormats = new Dictionary<int, string>
        {
            
            // : DELIMITADOR PARA 'OR' LÓGICO (||)
            // ; DELIMITADOR PARA 'AND' LÓGICO (&&)
            //   SOLO PUEDE HABER UNO POR TOKEN

            {0, "EASY RETAIL S.A.:17990424"},
            {1, "Cencosud Retail S.A."},
            {2, "Cencosud S.A"},
            {-1, "CeCo SAP FICO"},
            {3, "INDRA SISTEMAS CHILE S.A."},
            {4, "SECURITAS S.A."}, //SECURITAS S.A.
            {
                5, "Universidad Nacional Andrés Bello:Laureate Chile II SpA:Inmobiliaria Educ SPA (IESA):" +
                   "Servicios Profesionales Andrés Bello SPA"
            },
            {-5, "Corp Universidad Andres Bello"},
            {6, "Empresa Servic.Externos ACHS Transp.S.A."},
            {7, "BHP Billiton Ltd"},
            {8, "CLINICA DAVILA Y SERVICIOS MEDICOS S.A."},
            {-8, "ANEXO PARA ENTREGAS DIFERIDAS DE OC"},
            {9, "EMPRESAS CAROZZI S.A."},
            {10, "Securitas Austral Ltda."},
            {11, "DOLE CHILE S.A."},
            {12, "Capacitaciones Securicap S.A"},
            {13, "Universidad de las Americas"},
            {14, "Instituto Profesional AIEP SPA"},
            {15, "Clínica Alemana de Santiago S.A."},
            {-15, " Clínica Alemana "},
            {16, "Univ. de Viña del Mar,Chile Op"},
            {17, "DELLANATURA S.A"},
            {18, "COMERCIAL TOC´S LIMITADA"},
            {19, "Komatsu Chile S.A."},
            {20, "Komatsu Cummins Chile LTDA"},
            {21, "INVERSIONES ALSACIA S.A.;99577400"},
            {22, "EXPRESS DE SANTIAGO UNO S.A.;99577390"},
            {23, "ISAPRE CONSALUD S.A."},
            {24, "IANSAGRO S.A.:EMPRESAS IANSA S.A."},
            {25, "TNT EXPRESS CHILE LTDA:TNT Exp WW (Chile) Carga"},
            {26, "Servicios Comerciales S.A."},
            {27, "Constructora Ingevec S.A."},
            {28, "VITAMINA WORK LIFE S.A."},
            {29, "MATERIALES Y SOLUCIONES S.A."},
            {30, "Clariant (Chile) Ltda.:ARCHROMA CHILE LTDA.:Clariant Plastics & Coatings (Chile) Ltda"},
            {31, "CAMDEN SERVICIOS SPA"},
            {32, "Servicios Andinos SpA"},
            {33, "GESTION DE PERSONAS Y SERVICIOS LIMITADA"},
            {34, "HORMIGONES TRANSEX LTDA."},
            {35, "OFFICE STORE SpA "},
            {36, "CLINICA LAS LILAS S.A."},
            {37, "Abengoa Chile"},
            {38, "Clínica de la Universidad de los Andes"},
            {39, "Food and Fantasy"},
            {40, "Bupa Chile Servicios Corporativos Spa:Exámenes de Laboratorio S.A.:Integramedica S.A"},
            {41, "ECORILES S.A."},
            {42, "Komatsu Cummins Chile Arrienda S.A"},
            {43, "Integramedica Establ. medicos Atencion Ambulatoria:Sonorad I S.A."},
            {44, "Komatsu Reman Center Chile S.A:76.492.400"},
            {45, "Distribuidora Cummins Chile"},
            {46, "GEPYS EST LIMITADA"},
            {47, "76.016.649:76016649" },
            {48, "CIA DE SEG. DE VIDA CONSORCIO NAC. DE SEG. S.A.:CN LIFE" },
            {49, "MEGASALUD S.A." },
            {50, "Celulosa Arauco y Constitución S.A.:Paneles Arauco S.A."},
            {51, "KAEFER BUILDTEK S.p.A.;Item Código Descripción Cantidad" },
            {52, "Razón Social ASESORIAS Y SERVICIOS DE CAPACITACION ICYDE LTDA." }
        };

        public static readonly Dictionary<int, string> XlsFormat = new Dictionary<int, string>
        {
            // ; DELIMITADOR PARA 'AND' LÓGICO (&&)
            // * DELIMITADOR PARA 'OR' LÓGICO (||)
            //   SOLO PUEDE HABER UNO POR TOKEN
            {0, "Tienda : Flores*Tienda : Tienda Flores"},
            {2, "76008982*76021762*77335750*76.008.982-6*77.457.040-3"},
            {1, "PLANILLA_ESTANDAR" }
            //76021762-K
        };

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

        public static string[] GetMainEmailDebug()
        {
            return ConfigurationManager.AppSettings.Get("MainEmailDebug").Split(';').ToArray();
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