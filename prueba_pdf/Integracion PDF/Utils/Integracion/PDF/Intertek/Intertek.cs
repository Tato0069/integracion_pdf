using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Intertek
{
    class Intertek
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {// H502910 3657 7314
            {0, @"[a-zA-Z]{1,2}\d{5,6}\s\d{1,}\s\d{1,}$"},
            {1, @"[a-zA-Z]{1,2}\d{5,6}\s\d{1,}\s\d{1,}\s\d{1,}$" }
        };
        private const string RutPattern = "RUT.:";
        private const string OrdenCompraPattern = "OC N°:";
        private const string ItemsHeaderPattern =
            "Unitario (neto)";

        private const string CentroCostoPattern = "CC %";
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

        public Intertek(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = _pdfReader.ExtractTextFromPdfToArrayDefaultModeDeleteHexadeximalNullValues();
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
                //CentroCosto = "0",
                TipoPareoCentroCosto = TipoPareoCentroCosto.PareoDescripcionLike
            };
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        Console.WriteLine($"OC: {_pdfLines[i]}");
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

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        var p = 0.0;
                        var cc = "0";
                        for(var j = i; j < _pdfLines.Length - 1; j++)
                        {
                            if (Regex.Match(_pdfLines[j], @"^\d{1,}-\d{3}-\d{2}\s").Success)
                            {
                                var raw = _pdfLines[j].Trim().DeleteContoniousWhiteSpace();
                                //if (raw.Contains("IVA 19%"))
                                //{
                                //    var splitAux = raw.Split(' ');
                                //    raw = splitAux.ArrayToString(0, splitAux.Length - 3);
                                //}
                                //var split = raw.Split(' ');
                                //if (p < float.Parse(split[split.Length - 3]))
                                //{
                                //    p = float.Parse(split[split.Length - 3]);
                                //    cc = split.ArrayToString(0, split.Length- 3);
                                //}
                                var f = 0.0;
                                var raw2 = "";
                                foreach (var st in raw.Split(' '))
                                {
                                    if(!double.TryParse(st,out f))
                                    {
                                        raw2 += $" {st}";
                                    }else
                                    {
                                        raw2 += $" {st}";
                                        break;
                                    }
                                }
                                Console.WriteLine($"RAW2: {raw2}");
                                var split = raw2.Split(' ');
                                if (p < float.Parse(split[split.Length - 1]))
                                {
                                    p = float.Parse(split[split.Length - 1]);
                                    cc = split.ArrayToString(0, split.Length - 1);
                                }
                            }
                        }
                        OrdenCompra.CentroCosto = cc.ToUpper().Replace(".","").Replace(",","");//GetCentroCosto(_pdfLines[i]);
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
            {
                var aux = pdfLines[i].Trim().Replace("$","").Replace(",","").DeleteContoniousWhiteSpace();
                var hex = aux.DeleteNullHexadecimalValues();
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(hex);
                switch (optItem)
                {
                    case 0:
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Sku = test0[test0.Length - 3].ToUpper(),
                            Cantidad = test0[0],
                            Precio = test0[test0.Length - 2],
                            TipoPareoProducto = TipoPareoProducto.SinPareo
                        };
                        items.Add(item0);
                        break;
                    case 1:
                        var test1 = aux.Split(' ');
                        var item1 = new Item
                        {
                            Sku = test1[test1.Length - 4].ToUpper(),
                            Cantidad = test1[0],
                            Precio = $"{test1[test1.Length - 3]}{test1[test1.Length - 2]}",
                            TipoPareoProducto = TipoPareoProducto.SinPareo
                        };
                        items.Add(item1);
                        break;
                }
            }
            //SumarIguales(items);
            return items;
        }

        private string GetSku(string[] test1)
        {
            var ret = "W102030";
            var skuDefaultPosition = test1[5].Replace("#", "");
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
        {//OC N°: GO/16/C1793
            var aux = str.Split(' ');
            str = aux[aux.Length - 1];
            return str;
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
            var ret = split[split.Length - 1];
            return ret;
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.Replace(",","").DeleteContoniousWhiteSpace();
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
