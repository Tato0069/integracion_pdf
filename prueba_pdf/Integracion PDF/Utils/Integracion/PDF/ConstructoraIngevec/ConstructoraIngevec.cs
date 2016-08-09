using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ConstructoraIngevec
{
    public class ConstructoraIngevec
    {
        #region Variables

        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"[a-zA-Z]{4}\d{4}"}//\s|\w{4}\d{4}\w" },//MBMA0905Lapiz
            //{1, @"\w{4}\d{4}\w{1,}" }//MBMA0905Lapiz
        };

        private readonly string _skuPattern = @"[a-zA-Z]{1,2}\d{5,6}";
        private const string RutPattern = "Rut: ";
        private const string OrdenCompraPattern = "Orden de Compra Nº";
        private const string ItemsHeaderPattern =
            "Items Producto Unidad Cantidad Precio Unitario Valor Neto";

        private const string CentroCostoPattern = "Obra Nº ";
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

        public ConstructoraIngevec(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
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

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            //str = str.DeleteDotComa();
            foreach (var it in _itemsPatterns.Where(it => Regex.Match(str, it.Value).Success))
            {
                //Console.WriteLine($"{str} ==>Match:{it.Value} ; R: {Regex.Match(str, it.Value).Success}; INDEX: {Regex.Match(str, it.Value).Index}\n{str.Substring(Regex.Match(str, it.Value).Index)}\n\n");
                ret = it.Key;
            }
            return ret;
        }

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
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                    }
                }

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        OrdenCompra.CentroCosto = int.Parse(GetCentroCosto(_pdfLines[i])).ToString();
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
            //var count = 0;
            for (; i < pdfLines.Length; i++)
            //foreach(var str in pdfLines)
            {
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(aux);
                //if (count == 12) break;
                switch (optItem)
                {
                    case 0:
                        //count++;
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Sku = "",
                            Cantidad = test0[test0.Length - 3].Split(',')[0].Replace(".", ""),
                            Precio = test0[test0.Length - 2].Split(',')[0].Replace(".", "")
                        };
                        var c = i;
                        for (; i+1 < pdfLines.Length && item0.Sku.Equals("");)
                        {
                            var str = pdfLines[++i].DeleteContoniousWhiteSpace();
                            if (IsSkuPatterns(str))
                            {
                                item0.Sku = GetSku(str);
                            }
                            else
                            {
                                var op = GetFormatItemsPattern(str);
                                if (op == 0) item0.Sku = "W102030";

                            }
                        }
                        i = c+1;
                        items.Add(item0);
                        break;
                }
            }
            //SumarIguales(items);
            return items;
        }

        private string GetSku(string str)
        {
            var sku1 = @"[a-zA-Z]{1,2}\d{5,6}";
            var sku2 = @"[a-zA-Z]{1,2}\s\d{5,6}";
            int index = 0, length = 0;
            if (Regex.Match(str, sku1).Success)
            {
                index = Regex.Match(str, sku1).Index;
                length = Regex.Match(str, sku1).Length;
            }else if (Regex.Match(str, sku2).Success)
            {
                index = Regex.Match(str, sku2).Index;
                length = Regex.Match(str, sku2).Length;
            }
            return str.Substring(index, length).Trim().Replace(" ","");
        }


        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato (X123)
        /// </summary>
        /// <param name="str">Linea de texto</param>
        /// <returns></returns>
        private static string GetCentroCosto(string str)
        {
            var index = Regex.Match(str, @"\d{4}").Index;
            return str.Substring(index,4);
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            var index = Regex.Match(str, @"\d{6}").Index;
            return str.Substring(index, 6);
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Split(' ')[1];
            return aux;
        }

        #endregion


        #region Funciones Is

        private bool IsHeaderItemPatterns(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(ItemsHeaderPattern);
        }

        private bool IsSkuPatterns(string str)
        {
            return Regex.Match(str, _skuPattern).Success;
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