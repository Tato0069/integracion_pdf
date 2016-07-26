using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace IntegracionPDF.Integracion_PDF.Utils
{
    public class ExcelReader
    {
        private readonly string _excelPath;

        /// <summary>
        /// Ruta Completa de Archivo PDF
        /// </summary>
        public string ExcelPath => _excelPath;

        /// <summary>
        /// Ruta Base de Archivo PDF
        /// </summary>
        public string ExcelRootPath
            => _excelPath.Substring(0, _excelPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

        /// <summary>
        /// Nombre de Archivo PDF
        /// </summary>
        public string ExcelFileName => _excelPath.Substring(_excelPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

        /* ==================================
             *               XLSX
             * ==================================
             * Provider: Microsoft.ACE.OLEDB.12.0
             * Extended Properties: Excel 12.0
             * 
             * ==================================
             *              XLS
             * ==================================
             * Provider: Microsoft.Jet.OLEDB.4.0
             * Extended Properties: Excel 8.0
             * 
             */

        private const string ProviderXlsx = "Microsoft.ACE.OLEDB.12.0";
        private const string ExtendedPropertiesXlsx = "Excel 12.0";
        private const string ProviderXls = "Microsoft.Jet.OLEDB.4.0";
        private const string ExtendedPropertiesXls = "Excel 8.0";
        private readonly string _conectionString;
        public string[] RawArrayString { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="excelPath">Ruta de Excel</param>
        public ExcelReader(string excelPath)
        {
            _excelPath = excelPath;
            var extension = ExcelFileName
                .Substring(ExcelFileName
                    .LastIndexOf(".", StringComparison.Ordinal)).ToUpper().Replace(".", "");
            if (extension.Equals("XLSX"))
                _conectionString = $@"Provider={ProviderXlsx};Data Source={ExcelPath};" +
                                   $@"Extended Properties='{ExtendedPropertiesXlsx};HDR=Yes;'";
            else
                _conectionString = $@"Provider={ProviderXlsx};Data Source={ExcelPath};" +
                                   $@"Extended Properties='{ExtendedPropertiesXls};HDR=Yes;'";
        }

        /// <summary>
        /// Transforma el Excel en PDF y retorna el Texto como una sola linea,
        /// usada para identificar el Formato del Excel (Empresa)
        /// </summary>
        /// <returns>Texto Raw de Excel</returns>
        public string ExtractTextLikePdfRaw()
        {
            var extension = _excelPath
                .Substring(_excelPath
                    .LastIndexOf(".", StringComparison.Ordinal) + 1)
                .ToUpper();
            var pdfOutput = extension.Equals("XLSX")
                ? _excelPath.Replace(".xlsx", ".pdf").Replace(".XLSX", ".pdf")
                : _excelPath.Replace(".xls", ".pdf").Replace(".XLS", ".pdf");
            MainConverter.SaveExcelToPdf(_excelPath, pdfOutput);
            var pdfReader = new PDFReader(pdfOutput);
            var ret = pdfReader.ExtractTextFromPdfToString();
            RawArrayString = pdfReader.ExtractTextFromPdfToArray();
            Main.Main.DeleteFile(pdfOutput);
            return ret;
        }


        //TODO USAR MAS SIMPLE
        public List<List<string>> ExtractTextToMatrixFromSheet(string hoja)
        {

            using (var connection = new OleDbConnection(_conectionString))
            {
                try
                {
                    var excelHojasMatriz = new List<List<string>>();
                    connection.Open();
                    var command = new OleDbCommand($"select * from [{hoja}$]", connection);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var ret = new List<string>();
                            for (var i = 0; i < dr.VisibleFieldCount; i++)
                            {
                                ret.Add(dr[i].ToString());
                            }
                            if (ret.Aggregate("", (current, y) => current + y).Replace(" ", "").Length > 0)
                                excelHojasMatriz.Add(ret);
                        }
                    }
                    return excelHojasMatriz;
                }
                catch (OleDbException)
                {
                }
                return null;

            }
        }



        /// <summary>
        /// Retorna el texto del Excel en una Lista de Listas (Arreglo de Arreglos) //Matriz
        /// </summary>
        /// <returns>texto[Fila][Columna]</returns>
        public List<List<List<string>>> ExtractTextToMatrix(List<string> hojas)
        {

            using (var connection = new OleDbConnection(_conectionString))
            {
                connection.Open();
                var excelHojasMatriz = new List<List<List<string>>>();
                foreach (var command in 
                    hojas
                        .Select(hoja
                            => new OleDbCommand($"select * from [{hoja}$]", connection)))
                {
                    try
                    {
                        using (var dr = command.ExecuteReader())
                        {
                            var excelList = new List<List<string>>();
                            while (dr.Read())
                            {
                                var ret = new List<string>();
                                for (var i = 0; i < dr.VisibleFieldCount; i++)
                                {
                                    ret.Add(dr[i].ToString());
                                }
                                if (ret.Aggregate("", (current, y) => current + y).Replace(" ", "").Length > 0)
                                    excelList.Add(ret);
                            }
                            excelHojasMatriz.Add(excelList);
                        }
                    }
                    catch (OleDbException)
                    {
                    }
                }
                return excelHojasMatriz;
            }
        }



        /// <summary>
        /// Extrae Texto de Excel y retorna una Lista con dicho texto.
        /// </summary>
        /// <returns>Lista con Texto del Excel</returns>
        public List<List<string>> ExtractTextToArray(List<string> hojas)
        {
            using (var connection = new OleDbConnection(_conectionString))
            {
                connection.Open();
                var excelHojaList = new List<List<string>>();
                foreach (var command in 
                    hojas
                        .Select(hoja
                            => new OleDbCommand($"select * from [{hoja}$]", connection)))
                {
                    try
                    {
                        using (var dr = command.ExecuteReader())
                        {
                            var exelHoja = new List<string>();
                            
                            while (dr.Read())
                            {
                                var ret = "";
                                for (var i = 0; i < dr.VisibleFieldCount; i++)
                                {
                                    ret += $" {dr[i]}";
                                }
                                if (ret.Aggregate("", (current, y) => current + y).Replace(" ", "").Length > 0)
                                    exelHoja.Add(ret.Trim().DeleteContoniousWhiteSpace());
                            }
                            excelHojaList.Add(exelHoja);
                        }
                    }
                    catch (OleDbException)
                    {
                    }
                }
                return excelHojaList;
            }
        }



    }
}