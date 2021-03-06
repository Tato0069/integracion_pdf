﻿using System;
using System.Linq;
using System.Collections.Generic;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;
using System.Text.RegularExpressions;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ACH
{
    public class Esach
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^\d{1,}\s\d{1,}\sC\/U\s\d{6}\s"},
        };
        private const string RutPattern = "R.U.T.:";
        private const string OrdenCompraPattern = "N° O.C.:";
        private const string ItemsHeaderPattern =
            "Pedida Antiguo Unitario Desc Línea";

        private const string CentroCostoPattern = "Recepción:";
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

        public Esach(PDFReader pdfReader)
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
                CentroCosto = "0",
                TipoPareoCentroCosto = TipoPareoCentroCosto.PareoDescripcionMatch
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
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                    }
                }

                if (!_readCentroCosto)
                {
                    if (IsCentroCostoPattern(_pdfLines[i]))
                    {
                        //var ccaux_ = _pdfReader.ExtractTextByCoOrdinate(1, 930, 550, 1170, 580);
                        //var ccaux2_ = _pdfReader.ExtractTextByCoOrdinate(1, 68, 34, 80, 50);
                        //Console.WriteLine($"CCAUX:_ {ccaux_}, AUX2: {ccaux2_}");
                        var _cc = $"{GetCentroCosto(_pdfLines[i]).ToUpper()} {_pdfLines[i + 1]}".ToUpper().Replace("."," ");
                        if (_cc.Contains("SOLICITANTE"))
                        {
                            var _ccSplit = _cc.Split(':');
                            _cc = _ccSplit[_ccSplit.Length - 1];
                        }
                        var _ccAux = GetCentroCosto(_pdfLines[i]).ToUpper().Replace("ENTREGAR","")
                            
                            .Replace("AT", "")
                            .Replace(".", " ")
                            .Replace("LU A VI","")
                            .Replace("/"," ")
                            .Replace(",", " ")
                            .Replace(" AV ","")
                            .Replace("ESACHS","")
                            .Replace("BOD ","")
                            //.Replace(" SJOAQUIN","")
                            .DeleteAcent()
                            .DeleteContoniousWhiteSpace();
                        Console.WriteLine(_ccAux);
                        if (_ccAux.Contains("-"))
                        {
                            var split = _ccAux.Split('-');
                            var _ccAux2 = split.ArrayToString(1, split.Length - 1);
                            _ccAux = _ccAux2.Replace("-"," ").DeleteContoniousWhiteSpace();
                        }
                        OrdenCompra.CentroCosto = _ccAux.Equals("")?"GRECIA":_ccAux;
                        OrdenCompra.Observaciones += $" Dirección: {OrdenCompra.CentroCosto}";
                        Console.WriteLine($"================================\nCC: {_ccAux}");
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
                            Sku = test0[3],
                            Cantidad = test0[1].Split(',')[0],
                            Precio = test0[test0.Length - 4].Split(',')[0],
                            TipoPareoProducto = TipoPareoProducto.PareoCodigoCliente
                        };
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
            var skuDefaultPosition = test1[5].Replace("#", "");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length -1 );
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
            var split = str.Split(':');
            var split2 = split[split.Length - 1].Trim().Split('-');
            var ret = split2.ArrayToString(1, split2.Length-1);
            return split[split.Length -1].Trim();
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