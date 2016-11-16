using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Arauco
{
    class Arauco
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s\w{3}\d{5,6}\s\d{3,}\s\d{1,}\s\d{1,}"},
            {1, @"\s\d{18}$"},  //Código\sde\smaterial\sArauco
            {2,@"\s\d{1,}\s\d{1,}\s\d{1,}\s\d{1,}$" }

            //000000000000040951
            //D91 100 01/08/2016 16:00 1.929,00 192.900,00
        };
        private const string RutPattern = "Rut";
        private const string OrdenCompraPattern = "Núm. Pedido";
        private const string ItemsHeaderPattern =
            "Unitario Precio Total";// Plazo Entrega

        private const string CentroCostoPattern = "de entrega:";
        private const string ObservacionesPattern = "Sirvase suministrar a :";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public Arauco(PDFReader pdfReader)
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
                CentroCosto = "0"

            };
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                //Console.WriteLine("    -----" + _pdfLines[i]);
                if (!_readOrdenCompra)
                {

                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        i += 2;
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                    }
                }
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        Console.WriteLine("    RUT" + _pdfLines[i]);
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
                        var obsAux = "";
                        for(var x = i+3; x < i + 10 && !_pdfLines[x].Contains("Cond. Entrega:"); x++)
                        {
                            if (_pdfLines[x].Contains("documentos guía de despacho o factura")) continue;
                            if (_pdfLines[x].Contains("ejecutivo-cedible")) continue;
                            if (_pdfLines[x].Contains("ejecutivo-cedible")) continue;
                            obsAux += $" {_pdfLines[x]}";
                        }
                        OrdenCompra.Observaciones += obsAux;
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
                        var item0 = new Item
                        {
                            Sku = test0[6],
                            Cantidad = test0[4].Split(',')[0],
                            Precio = test0[test0.Length - 2].Split(',')[0]
                        };
                        items.Add(item0);
                        break;
                    case 1:
                        var previous = pdfLines[i-1].Trim().DeleteContoniousWhiteSpace();
                        //var previous1 = pdfLines[i - 4].Trim().DeleteContoniousWhiteSpace();
                        var pTest1 = previous.Split(' ');
                       //  var pTest2 = previous1.Split(' ');
                        var test1 = aux.Split(' ');
                        var item1 = new Item
                        {
                            Sku = int.Parse(test1[test1.Length - 1]).ToString(), //pTest2[pTest2.Length -1].Trim()
                            Cantidad = pTest1[pTest1.Length - 5],
                            Precio = pTest1[pTest1.Length - 2].Replace(".", "").Split(',')[0]
                        };
                        items.Add(item1);
                        break;
                    case 2:
                        var next = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace();
                        var nextSplit = next.Split(' ');
                        var test2 = aux.Split(' ');
                        var item2 = new Item
                        {
                            Sku = int.Parse(nextSplit[nextSplit.Length - 1]).ToString(),
                            Cantidad = test2[test2.Length - 4],
                            Precio = test2[test2.Length - 2].Replace(".", "").Split(',')[0]
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
            var ret = "W102030";
            var skuDefaultPosition = test1[5].Replace("#", "");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length-1);
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
            return str.Trim().Split(' ')[0];//str;
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            return str.Split(' ')[1];
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
            return str.ToLower().Trim().DeleteContoniousWhiteSpace().Contains(ItemsHeaderPattern.ToLower());
        }

        private bool IsObservacionPattern(string str)
        {
            return str.ToLower().Trim().DeleteContoniousWhiteSpace().Contains(ObservacionesPattern.ToLower());
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.ToLower().Trim().DeleteContoniousWhiteSpace().Contains(OrdenCompraPattern.ToLower());
        }
        private bool IsRutPattern(string str)
        {
            return str.Replace(".","").ToLower().Trim().DeleteContoniousWhiteSpace().Contains(RutPattern.Replace(".", "").ToLower());
        }

        private bool IsCentroCostoPattern(string str)
        {
            return str.ToLower().Trim().DeleteContoniousWhiteSpace().Contains(CentroCostoPattern.ToLower());
        }

        #endregion

    }
}