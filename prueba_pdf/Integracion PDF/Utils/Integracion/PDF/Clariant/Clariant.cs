using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Clariant
{
    public class Clariant
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{5}\s"}
        };
        private const string RutPattern = "R.U.T.:";
        private const string OrdenCompraPattern = "Núm. pedido/Fecha";
        private const string ItemsHeaderPattern =
            "Pos. Material Descripción";

        private const string CentroCostoPattern = "Contacto";
        private const string ObservacionesPattern = "?";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Clariant(PDFReader pdfReader)
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
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        ++i;
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                    }
                    else if (_pdfLines[i].Contains("Alberto Pepper 1784"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                    }
                }

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        ++i;
                        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[++i]);
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
                if (OrdenCompra.Rut.Equals("76283799")
                    || OrdenCompra.Rut.Equals("76513677"))
                    OrdenCompra.CentroCosto = "0";
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
                        //Console.WriteLine(aux);
                        i += pdfLines[i+1].Contains("Lugar de entrega:") ? 2 : 1;
                        aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Cantidad = test0[0].Split(',')[0],
                            Precio = test0[test0.Length - 2].Split(',')[0]
                        };
                        aux = pdfLines[++i].Trim().DeleteContoniousWhiteSpace();
                        test0 = aux.Split(' ');
                        item0.Sku = GetSku(test0);
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
            //Console.WriteLine(test1.ArrayToString(0, test1.Length));
            var skuDefaultPosition = test1[test1.Length - 1].ToUpper();
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length);
                if (Regex.Match(str, @"[a-zA-Z]{1}\d{6}").Success)
                {
                    var index = Regex.Match(str, @"[a-zA-Z]{1}\d{6}").Index;
                    var length = Regex.Match(str, @"[a-zA-Z]{1}\d{6}").Length;
                    ret = str.Substring(index, length).Trim();
                }
                else if (Regex.Match(str, @"[a-zA-Z]{2}\d{5}").Success)
                {
                    var index = Regex.Match(str, @"[a-zA-Z]{2}\d{5}").Index;
                    var length = Regex.Match(str, @"[a-zA-Z]{2}\d{5}").Length;
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
            return str;
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            Console.WriteLine("OC: "+str);
            var aux = str.Split('/')[0].Trim();
            return aux;
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
            return aux[aux.Length - 1].Trim();
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
            var s = str.Trim().DeleteContoniousWhiteSpace();
            return s.Contains(OrdenCompraPattern);
        }
        private bool IsRutPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(RutPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Equals(CentroCostoPattern);
        }

        #endregion

    }
}