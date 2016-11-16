using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.LarrainPrieto
{
    class LarrainPrieto
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^ZB\d{1,}\s\d{1,}$"},
            //{0, @"^\d{1,}\s[a-zA-Z]{1,}\s\d{1,}\sZB\d{1,}\s"},
           {1, @"^[a-zA-Z]{2}\s\d{1,}\sZB" },
           {2, @"^ZB\d{1,}$" },
           {3, @"^\w{3}\sZB\d{1,}\s" }
        };
        private const string RutPattern = "Facturar a : Larraín Prieto Risopatron S.A. Proveedor : DIMERC S.A.";
        private const string OrdenCompraPattern = "ORDEN DE COMPRA N°:";
        private const string ItemsHeaderPattern = "Valor Unitario Descuento Valor";
        private const string ItemsHeaderPattern2 = "Descripción Valor Unitario Valor";

        private const string CentroCostoPattern = "Entregar en :";
        private const string ObservacionesPattern = "";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        #endregion

        public LarrainPrieto(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = _pdfReader.ExtractTextFromPdfToArrayDefaultMode();
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
                if (!_readCentroCosto)
                {
                   if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                       OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                        var getCC = GetCentroCosto(_pdfLines[i]);
                        Console.WriteLine("VARIABLE GETCC ->" + getCC);
                        _readCentroCosto = true;
                    }
                }
                
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i])) {

                        OrdenCompra.Rut =  "80536800"; //GetRut(_pdfLines[i + 1]);


                        _readRut = true;
                    }

                }
                //var getCC = GetCentroCosto(_pdfLines[i]);
                //if (!_readRut)
                //{
                //    if (IsRutPattern(_pdfLines[i]))
                //    {
                //        if (getCC.Contains("Cerrillos")) {

                //            OrdenCompra.Rut = GetRut(_pdfLines[i+1]);
                //        }
                //        OrdenCompra.Rut = "805368002";

                //    }
                //    _readRut = true;
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
                    else if (IsHeaderItemPatterns2(_pdfLines[i]))
                    {
                        var items = GetItems(_pdfLines, i);
                        if(items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                            _readItem = true;

                        }

                    }
                }
            }
            if (OrdenCompra.NumeroCompra.Equals(""))
            {
                OrdenCompra.NumeroCompra = _pdfReader.PdfFileNameOC;
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
                        var test01 = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace().Split(' ');
                        var item0 = new Item
                        {
                            Sku = test0[0],
                            Cantidad = test01[0].Split(',')[0].Replace(".",""),
                            Precio = test01[test01.Length - 1].Split(',')[0].Replace(".", ""),
                            TipoPareoProducto = TipoPareoProducto.PareoCodigoCliente
                        };
                        //Concatenar todo y Buscar por Patrones el SKU DIMERC
                        //var concatAll = "";
                        //aux = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace();
                        //for (var j = i + 2; j < pdfLines.Length && GetFormatItemsPattern(aux) == -1; j++)
                        //{
                        //    concatAll += $" {aux}";
                        //    aux = pdfLines[j].Trim().DeleteContoniousWhiteSpace();
                        //}
                        //item0.Sku = GetSku(concatAll.DeleteContoniousWhiteSpace().Split(' '));
                        items.Add(item0);
                        break;
                    case 1:
                        Console.WriteLine("==================ITEM CASE 1=====================");
                        var test02 = aux.Split(' ');
                        var test022 = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace().Split(' ');
                        var item1 = new Item
                        {
                            Sku = test02[2],
                            Cantidad = test022[0].Split(',')[0].Replace(".", ""),
                            Precio = test02[test02.Length - 2].Split(',')[0],
                            TipoPareoProducto = TipoPareoProducto.PareoCodigoCliente
                        };
                        items.Add(item1);
                        break;

                    case 2:
                        Console.WriteLine("==================ITEM CASE 2=====================");
                        var test03 = aux.Split(' ');
                        var test04 = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace().Split(' ');
                        var test05 = pdfLines[i + 2].Trim().DeleteContoniousWhiteSpace().Split(' ');
                        var item2 = new Item
                        {
                            Sku = test03[0],
                            Cantidad = test05[0].Split(',')[0],
                            Precio = test04[test04.Length - 2].Split(',')[0],
                            TipoPareoProducto = TipoPareoProducto.PareoCodigoCliente
                        };
                        items.Add(item2);
                        break;
                    case 3:
                        Console.WriteLine("==================ITEM CASE 3=====================");
                        var test06 = aux.Split(' ');
                        var test07 = pdfLines[i + 1].Trim().DeleteContoniousWhiteSpace().Split(' ');
                         var item3 = new Item
                        {
                            Sku = test06[1].Trim(),
                            Cantidad = test07[0].Split(',')[0],
                            Precio = test06[test06.Length - 2].Split(',')[0],
                             TipoPareoProducto = TipoPareoProducto.PareoCodigoCliente
                        };
                        items.Add(item3);
                        break;

                }
            }
            //SumarIguales(items);
            return items;
        }

        private string GetSku(string[] test1)
        {
            var ret = "W102030";
            var skuDefaultPosition = test1[0].Replace("#", "");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
            {
                var index = Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Index;
                var length = Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Length;
                ret = skuDefaultPosition.Substring(index, length).Trim();
            }
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
            var aux = str.Trim();
            Console.WriteLine("linea donde va CC"  + aux);
            
            if (aux.Contains("Cerrillos")) {

                aux = "0";
            }
            if (aux.Contains("Octava"))
            {
                aux = "10";

            }
            if (aux.Contains("Gomeros"))
            {
                aux = "11";

            }
            return aux;
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
            var split = str.Split(' ');
            return split[2];
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.Replace(".", "").Replace(",","");
            foreach (var it in _itemsPatterns.Where(it => Regex.Match(str, it.Value).Success))
            {
                ret = it.Key;
            }
            //Console.WriteLine($"STR: {str}, RET: {ret}");
            return ret;
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

        private string GetPrecio(string[] test0)
        {
            var ret = "-1";
            for (var i = 0; i < test0.Length; i++)
            {
                if (test0[i].Equals("CLP"))
                    return ret = test0[i + 1];
            }
            return ret;
        }

        private string GetCantidad(string[] test0)
        {
            var ret = "-1";
            for (var i = 0; i < test0.Length; i++)
            {
                if (test0[i].Equals("CLP"))
                    return ret = test0[i - 1];
            }
            return ret;
        }


        #endregion


        #region Funciones Is
        private bool IsHeaderItemPatterns(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(ItemsHeaderPattern);
        }
        private bool IsHeaderItemPatterns2(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(ItemsHeaderPattern2);
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
