using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using IntegracionPDF.Integracion_PDF.Utils;

namespace IntegracionPDF.Integracion_PDF.View
{
    /// <summary>
    /// Lógica de interacción para IntegracionPDF.xaml
    /// </summary>
    public partial class IntegracionPdf
    {
        public enum AppState
        {
            Inactivo,
            AnalizandoOrdenes,
        }

        public AppState State;
        private static IntegracionPdf _instance;
        public static IntegracionPdf Instance => _instance ?? (_instance = new IntegracionPdf());
        
        public IntegracionPdf()
        {
            InitializeComponent();
            miDebug.Visibility = InternalVariables.ShowDebug();
        }

        public void ShowBalloon(string title, string desc, BalloonIcon icon)
        {
            NotifyIcon.ShowBalloonTip(title, desc, icon);
        }
    }
}
