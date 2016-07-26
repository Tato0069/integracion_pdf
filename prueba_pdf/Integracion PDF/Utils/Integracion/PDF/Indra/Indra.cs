using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Indra
{
    public class Indra
    {
        private bool _readOrdenCompra = true;
        private bool _readRut;
        private bool _centroCosto = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Indra(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }

        private static bool IsComment(string str)
        {
            return str.Contains("#");
        }
        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            for (; !pdfLines[firstIndex].Trim().Contains("Valor neto total sin IVA CLP"); firstIndex++)
            {
                //for(var i = 0; i < str.Length; i++) { 
                //Es una linea de Items
                if (Regex.Match(pdfLines[firstIndex], @"\d{2,4}\s\d{10}\s\w+").Success)
                {
                    var test = pdfLines[firstIndex++].Split(' ');
                    var test2 = pdfLines[++firstIndex].Split(' ');
                    var item = new Item
                    {
                        Sku = test[1],
                        Cantidad = test2[0].DeleteDecimal().ReplaceDot(),
                        Precio = test2[2].ReplaceDot()
                    };

                    items.Add(item);
                }
                
        }
            return items;
        }


        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato (X123)
        /// </summary>
        /// <param name="str">Linea de texto</param>
        /// <returns></returns>
        private static string GetCentroCosto(string[] str)
        {
            return str[str.Length - 1].Split('|')[1];
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            var aux = str.Split(' ');
            return aux[4].Trim();
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

        public Integracion_PDF.Utils.OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new Integracion_PDF.Utils.OrdenCompra.OrdenCompra();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains("RUT:"))
                    {
                        _readRut = true;
                        _readOrdenCompra = false;
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Equals("DIMERC S.A. Núm. pedido/Fecha"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                        _centroCosto = false;
                    }
                }
                if (!_centroCosto)
                {
                    var split = _pdfLines[i].Split(' ');
                    if (split.Length > 0)
                    {
                        if (split[0].Equals("Proyecto:") && split.Length == 4)
                        {
                            OrdenCompra.CentroCosto = GetCentroCosto(split);
                            _centroCosto = true;
                        }
                    }
                }

                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim()
                            .Equals("la posición contiene los siguientes servicios:"))
                    {
                        itemForPage++;
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
