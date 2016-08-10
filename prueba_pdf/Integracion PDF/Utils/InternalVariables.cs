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
            // ; DELIMITADOR PARA 'AND' LÓGICO (&&) === SOLO PUEDE HABER UNO POR TOKEN

            {0, "EASY RETAIL S.A.:17990424"},//MZAPATA
            {1, "Cencosud Retail S.A."},//MZAPATA
            {2, "Cencosud S.A:Cencosud Shopping"},//MZAPATA
            {-1, "CeCo SAP FICO"},//MZAPATA
            {3, "INDRA SISTEMAS CHILE S.A."},//MZAPATA
            {4, "SECURITAS S.A."}, //MZAPATA
            {
                5, "Universidad Nacional Andrés Bello:Laureate Chile II SpA:Inmobiliaria Educ SPA (IESA):" +
                   "Servicios Profesionales Andrés Bello SPA"
            },//MZAPATA
            {-5, "Corp Universidad Andres Bello"},//MZAPATA
            {6, "Empresa Servic.Externos ACHS Transp.S.A."},//MZAPATA
            {7, "BHP Billiton Ltd"},//MZAPATA
            {8, "CLINICA DAVILA Y SERVICIOS MEDICOS S.A."},//MZAPATA
            {-8, "ANEXO PARA ENTREGAS DIFERIDAS DE OC"},//MZAPATA
            {9, "EMPRESAS CAROZZI S.A."},//MZAPATA
            {10, "Securitas Austral Ltda."},//MZAPATA
            {11, "DOLE CHILE S.A."},//MZAPATA
            {12, "Capacitaciones Securicap S.A"},//MZAPATA
            {13, "Universidad de las Americas"},//MZAPATA
            {14, "Instituto Profesional AIEP SPA"},//MZAPATA
            {15, "Clínica Alemana de Santiago S.A."},//MZAPATA
            {-15, " Clínica Alemana "},//MZAPATA
            {16, "Univ. de Viña del Mar,Chile Op"},//MZAPATA
            {17, "DELLANATURA S.A"},//MZAPATA
            {18, "COMERCIAL TOC´S LIMITADA"},//MZAPATA
            {19, "Komatsu Chile S.A."},//MZAPATA
            {20, "Komatsu Cummins Chile LTDA"},//MZAPATA
            {21, "INVERSIONES ALSACIA S.A.;99577400"},//MZAPATA
            {22, "EXPRESS DE SANTIAGO UNO S.A.;99577390"},//MZAPATA
            {23, "ISAPRE CONSALUD S.A."},//MZAPATA
            {24, "IANSAGRO S.A.:EMPRESAS IANSA S.A."},//MZAPATA
            {25, "TNT EXPRESS CHILE LTDA:TNT Exp WW (Chile) Carga"},//MZAPATA
            {26, "Servicios Comerciales S.A."},//MZAPATA
            {27, "Constructora Ingevec S.A."},//MZAPATA
            {28, "VITAMINA WORK LIFE S.A."},//MZAPATA
            {29, "MATERIALES Y SOLUCIONES S.A."},//MZAPATA
            {30, "Clariant (Chile) Ltda.:ARCHROMA CHILE LTDA.:Clariant Plastics & Coatings (Chile) Ltda"},//MZAPATA
            {31, "CAMDEN SERVICIOS SPA"},//MZAPATA
            {32, "Servicios Andinos SpA"},//MZAPATA
            {33, "GESTION DE PERSONAS Y SERVICIOS LIMITADA"},//MZAPATA
            {34, "HORMIGONES TRANSEX LTDA."},//MZAPATA
            {35, "OFFICE STORE SpA "},//MZAPATA
            {36, "CLINICA LAS LILAS S.A."},//MZAPATA
            {37, "Abengoa Chile"},//MZAPATA
            {38, "Clínica de la Universidad de los Andes"},//MZAPATA
            {39, "Food and Fantasy"},//MZAPATA
            {40, "Bupa Chile Servicios Corporativos Spa:Exámenes de Laboratorio S.A.:Integramedica S.A"},//MZAPATA
            {41, "ECORILES S.A."},//MZAPATA
            {42, "Komatsu Cummins Chile Arrienda S.A"},//MZAPATA
            {43, "Integramedica Establ. medicos Atencion Ambulatoria:Sonorad I S.A."},//MZAPATA
            {44, "Komatsu Reman Center Chile S.A:76.492.400"},//MZAPATA
            {45, "Distribuidora Cummins Chile"},//MZAPATA
            {46, "GEPYS EST LIMITADA"},//MZAPATA
            {47, "76.016.649:76016649" },//MZAPATA
            {48, "CIA DE SEG. DE VIDA CONSORCIO NAC. DE SEG. S.A.:CN LIFE" },//MZAPATA
            {49, "MEGASALUD S.A." },//MZAPATA
            {50, "Celulosa Arauco y Constitución S.A.:Paneles Arauco S.A."},//MZAPATA
            {51, "KAEFER BUILDTEK S.p.A.;Item Código Descripción Cantidad" },//MZAPATA
            {52, "Razón Social ASESORIAS Y SERVICIOS DE CAPACITACION ICYDE LTDA." },//MZAPATA
            {53, "Intertek Caleb Brett Chile S.A." },//APARDO
            {54, "ANÁLISIS AMBIENTALES S.A." }, //MZAPATA
            {55, "AFP Habitat S.A." },//MZAPATA
            {56,"Corporación de Desarrollo Tecnológico" }, //MZAPATA
            {57, "AGUAS ANDINAS S.A." }, //MZAPATA
            {58, "INGENPROJECT LTDA." }, //MZAPATA
            {59, "Sociedad Educacional La Araucana S.A."}, //MZAPATA
            {60, "FAGASE S.A. - DUNKIN' DONUTS" }, //MZAPATA
            {61, "Teveuk Ltda." }, //MZAPATA
            {62, "76.858.590-3:76858590-3:76.858.590" }, //MZAPATA - Colegio Coya
            {63, "82.648.400-4:82648400-4:8264840"}, //MZAPATA - Sociedad de Instrucción
            {64, "Shaw Almex Chile S.A." }, //MZAPATA
            {65, "UNITED NATIONS" },//MZAPATA
            {66, "INGENIERIA Y COMERCIALIZADORA RIEGO 2010 S.A." }, //MZAPATA
            {67, "ENVASADOS MOVIPACK CHILE LTDA." }, //MZAPATA
            {68, "Larraín y Asociados Ltda." },//MZAPATA
            {69, "Laboratorios LBC Limitada" }, //MZAPATA
            {70, "76.053.505-2:76053505" }, //MZAPATA PROYEKTA S.A.
            {71, "Biomedical Distribution Chile Ltda" }, //MZAPATA
            {72, "LOGISTICA INDUSTRIAL S.A" }, //MZAPATA - AGREGAR CC
            {73, "Depósitos y Contenedores S.A" }, //MZAPATA - AGREGAR
            {74, "ARIDOS SANTA FE S.A:INGENIERIA Y CONSTRUCCIONES SANTA FE" }, //MZAPATA            
            {75, "CEMENTOS TRANSEX LTDA" }, //MZAPATA
            {76, "EZENTIS CHILE S.A."}, //MZAPATA
            {77, "RIMASA LTDA." }, //MZAPATA
            {78, "GRUPO EULEN CHILE S.A.;96.997.370-7" },//MZAPATA
            {79, "EULEN CHILE S.A.;96.937.270-3" },//MZPATA
            {80, "PETROBRAS CHILE RED LTDA" }, //MZAPATA
            {81, "PETROBRAS CHILE DISTRIBUCIÓN LTDA" }, //MZAPATA
            {82, "EULEN SEGURIDAD S.A." }, //MZAPATA
            {83, "Empresa de Serv. Externos ACHS S.A." }, //MZAPATA





            {200, "CERVECERA CCU CHILE LTDA." }, //APARDO
            {201, "CONSTRUCTORA SANTAFE S.A." }, //APARDO
            {202, "SEIDOR CHILE S.A" }, //APARDO
            {203, "CFT SAN AGUSTIN DE TALCA" }, //APARDO
            {204, "77.177.430-K" }, //APARDO Alimentos San Martin 
            {205, "77.038.090-1" }, // APARDO Zical
            {206, "CLUB AEREO DEL PERSONAL DE CARABINEROS" }, //APARDO
            {207, "Maestranza Mining Parts Ltda." }, //APARDO under construction 
            {208, "CONSTRUCTORA VRK S.P.A.;GLOSA PROV" }, //APARDO under construction
            {209, "Constructora Lampa Oriente S.A." }//APARDO
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