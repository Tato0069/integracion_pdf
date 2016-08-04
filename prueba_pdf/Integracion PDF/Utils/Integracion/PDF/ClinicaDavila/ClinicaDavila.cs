using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClinicaDavila
{
    public class ClinicaDavila
    {
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readDir;
        private bool _readUnidadSolicitante;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;
        public string AnexoPath;
        private OrdenCompraClinicaDavila OrdenCompra { get; set; }
        private List<OrdenCompraClinicaDavila> OrdenesCompras { get; set; }

        public ClinicaDavila(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
        }

        public OrdenCompraClinicaDavila GetOrdenCompraProcesada()
        {
            return OrdenCompra;
        }

        public bool HaveAnexo()
        {
            GetOrdenCompra();
            var pdfAnexos = Directory
                .GetFiles(_pdfReader.PdfRootPath, "*.pdf").ToList();//@InternalVariables.GetOcAProcesarFolder(), "*.pdf")
                //.Where(pdf => pdf.Contains("cd_anexo.pdf")).ToList();

            //Busca entre los Anexos el que posea el numero de Pedido de la Orden
            var pdfAnexo = pdfAnexos.FirstOrDefault(pdfAn => new PDFReader(pdfAn).ExtractTextFromPdfToString()
                .Replace(".", "")
                .Contains($"ANEXO PARA ENTREGAS DIFERIDAS DE OC Nº{OrdenCompra.NumeroCompra}"));
            AnexoPath = pdfAnexo;
            if (pdfAnexo != null)
            {
                OrdenesCompras = ReadAnexo(AnexoPath);
            }
            return pdfAnexo != null;
        }

        public List<OrdenCompraClinicaDavila> GetOrderFromAnexo()
        {
            return OrdenesCompras;
        }

        private List<OrdenCompraClinicaDavila> ReadAnexo(string pdf)
        {
            var item = false;
            var list = new List<string>();
            var pdfReader = new PDFReader(pdf);
            var pdfLines = pdfReader.ExtractTextFromPdfToArrayDefaultMode();
            foreach (var rawLin in pdfLines)
            {
                if (!item)
                {
                    if (rawLin.Contains("UNIDAD SOLICITANTE DESCRIPCION DEL PRODUCTO ENTREGA LUGAR ENTREGA CANT."))
                    {
                        item = true;
                        continue;
                    }
                }
                if (item)
                {
                    if (rawLin.Length == 7) continue; //Saltar Lineasn con 4? ?4 
                    if (rawLin.Contains("CLINICA DAVILA Y SERVICIOS MEDICOS S.A. Fecha :"))
                    {
                        item = false;
                        continue;
                    }                    
                    list.Add(rawLin);
                }
                //Console.WriteLine($"RawLine: {rawLin}");
            }
           return CreateAllOrden(list);
        }

        private List<OrdenCompraClinicaDavila> CreateAllOrden(List<string> list)
        {
            var desCenCos = "";
            var listOrden = new List<OrdenCompraClinicaDavila>();
            //foreach (var rawLin in list)
            for(var i = 0; i < list.Count;i++)//.Count; i++)
            {
                //00 ff 03 05 c0 00 ea 04 00 00
                var rawLin = list[i];
                var aux = rawLin.Split(' ');
                var cant = aux[aux.Length - 1];
                var rawLine = aux.ArrayToString(0, aux.Length - 2);
                //Console.WriteLine($"RAWLIN: {rawLin.ConvertStringToHex()}");
                //Console.WriteLine($"RAWLINE: {rawLine.ConvertStringToHex()}");
                if (rawLine.Equals("?")) continue;
                if (aux[aux.Length - 2].Contains("UNI"))
                    rawLine += " UNI";
                var pro = GetProduct(rawLine);
                if (pro == null) continue;
                var auxCenCos = rawLin.Substring(0,
                    rawLin.IndexOf(pro.Descripcion,
                    StringComparison.Ordinal));
                var itemErroneo = false;
                if (!desCenCos.Equals(auxCenCos))
                {
                    if (desCenCos.Length > 0)
                    {
                        var splitDesCc = desCenCos.Split(' ');
                        if (auxCenCos.Length > 0)
                        {
                            var splitAux = auxCenCos.Split(' ');
                            if (splitDesCc.Count() < splitAux.Count())
                            {
                                if (desCenCos.Replace(" ", "")
                                    .Equals(splitAux
                                        .ArrayToString(0, splitDesCc.Length - 1).Replace(" ", "")))
                                {
                                    //Se SOLAPA Descripcion de ITEM, ITEM ERRONEO
                                    //Console.WriteLine(rawLin.Substring(desCenCos.Length));
                                    itemErroneo = true;
                                    goto test;
                                }
                            }
                        }
                    }
                    var ord = new OrdenCompraClinicaDavila
                    {
                        NumeroCompra = OrdenCompra.NumeroCompra,
                        CentroCosto = auxCenCos,
                        Rut = OrdenCompra.Rut
                    };
                    var its = new ItemDavila
                    {
                        Cantidad = cant,
                        Precio = pro.Precio,
                        Sku = pro.Sku
                    };
                    ord.AddItemDavila(its);
                    desCenCos = auxCenCos;
                    listOrden.Add(ord);
                    continue;
                }
                test:
                if (itemErroneo)
                {
                    pro = GetItemFromDescripcion(rawLin.Substring(desCenCos.Length));
                    itemErroneo = false;
                }
                var it = new ItemDavila
                {
                    Cantidad = cant,
                    Precio = pro.Precio,
                    Sku = pro.Sku,
                    Descripcion = pro.Descripcion
                };
                var or = listOrden.First(o => o.CentroCosto.Equals(desCenCos));
                or.AddItemDavila(it);
            }
            return listOrden;
        }

        private ItemDavila GetItemFromDescripcion(string desc)
        {
            var aux = desc.Split(' ');
            var realDesc = aux.ArrayToString(0, aux.Length - 2);
            return OrdenCompra.ItemsClinicaDavila.FirstOrDefault(x => x.Descripcion.Equals(realDesc));
        }


        /// <summary>
        /// Devuelve el Producto que calce con la descripción
        /// Si no existe, retorna NULL
        /// </summary>
        /// <param name="rawLine"></param>
        /// <returns></returns>
        private ItemDavila GetProduct(string rawLine)
        {
            return OrdenCompra.ItemsClinicaDavila.FirstOrDefault(it => rawLine.Contains(it.Descripcion));
        }


        private List<ItemDavila> GetItems(string[] pdfLines, int firstIndex)
        {
            var items = new List<ItemDavila>();
            for (; firstIndex < pdfLines.Length; firstIndex++)//.Count; firstIndex++)
            {
                //Es una linea de Items
                if (Regex.Match(pdfLines[firstIndex], @"\d{8}\s").Success)
                {
                    //es una linea que contiene items
                    var test = pdfLines[firstIndex].Split(' ');
                    var item = new ItemDavila
                    {
                        Sku = test[0],
                        Cantidad = test[test.Length - 3],
                        Precio = test[test.Length - 2].Split(',')[0],
                        Descripcion = test.ArrayToString(1, test.Length - 3)
                    };
                    items.Add(item);
                }
            }
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
            var aux = str.Split(':');
            return aux[1].Trim();
        }

        public OrdenCompraClinicaDavila GetOrdenCompra()
        {
            OrdenCompra = new OrdenCompraClinicaDavila
            {
                CentroCosto = "0"
            };
            var itemForPage = 0;
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readRut)
                {
                    if (_pdfLines[i].Contains(" Rut : "))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i]);
                        _readRut = true;
                    }
                    
                }
                if (!_readUnidadSolicitante)
                {
                    if (_pdfLines[i].Contains("Solicitante"))
                    {
                        OrdenCompra.CentroCosto = GetSolicitante(_pdfLines[i]);
                    }
                }
                if (!_readOrdenCompra)
                {
                    if (_pdfLines[i].Trim().Contains(" ORDEN DE COMPRA Nº "))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[i]);
                        _readOrdenCompra = true;
                    }
                }
                if (!_readDir)
                {
                    if (_pdfLines[i].Trim().Contains("Lugar Entrega : "))
                    {
                        _readDir = true;
                        var dir = GetDir(_pdfLines[i]);
                        OrdenCompra.Direccion = dir;
                        OrdenCompra.Observaciones = dir;
                    }
                }

                if (itemForPage < _pdfReader.NumerOfPages)
                {
                    if (_pdfLines[i].Trim().Equals("CODIGO DESCRIPCION DEL PRODUCTO CANTIDAD P. UNITARIO TOTAL"))
                    {
                        itemForPage++;
                        var items = GetItems(_pdfLines, i);
                        if (items.Count > 0)
                        {
                            OrdenCompra.ItemsClinicaDavila.AddRange(items);
                        }
                    }
                }
            }
            return OrdenCompra;
        }

        private string GetSolicitante(string str)
        {
            var ret = "0";
            var split = str.Split(':');
            ret = split[1].Replace("Fax","").DeleteContoniousWhiteSpace();
            return ret;
        }

        private string GetDir(string str)
        {
            var aux = str.Split(':');
            return aux[1].Substring(0, aux[1].IndexOf("Comprador", StringComparison.Ordinal)).Trim();
        }
    }
}