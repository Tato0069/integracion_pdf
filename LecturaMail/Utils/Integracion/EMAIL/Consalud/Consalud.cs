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
    class Consalud
    {
        #region Variables
        private const string ItemPattern = @"^\d{1,}\s\d{1,}\s\w{1,}\s\w{1}\d{6}\s";
        /*
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s\w{3}\d{5,6}\s\d{3,}\s\d{1,}\s\d{1,}"},
            {1, @"^\d{1,}\s\w{3}\d{5,6}\s\d{1,}\s" }
        };
        */
        private const string RutPattern = "Razón Social";
        private const string OrdenCompraPattern = "Número";
        private const string ItemsHeaderPattern =
            "Nro Cantidad Unidad Código";
        /*
        private readonly List<string> _itemsHeaderPatterns = new List<string>
        {
            {"Nro Cantidad Unidad Código Descripción del Producto Total"},
            {"Nro Cantidad Unidad Código Total "}
        };
        */
        private const string ItemsFooterPattern = "Total Neto";
        private const string CentroCostoPattern = "Centro Costo:";
        private bool _readOrdenCompra;
        private bool _readRut = true;
        private bool _readItems = true;
        private bool _readCentroCosto;
        private readonly IMail _email;
        private readonly string[] _emailBodyLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public Consalud(IMail mail)
        {
            _email = mail;
            _emailBodyLines = _email.GetBodyAsList().ToArray();
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
                var aux = pdfLines[i].DeleteContoniousWhiteSpace().Trim();
                if (IsItemsFooterPattern(aux)) break;
                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(aux))
                    {
                        var st = pdfLines[++i];
                        Console.WriteLine(st);
                        OrdenCompra.CentroCosto = GetCentroCosto(st);
                        _readCentroCosto = true;
                    }
                }
                if (IsItem(aux))
                {
                    var test = aux.Split(' ');
                    var item = new Item
                    {
                        Sku = test[3],
                        Cantidad = test[1],
                        Precio = test[test.Length - 1].Equals("$") ?
                        test[test.Length - 2].Replace(".", "") :
                        test[test.Length - 3].Replace(".", "")
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
            str = str.DeleteContoniousWhiteSpace();

            var index = Regex.Match(str, @"\s\d{4,5}-\w{1,}").Index;
            //var length = Regex.Match(str, @"\s\d{4,5}-\w{1,}-").Length;
            //var index = Regex.Match(str, @"\s\d{4,5}-").Index;
            //var length = Regex.Match(str, @"\s\d{4,5}-").Length;
            //return str.Substring(index, length - 1).Trim();
            return str.Substring(index).Split('-')[0].Replace(" ", "");
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            str = str.DeleteContoniousWhiteSpace();
            var aux = str.Trim().Split(' ');
            return str;//return aux[1];
            //return str.Contains("RUT") ? aux[1] : aux[aux.Length - 1];
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Trim().Split(' ');
            return aux[aux.Length - 1];
         
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra
            {
                CentroCosto = "0"
            };
            for (var i = 0; i < _emailBodyLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_emailBodyLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_emailBodyLines[i+1]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }

                }
                if (!_readRut)
                {
                    if (IsRutPattern(_emailBodyLines[i]))
                    {
                        _readRut = true;
                        OrdenCompra.Rut = GetRut(_emailBodyLines[i+3]);
                        _readItems = false;
                    }
                }
                if (!_readItems)
                {
                    if (IsItemHeaderPattern(_emailBodyLines[i]))
                    {
                        var items = GetItems(_emailBodyLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                        }
                    }
                }
            }
            return OrdenCompra;
        }


        private bool IsItemHeaderPattern(string str)
        {
            return str.Trim().Contains(ItemsHeaderPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return str.Contains(CentroCostoPattern);
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.Trim().Contains(OrdenCompraPattern);
        }
        private bool IsItemsFooterPattern(string str)
        {
            return str.Trim().Contains(ItemsFooterPattern);
        }



        private bool IsRutPattern(string str)
        {
            return str.Trim().Contains(RutPattern);
        }
    }
}
