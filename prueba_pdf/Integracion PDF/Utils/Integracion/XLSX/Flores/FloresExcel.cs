using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX.Flores
{
    public class FloresExcel
    {
        private readonly List<string> _nombredHojas = new List<string>{ "Hoja1" , "OFICINA"};
        private const string RutPattern = "*";
        private const string OrdenCompraPattern = "Orden de compra N°";
        private const string CentroCostoPattern = "Centro de Costo Dimerc N°";

        private readonly Dictionary<int, string> _itemsHeaderPattern = new Dictionary<int, string>
        {
            {0, "ITEM Código proveedor "}
        };

        private const int IndexColumnIdItem = 0;
        private const int IndexColumnSku = 1;
        private const int IndexColumnCantidad = 3;
        private const int IndexColumnPrecio = 4;
        //private Dictionary<int, string> SkuColumnName = new Dictionary<int, string>
        //{
        //    {0, "Código proveedor"}
        //};

        //private Dictionary<int, string> CantidadColumnName = new Dictionary<int, string>
        //{
        //    {0, "Precio Unitario"}
        //};
        //SkuColumnName,PrecioColumnName

        private Dictionary<bool, string> _linesToAddFromPdfRawText = new Dictionary<bool, string>
        {
            {false, "Centro de Costo Dimerc N°"}
        };

        private int _formatItems = 0;

        private const string ObservacionesPattern = "Tienda :";
        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readItem;
        private readonly ExcelReader _excelReader;

        public string[] RawArrayTextPdf => _excelReader.RawArrayString;

        //Texto de Excel por Hojas y Linea
        private readonly List<List<string>> _excelHojaArrayLines;
        //Texto de Excel por Hojas, Filas y Columnas
        private readonly List<List<List<string>>> _excelHojaMatizLines;
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public FloresExcel(ExcelReader excel)
        {
            _excelReader = excel;
            _excelHojaArrayLines = _excelReader.ExtractTextToArray(_nombredHojas);
            _excelHojaMatizLines = _excelReader.ExtractTextToMatrix(_nombredHojas);
        }

        public List<List<string>> GetHojaArrayLines()
        {
            return _excelHojaArrayLines;
        }

        public List<List<List<string>>> GetHojaMatizLines()
        {
            return _excelHojaMatizLines;
        }


        public OrdenCompra.OrdenCompra GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompra.OrdenCompra();
            var indexHoja = 0;
            //foreach (var hoja in _excelHojaArrayLines)
            //{
                //for (var i = 0; i < hoja.Count; i++)
                //{
                //    var line = hoja[i];

                //}

                for (var i = 0; i < RawArrayTextPdf.Length; i++)
                {
                    var line = RawArrayTextPdf[i];
                    if (!_readCentroCosto)
                    {
                        Console.WriteLine($"CC: {line}");
                        if (IsCentroCostoPattern(line))
                        {
                            var cc = GetCentroCosto(line);
                            Console.WriteLine($"CC: {cc}");
                            OrdenCompra.CentroCosto = cc;
                            OrdenCompra.Rut = OrdenCompra.CentroCosto.Equals("0")
                                ? "92987000"
                                : "76129552";
                            _readCentroCosto = true;
                        }
                    }
                    if (!_readOrdenCompra)
                    {
                        if (IsOrdenCompraPattern(line))
                        {
                            OrdenCompra.NumeroCompra = GetOrdenCompra(line);
                            _readOrdenCompra = true;
                            _readObs = false;
                        }
                    }
                    if (!_readObs)
                    {
                        if (IsObservacionPattern(line))
                        {
                            OrdenCompra.Observaciones +=
                                $"{line.Trim().DeleteContoniousWhiteSpace()}, " +
                                $"{RawArrayTextPdf[++i].Trim().DeleteContoniousWhiteSpace()}";
                            _readObs = true;
                            _readItem = false;
                        }
                    }
                    if (!_readItem)
                    {
                        GetAllItems();
                    }
                //}
                indexHoja++;
            }
            return OrdenCompra;
        }

        private void GetAllItems()
        {
            var indexHoja = 0;
            foreach (var hoja in _excelHojaArrayLines)
            {
                for (var i = 0; i < hoja.Count; i++)
                {
                    var line = _excelHojaArrayLines[indexHoja][i];
                    var optionHeader = GetFormatItemsPattern(line);
                    if (optionHeader != -1)
                    {
                        var items = GetItems(indexHoja, ++i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.Items.AddRange(items);
                            _readItem = true;
                        }
                    }
                }
                indexHoja++;
            }
        }

        private List<Item> GetItems(int indexHoja, int indexFila)
        {
            Console.WriteLine(indexFila+"========>INDEXFILA");
            var items = new List<Item>();
            //var columnName = 
            var hoja = _excelHojaMatizLines[indexHoja];
            for (; indexFila < hoja.Count && !hoja[indexFila][IndexColumnIdItem].Trim().Equals(""); indexFila++)
            {
                var item = new Item
                {
                    Sku = hoja[indexFila][IndexColumnSku],
                    Cantidad = hoja[indexFila][IndexColumnCantidad],
                    Precio = hoja[indexFila][IndexColumnPrecio].Replace("$","").DeleteContoniousWhiteSpace()
                };
                //Console.WriteLine($"{hoja[indexFila][IndexColumnSku].GetType().FullName}, VALUE:,{hoja[indexFila][IndexColumnSku]},");
                items.Add(item);
            }
            var realItem = DeleteItemsWithNullValues(items);

            return items;
        }

        private IEnumerable<Item> DeleteItemsWithNullValues(List<Item> items)
        {
            for(var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.Sku.Equals("")
                    || item.Cantidad.Equals("")
                    || item.Precio.Equals(""))
                {
                    items.Remove(item);
                    i--;
                }
            }
            return items;
        }


        private static string NormaliceItemLine(string str)
        {
            var ret = str;
            if (str.Contains(" ."))
            {
                ret = str.Replace(" .", ".");
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
            var aux = str.Split('°');
            return aux[1].Trim();
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            var aux = str.Split('°');
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

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            foreach (var it in _itemsHeaderPattern
                .Where(it => str.Contains(it.Value)))
            {
                ret = it.Key;
            }
            return ret;
        }

        #region Funciones Is
        //private bool IsHeaderItemPatterns(string str)
        //{
        //    return str.Trim().DeleteContoniousWhiteSpace().Contains(ItemsHeaderPattern);
        //}

        private bool IsObservacionPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(ObservacionesPattern);
        }

        private bool IsOrdenCompraPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(OrdenCompraPattern);
        }

        private bool IsCentroCostoPattern(string str)
        {
            return str.Trim().DeleteContoniousWhiteSpace().Contains(CentroCostoPattern);
        }

        #endregion


    }
}