using System;
using System.Collections.Generic;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.BHPBilliton
{
    public class BhpBilliton
    {
        private bool _readOrdenCompra;
        private bool _readRut = true;
        //private static bool _readCentroCosto = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public BhpBilliton(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            for (; !pdfLines[firstIndex].Trim().Contains("TotalNoOfLines"); firstIndex++)
            {
                //Es una linea de Items
                if (pdfLines[firstIndex].Equals("Seller : "))
                {
                    //es una linea que contiene items
                    var test = pdfLines[++firstIndex].Split(' ');
                    if (test.Length == 1) test = pdfLines[++firstIndex].Split(' ');
                    //Aveces iinea se solapa con numero de tel
                    var item = new Item
                    {
                        Sku = test[1],
                        Cantidad = test[3],
                        Precio = test[4]
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
            return str.Split(' ')[3].Trim();
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Split(':');
            return aux[1].Trim();
        }

        public Integracion_PDF.Utils.OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new Integracion_PDF.Utils.OrdenCompra.OrdenCompra();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
               
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains("Order Number : "))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                        //_readCentroCosto = false;
                        _readRut = false;
                    }
                }
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("VatNumber : "))
                    {
                        OrdenCompra.CentroCosto = "0";
                        var aux2 = _pdfLines[i - 1];
                        var aux1 = _pdfLines[i - 3];
                        var dir = "Destinatario: " + aux1.Trim() + " ," + aux2.Trim();
                        OrdenCompra.Observaciones = dir;
                        OrdenCompra.Direccion = dir;
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
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
                            .Equals("Part Number Line Descripción UM Cantida Precio Monto del Total DeliveryInfo"))
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