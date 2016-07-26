using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.IntegraMedica
{
    public class IntegraMedica
    {
        #region Variables

        public readonly Dictionary<int, string> Cc96845430Observaciones = new Dictionary<int, string>
        {
            {0, "AV CERRO COLORADO 5240-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {1, "LOS MILITARES 4777 PISO 7-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"}
        };

        public readonly Dictionary<int, string> Cc96986050Observaciones = new Dictionary<int, string>
        {
            {0, "Exámenes de Laboratorio-2000-LISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {1, "AV.EL LLANO SUBERCASEUX Nº3965 SAN MIGUEL 1° piso"},
            {2, "AV. LIBERTADOR BERNARDO O´HIGGINS # 654 SANTIAGO 5° piso"}
        };

        public readonly Dictionary<int, string> Cc79716500Observaciones = new Dictionary<int, string>
        {
            {0, "Sonorad Bellotas-8100-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {1, "Sonorad Huerfanos-8110-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {2, "Sonorad La Florida-8120-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {3,"Sonorad Maipu-8130-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" },
            {4,"Sonorad Viña-8140-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" },
            {5,"Puente Alto-8150-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" },
            {6,"Sonorad Independencia-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" },
            {7,"8301 Sonorad Alameda -Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" },
            {8,"8300 Sonorad Irarrazabal-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" },
            {9,"Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs" }
        };

        public readonly Dictionary<int, string> Cc76098454Observaciones = new Dictionary<int, string>
        {
            {0, "IntegraMédica Barcelona-4010-IBA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {
                1,
                "IntegraMédica Las Condes-4020-ICO-Bultos Grandes hasta las 11:00 de la mañana, Bultos Pqueños,  por el ascensor interno de integramedica ubicado en el -1 de integramedica , Llamar a seguridad al 2996418 o al 6796555 de la bodega, hasta las 18.30."
            },
            {2, "IntegraMédica Tobalaba-4030-ITO-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {3, "IntegraMédica Oeste-4040-IMO-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {4, "IntegraMédica Est. Central-4050-IEC-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {5, "IntegraMédica La Florida-4070-IFS-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {6, "IntegraMédica Centro-4080-ICE-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {7, "IntegraMédica Alameda-4090-IPF-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {8, "IntegraMédica Norte-4100-INO-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {9, "IntegraMédica San Miguel-4120-ISM-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {10, "IntegraMédica La Serena-4130-ILS-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {11, "IntegraMédica El Trébol-4140-ITR-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {12, "IntegraMédica Manquehue-4150-IMQ-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {13, "IntegraMédica Puente Alto-4160-IPA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {14, "IntegraMédica Talca-4180-ITA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {15, "IntegraMédica Maipú-4190-IMP-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {16, "IntegraMédica Bío- Bío-4200-IBB-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {17, "IntegraMédica Bandera-4210-IBN-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {18, "IntegraMédica Santa Lucía-4220-ISL-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {19, "Integramedica Plaza Vespucio-4230-IPV-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {20, "Integramedica Plaza Sur-4240-IPS-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {21, "IntregraMédica Plaza Egaña-4250-IPE-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {22, "Integramedica Viña del Mar-4260-IVM-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {23, "IntegraMédica Rancagua-4280-IRA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {24, "Pilar Gazmuri-4300-PG-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {25, "Gerencia de Marketing-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {26, "Call Center IntegraMédica-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {27, "Gerencia de Imágenes-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {28, "Unidad Gestión Paciente-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {29, "Kinevid-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {30, "IntegraMédica S.A.-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {31, "IntegraMédica S.A.-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {32, "Gerencia IntegraMédica-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {33, "Gerencia Comercial-4500-ISA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"},
            {34, "CD INTEGRAMEDICA-Lunes a Viernes de 9:00 a 13:00 hrs *  15:00 hrs a 17:00 hrs"}
        };
        

        private readonly Dictionary<int, string> _itemsPatterns = new Dictionary<int, string>
        {
            {0, @"\d{1,}\s\d{2}\.\d{2}\.\d{4}\s\d{8}\s"}
            //10 06.07.2016 60000863 
        };
        private const string RutPattern = "ORDENDECOMPRA";//IN or NEXT
        private const string OrdenCompraPattern = "PedidoCompra";//NEXT
        private const string ItemsHeaderPattern =
            "Pos.Fe.Entrega Material Denominación";

        private const string CentroCostoPattern = "de entrega:";
        private const string ObservacionesPattern = "Sírvaseentregara";

        private bool _readCentroCosto;
        private bool _readOrdenCompra;
        private bool _readRut;
        private bool _readObs;
        private bool _readDespacho;
        private bool _readItem;
        private readonly PDFReader _pdfReader;
        private readonly string[] _pdfLines;

        #endregion
        private OrdenCompra.OrdenCompra OrdenCompra { get; set; }

        public IntegraMedica(PDFReader pdfReader)
        {
            _pdfReader = pdfReader;
            _pdfLines = _pdfReader.ExtractTextFromPdfToArray();
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
            OrdenCompra = new OrdenCompra.OrdenCompra();
            var firstCentroCosto = "";
            var secondCentroCosto = "";
            for (var i = 0; i < _pdfLines.Length; i++)
            {
                if (!_readOrdenCompra)
                {
                    if (IsOrdenCompraPattern(_pdfLines[i]))
                    {
                        OrdenCompra.NumeroCompra = GetOrdenCompra(_pdfLines[++i]);

                        _readOrdenCompra = true;
                    }
                }
                if (!_readRut)
                {
                    if (IsRutPattern(_pdfLines[i]))
                    {
                        OrdenCompra.Rut = GetRut(_pdfLines[i],_pdfLines[++i]);
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
                if (!_readObs)
                {
                    if (IsObservacionPattern(_pdfLines[i]))
                    {
                        i+=3;
                        //for (; !_pdfLines[i].Contains("Sunúmerodeproveedorennuestraempresaes"); i++)
                        //{
                        //    firstCentroCosto += _pdfLines[i]
                        //        .Replace("RM - Santiago ", "")
                        //        .Replace("RM- Santiago ", "")
                        //        .Replace("RM-Santiago ", "")
                        //        .Replace("Teléfono ", "")
                        //        .Replace("6808080 ", "")+", ";
                        //}

                        for (; !_pdfLines[i].Contains("los días LUNES,"); i++)
                        {
                            firstCentroCosto += _pdfLines[i]
                                .Replace("RM - Santiago ", "")
                                .Replace("RM- Santiago ", "")
                                .Replace("RM-Santiago ", "")
                                .Replace("Teléfono ", "")
                                .Replace("6808080 ", "")
                                .Replace("Sunúmerodeproveedorennuestraempresaes: ","")
                                .Replace("301315 Moneda: CLP","")
                                .Replace("Observaciones: Toma Muestra: ","")
                                .Replace("-Facturar RUT indicado Cabecera Documento","")
                                .Replace("-Facturas Sólo se recibirán en Av. Lib Bernardo O'Higgins 654 ,Piso 2, Santiago", "")
                                + ", ";
                        }
                        //OrdenCompra.Observaciones += firstCentroCosto;
                        //firstCentroCosto = secondCentroCosto;
                        _readObs = true;
                    }
                }
                if (_readObs && !_readDespacho && OrdenCompra.Rut.Equals("96845430"))
                {
                    if (_pdfLines[i].Contains("Observaciones:"))
                    {
                        var despacho1 = _pdfLines[i]
                            .Replace("Observaciones:", "")
                            .Replace("DESPACHAR A", "")
                            .DeleteContoniousWhiteSpace();
                        var despacho2 = _pdfLines[++i]
                            .Replace("-Facturar RUT indicado Cabecera Documento", "")
                            .DeleteContoniousWhiteSpace();
                        secondCentroCosto = despacho2.Equals("")
                            ? despacho1
                            : $"{despacho1}, {despacho2}";
                        _readDespacho = true;
                    }
                    
                }
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
            //Console.WriteLine($"F: {firstCentroCosto}, S: {secondCentroCosto}");
            OrdenCompra.CentroCosto = secondCentroCosto.Equals("")
                ? firstCentroCosto
                : secondCentroCosto;
            OrdenCompra.CentroCosto = OrdenCompra.CentroCosto.ToUpper()
                .Replace(",", "")
                .Replace("´", "")
                .Replace("'", "");
            switch (OrdenCompra.Rut)
            {
                case "96986050":
                    if (OrdenCompra.CentroCosto.Contains("LLANO")
                        && OrdenCompra.CentroCosto.Contains("SUBERCASEUX"))
                    {
                        OrdenCompra.CentroCosto = "EL LLANO SUBERCASEUX";
                    }else if (OrdenCompra.CentroCosto.Contains("LIBERTADOR")
                        && OrdenCompra.CentroCosto.Contains("BERNARDO")
                        && OrdenCompra.CentroCosto.Contains("OHIGGIN")) //
                    {
                        OrdenCompra.CentroCosto = "LIBERTADOR BERNARDO OHIGGINS 5 PISO";
                    }
                    break;
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
                //var x = Regex.Match(aux, @"^\d{1,}\s\d{2}.\d{2}.\d{4}\s\d{8}\s").Success;
                //Es una linea de Items 
                var optItem = GetFormatItemsPattern(aux);
                //Console.WriteLine($"AUX: {aux}, OPITEM: {optItem}, x: {x}");
                switch (optItem)
                {
                    case 0:
                        var test0 = aux.Split(' ');
                        var item0 = new Item
                        {
                            Sku = test0[2],
                            Cantidad = test0[test0.Length - 4].Split(',')[0].Replace(".", ""),
                            Precio = test0[test0.Length - 2].Split(',')[0].Replace(".","")
                        };
                        items.Add(item0);
                        break;
                }
            }
            //SumarIguales(items);
            return items;
        }

        private string GetSku(string[] test1)
        {
            var ret = "W102030";
            var skuDefaultPosition = test1[5].Replace("#", "");
            if (Regex.Match(skuDefaultPosition, @"[a-zA-Z]{1,2}\d{5,6}").Success)
                ret = skuDefaultPosition;
            else
            {
                var str = test1.ArrayToString(0, test1.Length);
                if (Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}").Success)
                {
                    var index = Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}").Index;
                    var length = Regex.Match(str, @"\s[a-zA-Z]{1}\d{6}").Length;
                    ret = str.Substring(index, length).Trim();
                }
                else if (Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}").Success)
                {
                    var index = Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}").Index;
                    var length = Regex.Match(str, @"\s[a-zA-Z]{2}\d{5}").Length;
                    ret = str.Substring(index, length).Trim();
                }
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
            var aux = str.Split(':');
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
            var split = str.Split(' ');
            return split[2];
        }
        
        private static string GetRut(string str1, string str2)
        {
            var rut = "";
            //96986050-3
            if (str2.Length == 10 && Regex.Match(str2, @"\d{8}-\d{1}").Success)
            {
                rut = str2;
            }
            else if(Regex.Match(str1, @"\d{8}-\d{1}").Success)
            {
                rut = str1.Split(' ')[0];
            }
            return rut;
        }

        private int GetFormatItemsPattern(string str)
        {
            var ret = -1;
            //str = str.DeleteDotComa();
            foreach (var it in _itemsPatterns.Where(it => Regex.Match(str, @it.Value).Success))
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