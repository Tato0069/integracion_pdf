using System;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace IntegracionPDF.Integracion_PDF.Utils
{
    public class PDFReader
    {
        public int NumerOfPages { get; private set; }

        private readonly string _pdfPath;

        /// <summary>
        /// Ruta Completa de Archivo PDF
        /// </summary>
        public string PdfPath => _pdfPath;

        /// <summary>
        /// Ruta Base de Archivo PDF
        /// </summary>
        public string PdfRootPath => _pdfPath.Substring(0, _pdfPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

        /// <summary>
        /// Nombre de Archivo PDF
        /// </summary>
        public string PdfFileName => _pdfPath.Substring(_pdfPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdfPath">Ruta de PDF</param>
        public PDFReader(string pdfPath)
        {
            _pdfPath = pdfPath;
        }

        /// <summary>
        /// Extraer el Texto del PDF como una sola linea 
        /// </summary>
        /// <returns>Texto PDF</returns>
        public string ExtractTextFromPdfToString()
        {
            using (var reader = new PdfReader(_pdfPath))
            {
                NumerOfPages = reader.NumberOfPages;
                var text = new StringBuilder();
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i)); //.DeleteContoniousWhiteSpace());
                }
                return text.ToString().Replace("\n", " ");
            }
        }

        /// <summary>
        /// Extrae Texto de PDF y retorna un Arreglo con dicho texto.
        /// </summary>
        /// <returns>Arreglo con Texto del PDF</returns>
        public string[] ExtractTextFromPdfToArray()
        {
            using (var reader = new PdfReader(_pdfPath))
            {
                NumerOfPages = reader.NumberOfPages;
                var text = new StringBuilder();
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, i).DeleteContoniousWhiteSpace());
                }
                return text.ToString().Split('\n');
            }
        }

        /// <summary>
        /// Extrae Texto de PDF sólo de la Página "i"
        /// </summary>
        /// <param name="index">Número de Página</param>
        /// <returns>texto[]</returns>
        public string[] ExtractTextFromPageOfPdfToArray(int index)
        {
            using (var reader = new PdfReader(_pdfPath))
            {
                var text = new StringBuilder();
                text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, index)); //.DeleteContoniousWhiteSpace());
                return text.ToString().Split('\n');
            }
        }

    }
}