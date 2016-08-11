using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AridosSantaFeSA
{
    class MaquinariasSantaFe
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s\d{1,}\s[a-zA-Z]{1,}\s"},
            {1, @"^\d{1,}\s[a-zA-Z0-9]{10}\s\d{1,}\s[a-zA-Z]{1,}\s" },
            {2, @"^\d{1,}\s[a-zA-Z]{1,}\s\d{1,}\s[a-zA-Z]{1,}\s"},
            {3, @"^\d{1,}\s\w{1,}\d{1,}\s[a-zA-Z]{1,}\s" }
            //12 C1Z1Z11455 2,00 
        };
        private const string RutPattern = "RUT";
        private const string OrdenCompraPattern = "Orden de Compra N";
        private const string ItemsHeaderPattern =
            "Item Precio Unitario";

        private const string CentroCostoPattern = "de entrega:";
        private const string ObservacionesPattern = "Tienda :";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public MaquinariasSantaFe(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = _pdfReader.ExtractTextFromPdfToArrayDefaultMode();
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
                CentroCosto = "0",
                TipoPareoCentroCosto = TipoPareoCentroCosto.SinPareo
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
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                        _readRut = true;
                    }
                }

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                        _readCentroCosto = true;
                    }
                }
                //if (!_readObs)
                //{
                //    if (IsObservacionPattern(_pdfLines[i]))
                //    {
                //        OrdenCompra.Observaciones +=
                //            $"{_pdfLines[i].Trim().DeleteContoniousWhiteSpace()}, " +
                //            $"{_pdfLines[++i].Trim().DeleteContoniousWhiteSpace()}";
                //        _readObs = true;
                //        _readItem = false;
                //    }
                //}
                if (!_readItem)
                {
                    if (IsHeaderItemPatterns(_pdfLines[i]))
                    {
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
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
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(aux);
                switch (optItem)
                {
                    case 0:
                        Console.WriteLine("==================ITEM CASE 0=====================");
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Sku = GetSku(pdfLines[++i].Trim().DeleteContoniousWhiteSpace().Split(' ')),
                            Cantidad = test0[1].Split('.')[0],
                            Precio = test0[test0.Length - 2].Replace(".", "").Split(',')[0],
                            TipoPareoProducto = TipoPareoProducto.SinPareo
                        };
                        items.Add(item0);
                        break;
                    case 1:
                        Console.WriteLine("==================ITEM CASE 1=====================");
                        var test1 = aux.Split(' ');
                        var item1 = new Item
                        {
                            Sku = pdfLines[++i].Trim().DeleteContoniousWhiteSpace(),
                            Cantidad = test1[2].Split('.')[0],
                            Precio = test1[test1.Length - 2].Replace(".", "").Split(',')[0],
                            TipoPareoProducto = TipoPareoProducto.SinPareo
                        };
                        items.Add(item1);
                        break;
                    case 2:
                        Console.WriteLine("==================ITEM CASE 2=====================");
                        var test2 = aux.Split(' ');
                        var item2 = new Item
                        {
                            Sku = "W102030",
                            Cantidad = test2[1].Split('.')[0],
                            Precio = test2[test2.Length - 2].Replace(".", "").Split(',')[0],
                            TipoPareoProducto = TipoPareoProducto.SinPareo
                        };
                        //pdfLines[++i].Trim().DeleteContoniousWhiteSpace()
                        var concatAll = "";
                        aux = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace();
                        for (var j = i + 2; j < pdfLines.Length && GetFormatItemsPattern(aux) == -1; j++)
                        {
                            concatAll += $" {aux}";
                            aux = pdfLines[j].Trim().DeleteContoniousWhiteSpace();
                        }
                        item2.Sku = GetSku(concatAll.DeleteContoniousWhiteSpace().Split(' ')).Replace("/", "");
                        items.Add(item2);
                        break;
                    //case 3:
                    //    Console.WriteLine("==================ITEM CASE 3=====================");
                    //    var test3 = aux.Split(' ');
                    //    var item3 = new Item
                    //    {
                    //        Sku = "W102030",
                    //        Cantidad = GetCantidad(test3),
                    //        Precio = test3[test3.Length - 2].Replace(".", "").Split(',')[0],
                    //        TipoPareoProducto = TipoPareoProducto.SinPareo
                    //    };
                    //    //pdfLines[++i].Trim().DeleteContoniousWhiteSpace()
                    //    concatAll = "";
                    //    aux = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace();
                    //    for (var j = i + 2; j < pdfLines.Length && GetFormatItemsPattern(aux) == -1; j++)
                    //    {
                    //        concatAll += $" {aux}";
                    //        aux = pdfLines[j].Trim().DeleteContoniousWhiteSpace();
                    //    }
                    //    item3.Sku = GetSku(concatAll.DeleteContoniousWhiteSpace().Split(' ')).Replace("/", "");
                    //    items.Add(item3);
                    //    break;
                }
            }
            //SumarIguales(items);
            return items;
        }

        private string GetCantidad(string[] test0)
        {
            var ret = "-1";
            for (var i = 0; i < test0.Length; i++)
            {
                if (test0[i].Contains(","))
                {
                    var cantidad = -1;
                    if (int.TryParse(test0[i].Replace(",", ""), out cantidad))
                    {
                        return test0[i].Split(',')[0];
                    }
                }
            }
            return ret;
        }

        private string GetSku(string[] test1)
        {
            var ret = "W102030";
            var skuDefaultPosition = test1[0].Replace("#", "");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length - 1);
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
            return split[split.Length - 1].Trim();
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var split = str.Split(' ');
            return split[0];
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.DeleteDotComa();
            foreach (var it in _itemsPatterns.Where(it => Regex.Match(str.Replace("Ñ", "N").Replace(",", "").Replace(".", ""), it.Value).Success))
            {
                return it.Key;
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
