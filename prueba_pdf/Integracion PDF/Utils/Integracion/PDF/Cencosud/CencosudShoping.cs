using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Cencosud
{
    public class CencosudShoping
    {
        #region Variables
        private const string ItemPattern = @"^\d{1,}\s\d{7}\s";
        private const string RutPattern = "Datos de facturación";
        private const string OrdenCompraPattern = "Número orden";
        private const string ItemsHeaderPattern =
            "Item Código Descripción Lugar Entrega Cantidad  UMM Precio Pr. lista";
        private const string ItemsFooterPattern = "Neto total sin IVA CLP";
        private const string CentroCostoPattern = @"\(\w{1}\d{3}\)";
        private bool _readOrdenCompra;
        private bool _readRut = true;
        private bool _readItems = true;
        private bool _readCentroCosto;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public CencosudShoping(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        private bool IsItem(string str)
        {
            str = str.DeleteDotComa();
            return Regex.Match(str, ItemPattern).Success;
        }
        private List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
            for (; i < pdfLines.Length; i++)
            {
                var aux = pdfLines[i].DeleteContoniousWhiteSpace().Trim();
                if (IsItemsFooterPattern(aux)) break;
                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(pdfLines[i]))
                    {
                        var centroCosto = GetCentroCosto(pdfLines[i]);
                        if (centroCosto.Length == 0) continue;
                        OrdenCompra.CentroCosto = centroCosto;
                        _readCentroCosto = true;
                    }
                    
                }
                if (IsItem(aux))
                {
                    var test = aux.Split(' ');
                    var item = new Item
                    {
                        Sku = test[3],
                        Cantidad = test[1],
                        Precio = test[test.Length - 1].Equals("$") ?
                        test[test.Length - 2].Replace(".", "") :
                        test[test.Length - 3].Replace(".", "")
                    };
                    items.Add(item);
                }
            }
            //SumarIguales(items);
            return items;
        }

        private static void SumarIguales(List<Item> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                for (var j = i + 1; j < items.Count; j++)
                {
                    if (items[i].Sku.Equals(items[j].Sku))
                    {
                        items[i].Cantidad = (int.Parse(items[i].Cantidad) + int.Parse(items[j].Cantidad)).ToString();
                        items.RemoveAt(j);
                        j--;
                        Console.WriteLine($"Delete {j} from {i}");
                    }
                }
            }
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
            var aux = str.Trim().Split(' ');
            return aux[aux.Length - 1];
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var index = str.IndexOf("RUT:", StringComparison.Ordinal);
            var aux = str.Substring(index);
            Console.WriteLine("AUX: "+aux);
            return aux.Split(';')[1];
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra
            {
                CentroCosto = "0"
            };
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }

                }
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        _readRut = true;
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                        _readItems = false;
                    }
                }
                if (!_readItems)
                {
                    if (IsItemHeaderPattern(_pdfLines[i]))
                    {
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


        private bool IsItemHeaderPattern(string str)
        {
            return str.Trim().Contains(ItemsHeaderPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return Regex.Match(str, ItemPattern).Success;
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.Trim().Contains(OrdenCompraPattern);
        }
        private bool IsItemsFooterPattern(string str)
        {
            return str.Trim().Contains(ItemsFooterPattern);
        }

        private bool IsRutPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(RutPattern);
        }
    }
}