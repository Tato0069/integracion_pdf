using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Dellanatura
{
    public class Dellanatura
    {
        private bool _readOrdenCompra = true;
        private bool _readRut;
        private bool _readObs;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Dellanatura(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }



        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            for (; firstIndex + 1 < pdfLines.Length; firstIndex++)
            {
                var str = pdfLines[firstIndex];//.DeleteContoniousWhiteSpace();
                //Es una linea de Items
                if (Regex.Match(str, @"\s\w{1,2}\d{6}\s").Success)
                {
                    Console.WriteLine(pdfLines[firstIndex]);
                    str = str.DeleteContoniousWhiteSpace();
                    var test = str.Split(' ');
                    var item = new Item
                    {
                        Sku = GetSku(str),
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2].Split(',')[0].Replace(".", "")
                    };
                    items.Add(item);
                }
                //else if (Regex.Match(pdfLines[firstIndex], @"\s\w{1}\d{6}\s\d{4,}\s\d{6,}\s").Success)
                //{
                //    var test = pdfLines[firstIndex].Split(' ');
                //    var item = new Item
                //    {
                //        Sku = test[test.Length - 6],
                //        Cantidad = test[test.Length - 3],
                //        Precio = test[test.Length - 2]
                //    };
                //    items.Add(item);
                //}
                //else if (Regex.Match(pdfLines[firstIndex].Replace(".", ""), @"\s\d{4,}\s\d{6,}\s\d{1,}\s\d{1,}\s\d{1,}").Success)
                //{
                //    var test = pdfLines[firstIndex].Split(' ');
                //    var item = new Item
                //    {
                //        Sku = "W102030",
                //        Cantidad = test[test.Length - 3],
                //        Precio = test[test.Length - 2]
                //    };
                //    items.Add(item);
                //}
            }
            return items;
        }

        private string GetSku(string str)
        {
            var index = Regex.Match(str, @"\s[a-zA-Z]{1,2}\d{6}\s").Index;
            var length = Regex.Match(str, @"\s[a-zA-Z]{1,2}\d{6}\s").Length;
            return str.Substring(index, length).Trim();
        }


        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato: Despachar en ......
        /// </summary>
        /// <param name="str">Linea de texto</param>
        /// <returns></returns>
        private static string GetCentroCosto(string str)
        {
            return str.Substring(str.IndexOf("Despachar en ", StringComparison.Ordinal));
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            return str;
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.DeleteNullHexadecimalValues().Split(' ');
            return aux[1];
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra {CentroCosto = "0"};
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("R.U.T.:"))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                        _readOrdenCompra = false;
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains("ORDEN DE COMPRA"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i + 1]);
                        _readOrdenCompra = true;
                    }
                }

                if (!_readObs)
                {
                    if (_pdfLines[i].Trim().Contains("DESPACHO: "))
                    {
                        _readObs = true;
                        OrdenCompra.Observaciones += _pdfLines[i].Trim();
                    }
                }
                if (_pdfLines[i].Trim()
                    .Equals("CODIGO COD TEC DESCRIPCION CANT. P.NETO P.TOTAL"))
                {
                    var items = GetItems(_pdfLines, i++);
                    if (items.Count > 0)
                    {
                       OrdenCompra.Items.AddRange(items);
                    }
                }
            }
            return OrdenCompra;
        }
    }
}