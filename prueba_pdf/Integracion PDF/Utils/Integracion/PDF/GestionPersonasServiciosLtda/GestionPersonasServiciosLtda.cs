using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.GestionPersonasServiciosLtda
{
    public class GestionPersonasServiciosLtda
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s\d{1,}\s[[a-zA-Z]{1,2}\d{5,6}\s]?"},
            {1, @"^\d{1,}\s\d{1,}\s[a-zA-Z]{1,2}\s" },//|\s\d{2,}\s\d{2,}$
            {2, @"^\d{1,}\s\d{1,}\s[a-zA-Z]{2}\d{4}\s"},
        };
        private const string RutPattern = "RUT:";
        private const string OrdenCompraPattern = "Orden de Compra";
        private const string ItemsHeaderPattern =
            "# Descripción Precio Total";

        private const string CentroCostoPattern = "de entrega:";
        private const string ObservacionesPattern = "DESPACHAR FACTURA A:";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public GestionPersonasServiciosLtda(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = _pdfReader.ExtractTextFromPdfToArrayDefaultMode();
            //var success = Regex.Match("asdasd", @"^\d{1,}\s\d{1,}\w{1,}\s|\s($)\s\d{2,}\s($)\s\d{2,}$").Success;
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

        #region Funciones Get
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
                    }
                }
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                    }
                }
                //if (!_readCentroCosto)
                //{
                //    if (IsCentroCostoPattern(_pdfLines[i]))
                //    {
                //        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                //        _readCentroCosto = true;
                //    }
                //}
                if (!_readObs)
                {
                    if (IsObservacionPattern(_pdfLines[i]))
                    {
                        var d1 = _pdfLines[++i];
                        var pattern = "GESTION DE PERSONAS Y SERVICIOS LTDA.";
                        d1 = d1.Substring(pattern.Length).Trim();
                        var d2 = _pdfLines[++i];
                        var pattern2 = "RUT: 78.092.910-3";
                        d2 = d2.Substring(pattern2.Length).Trim();
                        i += 2;
                        var d3 = _pdfLines[i];
                        if (d3.ToUpper().Contains("SANTIAGO"))
                        {
                            var split = d3.Split(' ');
                            d3 = split.Length > 1 ? split.ArrayToString(1, split.Length - 1) : "";
                        }
                        OrdenCompra.Observaciones +=
                            d3.Equals("")
                                ? $"Lugar de entrega: {d1}, {d2}"
                                : $"Lugar de entrega: {d1}, {d2}, {d3}";
                        _readObs = true;
                    }
                }
                if (!_readItem)
                {
                    if (IsHeaderItemPatterns(_pdfLines[i]))
                    {
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
                            //Console.WriteLine(items.Count+"<==========================");
                            OrdenCompra.Items.AddRange(items);
                            _readItem = true;
                        }
                    }
                }
            }
            return OrdenCompra;
        }


        private List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
            for (; i < pdfLines.Length; i++)
            //foreach(var str in pdfLines)
            { 
                var aux = pdfLines[i].Replace("-"," ");
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(aux);
                //Console.WriteLine($"{aux}, {optItem}");
                switch (optItem)
                {
                    case 0:
                        Console.WriteLine("=========0======");
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Sku = test0[2],
                            Cantidad = test0[1],
                            Precio =
                                (int.Parse(test0[test0.Length - 1].Replace(".", "")) / int.Parse(test0[1])).ToString()
                        };
                        items.Add(item0);
                        break;
                    case 1:
                        Console.WriteLine("=========1======");
                        var test1 = aux.Split(' ');
                        var item1 = new Item
                        {
                            Sku = GetSku(test1),
                            Cantidad = test1[1].Split(',')[0],
                            Precio = (int.Parse(test1[test1.Length - 1].Replace(".", "")) / int.Parse(test1[1])).ToString()
                        };
                        if (item1.Sku.Equals("W102030")
                            && item1.Cantidad.Equals("22")
                            && item1.Precio.Equals("161592"))
                            break;
                        items.Add(item1);
                        break;
                    case 2:
                        Console.WriteLine("=========2======");
                        var test2 = aux.Split(' ');
                        var item2 = new Item
                        {
                            Sku = test2[2],
                            Cantidad = test2[1],
                            Precio =
                                (int.Parse(test2[test2.Length - 1].Replace(".", "")) / int.Parse(test2[1])).ToString()
                        };
                        items.Add(item2);
                        break;
                }
            }
            //SumarIguales(items);
            return items;
        }


        private string GetSku(string[] test1)
        {
            const int  defaulSkuPosition = 2;
            var ret = "W102030";
            var skuDefaultPosition = test1[defaulSkuPosition].Replace("#", "").Replace("-","");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length);
                if (Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}").Success)
                {
                    var index = Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}").Index;
                    var length = Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}").Length;
                    ret = str.Substring(index, length).Trim();
                }
                else if (Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}").Success)
                {
                    var index = Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}").Index;
                    var length = Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}").Length;
                    ret = str.Substring(index, length).Trim();
                }
            }
            return ret;
        }

        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato (X123)
        /// </summary>
        /// <param name="str">Linea de texto</param>
        /// <returns></returns>
        private static string GetCentroCosto(string str)
        {
            var aux = str.Split(':');
            return aux[1].Trim();
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            var split = str.Split(' ');
            return split[split.Length - 1];
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Split(':')[1].Substring(0, 13);
            return aux.Trim();
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.DeleteDotComa().Trim().DeleteContoniousWhiteSpace();
            foreach (var it in _itemsPatterns.Where(it => Regex.Match(str, it.Value).Success))
            {
                ret = it.Key;
            }
            return ret;
        }

        #endregion


        #region Funciones Is
        private bool IsHeaderItemPatterns(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(ItemsHeaderPattern);
        }

        private bool IsObservacionPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(ObservacionesPattern);
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(OrdenCompraPattern);
        }
        private bool IsRutPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(RutPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(CentroCostoPattern);
        }

        #endregion

    }
}