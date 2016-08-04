using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Cencosud
{
    public class CencosudRetailSa
    {
        #region Variables

        private const string ItemPattern = @"^\d{1,}\s\d{7}\s";
        private const string RutPattern = "Datos de facturación";
        private const string OrdenCompraPattern = "Datos del proveedor Orden de compra";
        private const string ItemsHeaderPattern =
            "Item Código Descripción Lugar Entrega Cantidad UMM Precio Pr. lista";
        private const string ItemsFooterPattern = "Neto total sin IVA CLP";
        private const string CentroCostoPattern = @"\(\w{1}\d{3}\)";
        private const string MailPattern = "Correo";
        private const string CompradorPattern = "Comprador";
        private bool _readOrdenCompra;
        private bool _readRut = true;
        private bool _readItems = true;
        private bool _readCorreo = true;
        private bool _readComprador = true;
        private bool _readCentroCosto;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public CencosudRetailSa(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        private bool IsItem(string str)
        {
            str = str.DeleteDotComa();
            return Regex.Match(str, ItemPattern).Success;
        }
        private List<Item> GetItems(string[] pdfLines, int i)
        {
            var items = new List<Item>();
            var itemsArray = new List<string>();
            for (; i < pdfLines.Length; i++)
            {
                
                var aux = pdfLines[i].DeleteContoniousWhiteSpace().Trim();
                Console.WriteLine(aux);
                if (IsItemsFooterPattern(aux)) break;
                if (IsItem(aux))
                {//es una linea que contiene items
                    var test = aux.Split(' ');
                    var item = new Item
                    {
                        Sku = test[1],
                        Cantidad = test[test.Length - 4],
                        Precio = test[test.Length - 2]
                    };
                    aux = test.ArrayToString(2, test.Length - 4);
                    items.Add(item);
                }
                itemsArray.Add(aux);
                if (!_readCentroCosto)
                {
                    var centroCosto = GetCentroCosto(aux);
                    if (centroCosto.Length == 0) continue;
                    OrdenCompra.CentroCosto = centroCosto;
                    _readCentroCosto = true;
                }
            }
            var dir = itemsArray.GetDireccionCencosud();
            OrdenCompra.Direccion = dir;
            //SumarIguales(items);
            return items;
        }

        private string GetComprador(string st)
        {
            var aux = st.Split(':');
            return aux[aux.Length - 1].Trim();
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
            return aux[2].Trim();
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
            return aux[2].Substring(0, 8);
        }

        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra
            {
                CentroCosto = "0"
            };
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                //var aux = _pdfLines[i].Trim().DeleteContoniousWhiteSpace();
                var obs = "";
                if (!_readComprador)
                {
                    if (IsCompradorPattern(_pdfLines[i]))
                    {
                        var comprador = GetComprador(_pdfLines[i]);
                        obs += $"Comprador: {comprador}, ";
                        OrdenCompra.Observaciones += obs;
                        _readComprador = true;
                    }
                }
                if (!_readCorreo)
                {
                    if (IsCorreoPattern(_pdfLines[i]))
                    {
                        var correo = GetCorreo(_pdfLines[i]);
                        obs += $"Correo: {correo}";
                        OrdenCompra.Observaciones += obs;
                        _readCorreo = true;
                    }

                }
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }

                }
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        _readRut = true;
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                        _readItems = false;
                    }
                }
                if (!_readItems)
                {
                    if (IsItemHeaderPattern(_pdfLines[i]))
                    {
                        Console.WriteLine(_pdfLines[i]);
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                            _readItems = true;
                        }
                    }
                }
            }
            return OrdenCompra;
        }

        private string GetCorreo(string st)
        {
            var aux = st.Split(':');
            return aux[aux.Length - 1].Trim();
        }


        private bool IsItemHeaderPattern(string str)
        {
            return str.Trim().Contains(ItemsHeaderPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return Regex.Match(str, ItemPattern).Success;
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.Trim().Contains(OrdenCompraPattern);
        }
        private bool IsItemsFooterPattern(string str)
        {
            return str.Trim().Contains(ItemsFooterPattern);
        }

        private bool IsCompradorPattern(string str)
        {
            return str.Trim().Contains(CompradorPattern);
        }
        private bool IsCorreoPattern(string str)
        {
            return str.Trim().Contains(MailPattern);
        }

        private bool IsRutPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(RutPattern);
        }
    }
}