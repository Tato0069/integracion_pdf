using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.MTS
{
    public class Mts
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s[a-zA-Z]{1,2}\d{5,6}\s"}
        };
        private const string RutPattern = "R.U.T. ";
        private const string OrdenCompraPattern = "MATERIALES Y SOLUCIONES S.A.";
        private const string ItemsHeaderPattern =
            "N CODIGO DESCRIPCION UN CANT PRECIO %";

        private const string CentroCostoPattern = "Socio MTS";
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

        public Mts(PDFReader pdfReader)
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
            OrdenCompra = new OrdenCompra.OrdenCompra();
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
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
                        var test0 = aux.Split(' ');
                        var auxCant = test0[test0.Length - 6].Split('.')[0];
                        var haveExtraColumn = false;
                        haveExtraColumn = (auxCant.Equals("UN")
                                           || auxCant.Equals("CU"));
                            //auxCant = test0[test0.Length - 5].Split('.')[0];
                        var item0 = new Item
                        {
                            Sku = test0[1],
                            Cantidad =
                                haveExtraColumn
                                    ? test0[test0.Length - 5].Split('.')[0]
                                    : test0[test0.Length - 6].Split('.')[0],
                            Precio =
                                haveExtraColumn
                                    ? test0[test0.Length - 4].Split('.')[0]
                                    : test0[test0.Length - 5].Split('.')[0]
                        };
                        items.Add(item0);
                        break;
                }
            }
            //SumarIguales(items);
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
            var index = str.IndexOf("Socio MTS") + 9;
            var x = str
                .Substring(
                    index
                    , str.IndexOf("Rut", StringComparison.Ordinal) - index);
            var ret = x.Trim().ToUpper()
                .DeleteNullHexadecimalValues()
                .Replace(".", "")
                .Replace(" SA", "")
                .Replace(" HNOS", "")
                .Replace("MANUELNDOVAL", "MANUEL SANDOVAL");
            return ret.Contains("GIDI")
                ? "GIDI"
                : ret;
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
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
            var aux = str.Split(':')[1];
            var hex = aux.ConvertStringToHex();
            Console.WriteLine(hex);
            Console.WriteLine(hex.Replace("c2","").Replace("a0","").ConvertHexToString());
            //return aux.Replace(" ", "").Contains("-")? aux.Split('-')[0].Trim():aux.Trim();
            return "96692790";
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