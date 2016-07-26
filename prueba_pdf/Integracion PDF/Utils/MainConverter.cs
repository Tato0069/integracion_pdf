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


        public static void ExtractText(string path)
        {
            var pdfReader = new PDFReader(path);
            foreach (var line in pdfReader.ExtractTextFromPdfToArray())
            {
                SavePdfToTxt(
                    path.
                    Substring(0, path.LastIndexOf(".", StringComparison.Ordinal)) + ".txt", line);
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

    }
}