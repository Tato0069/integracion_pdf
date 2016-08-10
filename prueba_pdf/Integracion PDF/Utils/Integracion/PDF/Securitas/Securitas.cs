using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Securitas
{
    public class Securitas
    {
        private bool _readOrdenCompra;
        private bool _readRut = true;
        private bool _centroCosto = true;
        private bool _readObs;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompraSecuritas OrdenCompra { get; set; }

        public Securitas(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        

        private List<ItemSecuritas> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<ItemSecuritas>();
            for (; !pdfLines[firstIndex].Trim().Equals("Totales") && firstIndex+1 < pdfLines.Length; firstIndex++)
            {
                //Es una linea de Items
                if (Regex.Match(pdfLines[firstIndex], @"^\w{1}\d{6}\s|^\w{2}\d{5}\s").Success)
                {
                    Console.WriteLine("=================11=====================");
                    //es una linea que contiene items
                    var test = pdfLines[firstIndex].Split(' ');
                    //Console.WriteLine(@"MATCH: \w{1}\d{6}\s \n" + pdfLines[firstIndex]);
                    var item = new ItemSecuritas
                    {
                        Sku = test[0].ToUpper(),
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2],
                        CodigoProyectoSecuritas = test[test.Length - 5]
                    };
                    items.Add(item);
                }
                else if (Regex.Match(pdfLines[firstIndex], @"\s\w{1}\d{6}\s\d{4,}\s\d{6,}\s|\s\w{2}\d{4}\s\d{4,}\s\d{6,}\s").Success)
                {
                    Console.WriteLine("=================22=====================");
                    var test = pdfLines[firstIndex].Split(' ');
                    //Console.WriteLine(@"MATCH: \s\w{1}\d{6}\s\d{4,}\s\d{6,}\s \n" + pdfLines[firstIndex]);
                    var item = new ItemSecuritas
                    {
                        Sku = GetSku(test),
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2],
                        CodigoProyectoSecuritas = test[test.Length - 5]
                    };
                    //Console.WriteLine("ITEM:"+item.ToString());
                    items.Add(item);
                }

                //TODO AGREGAR CASO DE QUE SKU ESTE EN MEDIO
                //else if (
                //    Regex.Match(pdfLines[firstIndex].Replace(".", ""), @"[a-zA-Z]{1}\d{6}|[a-zA-Z]{2}\d{5}")
                //        .Success)
                else if (Regex.Match(pdfLines[firstIndex].Replace(".", ""), @"\s\d{4,}\s\d{6,}\s\d{1,}\s\d{1,}\s\d{1,}").Success)
                {
                    Console.WriteLine("=================33=====================");
                    var test = pdfLines[firstIndex].Split(' ');
                    var item = new ItemSecuritas
                    {
                        Sku = "W102030",
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2],
                        CodigoProyectoSecuritas = test[test.Length - 5]
                    };
                    items.Add(item);
                }
               
                //if (_centroCosto) continue;
                    //var centroCosto = GetCentroCosto(pdfLines[firstIndex]);
                    //if (centroCosto.Length == 0) continue;
                    //OrdenCompra.CentroCosto = centroCosto;
                    //_centroCosto = true;
                }
            items = new List<ItemSecuritas>(items.OrderBy(it => it.CodigoProyectoSecuritas));
            return items;
        }

        private string GetSku(string[] test1)
        {
            var ret = "W102030";
            var skuDefaultPosition = test1[test1.Length - 6].Replace("#", "");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length -1);
                if (Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}|\s[a-zA-Z]{1}\s\d{6}").Success)
                {
                    var index = Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}|\s[a-zA-Z]{1}\s\d{6}").Index;
                    var length = Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}|\s[a-zA-Z]{1}\s\d{6}").Length;
                    ret = str.Substring(index, length).DeleteContoniousWhiteSpace();
                }
                else if (Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}|\s[a-zA-Z]{2}\s\d{5}").Success)
                {
                    var index = Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}|\s[a-zA-Z]{2}\s\d{5}").Index;
                    var length = Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}|\s[a-zA-Z]{2}\s\d{5}").Length;
                    ret = str.Substring(index, length).DeleteContoniousWhiteSpace();
                }
            }
            return ret.Replace(" ","");
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
            var aux = str.Split('°');
            return aux[1].Trim();
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

        public OrdenCompraSecuritas GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompraSecuritas();
            var itemForPage = 0;
            var sol = false;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains("Orden de compra N°"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }
                }
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("R.U.T."))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                        //_readCentroCosto = false;
                    }
                }
                if (!sol)
                {
                    if (_pdfLines[i].Trim().Contains("Solicitante "))
                    {
                        var solicitante = GetSolicitante(_pdfLines[i]);
                        OrdenCompra.NumeroCompra = $"{OrdenCompra.NumeroCompra}/ {solicitante}";
                        //OrdenCompra.Observaciones += $"{solicitante}, N° OC: {OrdenCompra.NumeroCompra}";
                        sol = true;
                    }
                }
                //if (!_readObs)
                //{
                //    if (_pdfLines[i].Trim().Contains("Despachar en "))
                //    {
                //        _readObs = true;
                //        var str = _pdfLines[i];
                //        var dir = str.Substring(str.IndexOf("Despachar en ", StringComparison.Ordinal));
                //        OrdenCompra.Direccion = dir;
                //        OrdenCompra.Observaciones = dir;
                //    }
                //}

                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim()
                            .Equals("Sirvanse proporcionarnos los siguientes productos :"))
                    {
                        itemForPage++;
                        //Siguientes 3 lineas cabecera de tabla
                        var items = GetItems(_pdfLines, i++);
                        if (items.Count > 0)
                        {
                            foreach (var it in items)
                            {
                                OrdenCompra.AddItemSecuritas(it);
                            }
                            //OrdenCompra.Items.AddRange(items);
                        }
                    }
                }
            }
            return OrdenCompra;
        }

        private string GetSolicitante(string str)
        {
            str = str.DeleteContoniousWhiteSpace();
            var split = str.Split(' ');
            var nombre = split[2].Trim();
            var apellido = split[1].Trim();
            var ret = $"{nombre.ToUpper().ToCharArray()[0]}.{apellido}".Replace(",","");
            return ret;
        }
    }
}