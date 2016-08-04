using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.VitaminaWorkLife
{
    public class VitaminaWorkLife
    {
        #region Variables
        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"^[a-zA-Z]{1}\d{6}\s|^[a-zA-Z]{2}\d{5}\s"}
        };
        private const string RutPattern = "R.U.T. ";
        private const string OrdenCompraPattern = "ORDEN DE COMPRA N°";
        private const string ItemsHeaderPattern =
            "Codigo Detalle Cantidad Precio Total";

        private const string CentroCostoPattern = "de entrega:";
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

        public VitaminaWorkLife(PDFReader pdfReader)
        {
            if (pdfReader.PdfFileName.Contains("OC_Nro_"))
            {
                var pdfResumen = pdfReader.PdfPath.Replace(".pdf", "_Resumen.pdf");
                Console.WriteLine(pdfResumen);
                if (System.IO.File.Exists(pdfResumen))
                {
                    //Main.Main.DeleteFile(pdfResumen);
                    _pdfReader = pdfReader;
                    Console.WriteLine($"Exist: {pdfResumen}");
                }
                else if (System.IO.File.Exists(pdfReader.PdfPath.Replace("OCDistribucion_Nro", "OC_Nro")))
                {
                    _pdfReader = new PDFReader(pdfReader.PdfPath.Replace("OCDistribucion_Nro", "OC_Nro"));
                }
            }
            else
            {
                _pdfReader = pdfReader;
            }
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
                CentroCosto = int.Parse(GetCentroCosto()).ToString()
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

                //if (!_readCentroCosto)
                //{
                //    if (IsCentroCostoPattern(_pdfLines[i]))
                //    {
                //        OrdenCompra.CentroCosto = GetCentroCosto(_pdfLines[i]);
                //        _readCentroCosto = true;
                //    }
                //}
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
            foreach (var it in OrdenCompra.Items)
            {
                Console.WriteLine("IT ==>: "+it.ToString());
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
                            Sku = test0[0],
                            Cantidad = test0[test0.Length - 3].Split(',')[0],
                            Precio = test0[test0.Length - 2].Replace(".", "")
                        };
                        items.Add(item0);
                        break;
                }
            }
            //SumarIguales(items);
            return items;
        }



        /// <summary>
        /// Obtiene el Centro de Costo de una Linea
        /// Con el formato (X123)
        /// </summary>
        /// <returns></returns>
        private string GetCentroCosto()
        {

            var pdfCc = "";
            if (System.IO.File.Exists(_pdfReader.PdfPath.Replace("OC_Nro", "OCDistribucion_Nro")))
            {
                pdfCc = _pdfReader.PdfPath.Replace("OC_Nro", "OCDistribucion_Nro");
            }
            else if (System.IO.File.Exists(_pdfReader.PdfPath.Replace(".pdf", "_Resumen.pdf")))
            {
                pdfCc = _pdfReader.PdfPath; //.Replace(".pdf", "_Resumen.pdf");
            }
            else
            {
                pdfCc = _pdfReader.PdfPath;
            }
            Console.WriteLine(pdfCc);
            var pdf = new PDFReader(pdfCc);
            var ret = "0";
            var item = false;
            foreach (var s in pdf.ExtractTextFromPdfToArrayDefaultMode())
            {
                //Console.WriteLine($"CENCOS : {s}");
                if (s.Contains("Codigo Detalle Centro de Costo Cantidad Precio")) item = true;
                if (item)
                {
                    //Console.WriteLine($"{s} - REGEX: {Regex.Match(s, @"^\d{12}\s").Success}");
                    if (Regex.Match(s, @"^\d{12}\s").Success)
                    {
                        //Console.WriteLine($"SSMatch: {s}");
                        //var split = s.DeleteContoniousWhiteSpace().Split(' ');
                        ret = GetInternalCentroCosto(s.DeleteContoniousWhiteSpace());
                        break;
                    }
                }
            }
            if (ret.Contains("_"))
                ret = ret.Substring(0, ret.IndexOf("_", StringComparison.Ordinal));

            Console.WriteLine($"RET: CC: {ret}");
            return ret;
        }

        private string GetInternalCentroCosto(string str)
        {

            //\d{3}_\w
            //var index = Regex.Match(str, @"\s\d{3}_").Index;
            //var ret = str.Substring(index, 4);
            //Console.WriteLine($"CC: {ret}");
            foreach (var s in str.Split(' '))
            {
                if (Regex.Match(str, @"\s\d{3}_").Success)
                {
                    var index = Regex.Match(str, @"\s\d{3}_").Index;
                    var ret = str.Substring(index, 4);
                    return ret.Trim().Replace("_", "");
                }
            }
            return "0";
            //var split = str.Split(' ');
            //return split[split.Length - 3].Substring(0, 4).Trim().Replace("_", "");
        }


        /// <summary>
        /// Obtiene Orden de Compra con el formato:
        ///         Número orden : 1234567890
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetOrdenCompra(string str)
        {
            str = str.DeleteContoniousWhiteSpace();
            var aux = str.Split(' ');
            return aux[aux.Length - 1];
        }

        /// <summary>
        /// Obtiene el Rut de una linea con el formato:
        ///         RUT:12345678-8
        /// </summary>
        /// <param name="str">Linea de Texto</param>
        /// <returns>12345678</returns>
        private static string GetRut(string str)
        {
            str = str.DeleteContoniousWhiteSpace();
            var aux = str.Split(' ');
            return aux[aux.Length - 1];
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            str = str.DeleteDotComa();
            //Console.WriteLine("RAW IT===>"+str);
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