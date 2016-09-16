using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LecturaMail.Utils.OrdenCompra;
using Limilabs.Mail;
using LecturaMail.Utils;

namespace LecturaMail.Utils.Integracion.EMAIL.Cinemark
{
    class Cinemark
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"\s\d{1,}\s\d{1,}\s\d{1,}$"},
        };
        private const string RutPattern = "RUT:";
        private const string OrdenCompraPattern = "OP Nº:";
        private const string ItemsHeaderPattern =
            "Cantidad";

        private const string ItemsFooterPattern = "_____";

        private const string CentroCostoPattern = "CINE:";
        private const string ObservacionesPattern = "Tienda :";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItems;
        private readonly IMail _email;
        private readonly string[] _emailBodyLines;

        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        #endregion

        public Cinemark(IMail mail)
        {
            _email = mail;
            _emailBodyLines = _email.GetBodyAsList().ToArray();
        }

        #region Funciones Get
        public List<OrdenCompra.OrdenCompra> GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra
            {
                CentroCosto = "0",
                TipoPareoCentroCosto = TipoPareoCentroCosto.PareoDescripcionMatch
            };

            var listOrdenesCompra = new List<OrdenCompra.OrdenCompra>();
            var lastOc = "";
            var rut = "96659800";
            var obs = "";
            var ordenC = "";
            var cc = "";
            var items = new List<Item>();
            for (var i = 0; i < _emailBodyLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_emailBodyLines[i]))
                    {
                        if (!lastOc.Equals(_emailBodyLines[i]))
                        {
                            lastOc = _emailBodyLines[i];
                            ordenC = GetOrdenCompra(_emailBodyLines[i]);
                        }
                    }
                }

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_emailBodyLines[i]))
                    {
                        cc = GetCentroCosto(_emailBodyLines[i]);
                    }
                }
                if (!_readItems)
                {
                    if (IsItemHeaderPattern(_emailBodyLines[i]))
                    {
                        var itemsAux = GetItems(_emailBodyLines, i+2);
                        if (itemsAux.Count > 0)
                        {
                            items.AddRange(itemsAux);
                            Console.WriteLine($"OC: {ordenC}, Items.Count: {items.Count}, ");
                            listOrdenesCompra.Add(new OrdenCompra.OrdenCompra
                            {
                                Rut = rut,
                                NumeroCompra = ordenC,
                                CentroCosto = cc,
                                Observaciones = obs,
                                Items = items,
                                TipoPareoCentroCosto = TipoPareoCentroCosto.PareoDescripcionExacta
                            });
                            items = new List<Item>();
                            //_readRut = false;
                        }
                        //else _readRut = false;
                        //_readItems = true;
                    }
                }
            }
            if (OrdenCompra.NumeroCompra.Equals(""))
            {
                //OrdenCompra.NumeroCompra = _pdfReader.PdfFileNameOC;
            }
            return listOrdenesCompra;
        }


        private List<Item> GetItems(string[] emailLines, int i)
        {
            var items = new List<Item>();
            for (; i < emailLines.Length; i++)
            {
                var aux1 = emailLines[i].Trim().DeleteContoniousWhiteSpace();
                var aux2 = emailLines[i+1].Trim().DeleteContoniousWhiteSpace();
                var aux3 = emailLines[i+2].Trim().DeleteContoniousWhiteSpace();
                var aux4 = emailLines[i+3].Trim().DeleteContoniousWhiteSpace();
                var pattern = $"{aux1} {aux2} {aux3} {aux4}".DeleteContoniousWhiteSpace();
                if (IsItemsFooterPattern(aux1)) break;
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(pattern.Replace(".",""));
                switch (optItem)
                {
                    case 0:
                        var test0 = pattern.Split(' ');
                        var item0 = new Item
                        {
                            Sku = "W102030",
                            Cantidad = test0[test0.Length - 2].Replace(".", ""),
                            Precio = test0[test0.Length - 3].Replace(".",""),
                            Descripcion = test0.ArrayToString(0, test0.Length-4),
                            TipoPareoProducto = TipoPareoProducto.PareoSkuClienteDescripcionTelemarketing
                        };
                        items.Add(item0);
                        break;
                }
            }
            //SumarIguales(items);
            return items;
        }

        private bool IsItemsFooterPattern(string str)
        {
            return str.DeleteContoniousWhiteSpace().Trim().Contains(ItemsFooterPattern);
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
            var aux = str.Split(':');
            return aux[1].Trim().Split(';')[0].Trim();
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
            var split = str.Split(':');
            return split[1];
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            //str = str.DeleteDotComa();
            str = str.Replace(".", "");
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

        private bool IsItemHeaderPattern(string str)
        {
            //Console.WriteLine(str);
            return str.Trim().Contains(ItemsHeaderPattern);
        }
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