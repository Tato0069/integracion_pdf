using LecturaMail.Utils.OrdenCompra;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LecturaMail.Utils.Integracion.EMAIL
{
    class ConsaludArtikos
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"[a-zA-Z]{1,2}\d{5,6}"},
            //{1, @"^\d{1,}\s\w{3}\d{5,6}\s\d{1,}\s" }
        };
        private const string RutPattern = "RUT";
        private const string OrdenCompraPattern = "Número";
        private const string ItemsHeaderPattern =
            "*Descripción del Producto*"; //Lista de Productos

        private const string CentroCostoPattern = "Centro Costo:";
        private const string ObservacionesPattern = "Tienda :";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly IMail _email;
        private readonly string[] _emailBodyLines;

        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        #endregion

        public ConsaludArtikos(IMail mail)
        {
            _email = mail;
            _emailBodyLines = _email.GetBodyAsList().ToArray();
        }

        #region Funciones Get
        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra
            {
                CentroCosto = "0",
                TipoPareoCentroCosto = TipoPareoCentroCosto.PareoDescripcionExacta
            };
            for (var i = 0; i < _emailBodyLines.Length; i++)
            {

                Console.WriteLine("str: " + _emailBodyLines[i]);
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_emailBodyLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_emailBodyLines[i+1]);
                        _readOrdenCompra = true;
                    }
                }
                if (!_readRut)
                {
                    if (IsRutPattern(_emailBodyLines[i]))
                    {
                        OrdenCompra.Rut = "96856780";//GetRut(_emailBodyLines[i+1]);
                        _readRut = true;
                    }
                }

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_emailBodyLines[i]))
                    {
                        OrdenCompra.CentroCosto = GetCentroCosto(_emailBodyLines[i+1]);
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
                    if (IsHeaderItemPatterns(_emailBodyLines[i]))
                    {
                        var items = GetItems(_emailBodyLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                            _readItem = true;
                        }
                    }
                }
            }
            if (OrdenCompra.NumeroCompra.Equals(""))
            {
                //OrdenCompra.NumeroCompra = _pdfReader.PdfFileNameOC;
            }
            return OrdenCompra;
        }


        private List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
            for (; i < pdfLines.Length -2; i++)
            //foreach(var str in pdfLines)
            {
               
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                var aux1 = pdfLines[i -2].Trim().DeleteContoniousWhiteSpace();
                var aux2 = pdfLines[i + 2].Trim().DeleteContoniousWhiteSpace();
                Console.WriteLine($"AUX: {aux}");
                //Console.WriteLine($"AUX1: {aux1}");
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(aux);
                switch (optItem)
                {
                    case 0:
                        Console.WriteLine("==================ITEM CASE 0=====================");
                        var test0 = aux.Split(' ');
                        //var test1 = aux1.Trim(); 
                        var item0 = new Item
                        {
                            Sku = test0[0],
                            Cantidad = aux1.Trim(),
                            Precio = aux2.Trim().Replace("$",""),
                            TipoPareoProducto = TipoPareoProducto.SinPareo
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
            var aux = str.Split('-');
            return aux[0].Trim();
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
            return split[1];
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            //str = str.DeleteDotComa();
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

        private string GetCantidad(string[] test1)
        {

            var ret = "-100";
            for (var i = 0; i < test1.Length - 3; i++)
            {
                Console.WriteLine("test1: " + test1[0]);
                    return ret = test1[0].Trim();
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
