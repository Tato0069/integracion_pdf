using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.UNAB
{
    public class Unab
    {

        private bool _readOrdenCompra;
        private bool _readRut = true;
        private readonly PDFReader _pdfReader;
        private bool _readObs;
        private bool _readItems = true;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Unab(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        
        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            var lastSku = "W102030";
            for (; firstIndex < pdfLines.Length; firstIndex++)
            {
                var str = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace();
                //Console.WriteLine(str);
                if (Regex.Match(str, @"1\s\w{1}\d{6}").Success) //\s\w{1}\d{6}$
                {
                    lastSku= GetSku(str);
                }
                if (Regex.Match(str, @"^\d{1,}\s-\s\d{1,}\s\w{1}\d{6}\s").Success)
                {
                    var test = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace().Split(' ');
                    var item = new Item
                    {
                        Sku = test[3],
                        Cantidad = test[test.Length - 3].Split('.')[0].Replace(",", "."),
                        Precio = test[test.Length - 2].Split('.')[0].Replace(",", ".")
                    };
                    items.Add(item);
                }
                else if (Regex.Match(str, @"^\w{1}\d{6}\s").Success &&
                         Regex.Match(str.Replace(",", "").Replace(".", ""),
                             @"\d{1,}\s\d{1,}\s\d{1,}$").Success)
                {
                    var test = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace().Split(' ');
                    var item = new Item
                    {
                        Sku = test[0],
                        Cantidad = test[test.Length - 3].Split('.')[0].Replace(",", "."),
                        Precio = test[test.Length - 2].Split('.')[0].Replace(",", ".")
                    };
                    items.Add(item);
                }else if (str.Contains("(CODIGO"))
                {
                    Console.WriteLine(str.Replace(",", "").Replace(".", "") +" === "+lastSku);
                    var test = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace().Split(' ');
                    var sku = GetSku(str);
                    if (sku.Equals(""))
                    {
                       sku = SearchNextSku(pdfLines, ++firstIndex);
                    }
                    var item = new Item
                    {
                        Sku = sku,
                        Cantidad = test[test.Length - 3].Split('.')[0].Replace(",", "."),
                        Precio = test[test.Length - 2].Split('.')[0].Replace(",", ".")
                    };
                    items.Add(item);

                }
                else if (Regex.Match(str.Replace(",", "").Replace(".", ""),
                    @"\d{1,}\s\d{1,}\s\d{1,}$").Success)
                {
                    var test = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace().Split(' ');
                    var item = new Item
                    {
                        Sku = lastSku,
                        Cantidad = test[test.Length - 3].Split('.')[0].Replace(",", "."),
                        Precio = test[test.Length - 2].Split('.')[0].Replace(",", ".")
                    };
                    items.Add(item);
                }

            }
            return items;
        }

        private string SearchNextSku(string[] pdfLines, int firstIndex)
        {
            string ret = "W102030";
            while (firstIndex < pdfLines.Length && ret.Equals("W102030"))
            {
                ret = GetSku2(pdfLines[firstIndex++]);
            }
            return ret;
        }

        private string GetSku(string str)
        {
            var index = Regex.Match(str, @"\s\w{1}\d{6}").Index;
            return index == -1 || index == 0 ? "" : str.Substring(index, 8).Trim();
        }

        private string GetSku2(string str)
        {
            var index = Regex.Match(str, @"\w{1}\d{6}\)").Index;
            return index == -1 ? "W102030" : str.Substring(index, 7).Trim();
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
            var index = Regex.Match(str, @"\s\w{3}\d{2}-\d{10}\s").Index;
            var length = Regex.Match(str, @"\s\w{3}\d{2}-\d{10}\s").Length;
            return str.Substring(index, length).Trim();
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Split(' ');
            return aux[1];
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra {CentroCosto = "0"};

            for (var i = 0; i < _pdfLines.Length; i++)
            {
                //if (!_readObs)
                //{
                //    if (_pdfLines[i].Contains("Entregar bienes en:"))
                //    {
                //        var a1 = _pdfLines[i].Split(':')[1].Trim();
                //        var a2 = _pdfLines[i + 1].Substring(_pdfLines[i + 1].IndexOf("96670840-9", StringComparison.Ordinal) + 11);
                //        var a3 = _pdfLines[i + 2].Substring(11);
                //        var dir = a1 + " " + a2 + " " + a3;
                //        OrdenCompra.Observaciones = "Entregar bienes en: "+ dir;
                //        OrdenCompra.Direccion = dir;
                //        _readObs = true;
                //    }
                //}
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains(" Orden de Compra "))
                    {
                        OrdenCompra.CentroCosto = "0";
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }
                }
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("R.U.T.: "))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                        _readItems = false;
                    }
                }
                //if (itemForPage < _pdfReader.NumerOfPages)
                //{
                if (!_readItems) { 
                    if (_pdfLines[i].Trim()
                        .Contains("Lín-Env"))
                            //.Equals("Lín-Env Art/Descripción Id Fabricante Cantidad Precio U. Neto Total Neto"))
                    {
                        //itemForPage++;
                        var items = GetItems(_pdfLines, ++i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                            _readItems = true;
                        }
                    }
                }
            }
            return OrdenCompra;
        }
    }
}