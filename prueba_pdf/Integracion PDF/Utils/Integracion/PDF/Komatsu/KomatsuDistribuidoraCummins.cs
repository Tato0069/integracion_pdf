using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Komatsu
{
    public class KomatsuDistribuidoraCummins
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{5}\s[a-zA-Z]{1,2}\d{5,6}\s"}
            //00010 H350120 
        };
        private const string RutPattern = "Distribuidora Cummins Chile";
        private const string OrdenCompraPattern = "Nuestro Pedido";
        private const string ItemsHeaderPattern =
            "Parte Descripción Cant";

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

        public KomatsuDistribuidoraCummins(PDFReader pdfReader)
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
                CentroCosto = "1"
            };
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    //Console.WriteLine($"OC_1: {_pdfLines[i]}");
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        Console.WriteLine($"OC_: {_pdfLines[i]}");
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                    }
                }
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        //Console.WriteLine($"RUT_: {_pdfLines[i]}");
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
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
                var aux = pdfLines[i].DeleteContoniousWhiteSpace();
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(aux);
                //Console.WriteLine($"AUX: {aux}, opT: {optItem}");
                switch (optItem)
                {
                    case 0:
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Sku = test0[1],
                            //Cantidad = test0[4].Split(',')[0],
                            //Precio = test0[test0.Length - 2].Split(',')[0]
                        };
                        for (var j = test0.Length - 1; j >= 0; j--)
                        {
                            var x = test0[j].Replace(".", "");
                            var number = 0;
                            if (!int.TryParse(x, out number)) continue;
                            j -= 2;
                            item0.Precio = test0[j].Replace(".", "");
                            j -= 2;
                            item0.Cantidad = test0[j].Replace(".", "");
                            break;
                        }
                        if (item0.Sku.Equals("Z664424")
                        || item0.Sku.Equals("Z664524"))
                        {
                            item0.Cantidad = $"{int.Parse(item0.Cantidad) * 12}";
                        }
                        else if (item0.Sku.Equals("H350120"))
                        {
                            item0.Cantidad = $"{int.Parse(item0.Cantidad) * 10}";
                        }
                        items.Add(item0);
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
                var str = test1.ArrayToString(0, test1.Length -1);
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
            var split = str.Split(':');
            return split[1].Trim();
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            return str;
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.DeleteDotComa();
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