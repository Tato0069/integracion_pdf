using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX.PlanillaEstandar
{
    public class PlanillaEstandar
    {
        #region Variables
        //private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        //{
        //    {0, @"^\d{1,}\s\w{3}\d{5,6}\s\d{3,}\s\d{1,}\s\d{1,}"},
        //    {1, @"^\d{1,}\s\w{3}\d{5,6}\s\d{1,}\s" }
        //};
        //private const string RutPattern = "RUT:";
        //private const string OrdenCompraPattern = "Orden de Compra";
        //private const string ItemsHeaderPattern =
        //    "Item Material/Description Cantidad UM Precio Unit. Valor";

        //private const string CentroCostoPattern = "de entrega:";
        //private const string ObservacionesPattern = "Tienda :";

        //private bool _readCentroCosto;
        //private bool _readOrdenCompra;
        //private bool _readRut;
        //private bool _readObs;
        //private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public PlanillaEstandar(PDFReader pdfReader)
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
        public List<OrdenCompra.OrdenCompra> GetOrdenCompra()
        {
            var ret = new List<OrdenCompra.OrdenCompra>();
            var ocEstandarList = new List<OrdenCompraEstandar>();
            for (var i = 0; i < _pdfLines.Length && !_pdfLines[i].Equals("PLANILLA_ESTANDAR"); i++)
            {
                if (_pdfLines[i].Trim().Replace(" ", "").Equals("")
                    || _pdfLines[i].Contains("RUT OC CC SKU CANTIDAD PRECIO SUBTOTAL"))
                    continue;
                var split = _pdfLines[i].Split(' ');
                var ocEstandar = new OrdenCompraEstandar
                {
                    Rut = split[0],
                    NumeroCompra = split[1],
                    CentroCosto = split[2],
                    Sku = split[3],
                    Cantidad = split[4],
                    Precio = split[5]
                };
                ocEstandarList.Add(ocEstandar);
            }
            var lastOc = "";
            var lastRutcli = "";
            var lastItems = new List<Item>();
            var lastCencos = "";
            foreach (var ocE in ocEstandarList.OrderBy(o => o.NumeroCompra))
            {
                if (lastCencos.Equals(""))
                {
                    lastCencos = ocE.CentroCosto;
                    lastOc = ocE.NumeroCompra;
                    lastItems = new List<Item>();
                    lastRutcli = ocE.Rut;
                }
                else if (!lastCencos.Equals(ocE.CentroCosto) 
                    || !lastOc.Equals(ocE.NumeroCompra)
                    || !lastRutcli.Equals(ocE.Rut)
                    )
                {
                    var oc = new OrdenCompra.OrdenCompra
                    {
                        Rut = lastRutcli,
                        CentroCosto = lastCencos,
                        NumeroCompra = lastOc,
                        Items = lastItems
                    };
                    lastCencos = ocE.CentroCosto;
                    lastOc = ocE.NumeroCompra;
                    lastItems = new List<Item>();
                    lastRutcli = ocE.Rut;
                    ret.Add(oc);
                }
                var item = new Item
                {
                    Sku = ocE.Sku,
                    Cantidad = ocE.Cantidad,
                    Precio = ocE.Cantidad
                };
                lastItems.Add(item);

            }

            Console.WriteLine("==========================00");
            foreach (var oc in ret)
            {
                Console.WriteLine(oc.ToString());
            }
            return ret;
        }


        #endregion


       

    }

    internal class OrdenCompraEstandar
    {
        public OrdenCompraEstandar()
        {
            Rut = "";
            NumeroCompra = "";
            CentroCosto = "";
            Observaciones = "";
            Direccion = "";
            Sku = "";
            Cantidad = "";
            Precio = "";
        }

        public string Sku { get; set; }

        public string Cantidad { get; set; }

        public string Precio { get; set; }

        public string Direccion
        {
            get { return _direccion.Replace("'", "''"); }
            set { _direccion = value; }
        }

        private string _direccion;

        public string Rut
        {
            get { return _rut; }
            set
            {
                _rut = value;
                if (_rut.Contains('-'))
                {
                    _rut = _rut.Split('-')[0];
                }
                _rut = _rut.Replace(".", "");
            }
        }

        private string _rut;
        public string NumeroCompra { get; set; }
        public string CentroCosto { get; set; }
        public string Observaciones { get; set; }

        
        public override string ToString()
        {
            //var items = Items.Aggregate("", (current, item) => current + string.Format($"{item}\n"));
            //var cont = 1;
            //var items = Items.Aggregate("", (current, ir) => current + $"{cont++}.- {ir}\n");
            return $"Rut: {Rut}, N° Compra: {NumeroCompra}, Centro Costo: {CentroCosto}, Observaciones: {Observaciones}, Dirección: {Direccion}";//\nItems:\n{items}";
        }

    }
}