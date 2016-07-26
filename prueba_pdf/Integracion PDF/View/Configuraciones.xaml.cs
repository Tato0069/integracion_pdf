using System;
using System.Windows;
using System.Windows.Forms;
using IntegracionPDF.Integracion_PDF.Utils;
using IntegracionPDF.Integracion_PDF.ViewModel;

namespace IntegracionPDF.Integracion_PDF.View
{
    /// <summary>
    /// Lógica de interacción para Configuraciones.xaml
    /// </summary>
    public partial class Configuraciones
    {
        public Configuraciones()
        {
            InitializeComponent();
            TxtOcAProcesarFolder.Text = InternalVariables.GetOcAProcesarFolder();
            TxtLogFolder.Text = InternalVariables.GetLogFolder();
            TxtOCProcesadasFolder.Text = InternalVariables.GetOcProcesadasFolder();
        }

        private void SetHoraInicio(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //var x = new TimePicker(this) { Owner = this };
            //x.Show();
        }

        private void SetLogFolder(object sender, RoutedEventArgs e)
        {
            var openFolder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                Description = Properties.Resources.Configuraciones_SetOutputFolder_Seleccione_la_Carpeta_donde_se_Guardaran_los_Archivos_del_Log,
                ShowNewFolderButton = true
            };
            if (openFolder.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            TxtLogFolder.Text = openFolder.SelectedPath; 

        }

        private void SetOcProcesadasFolder(object sender, RoutedEventArgs e)
        {
            var openFolder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                Description = Properties.Resources.Configuraciones_SetOCProcesadasFolder_Seleccione_la_Carpeta_donde_se_almacenaran_las_Ordenes_de_Compra_Procesadas,
                ShowNewFolderButton = true
            };

            if (openFolder.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            TxtOCProcesadasFolder.Text = openFolder.SelectedPath;
        }

        private void SetOaProcesarPath(object sender, RoutedEventArgs e)
        {
            var openFolder = new FolderBrowserDialog
            {
                Description = Properties.Resources.Configuraciones_SetOaProcesarPath_Seleccione_la_Carpeta_donde_se_Cargarán_las_Ordenes_de_Compra_a_Procesadar,
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true
            };
            if (openFolder.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            TxtOcAProcesarFolder.Text = openFolder.SelectedPath;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            NotifyIconViewModel.SetCanShowConfigurationsCommand();
        }


        private void GuardarCambios(object sender, RoutedEventArgs e)
        {
            if (!TxtLogFolder.Text.Equals(InternalVariables.GetLogFolder()))
            {
                InternalVariables.ChangeLogFolder(TxtLogFolder.Text);
            }
            if (!TxtOCProcesadasFolder.Text.Equals(InternalVariables.GetOcProcesadasFolder()))
            {
                InternalVariables.ChangeOcProcesadasFolder(TxtOCProcesadasFolder.Text);
            }
            if (!TxtOcAProcesarFolder.Text.Equals(InternalVariables.GetOcAProcesarFolder()))
            {
                InternalVariables.ChangeOCaProcesarFolder(TxtOcAProcesarFolder.Text);
            }
            NotifyIconViewModel.SetCanShowConfigurationsCommand();
            Close();
        }

        private void Salir(object sender, RoutedEventArgs e)
        {
            NotifyIconViewModel.SetCanShowConfigurationsCommand();
            Close();
        }
    }
}
