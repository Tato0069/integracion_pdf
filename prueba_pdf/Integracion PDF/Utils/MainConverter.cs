using System;
using System.IO;
using System.Threading;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX;

namespace IntegracionPDF.Integracion_PDF.Utils
{
    public static class MainConverter
    {
        public static void SaveExcelToPdf(string path, string save)
        {
            var excel = new ExcelExport();
            excel.Convert(ExcelExport.FormatType.Pdf, path, save);
        }


        public static void ExtractTextDefaultMode(string path)
        {
            var pdfReader = new PDFReader(path);
            foreach (var line in pdfReader.ExtractTextFromPdfToArrayDefaultMode())
            {
                SavePdfToTxt(
                    path.
                    Substring(0, path.LastIndexOf(".", StringComparison.Ordinal)) + "_default_mode.txt", line);
            }
        }

        private static void SavePdfToTxt(string path, string save)
        {
            try
            {
                var writer = new StreamWriter($"{path}", true);
                writer.WriteLine(save);
                writer.Close();
            }
            catch (Exception e)
            {
                Thread.Sleep(2000);
                SavePdfToTxt(path, save);
                Console.WriteLine(e.Message);
            }
        }

        internal static void ExtractTextSimpleStrategy(string pdfP)
        {
            var pdfReader = new PDFReader(pdfP);
            foreach (var line in pdfReader.ExtractTextFromPdfToArraySimpleTextExtractionStrategy())
            {
                SavePdfToTxt(
                    pdfP.
                    Substring(0, pdfP.LastIndexOf(".", StringComparison.Ordinal)) + "_simple_strategy.txt", line);
            }
        }
    }
}