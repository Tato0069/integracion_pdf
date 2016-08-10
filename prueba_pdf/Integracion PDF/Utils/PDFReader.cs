using System;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;

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
        /// Nombre de Orden de Compra de PDF
        /// </summary>
        public string PdfFileNameOC => _pdfPath.Substring(_pdfPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1).Replace(".PDF","").Replace(".pdf","");

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
        public string[] ExtractTextFromPdfToArrayDefaultMode()
        {
            using (var reader = new PdfReader(_pdfPath))
            {
                NumerOfPages = reader.NumberOfPages;
                var text = new StringBuilder();
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, i).DeleteContoniousWhiteSpace());
                }
                var ret = text.ToString().Split('\n');
                for (var i = 0; i < text.Length; i++)
                {
                    for (var j = i + 1; j < ret.Length - 1; j++)
                    {
                        if (ret[i].Equals(ret[j]))
                        {
                            ret[j] = "-1*";
                        }
                    }
                }
                var ret2 = new List<string>();
                foreach (var x in ret)
                {
                    if (!x.Equals("-1*"))
                    {
                        ret2.Add(x);
                    }
                }
                return ret2.ToArray();
            }
        }

        /// <summary>
        /// Retorna texto de PDF eliminando valores Hexadecimales Nulos
        /// </summary>
        /// <returns></returns>
        public string[] ExtractTextFromPdfToArrayDefaultModeDeleteHexadeximalNullValues()
        {
            using (var reader = new PdfReader(_pdfPath))
            {
                NumerOfPages = reader.NumberOfPages;
                var text = new StringBuilder();
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, i)
                        .DeleteContoniousWhiteSpace().DeleteNullHexadecimalValues());
                }
                var ret = text.ToString().Split('\n');
                for (var i = 0; i < text.Length; i++)
                {
                    for (var j = i + 1; j < ret.Length - 1; j++)
                    {
                        if (ret[i].Equals(ret[j]))
                        {
                            ret[j] = "-1*";
                        }
                    }
                }
                var ret2 = new List<string>();
                foreach (var x in ret)
                {
                    if (!x.Equals("-1*"))
                    {
                        ret2.Add(x);
                    }
                }
                return ret2.ToArray();
            }
        }

        /// <summary>
        /// Extrae texto de PDF con Simple Strategy
        /// </summary>
        /// <returns></returns>
        public string[] ExtractTextFromPdfToArraySimpleStrategy()
        {
            Console.WriteLine("SimpleTextExtractionStrategy");
            using (var reader = new PdfReader(_pdfPath))
            {
                NumerOfPages = reader.NumberOfPages;
                var text = new StringBuilder();
                ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, i, its).DeleteContoniousWhiteSpace());

                }
                var ret = text.ToString().Split('\n');
                for (var i = 0;i < text.Length; i++)
                {
                    for(var j = i+1; j < ret.Length-1; j++)
                    {
                        if (ret[i].Equals(ret[j]))
                        {
                            ret[j] = "-1*";
                        }
                    }
                }
                var ret2 = new List<string>();
                foreach(var x in ret)
                {
                    if (!x.Equals("-1*"))
                    {
                        ret2.Add(x); 
                    }
                }
                return ret2.ToArray();
            }
        }

        /// <summary>
        /// Extrae texto de PDF con Local Strategy
        /// </summary>
        /// <returns></returns>
        public string[] ExtractTextFromPdfToArrayLocalStrategy()
        {
            Console.WriteLine("LocalTextExtractionStrategy");
            using (var reader = new PdfReader(_pdfPath))
            {
                NumerOfPages = reader.NumberOfPages;
                var text = new StringBuilder();
                ITextExtractionStrategy its = new LocationTextExtractionStrategy();
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, i, its)
                        .DeleteContoniousWhiteSpace().DeleteNullHexadecimalValues());

                }
                var ret = text.ToString().Split('\n');
                for (var i = 0; i < text.Length; i++)
                {
                    for (var j = i + 1; j < ret.Length - 1; j++)
                    {
                        if (ret[i].Equals(ret[j]))
                        {
                            ret[j] = "-1*";
                        }
                    }
                }
                var ret2 = new List<string>();
                foreach (var x in ret)
                {
                    if (!x.Equals("-1*"))
                    {
                        ret2.Add(x);
                    }
                }
                return ret2.ToArray();
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