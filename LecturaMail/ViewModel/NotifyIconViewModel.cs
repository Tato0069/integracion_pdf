using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using LecturaMail.Utils;
using LecturaMail.Utils.Oracle.DataAccess;
using LecturaMail.View;

namespace LecturaMail.ViewModel
{
    /// <summary>
    /// Clase que Interactua con el TaskbarIcon.DataContext de Model/GeneradorNotifyIcon
    /// </summary>
    public class NotifyIconViewModel
    {
        private static bool _canShowConfigurations = true;
        private static bool _canIntegrarOrdenes = true;
        private static bool _canShowDebug = true;
        private static bool _canShowAbout = true;
        private static Thread _hilo;

        public static void SetCanDebugCommand()
        {
            _canShowDebug = true;
        }

        public static void SetCanShowConfigurationsCommand()
        {
            _canShowConfigurations = true;
        }
        public static void SetCanShowAboutCommand()
        {
            _canShowAbout = true;
        }


        public static void SetCanProcessOrderCommand()
        {
            //LecturaMail.Instance.State = LecturaMail.AppState.Inactivo;
            _canIntegrarOrdenes = true;
        }
        public static void SetCantProcessOrderCommand()
        {
            _canIntegrarOrdenes = false;
        }



        public static ICommand IntegrarOrdenesCommand { get; } = new DelegateCommand
        {
            CanExecuteFunc = () => _canIntegrarOrdenes,
            CommandAction = () =>
            {
                _canIntegrarOrdenes = false;
                //_hilo = new Thread(Main.Main.ReadPdfOrderFromRootDirectory);
                //_hilo.Start();
            }
        };

        public static ICommand ShowConfigurationCommand { get; } = new DelegateCommand
        {
            CanExecuteFunc = () => _canShowConfigurations,
            CommandAction = () =>
            {
                //new Configuraciones().Show();
                _canShowConfigurations = false;

            }
        };

        public ICommand DebugCommand { get; } = new DelegateCommand
        {
            CanExecuteFunc = () => _canShowDebug,
            CommandAction = () =>
            {
                //new DebugWindow().Show();
                _canShowDebug = false;
            }
        };
        public ICommand ShowLogCommand { get; } = new DelegateCommand { CommandAction = () => Process.Start(InternalVariables.GetLogFolder()) };
        public ICommand ExitApplicationCommand { get; } = new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };

        public static ICommand ShowOrdenesProcesadasCommand { get; } = new DelegateCommand { CommandAction = () => Process.Start(InternalVariables.GetOcProcesadasFolder()) };
        
        public static ICommand UpdateTelemarketingCommand { get; } = new DelegateCommand { CommandAction = () => OracleDataAccess.GetNull() };
        public static ICommand ShowOrdenesAProcesarCommand { get; } = new DelegateCommand { CommandAction = () => Process.Start(InternalVariables.GetOcAProcesarFolder()) };

        public ICommand AboutCommand { get; } = new DelegateCommand
        {
            CanExecuteFunc = () => _canShowAbout,
            CommandAction = () =>
            {
                //new About().Show();
                _canShowAbout = false;
            }
        };
    }


    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
