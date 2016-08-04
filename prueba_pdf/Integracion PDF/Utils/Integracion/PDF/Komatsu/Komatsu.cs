using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Komatsu
{
    public class Komatsu
    {
        #region Variables
        private const string ItemPattern = @"\d{5}\s\w{1}\d{6}\s";
        private const string RutPattern = "Komatsu Chile S.A.";
        private const string OrdenCompraPattern = "Nuestro Pedido";
        private const string ItemsHeaderPattern =
            "Pos Nº Parte Descripción Cant. UM Valor Unit Total Línea Fe Entrega Solicitante";
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs = true;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public Komatsu(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        private static bool IsItem(string str)
        {
            return Regex.Match(str, ItemPattern).Success;
        }
        private static List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
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
                        Sku = test[1]
                    };
                    for(var j= 0; j < test.Length; j++)
                    {
                        if (test[j].Equals("C/U"))
                        {
                            item.Cantidad = test[j - 1];
                            item.Precio = test[j + 1];
                        }

                    }
                    if (item.Sku.Equals("Z664424")
                        || item.Sku.Equals("Z664524"))
                    {
                        item.Cantidad = $"{int.Parse(item.Cantidad)*12}";
                    }else if (item.Sku.Equals("H350120"))
                    {
                        item.Cantidad = $"{int.Parse(item.Cantidad) * 10}";
                    }
                    items.Add(item);
                }
            }
            SumarIguales(items);

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
            var aux = str.Split(':');

            return aux[1].Trim();
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

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra {CentroCosto = "1"};
            var readItem = false;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                //var obs = "";
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Contains(RutPattern))
                    {
                        _readRut = true;
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains(OrdenCompraPattern))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                        _readObs = false;
                    }
                }
                if (!_readObs)
                {
                    if (_pdfLines[i].Contains("Dirección De Entrega para el proveedor:"))
                    {
                        i += 2;
                        OrdenCompra.Observaciones += $"Dirección: {_pdfLines[i].Trim()}, {_pdfLines[i + 2].Trim()}";
                        i += 2;
                        _readObs = true;
                    }
                }
                if (!readItem)
                {
                    if (_pdfLines[i].Trim().Contains(ItemsHeaderPattern))
                    {
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                            readItem = true;
                        }
                    }
                }
            }
            return OrdenCompra;
        }
    }
}