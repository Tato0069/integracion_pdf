using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.TNT
{
    public class TNT
    {
        #region Variables
        private const string ItemPattern = @"^\d{5}\s\w{1}\d{6}\s|^\d{5}\s\w{2}\d{6}\s|^\d{5}\s\w{1}\d{6}#|^\d{5}\s\w{2}\d{6}#";
        private readonly Dictionary<int, string> ItemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{5}\s\w{1}\d{6}\s|^\d{5}\s\w{2}\d{6}\s|^\d{5}\s\w{1}\d{6}#|^\d{5}\s\w{2}\d{6}#"},
            {1, @"^\d{5}\s\w{1}\d{6}\w|^\d{5}\s\w{2}\d{6}\w" },
            {2,@"^\d{5}\s[a-zA-Z]{1,2}\d{5,6}-" },
            {3,@"^\d{5}\s[a-zA-Z]{1,2}\d{5,6}\)" },
            {4,@"^\d{5}\s[a-zA-Z]{1,2}\d{5,6}[a-zA-Z]{1,}" }
        };
        private const string RutPattern = "RUT:_";
        private const string OrdenCompraPattern = "Orden de Compra N°:";
        private const string ItemsHeaderPattern =
            "Cantidad Unidades Moneda Precio por unidad Total";

        private const string CentroCostoPattern = "Lugar de entrega:";
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

        public TNT(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.DeleteDotComa();
            foreach (var it in ItemsPatterns.Where(it => Regex.Match(str, it.Value).Success))
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
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[++i]);
                        i += 3;
                        var dir = _pdfLines[i];
                        i += 3;
                        var contacto = _pdfLines[++i];
                        OrdenCompra.Observaciones += $"{contacto}, {OrdenCompra.CentroCosto}, {dir}";
                        OrdenCompra.CentroCosto += contacto.ToUpper().Replace("NOMBRE DEL SOLICITANTE", "");
                        _readCentroCosto = true;
                        OrdenCompra.CentroCosto = OrdenCompra.CentroCosto.Replace("SANTIAGO", "");
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
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
                            Sku = test0[1].ToUpper()
                        };
                        aux = pdfLines[i+1].Trim().DeleteContoniousWhiteSpace();
                        var test02 = aux.Split(' ');
                        item0.Cantidad = test02[0];
                        item0.Precio = test02[test02.Length - 3].Replace(".", "");
                        //if (!item0.Sku.Contains("#") 
                        //    && (item0.Sku.Contains("ZZ") 
                        //    || item0.Sku.Contains("LL")))
                        //{
                        //    item0.Sku = item0.Sku.Substring(1);
                        //}
                        item0.Sku = NormaliceSku(item0.Sku);//.Split('#')[0];
                        items.Add(item0);
                        break;
                    case 1:
                        var test1 = aux.Split(' ');
                        var item1 = new Item
                        {
                            Sku = GetSku(test1[1]).ToUpper()
                        };
                        aux = pdfLines[i+1].Trim().DeleteContoniousWhiteSpace();
                        var test12 = aux.Split(' ');
                        item1.Cantidad = test12[0];
                        item1.Precio = test12[test12.Length - 3].Replace(".", "");
                        //if (!item1.Sku.Contains("#")
                        //    && (item1.Sku.Contains("ZZ")
                        //    || item1.Sku.Contains("LL")))
                        //{
                        //    item1.Sku = item1.Sku.Substring(1);
                        //}
                        item1.Sku = NormaliceSku(item1.Sku);//.Split('#')[0];
                        items.Add(item1);
                        break;
                    case 2:
                        var case2 = aux.Split(' ');
                        var itemCase2 = new Item
                        {
                            Sku = GetSku(case2[1]).ToUpper()
                        };
                        aux = pdfLines[++i].Trim().DeleteContoniousWhiteSpace();
                        var case21 = aux.Split(' ');
                        itemCase2.Cantidad = case21[0];
                        itemCase2.Precio = case21[case21.Length - 3].Replace(".", "");
                        //if (!item1.Sku.Contains("#")
                        //    && (item1.Sku.Contains("ZZ")
                        //    || item1.Sku.Contains("LL")))
                        //{
                        //    item1.Sku = item1.Sku.Substring(1);
                        //}
                        itemCase2.Sku = NormaliceSku(itemCase2.Sku);//.Split('#')[0];
                        items.Add(itemCase2);
                        break;
                    case 3:
                        var case3 = aux.Split(' ');
                        var itemCase3 = new Item
                        {
                            Sku = GetSku(case3[1].Replace(")", " ")).ToUpper()
                        };
                        aux = pdfLines[i+1].Trim().DeleteContoniousWhiteSpace();
                        var case31 = aux.Split(' ');
                        itemCase3.Cantidad = case31[0];
                        itemCase3.Precio = case31[case31.Length - 3].Replace(".", "");
                        itemCase3.Sku = NormaliceSku(itemCase3.Sku);//.Split('#')[0];
                        items.Add(itemCase3);
                        break;
                    case 4:
                        Console.WriteLine($"CASE 4:{aux}");
                        var case4 = aux.Split(' ');
                        var itemCase4 = new Item
                        {
                            Sku = GetSku(case4[1].Replace(")", " ")).ToUpper()
                        };
                        aux = pdfLines[i+1].Trim().DeleteContoniousWhiteSpace();
                        var case41 = aux.Split(' ');
                        itemCase4.Cantidad = case41[0];
                        itemCase4.Precio = case41[case41.Length - 3].Replace(".", "");
                        itemCase4.Sku = NormaliceSku(itemCase4.Sku);//.Split('#')[0];
                        items.Add(itemCase4);
                        break;
                }

            }
            //SumarIguales(items);
            return items;
        }

        private string NormaliceSku(string str)
        {
            int x;
            str = str.Split('#')[0];
            if (str.Contains("ZZ")
                || str.Contains("LL"))
            {
                str = str.Substring(1);
            }
            if (int.TryParse(str, out x))
            {
                var charArray = str.ToCharArray();
                if (charArray[0] == '5')
                {
                    str = $"S{str.Substring(1)}";
                }
            }
            return str;
        }

        private string GetSku(string str)
        {
            var it1 = @"[a-zA-Z]{1}\d{6}";
            var it2 = @"[a-zA-Z]{2}\d{5}";
            var index = 0;
            var length = 7;
            if (Regex.Match(str, it1).Success)
            {
                index = Regex.Match(str, it1).Index;
                length = Regex.Match(str, it1).Length;
            }else if (Regex.Match(str, it2).Success)
            {
                index = Regex.Match(str, it2).Index;
                length = Regex.Match(str, it2).Length;
            }
            return str.Substring(index, length).Trim();
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
            return str.Split(':')[1].Trim();
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            return str.Split('_')[1];
        }

        #endregion


        #region Funciones Is
        private static bool IsItem(string str)
        {
            return Regex.Match(str, ItemPattern).Success;
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