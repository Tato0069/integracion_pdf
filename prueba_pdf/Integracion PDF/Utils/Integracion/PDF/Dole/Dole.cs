using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Dole
{
    public class Dole
    {
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Dole(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }



        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            for (; !pdfLines[firstIndex].Trim().Contains("SOLICITADO POR:") && firstIndex + 1 < pdfLines.Length; firstIndex++)
            {
                Console.WriteLine("item: " + pdfLines[firstIndex]);
                //Es una linea de Items
                if (Regex.Match(pdfLines[firstIndex], @"^\d{1,}\s\w{2}\d{4}\s").Success )
                    //&&
                    //Regex.Match(pdfLines[firstIndex].Replace(".", ""), @"\s\d{1,}\s\d{1,}$").Success
                {
                    Console.WriteLine("itemMatch: "+pdfLines[firstIndex]);
                    //es una linea que contiene items
                    var test = pdfLines[firstIndex].Split(' ');
                    //Console.WriteLine(@"MATCH: \w{1}\d{6}\s \n" + pdfLines[firstIndex]);
                    var item = new Item
                    {
                        Sku = test[1],
                        Cantidad = test[0],
                        Precio = test[test.Length - 2]
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
            var aux = str.Replace(" ","").Split(':');
            return aux[1].Trim();
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("R.U.T:"))
                    {
                        Console.WriteLine(_pdfLines[i].Replace(" ", ""));
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                        _readOrdenCompra = false;
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i++].Trim().Contains("ORDEN DE COMPRA"))
                    {
                        OrdenCompra.NumeroCompra = _pdfLines[++i];
                        _readOrdenCompra = true;
                        _readObs = false;
                    }
                }
               
                
                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim()
                            .Equals("CANTIDAD CÓDIGO UNIDAD ESPECIFICACIONES VALOR UNITARIO VALOR TOTAL"))
                    {
                        itemForPage++;
                        //Siguientes 3 lineas cabecera de tabla
                        var items = GetItems(_pdfLines, i++);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                        }
                    }
                }
                if (!_readObs)
                {
                    if (_pdfLines[i++].Trim().Equals("OBSERVACIONES DE LA OC"))
                    {
                        _readObs = true;
                        for (;
                            i < _pdfLines.Length && !_pdfLines[i].Contains("- Se debe adjuntar esta OC a factura.");
                            i++)
                        {
                            OrdenCompra.Observaciones += ", " + _pdfLines[i];
                        }
                        if (OrdenCompra.Observaciones.Length > 2)
                            OrdenCompra.Observaciones = OrdenCompra.Observaciones.Substring(2);
                    }
                }
            }
            OrdenCompra.CentroCosto = "0";
            return OrdenCompra;
        }
    }
}