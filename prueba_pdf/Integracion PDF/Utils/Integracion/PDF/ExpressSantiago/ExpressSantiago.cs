using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ExpressSantiago
{
    public class ExpressSantiago
    {
        #region Variables
        private const string ItemPattern = @"^\d{1,}\s\w{3}\d{5,6}\s\d{3,}\s\d{1,}\s\d{1,}";
        private const string RutPattern = "EXPRESS DE SANTIAGO UNO S.A.";
        private const string OrdenCompraPattern = "Orden de Compra N° OS / ";
        private const string ItemsHeaderPattern =
            "Delivery";
        private const string ItemsFooterPattern = "Total Order";
        private bool _readOrdenCompra = true;
        private bool _readRut;
        private bool _readObs = true;
        private bool _readItems = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public ExpressSantiago(PDFReader pdfReader)
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
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                if (IsItemsFooterPattern(aux)) break;
                //Es una linea de Items 
                if (IsItem(aux))
                {
                    var test = aux.Split(' ');
                    var item = new Item
                    {
                        Sku = test[6],
                        Cantidad = test[4].Split(',')[0],
                        Precio = test[test.Length - 2].Split(',')[0]
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
            str.DeleteContoniousWhiteSpace();
            var aux = str.Substring(6).Trim();
            return aux.Split(' ')[0];
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            var aux = str.Split(' ');
            return aux[aux.Length - 1].Trim();
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

        private List<OrdenCompra.OrdenCompra> GetHeaderOrdenCompras()
        {
            var lastOc = "";
            var listOrdenesCompra = new List<OrdenCompra.OrdenCompra>();
            var rut = "";
            var obs = "";
            var ordenC = "";
            var cc = "";
            var items = new List<Item>();
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        _readRut = true;
                        rut = GetRut(_pdfLines[++i]);
                        _readOrdenCompra = false;
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        if (!lastOc.Equals(_pdfLines[i]))
                        {
                            lastOc = _pdfLines[i];
                            ordenC = GetOrdenCompra(_pdfLines[i]);
                            _readObs = false;
                        }
                        else
                        {
                            _readRut = false;
                        }
                        _readOrdenCompra = true;
                    }
                }
                if (!_readObs)
                {
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        cc = GetCentroCosto(_pdfLines[i]);
                        _readObs = true;
                        obs = _pdfLines[i];
                        _readItems = false;
                    }
                }
                if (!_readItems)
                {
                    if (IsItemHeaderPattern(_pdfLines[i]))
                    {
                        var itemsAux = GetItems(_pdfLines, i);
                        if (itemsAux.Count > 0)
                        {
                            items.AddRange(itemsAux);
                            Console.WriteLine($"OC: {ordenC}, Items.Count: {items.Count}");
                            listOrdenesCompra.Add(new OrdenCompra.OrdenCompra
                            {
                                Rut = rut,
                                NumeroCompra = ordenC,
                                CentroCosto = int.Parse(cc).ToString(),
                                Observaciones = obs,
                                Items = items
                            });
                            items = new List<Item>();
                            _readRut = false;
                        }
                        else _readRut = false;
                        _readItems = true;
                    }
                }
            }
            foreach (var o in listOrdenesCompra)
            {
                Console.WriteLine(o.ToString());
            }
            return listOrdenesCompra;
        }


        public List<OrdenCompra.OrdenCompra> GetOrdenCompra2()
        {
            var x = GetHeaderOrdenCompras();
            //var ret = new List<OrdenCompra.OrdenCompra>();
            //foreach (var ord in x)
            //{
            //    if (ord.NumeroCompra.Equals("16000997")
            //        || ord.NumeroCompra.Equals("16001003"))
            //        ret.Add(ord);
            //}
            //return ret;
            return x;
        }




        private bool IsItemHeaderPattern(string str)
        {
            return str.Trim().Contains(ItemsHeaderPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return str.Contains("C.C. :");
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.DeleteContoniousWhiteSpace().Trim().Contains(OrdenCompraPattern);
        }
        private bool IsItemsFooterPattern(string str)
        {
            return str.DeleteContoniousWhiteSpace().Trim().Contains(ItemsFooterPattern);
        }



        private bool IsRutPattern(string str)
        {
            return str.Trim().Equals(RutPattern);
        }
    }
}