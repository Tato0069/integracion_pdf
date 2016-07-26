using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Cencosud
{
    public class Easy : Cencosud
    {
        private bool _readOrdenCompra = true;
        private bool _readRut;
        private bool _readCentroCosto = true;
        
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        public Easy(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArray();
        }

        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public override OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra();
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (_pdfLines[i].Trim().Equals("EASY RETAIL S.A."))
                    {
                        _readRut = true;
                        _readOrdenCompra = false;
                        i++;//Saltar linea de telefono
                        OrdenCompra.Rut = GetRut(_pdfLines[++i]);
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Equals("Núm. Orden/ Fecha Creación"))
                    {
                        i++;//Saltar linea de Rut Dimerc
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);
                        _readOrdenCompra = true;
                        _readCentroCosto = false;
                    }
                }
                if (!_readCentroCosto)
                {
                    if (_pdfLines[i].Trim().Contains("Lugar de entrega :"))
                    {
                        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                        var aux = _pdfLines[i].Split(':');
                        var str = aux[1].Substring(0, aux[1].IndexOf("Comprador", StringComparison.Ordinal)).Trim();
                        aux = _pdfLines[++i].Split(':');
                        var str2 = aux[1].Substring(0, aux[1].IndexOf("Sección", StringComparison.Ordinal)).Trim();
                        var dir = str + " " + str2;
                        OrdenCompra.Observaciones = dir;
                        OrdenCompra.Direccion = dir;
                        _readCentroCosto = true;
                    }
                }
                
                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim().Equals("Proved. del Artículo Pedida de Empaque c/descuento"))
                    {
                        itemForPage++;
                        i++;//Saltar linea de Tabla
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                        }
                    }
                }
            }
            return OrdenCompra;
        }

        protected override List<Item> GetItems(string[] arg, int firstIndex)
        {
            var items = new List<Item>();
            for (; !arg[firstIndex].Trim().Contains("Neto total sin IVA CLP") &&
                !arg[firstIndex].Trim().Equals("Ref. Por fusión, EASY cambia de RUT y Razón Social"); firstIndex++)
            {
                //Linea de Item
                if (Regex.Match(arg[firstIndex], @"\d{5}\s\d{5}\s\d{13}\s").Success)
                {
                    var item = new Item();
                    var test = arg[firstIndex].Split(' ');
                    /* Index:
                     *  0 = Item
                     *  1 = Art.
                     *  2 = EAN
                     */
                    item.Sku = test[1];
                    item.Cantidad = HavePackingUnits(arg[firstIndex]) ? test[test.Length - 6] : test[test.Length - 4];
                    item.Precio = GetPrecioUnitario(item.Cantidad, test[test.Length-1]);
                    items.Add(item);
                }
            }
            return items;
        }

        private string GetPrecioUnitario(string cantidad, string precioTotal)
        {
            cantidad = cantidad.Replace(".", "");
            precioTotal = precioTotal.Replace(".", "");
            return (long.Parse(precioTotal)/long.Parse(cantidad)).ToString();
        }

        private string GetPrecioTotal(string str)
        {
            var aux = str.Split(' ');
            return aux[aux.Length-1];
        }

        protected override string GetCentroCosto(string str)
        {
            var aux = Regex.Match(str, @"\s\(\w{1}\d{3}\)\s").Value;

            return aux.Substring(2, aux.Length - 4);
        }

        protected override string GetSku(string str)
        {
            /* Index:
             *  0 = Item
             *  1 = Art.
             *  2 = EAN
             */
            return Regex.Match(str, @"\d{5}\s\d{5}\s\d{13}\s").Value.Split(' ')[1];
        }

        protected override string GetCantidad(string str)
        {
            var aux = str.Split(' ');
            return aux[aux.Length - 1];
        }

       protected override string GetPrecio(string str)
        {
            var st = str.Split(' ');
            return st[st.Length - 2];

        }

        protected override string GetOrdenCompra(string x)
        {
            var aux = x.Split('/');
            return aux[0].Trim();
        }

        protected override string GetRut(string x)
        {
            var aux = x.Split(':');
            return aux[1].Trim().Substring(0, aux[1].Trim().Length - 2);
        }

        private bool HavePackingUnits(string str)
        {
            return Regex.Match(str,
                @"\s\w{2}\s\d+\s\w{2}\/\w{2}\s\d{1}\s").Success;//{1,3}[\d+|.\d{3}]*
            //PORTA CRED.9X5.5CM C/MINICLIP+ALFILER 50            4 CS 50 UN/CS 0 10.496
        }

    }
}