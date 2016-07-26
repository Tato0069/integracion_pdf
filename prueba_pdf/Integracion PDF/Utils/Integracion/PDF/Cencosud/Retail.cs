using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using prueba_pdf.Utils.OrdenCompra;

namespace prueba_pdf.Utils.Cencosud
{
    public class Retail
    {
        private static bool _readOrdenCompra;
        private static bool _readRut = true;
        private static bool _centroCosto;
        private static bool _readItems = true;
        private PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }
        public Retail(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }
        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra();
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Equals("Datos del proveedor Orden de compra"))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                        _readRut = false;
                    }
                }
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Equals("Datos de facturación"))
                    {
                        _readRut = true;
                        _readItems = false;
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                    }
                }

                if (!_readItems)
                {
                    if (_pdfLines[i].Trim().Equals("Item Código Descripción Lugar Entrega Cantidad  UMM Precio Pr. lista"))
                    {
                        //Console.WriteLine(pdfLines[i].Trim());
                        i = i + 2;
                        OrdenCompra.Items = GetItems(_pdfLines, i);
                    }
                }
            }
            return OrdenCompra;
        }

        private List<Item> GetItems(IReadOnlyList<string> pdfLines, int firstIndex)
        {
            var items = new List<Item>();
            for (; !pdfLines[firstIndex].Trim().Contains("Neto total sin IVA CLP"); firstIndex++)
            {
                //Console.WriteLine(pdfLines[firstIndex]);
                if (Regex.Match(pdfLines[firstIndex], @"\d{3}\s\d{7} ").Success) {//es una linea que contiene items
                    var item = new Item {Sku = GetSku(pdfLines[firstIndex])};
                    var restStr = pdfLines[firstIndex].Substring(11);
                    item.Precio = GetPrecio(restStr);
                    restStr = restStr.Substring(0, restStr.IndexOf(item.Precio, StringComparison.Ordinal));
                    item.Cantidad = GetCantidad(restStr);
                    items.Add(item);
                }else if (!_centroCosto)
                {
                    OrdenCompra.CentroCosto = GetCentroCosto(pdfLines[firstIndex]);
                    _centroCosto = true;
                }
            }
            return items;
        }

        private string GetCentroCosto(string str)
        {
            var aux = Regex.Match(str, @"\(\w{1}\d{3}\)").Value;
            return aux.Substring(1,aux.Length-2);
        }

        private string GetSku(string str)
        {
            return Regex.Match(str, @"\s\d{7}\s").Value.Trim();
        }

        private string GetCantidad(string str)
        {
            var aux =
                Regex.Match(str,
                    @"\s\d{1,3}[.\d{3}]*\s\w+\s\z|\s\d{1,3}\s\w{2}\s\z").Value;

            return aux.Split(' ')[1];
        }

        private string GetPrecio(string str)
        {
            var st = str.Split(' ');
            return st[st.Length - 2];

        }

        private string GetOrdenCompra(string x)
        {
            var aux = x.Split(':');
            return aux[2].Trim();
        }

        private string GetRut(string x)
        {
            var aux = x.Split(':');
            return aux[2].Substring(0, 8);
        }
        
        
    }


}