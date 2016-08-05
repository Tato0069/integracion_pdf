using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using IntegracionPDF.Integracion_PDF.Utils;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;
using IntegracionPDF.Integracion_PDF.View;
using IntegracionPDF.Integracion_PDF.ViewModel;
using Timer = System.Timers.Timer;

namespace IntegracionPDF
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
        private  IntegracionPdf _inicializacion;

     
        protected override void OnStartup(StartupEventArgs e)
        {
            //OracleDataAccess.GetSkuWithMatcthDimercProductDescription("SOBRE 1/2 CARTA BLANCO 90GR 50UN DIMERC");
            //var R780134 = OracleDataAccess.GetPrecioProducto("96770100", "0", "R780134", "3");
            //var E204504 = OracleDataAccess.GetPrecioProducto("96770100", "0", "E204504", "3");
            //Console.Write($"R780134: {R780134}\nE204504: {E204504}");
            //var sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription("Azúcar Granulada Iansa 1 kg");
            //var precio = OracleDataAccess.GetPrecioConvenio("96853940", "0", sku, "0");
            //Console.WriteLine($"SKU: {sku}, PRECIO: {precio}");
            //var sku2 = "R284220";
            //var precio2 = OracleDataAccess.GetPrecioConvenio("76522179", "0", sku2, "0");
            //Console.WriteLine($"SKU: {sku2}, PRECIO: {precio2}");

            //var en = Encrypt.EncryptKey("17990424");
            //Console.WriteLine($"RUT: 17990424, CL_{en}, {en.Length}");
            InternalVariables.InitializeVariables();
            if (!FirstInstance)
            {
                MessageBox.Show("No se Puede Abrir la Aplicacion debido a que ya se esta Ejecutando.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
            }
            else
            {
                _inicializacion = IntegracionPdf.Instance;
                SetTimeToTimer();
                //CreateAllFolderIfNotExists();
                Log.StartApp();
            }
        }

        private static void CreateAllFolderIfNotExists()
        {
            if (!Directory.Exists(InternalVariables.GetOcAProcesarFolder()))
                Directory.CreateDirectory(InternalVariables.GetOcAProcesarFolder());
            if (!Directory.Exists(InternalVariables.GetLogFolder()))
                Directory.CreateDirectory(InternalVariables.GetLogFolder());
            if (Directory.Exists(InternalVariables.GetOcProcesadasFolder()))
                Directory.CreateDirectory(InternalVariables.GetOcProcesadasFolder());
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
                IntegracionPdf.Instance.ShowBalloon("Información", "Comenzando la Analisis de ordenes de Compra.",
                    BalloonIcon.Info);
                if (IntegracionPdf.Instance.State == IntegracionPdf.AppState.Inactivo)
                {
                    using (var worker = new BackgroundWorker())
                    {
                        worker.DoWork += (sender2, args2) => Integracion_PDF.Main.Main.ReadPdfOrderFromRootDirectory();
                        worker.RunWorkerAsync();
                    }
                }
                else
                {
                    Log.Save("Advertencia",
                        "No se Ejecutara el Analisis de Ordenes de Compra debido a que Actualmente se esta ejecutando");
                    IntegracionPdf.Instance.ShowBalloon("Advertencia",
                        "No se Ejecutara el Analisis de Ordenes de Compra debido a que Actualmente se esta ejecutando.", BalloonIcon.Warning);
                }
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
