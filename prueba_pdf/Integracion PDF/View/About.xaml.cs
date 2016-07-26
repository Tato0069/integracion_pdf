using System;
using System.Linq;
using System.Windows;
using IntegracionPDF.Integracion_PDF.Utils;
using IntegracionPDF.Integracion_PDF.ViewModel;

namespace IntegracionPDF.Integracion_PDF.View
{
    /// <summary>
    /// Lógica de interacción para About.xaml
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();
            var count = 1;
            foreach (var emp in InternalVariables.PdfFormats
                .Where(pdfFormat => pdfFormat.Key >= 0).Select(pdfFormat => pdfFormat.Value.Contains(";")
                    ? pdfFormat.Value.Split(';')[0]
                    : pdfFormat.Value))
            {
                listView.Items.Add(
                    new ListViewItem {ID = count++,
                        Empresa = emp});
            }

            foreach (var emp in InternalVariables.XlsFormat
                .Where(pdfFormat => pdfFormat.Key >= 0).Select(pdfFormat => pdfFormat.Value.Contains(";")
                    ? pdfFormat.Value.Split(';')[0]
                    : pdfFormat.Value))
            {
                listView.Items.Add(
                    new ListViewItem
                    {
                        ID = count++,
                        Empresa = emp
                    });
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            NotifyIconViewModel.SetCanShowAboutCommand();
        }

        private void About_OnClosed(object sender, EventArgs e)
        {
            NotifyIconViewModel.SetCanShowAboutCommand();
        }
    }

    internal class ListViewItem
    {
        public int ID { get; set; }
        public string Empresa { get; set; }
    }
}
