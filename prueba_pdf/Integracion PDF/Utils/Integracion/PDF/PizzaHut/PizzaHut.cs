using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.PizzaHut
{
    public class PizzaHut
    {
        #region Variables
        private const string ItemPattern = @"^\d{10}\s\d{1,}\s";
        private const string RutPattern = "R.U.T.";
        private const string OrdenCompraPattern = "ORDEN DE COMPRA Nº";
        private const string ItemsHeaderPattern =
            "Producto Cantidad Descripción Precio Unitario Total";

        private const string CentroCostoPattern = "Solicitado por :";
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

        public PizzaHut(PDFReader pdfReader)
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
        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra
            {
                CentroCosto = "0"
            };
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
                        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                        Console.WriteLine($"CC: {OrdenCompra.CentroCosto}");
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


        private static List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
            var c = 1;
            for (; i < pdfLines.Length; i++)
            //foreach(var str in pdfLines)
            {
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                //Es una linea de Items 
                if (IsItem(aux))
                {
                    var test = aux.Split(' ');
                    var item = new Item
                    {
                        Sku = test[0],
                        Cantidad = test[1],
                        Precio = test[test.Length - 2]
                            .Substring(0
                                , test[test.Length - 2]
                                    .LastIndexOf("."
                                        , StringComparison.Ordinal))
                            .Replace(",", "")
                    };
                    Console.WriteLine(c+++".- ITEM====>"+item);
                    items.Add(item);
                }
            }
            //SumarIguales(items);
            return items;
        }



        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato (X123)
        /// </summary>
        /// <param name="str">Linea de texto</param>
        /// <returns></returns>
        private static string GetCentroCosto(string str)
        {
            var aux = str.Split(':')[1].Trim();
            Console.WriteLine("===>"+aux);
            var lastIndexOf = aux.LastIndexOf("Total", StringComparison.Ordinal);
            Console.WriteLine(lastIndexOf);
            var ret = lastIndexOf == -1 ?
                aux 
                : aux.Substring(0, aux.LastIndexOf("Total", StringComparison.Ordinal));
            if (!ret.Contains("LOCAL") 
                && !ret.Contains("STOCK CAP") 
                && !ret.Contains("-")
                && !ret.Contains("DEPARTAMENTO")
                && !ret.Contains("DPTO")
                && !ret.Contains("DEPTO"))
                return "0";
            ret = ret.Replace("LOCAL ", "")
                .Replace("LOCALPHD ","PHD ")
                .Replace("IRARRAZAVAL", "IRARRAZABAL")
                .Replace("CUIDAD", "CIUDAD")
                .Replace("HUERFANOS", "HUERFANO")
                .Replace(@"///", "")
                .Replace("SOLICITADO POR","")
                .Replace("AV.OSSA", "AV. OSSA")
                .Replace("DPTO CALL CENTER", "CALL CENTER")
                .Replace("DPTO GERENCIA Y RECEPCION", "GERENCIA Y RECEPCION")
                .Replace("DPTO SELECCION", "SELECCION")
                .Trim();
            if (ret.Equals("OSSA")) ret = "AV. OSSA";
            else if (ret.Contains("DEPARTAMENTO DE MANTENCION Y DE ")) ret = "DEPARTAMENTO DE MANTENCION Y DESARROLLO";
            else if (ret.Contains("TICKET RESTAURA")) ret = "TICKET RESTAURANT";
            else if (ret.Contains("CONTABILIDA")) ret = "CONTABILIDAD";
            else if (ret.Contains("ADMINISTRACION ")) ret = "ADMINISTRACION Y DESARROLLO";
            else if (ret.Contains("MARKETING")) ret = "MARKETING";
            if (ret.Contains("-"))
                ret = ret.Split('-')[1].Trim();
            if (ret.Equals("CAP")) ret = "STOCK CAP";
            return ret;
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            var aux = str.Trim().Split(' ');
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
            var aux = str.Split(' ')[1];
            return aux;
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