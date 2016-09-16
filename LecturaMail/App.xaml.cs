using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Timer = System.Timers.Timer;
using LecturaMail.Utils;
using LecturaMail.ViewModel;

namespace LecturaMail
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App
    {
        private const int HORA = 3600000;
        private const int MINUTO = 60000;
        private static Timer _timer;
        private static bool FirstInstance
        {
            get
            {
                var proces = Process.GetCurrentProcess().ProcessName;
                return Process.GetProcessesByName(proces).Count() == 1;
            }
        }

        private static void CreateAllFolderIfNotExists()
        {
            if (!Directory.Exists(InternalVariables.GetRutaXmlProcesados()))
                Directory.CreateDirectory(InternalVariables.GetRutaXmlProcesados());
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            CreateAllFolderIfNotExists();
            Utils.MySql.DataAccess.MySqlDataAccess.TestConection();
            InternalVariables.InitializeVariables();
            //LecturaMail.Main.Main.ExecuteLecturaMail();
            LecturaMail.Main.Main.ExecuteLecturaIconstruyeMail();
            if (!FirstInstance)
            {
                MessageBox.Show("No se Puede Abrir la Aplicacion debido a que ya se esta Ejecutando.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
            }
            else
            {
                //_inicializacion = LecturaMai.Instance;
                SetTimeToTimer();
                Log.StartApp();
            }
        }

        

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Log.FinalizeApp();
        }
        private static void SetTimeToTimer()
        {
            if (InternalVariables.IsDebug()) return;
            var horas = InternalVariables.GetTiempoHorasCiclo();
            var minutos = InternalVariables.GetTiempoMinutosCiclo();
            _timer = new Timer(horas * HORA + minutos * MINUTO);
            _timer.Elapsed += (sender1, args1) =>
            {
                NotifyIconViewModel.SetCantProcessOrderCommand();
                //LecturaMail.Instance.ShowBalloon("Información", "Comenzando la Analisis de ordenes de Compra.",
                //    BalloonIcon.Info);
                //if (LecturaMail.Instance.State == LecturaMail.AppState.Inactivo)
                //{
                //    using (var worker = new BackgroundWorker())
                //    {
                //        worker.DoWork += (sender2, args2) => LecturaMail.Main.Main.ReadPdfOrderFromRootDirectory();
                //        worker.RunWorkerAsync();
                //    }
                //}
                //else
                //{
                //    Log.Save("Advertencia",
                //        "No se Ejecutara el Analisis de Ordenes de Compra debido a que Actualmente se esta ejecutando");
                //    LecturaMail.Instance.ShowBalloon("Advertencia",
                //        "No se Ejecutara el Analisis de Ordenes de Compra debido a que Actualmente se esta ejecutando.", BalloonIcon.Warning);
                //}
                //_timer.Enabled = false;
                //_timer.Stop();
                Thread.Sleep(10000);
                //SetTimeToTimer();
            };
            _timer.AutoReset = true;
            _timer.Enabled = true;
            //_timer.Start();
        }
    }
}
