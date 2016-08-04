using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClinicaAlemana
{
    public class ClinicaAlemana
    {

        private bool _readOrdenCompra = true;
        private bool _readRut;
        private readonly PDFReader _pdfReader;
        private bool _readObs;
        private bool _readItems = true;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public ClinicaAlemana(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }


        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            for (; firstIndex < pdfLines.Length; firstIndex++)
            {
                var str = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace();
                //80 300251174
                if (Regex.Match(str, @"^\d{2,}\s\d{6,10}\s").Success)
                {
                    var test = pdfLines[firstIndex].Trim().DeleteContoniousWhiteSpace().Split(' ');
                        //pdfLines[firstIndex].Trim().Split(' ');
                    var item = new Item
                    {
                        Sku = test[1],
                        Cantidad = test[test.Length - 5],
                        Precio = test[test.Length - 3].Replace("$","")
                    };
                    items.Add(item);
                }

            }
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
            var aux = str.Replace(" ", "").Split(':');
            return aux[1];
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Replace(" ","").Split(':');
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
                //        OrdenCompra.Observaciones = "Entregar bienes en: " + dir;
                //        OrdenCompra.Direccion = dir;
                //        _readObs = true;
                //    }
                //}

                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("RUT : "))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                        _readOrdenCompra = false;
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains("PEDIDO N° :"))
                    {
                        OrdenCompra.CentroCosto = "0";
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                        _readItems = false;
                    }
                }
                //if (itemForPage < _pdfReader.NumerOfPages)
                //{
                if (!_readItems)
                {
                    if (_pdfLines[i].Trim()
                        .Contains(
                            "N° Código Prod/Serv Descripción del Producto"))
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
