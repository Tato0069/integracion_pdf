using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Carozzi
{
    public class Carozzi
    {

        private const string ItemPattern = @"\d{5}\s\d{7}\s";
        private const string RutPattern = "RUT:";
        private const string OrdenCompraPattern = "Núm. pedido/Fecha";
        private const string ItemsHeaderPattern =
            "Cantd-pedido Unidad Precio por unidad Valor neto ($)";

        private const string CentroCostoPattern = "de entrega:";
        private const string ObservacionesPattern = "Fecha de entrega Día";

        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompraCarozzi OrdenCompra { get; set; }
        public Carozzi(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }
        

        private static List<ItemCarozzi> GetItems(string[] pdfLines, int i)
        {
            var items = new List<ItemCarozzi>();
            for (; i < pdfLines.Length; i++)
            //foreach(var str in pdfLines)
            {
                var aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                //Es una linea de Items 
                if (IsItem(aux))
                {//es una linea que contiene items
                    var test = aux.Split(' ');
                    var item = new ItemCarozzi
                    {
                        Sku = test[1]
                    };
                    //aux = test[test.Length - 1].Equals("/") ? test.ArrayToString(2, test.Length - 2) : test.ArrayToString(2, test.Length - 1);
                    var cantidad = false;
                    var precio = true;
                    var contin = false;
                    for (; i < pdfLines.Length && !contin; 
                        aux = pdfLines[i].Trim().DeleteContoniousWhiteSpace(),i++)
                    {
                        //Console.WriteLine($"aux: {aux}");
                        if (IsItem(aux)) continue;
                        if (!cantidad)
                        {
                            if(aux.Contains("UNIDAD") ||
                                aux.Contains("Kilogramo") ||
                                aux.Contains("Litro") ||
                                aux.Contains("Caja") || 
                                aux.Contains("Bolsa") ||
                                aux.Contains("RESMA") ||
                                aux.Contains("BLOCK") ||
                                aux.Contains("TARRO") ||
                                aux.Contains("UN."))
                            {
                                item.Cantidad = aux.Split(' ')[0].Split(',')[0];
                                cantidad = true;
                                precio = false;
                            }
                        }
                        if (!precio)
                        {
                            if (aux.Contains("Precio bruto"))
                            {
                                item.Precio =
                                    Math.Round(
                                        double.Parse(
                                            aux.Split(' ')[2]))
                                            .ToString(CultureInfo.InvariantCulture);
                                item.PrecioDecimal = double.Parse(aux.Split(' ')[2]);
                                var subTotalPdf = (item.PrecioDecimal * (int.Parse(item.Cantidad)));
                                item.SubTotal = subTotalPdf.ToString(CultureInfo.InvariantCulture);
                                precio = true;
                                contin = true;
                            }
                        }
                    }
                    items.Add(item);
                }
            }
            var index = 1;
            foreach (var it in items)
            {
                Console.WriteLine($"{index++}.-Raw Items Carozzi: {it.ToString()}");
            }
            SumarIguales(items);

            return items;
        }

        private static void SumarIguales(List<ItemCarozzi> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                for (var j = i + 1; j < items.Count; j++)
                {
                    //Console.Write($"|| {i} - {j} => {items[i].Sku} == {items[j].Sku}");
                    if (items[i].Sku.Equals(items[j].Sku))
                    {
                        //Console.WriteLine($"{i} - {j}\n {items[i]} === {items[j]}");
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
            var aux = Regex.Match(str, @"\(\w{1}\d{3}\)").Value;
            return aux.Length > 0 ? aux.Substring(1, aux.Length - 2) : aux;
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
            
            return aux[aux.Length-3].Trim();
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            var aux = str.Split(':');
            return aux[1];
        }

        public OrdenCompraCarozzi GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompraCarozzi { CentroCosto = "0"};
            var _readItem = false;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                //var obs = "";
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        _readRut = true;
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        //OrdenCompra.Observaciones += $"Orden N°: {OrdenCompra.NumeroCompra}, ";
                        _readOrdenCompra = true;
                        _readObs = false;
                    }
                }
                if (!_readObs)
                {
                    if (IsObservacionPattern(_pdfLines[i]))
                    {
                        OrdenCompra.Observaciones +=
                            _pdfLines[i].Substring(
                                _pdfLines[i].IndexOf("Fecha de entrega Día", StringComparison.Ordinal)) + ", ";
                    }
                    if (_pdfLines[i].Equals("Sírvase suministrar a:"))
                    {
                        if (_pdfLines[++i].Contains("HRS"))
                        {
                            OrdenCompra.Observaciones += $"HORARIO DE RECEPCION BODEGA: {_pdfLines[i++]}, ";
                            i++;
                        }

                        for (; !_pdfLines[i].Contains("Cond.pago :"); i++)
                        {

                            if (_pdfLines[i].Length > 1)
                            {
                                OrdenCompra.Observaciones += $"{_pdfLines[i]}, ";
                            }
                        }
                        OrdenCompra.Observaciones = OrdenCompra.Observaciones.Substring(0,
                            OrdenCompra.Observaciones.Length - 2);
                        _readObs = true;
                    }
                }
                if (!_readItem)
                {
                    if (IsHeaderItemPatterns(_pdfLines[i]))
                    {
                        //i += 2;
                        var items = GetItems(_pdfLines,i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.ItemsCarozzi.AddRange(items);
                            _readItem = true;
                        }
                    }
                }
            }
            return OrdenCompra;
        }
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