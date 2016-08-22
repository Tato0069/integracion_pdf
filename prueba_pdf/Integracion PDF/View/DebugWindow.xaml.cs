using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using IntegracionPDF.Integracion_PDF.Utils;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX;
using IntegracionPDF.Integracion_PDF.ViewModel;
using Microsoft.Win32;

namespace IntegracionPDF.Integracion_PDF.View
{

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class DebugWindow
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private void btn_DebugPdf(object sender, RoutedEventArgs ex)
        {
            //try
            //{
            var pdfPath = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "pdf (*.pdf) | *.pdf",
                Title = "Seleccione el Archivo PDF"
            };
            if (pdfPath.ShowDialog() != true) return;
            foreach (var pdfP in pdfPath.FileNames)
            {
                Main.Main.DebugAnalizar(pdfP);
            }
            Log.SendMails();
            //}
            //catch (Exception e)
            //{
            //    Log.TryError(e.Message);
            //    Log.TryError(e.ToString());
            //}
        }






        private void btn_ExtraerTexto(object sender, RoutedEventArgs ex)
        {
            try
            {
                var pdfPath = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "pdf (*.pdf) | *.pdf",
                    Title = "Seleccione el Archivo PDF"
                };
                if (pdfPath.ShowDialog() != true) return;
                foreach (var pdfP in pdfPath.FileNames)
                {
                    MainConverter.ExtractTextDefaultMode(pdfP);
                    MainConverter.ExtractTextSimpleStrategy(pdfP);
                    MainConverter.ExtractTextLocalStrategy(pdfP);
                }
            }
            catch (Exception e)
            {
                Log.TryError(e.Message);
                Log.TryError(e.ToString());
            }
        }

        private void DebugWindow_OnClosed(object sender, EventArgs e)
        {
            NotifyIconViewModel.SetCanDebugCommand();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var xlsPath = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "xls (*.xls, *.xlsx) | *.xls;*.xlsx",
                Title = "Seleccione el Archivo Excel"
            };
            if (xlsPath.ShowDialog() != true) return;
            foreach (var xls in xlsPath.FileNames)
            {
                var extension = xls.Substring(xls
                    .LastIndexOf(".", StringComparison.Ordinal)).ToUpper().Replace(".", "");
                //if (extension.Equals("XLSX"))
                var pdfOutput = extension.Equals("XLSX")
                    ? xls.Replace(".xlsx", ".pdf")
                    : xls.Replace(".xls", ".pdf");
                Console.WriteLine(xls);
                Console.WriteLine(pdfOutput);
                MainConverter.SaveExcelToPdf(xls, pdfOutput);
                Main.Main.DebugAnalizarXls(pdfOutput);
                //Main.Main.MoveFileToProcessFolder(xls);
                //Main.Main.DeleteFile(pdfOutput);
            }
        }

        private void test_generico_onclick(object sender, RoutedEventArgs ex)
        {
            try
            {
                var pdfPath = new OpenFileDialog
                {
                    Multiselect = false,
                    Filter = "pdf (*.pdf) | *.pdf",
                    Title = "Seleccione el Archivo PDF"
                };
                if (pdfPath.ShowDialog() != true) return;
                var pdf = new PDFReader(pdfPath.FileName);
                pdf.ObjectTest();
            }
            catch (Exception e)
            {
                Log.TryError(e.Message);
                Log.TryError(e.ToString());
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            var pdfPath = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "pdf (*.pdf) | *.pdf",
                Title = "Seleccione el Archivo PDF"
            };
            if (pdfPath.ShowDialog() != true) return;
            foreach (var pdfP in pdfPath.FileNames)
            {
                try
                {
                    Main.Main.DebugAnalizar(pdfP);
                }
                catch (Exception) { }
            }
        }
    }
}
