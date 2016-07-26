using System;
using System.Collections.Generic;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ACH
{
    public class Esach
    {
        private bool _readOrdenCompra = true;
        private bool _readRut;
        //private static bool _centroCosto = true;
        //private static bool _readCentroCosto = true;
        private bool _readObs;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Esach(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }

        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            var index = 1;
            for (; !pdfLines[firstIndex].Trim().Contains("Subtotal"); firstIndex++)
            {
                var test = pdfLines[firstIndex].DeleteContoniousWhiteSpace().Split(' ');

                var item = new Item
                {
                    Sku = test[4],
                    Cantidad = test[1],
                    Precio = test[test.Length - 4]
                };
                items.Add(item);
                //if (_centroCosto) continue;
                //var centroCosto = GetCentroCosto(pdfLines[firstIndex]);
                //if (centroCosto.Length == 0) continue;
                //OrdenCompra.CentroCosto = centroCosto;
                //_centroCosto = true;
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
            var aux = str.Split(':');
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
            var aux = str.Split(' ');
            return aux[1];
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("R.U.T.: "))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                        _readOrdenCompra = false;
                    }
                }
                if (!_readObs)
                {
                    if (_pdfLines[i].Contains("Lugar Realización o Entrega:"))
                    {
                        var aux = _pdfLines[i].Split(':');
                        var str1 = aux[aux.Length - 1].Trim();
                        aux = _pdfLines[++i].Split(':');
                        var str2 = aux[aux.Length - 1].Trim();
                        var dir = str1 + " " + str2;
                        OrdenCompra.Observaciones = "Lugar Realización o Entrega: " +dir;
                        OrdenCompra.Direccion = dir;
                        _readObs = true;
                    }
                    
                }
                
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains("ORDEN DE COMPRA"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                    }
                }
                //TODO AVERIGUAR CC
                //if (!_readCentroCosto)
                //{
                //    if (_pdfLines[i].Trim().Contains("Despachar en "))
                //    {
                //        _readRut = true;
                //        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                //    }
                //}

                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim()
                            .Equals("Pos. Cant. Un. Código SAP Código Descripción N° Cotiz. Valor % Total"))
                    {
                        itemForPage++;
                        //Siguientes 2 lineas cabecera de tabla
                        i += 2;
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                        }
                    }
                }
            }
            return OrdenCompra;
        }
    }
}