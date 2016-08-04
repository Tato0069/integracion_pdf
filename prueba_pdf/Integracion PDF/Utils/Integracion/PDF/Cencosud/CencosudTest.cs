using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Cencosud
{
    public class CencosudTest
    {
        private bool _readOrdenCompra;
        private bool _readRut = true;
        private bool _centroCosto;
        private bool _readCorreo;
        private bool _readComprador;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public CencosudTest(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        private List<Item> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            var itemsArray = new List<string>();
            for (; !pdfLines[firstIndex].Trim().Contains("Neto total sin IVA CLP"); firstIndex++)
            {
                var aux = pdfLines[firstIndex];
                //Es una linea de Items
                if (Regex.Match(pdfLines[firstIndex], @"^\d{1,}\s\d{7}\s").Success)
                {//es una linea que contiene items
                    var test = pdfLines[firstIndex].Split(' ');
                    var item = new Item
                    {
                        Sku = test[1],
                        Cantidad = test[test.Length - 4],
                        Precio = test[test.Length - 2]
                    };
                    aux = test.ArrayToString(2, test.Length - 4);
                    items.Add(item);
                }
                itemsArray.Add(aux);
                if (!_centroCosto)
                {
                    var centroCosto = GetCentroCosto(pdfLines[firstIndex]);
                    if (centroCosto.Length == 0) continue;
                    OrdenCompra.CentroCosto = centroCosto;
                    _centroCosto = true;
                }
            }
            var dir = itemsArray.GetDireccionCencosud();
            OrdenCompra.Direccion = dir;
            //OrdenCompra.Observaciones = dir;
            return items;
        }

       

        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato (X123)
        /// </summary>
        /// <param name="str">Linea de texto</param>
        /// <returns></returns>
        private static string GetCentroCosto(string str)
        {
            var aux = Regex.Match(str, @"\(\w{1}\d{3}\)").Value;
            return aux.Length > 0 ? aux.Substring(1, aux.Length - 2) : aux;
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
            return aux[2].Trim();
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
            return aux[2].Substring(0, 8);
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                var obs = "";
                if (!_readComprador)
                {
                    if (_pdfLines[i].Contains("Comprador"))
                    {
                        var aux = _pdfLines[i].Split(':');
                        obs += $"Comprador: {aux[aux.Length - 1].Trim()}, ";
                        OrdenCompra.Observaciones += obs;
                        _readComprador = true;
                    }
                }
                if (!_readCorreo)
                {
                    if (_pdfLines[i].Contains("Correo"))
                    {
                        var aux = _pdfLines[i].Split(':');
                        obs += $"Correo: {aux[aux.Length - 1].Trim()}";
                        OrdenCompra.Observaciones += obs;
                        _readCorreo = true;
                    }

                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Equals("Datos del proveedor Orden de compra"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }
                }
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Equals("Datos de facturación"))
                    {
                        _readRut = true;
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                    }
                }

                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim().Equals("Item Código Descripción Lugar Entrega Cantidad  UMM Precio Pr. lista"))
                    {
                        itemForPage++;
                        //i = i + 2;
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