using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Securitas
{
    public class SecuritasAustral
    {
        private bool _readOrdenCompra;
        private bool _centroCosto = true;
        private bool _readObs;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompraSecuritas OrdenCompra { get; set; }

        public SecuritasAustral(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }



        private List<ItemSecuritas> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<ItemSecuritas>();
            for (; !pdfLines[firstIndex].Trim().Equals("Totales") && firstIndex + 1 < pdfLines.Length; firstIndex++)
            {
                //Es una linea de Items
                if (Regex.Match(pdfLines[firstIndex], @"^\w{1}\d{6}\s").Success)
                {
                    //es una linea que contiene items
                    var test = pdfLines[firstIndex].Split(' ');
                    //Console.WriteLine(@"MATCH: \w{1}\d{6}\s \n" + pdfLines[firstIndex]);
                    var item = new ItemSecuritas
                    {
                        Sku = test[0],
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2],
                        CodigoProyectoSecuritas = test[test.Length - 5]
                    };
                    items.Add(item);
                }else if (Regex.Match(pdfLines[firstIndex], @"\s\w{1}\d{6}\s\d{4,}\s\d{6,}\s").Success)
                {
                    var test = pdfLines[firstIndex].Split(' ');
                    //Console.WriteLine(@"MATCH: \s\w{1}\d{6}\s\d{4,}\s\d{6,}\s \n" + pdfLines[firstIndex]);
                    var item = new ItemSecuritas
                    {
                        Sku = test[test.Length - 6],
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2],
                        CodigoProyectoSecuritas = test[test.Length - 5]
                    };
                    //Console.WriteLine("ITEM:"+item.ToString());
                    items.Add(item);
                }else if(Regex.Match(pdfLines[firstIndex].Replace(".",""), @"\s\d{4,}\s\d{6,}\s\d{1,}\s\d{1,}\s\d{1,}").Success)
                {
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
                if (_centroCosto) continue;
                var centroCosto = GetCentroCosto(pdfLines[firstIndex]);
                if (centroCosto.Length == 0) continue;
                OrdenCompra.CentroCosto = centroCosto;
                _centroCosto = true;
            }
            items = new List<ItemSecuritas>(items.OrderBy(it => it.CodigoProyectoSecuritas));
            return items;
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
            var aux = str.Split('-');
            return aux[0];
        }

        public OrdenCompraSecuritas GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompraSecuritas();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains("Orden de compra N°"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                    }
                }
                //if (!_readRut)
                //{
                //    if (_pdfLines[i].Trim().Contains("R.U.T. "))
                //    {
                //        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                //        _readRut = true;
                //        //_readCentroCosto = false;
                //    }
                //}
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
                                OrdenCompra.ItemsSecuritas.Add(it);
                            }
                            //OrdenCompra.Items.AddRange(items);
                        }
                    }
                }
            }

            return OrdenCompra;
        }
    }
}