using Hardcodet.Wpf.TaskbarNotification;
using LecturaMail.Utils;

namespace LecturaMail.View
{
    /// <summary>
    /// Lógica de interacción para IntegracionPDF.xaml
    /// </summary>
    public partial class LecturaMail
    {
        public enum AppState
        {
            Inactivo,
            AnalizandoOrdenes,
        }

        public AppState State;
        private static LecturaMail _instance;
        public static LecturaMail Instance => _instance ?? (_instance = new LecturaMail());
        
        public LecturaMail()
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
