using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Alsacia
{
    public class Alsacia
    {
        #region Variables
        private const string ItemPattern = @"^\d{1,}\s\w{3}\d{5,6}\s\d{3,}\s\d{1,}\s\d{1,}";
        private readonly Dictionary<int, string> ItemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s\w{3}\d{5,6}\s\d{3,8}\s\d{1,}\s\d{1,}"},
            //{1, @"^\d{1,}\s\w{3}\d{5,6}\s\d{1,}\s" }
        };
        private const string RutPattern = "INVERSIONES ALSACIA S.A.";
        private const string OrdenCompraPattern = "Orden de Compra N° OS / ";
        private const string ItemsHeaderPattern =
            "Delivery";
        private const string ItemsFooterPattern = "Total Order";
        private bool _readOrdenCompra = true;
        private bool _readRut;
        private bool _readObs = true;
        private bool _addHeader = true;
        private bool _readItems = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public Alsacia(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }
        public Alsacia(string[] pdfReader)
        {
            _pdfLines = pdfReader;
        }

        private bool IsItem(string str)
        {
            str = str.DeleteDotComa();
            return Regex.Match(str, ItemPattern).Success;
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.DeleteDotComa();
            foreach (var it in ItemsPatterns.Where(it => Regex.Match(str, it.Value).Success))
            {
                ret = it.Key;
            }
            return ret;
        }

        private List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
            for (; i < pdfLines.Length; i++) {
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                if (IsItemsFooterPattern(aux)) break;
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
                        var test1 = aux.Split(' ');
                        var item1 = new Item
                        {
                            Sku = test1[4],
                            Cantidad = test1[2].Split(',')[0],
                            Precio = test1[test1.Length - 2].Split(',')[0]
                        };
                        items.Add(item1);
                        break;
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
                            Console.WriteLine($"OC: {ordenC}, Items.Count: {items.Count}, ");
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


        public OrdenCompra.OrdenCompra GetItemsForHeader()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra { CentroCosto = "1" };
            var lastOc = "";
            var listOrdenesCompra = new List<OrdenCompra.OrdenCompra>();
            var rut = "";
            var obs = "";
            var ordenC = "";
            var newOrden = false;
            var items = new List<Item>();
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (IsRutPattern(_pdfLines[i]))
                {
                    //_readRut = true;
                    rut = GetRut(_pdfLines[++i]);
                }
                if (IsOrdenCompraPattern(_pdfLines[i]))
                {
                    //Console.WriteLine(_pdfLines[i]);
                    if (!lastOc.Equals(_pdfLines[i]))
                    {
                        newOrden = true;
                        lastOc = _pdfLines[i];
                        ordenC = GetOrdenCompra(_pdfLines[i]);
                    }
                }
                if (IsCentroCostoPattern(_pdfLines[i]))
                {
                    obs = _pdfLines[i];
                    //_readObs = true;
                }

                if (IsItemHeaderPattern(_pdfLines[i]) && newOrden)
                {
                    var itemsAux = GetItems(_pdfLines, i);
                    if (itemsAux.Count > 0)
                    {
                        items.AddRange(itemsAux);
                    }
                    newOrden = false;
                }

                var x = listOrdenesCompra.Find(oc => oc.NumeroCompra.Equals(ordenC));
                //Console.WriteLine("X ==> " + x);
                if (x == null)
                {
                    listOrdenesCompra.Add(new OrdenCompra.OrdenCompra
                    {
                        Rut = rut,
                        NumeroCompra = ordenC,
                        CentroCosto = obs,
                        Observaciones = obs,
                        Items = items
                    });
                    items = new List<Item>();
                }
            }

            listOrdenesCompra.Add(OrdenCompra);
            foreach (var oc in listOrdenesCompra)
            {
                Console.WriteLine(oc.ToString());
            }
            return OrdenCompra;
        }



        public List<OrdenCompra.OrdenCompra> GetOrdenCompra2()
        {
            var x = GetHeaderOrdenCompras();
            //var ret = new List<OrdenCompra.OrdenCompra>();
            //foreach (var o in x)
            //{
            //    if (o.NumeroCompra.Equals("16000645"))
            //    {
            //        ret.Add(o);
            //    }
            //}
            //return ret;
            return x;
        }




        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra {CentroCosto = "1"};
            var lastOc = "";
            var listOrdenesCompra = new List<OrdenCompra.OrdenCompra>();
            string[] pdfLinePage = {};
            var rut = "";
            var obs = "";
            var ordenC = "";
            var newOrden = false;
            var items = new List<Item>();
            for (var page = 1; page <= _pdfReader.NumerOfPages;
                pdfLinePage = _pdfReader.ExtractTextFromPageOfPdfToArray(page), page++)
            {
                //Console.WriteLine(page);
                for (var i = 0; i < pdfLinePage.Length; i++)
                {
                    if (IsRutPattern(pdfLinePage[i]))
                    {
                        //_readRut = true;
                        rut = GetRut(pdfLinePage[++i]);
                    }
                    if (IsOrdenCompraPattern(pdfLinePage[i]))
                    {
                        //Console.WriteLine(pdfLinePage[i]);
                        if (!lastOc.Equals(pdfLinePage[i]))
                        {
                            newOrden = true;
                            lastOc = pdfLinePage[i];
                            ordenC = GetOrdenCompra(pdfLinePage[i]);
                        }
                    }
                    if (IsCentroCostoPattern(pdfLinePage[i]))
                    {
                        obs = pdfLinePage[i];
                        //_readObs = true;
                    }
                    if (IsItemHeaderPattern(pdfLinePage[i]))
                    {
                        var itemsAux = GetItems(pdfLinePage, i);
                        if (itemsAux.Count > 0)
                        {
                            //items.AddRange(itemsAux);
                        }
                    }
                    if (newOrden)
                    {
                        var o = new OrdenCompra.OrdenCompra
                        {
                            Rut = rut,
                            NumeroCompra = ordenC,
                            CentroCosto = obs,
                            Observaciones = obs,
                            Items = items
                        };
                        newOrden = false;
                        listOrdenesCompra.Add(o);
                    }
                }
            }

            //for (var i = 0; i < _pdfLines.Length; i++)
            //{
            //    //var obs = "";
            //    if (!_readRut)
            //    {
            //        if(IsRutPattern(_pdfLines[i]))
            //        {
            //            _readRut = true;
            //            OrdenCompra.Rut = GetRut(_pdfLines[++i]);
            //        }
            //    }
            //    //if (_readOrdenCompra)
            //    //{
            //    if (IsOrdenCompraPattern(_pdfLines[i]))
            //    {
            //        lastOC = _pdfLines[i];
            //        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
            //        _readOrdenCompra = true;
            //        _readObs = false;
            //    }
            //    //}
            //    //if (!_readObs)
            //    //{
            //    if (IsCentroCostoPattern(_pdfLines[i]))
            //    {
            //        OrdenCompra.Observaciones += _pdfLines[i];
            //        _readObs = true;
            //    }
            //    //}
            //    //if (!readItem)
            //    //{
            //    if (IsItemHeader(_pdfLines[i]))
            //    {
            //        var items = GetItems(_pdfLines, i, lastOC);
            //        if (items.Count > 0)
            //        {
            //            OrdenCompra.Items.AddRange(items);
            //            readItem = true;
            //        }
            //    }
                //}
                listOrdenesCompra.Add(OrdenCompra);
            //}
            //foreach (var oc in listOrdenesCompra)
            //{
            //    Console.WriteLine(oc.ToString());
            //}
            return OrdenCompra;
        }

        private bool IsItemHeaderPattern(string str)
        {
            //Console.WriteLine(str);
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