using System;
using System.Linq;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterGeneric
    {
        public static OrdenCompraIntegracion AdapterGenericFormatToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku,true);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,

                    //Precio = precio, //int.Parse(it.Precio),
                    //SubTotal = (int.Parse(it.Cantidad) * precio)/multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// Pareo de Centro de Costo, SKU como viene en PDF
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        public static OrdenCompraIntegracion ParearCentroCostoSinSku(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, it.Sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = it.Sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = it.Sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,

                    //Precio = precio, //int.Parse(it.Precio),
                    //SubTotal = (int.Parse(it.Cantidad) * precio)/multiplo,
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// Convierte una Orden de Compra en una Orden de Integración
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        public static OrdenCompraIntegracion AdapterGenericFormatWithSkuToCompraIntegracion(this OrdenCompra oc)
        {
            Console.WriteLine($"CC: {oc.CentroCosto}");
            var auxint = 0;
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos =
                    int.TryParse(oc.CentroCosto, out auxint) ? oc.CentroCosto : OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var existSku = OracleDataAccess.ExistProduct(it.Sku);
                var precio = int.Parse(it.Precio);
                if (existSku)
                {
                    var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                    precio = int.Parse(pConv);
                }else
                {
                    it.Sku = "W102030";
                }              
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = precio, //int.Parse(it.Precio),
                    SubTotal = int.Parse(it.Cantidad) * precio,
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// Transforma a Integración con Descripción de Productos Dimerc
        /// </summary>
        /// <param name="oc">orden de Compra con Descripción</param>
        /// <returns></returns>
        public static OrdenCompraIntegracion AdapterGenericFormatDescripcionitemToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = oc.CentroCosto,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                //if (!OracleDataAccess.ExistProduct(it.Sku))
                //{
                //    it.Sku = "W102030";
                //}
                it.Sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription(it.Descripcion, first: true);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = it.Sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = it.Sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : (int.Parse(it.Cantidad) * precio),
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// Transforma una OC en una Orden de Compra de Integración sin Pareo de SKU ni Centro de Costos
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        public static OrdenCompraIntegracion ToCompraIntegracionSkuCentroCostoDePdf(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = oc.CentroCosto,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                if (!OracleDataAccess.ExistProduct(it.Sku))
                {
                    it.Sku = "W102030";
                }
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = it.Sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = it.Sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : (int.Parse(it.Cantidad) * precio),
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }



        public static OrdenCompraIntegracion AdapterGenericFormatWithSkuAndNumericCencosToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                if (!OracleDataAccess.ExistProduct(it.Sku))
                {
                    it.Sku = "W102030";
                }
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = it.Sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = it.Sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : (int.Parse(it.Cantidad) * precio),
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }


        /// <summary>
        /// Busca el Pareo de SKU y hace Match con Descripción de Centro de Costo
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        public static OrdenCompraIntegracion AdapterGenericFormatWithDescriptionCencosToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencosWithMatch(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku,true);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,

                    //Precio = precio, //int.Parse(it.Precio),
                    //SubTotal = (int.Parse(it.Cantidad) * precio)/multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        public static OrdenCompraIntegracion TraspasoUltimateIntegracion(this OrdenCompra oc)
        {
            var cencos = oc.CentroCosto.Replace("-", " ");
            switch (oc.TipoPareoCentroCosto)
            {
                case TipoPareoCentroCosto.SinPareo:
                    cencos = oc.CentroCosto;
                    break;
                case TipoPareoCentroCosto.PareoDescripcionMatch:
                    cencos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencosWithMatch(oc.Rut, cencos);
                    break;
                case TipoPareoCentroCosto.PareoDescripcionExacta:
                    cencos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, cencos);
                    break;
                case TipoPareoCentroCosto.PareoDescripcionLike:
                    cencos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencos(oc.Rut, cencos,true);
                    break;
            }
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = cencos,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = it.Sku.ToUpper().DeleteSymbol();
                switch (it.TipoPareoProducto)
                {
                    case TipoPareoProducto.SinPareo:
                        sku = it.Sku;
                        break;
                    case TipoPareoProducto.PareoCodigoCliente:
                        sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku,true);
                        break;
                    case TipoPareoProducto.PareoDescripcionTelemarketing:
                        sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription(it.Descripcion, first: true);
                        break;
                    case TipoPareoProducto.PareoDescripcionCliente:
                        sku = OracleDataAccess.GetSkuWithMatchClientProductDescription(oc.Rut, it.Descripcion);
                        break;
                    case TipoPareoProducto.PareoSkuClienteDescripcionTelemarketing:
                        if (!sku.Equals("W102030"))
                        {//CON SKU CLIENTE
                            sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku, mailFaltantes: false);
                        }
                        if (sku.Equals("W102030"))
                        {//CON DESCRIPCION CLIENTE
                            sku = OracleDataAccess.GetSkuWithMatchClientProductDescription(oc.Rut, it.Descripcion);
                        }
                        if (sku.Equals("W102030"))
                        {//NO POSEE PAREO DEFINIDO
                            sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription(it.Descripcion, first: true);
                            if (!sku.Equals("W102030"))
                            {
                                OracleDataAccess.InsertIntoReCodCli(oc.Rut, sku, it.Sku.Equals("W102030") ? "SIN_SKU" : it.Sku, it.Descripcion);
                            }
                        }
                        break;
                }
                sku = sku.Replace(".", "").Replace(" ","");
                if (!OracleDataAccess.ExistProduct(sku))
                {
                    sku = "W102030";
                }
                //if (sku.Equals("R397109"))
                //    sku = "R381114";
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                //if (! int.TryParse(it.Cantidad))
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio == 0 ?
                         int.Parse(it.Precio)
                         : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : precio == 0 ? 
                        (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : (int.Parse(it.Cantidad) * precio) / multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        private static void InsertIntoReCodCli(string rut, string sku1, string sku2)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Deja Centro de Costo como viene en PDF
        /// Hace Pareo de SKU
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        /// Parear solo Sku
        public static OrdenCompraIntegracion ParearSoloSKU(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = oc.CentroCosto,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku,true);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,

                    //Precio = precio, //int.Parse(it.Precio),
                    //SubTotal = (int.Parse(it.Cantidad) * precio)/multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// Parea solo Descripción asociada al Cliente
        /// </summary>
        /// <param name="oc">Orden de Compra</param>
        /// <returns></returns>
        public static OrdenCompraIntegracion ParearSoloDescripcionCliente(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = oc.CentroCosto,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = OracleDataAccess.GetSkuWithMatchClientProductDescription(oc.Rut, it.Descripcion.ReplaceSymbolWhiteSpace());
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,

                    //Precio = precio, //int.Parse(it.Precio),
                    //SubTotal = (int.Parse(it.Cantidad) * precio)/multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// No realiza pareo de Codigos ni de Centro de Costo, 
        /// sólo calcula precio del Cliente
        /// </summary>
        /// <param name="oc">Orden de Compra de PDF</param>
        /// <returns>Orden de Compra de Integración</returns>
        public static OrdenCompraIntegracion TraspasoSinPareo(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = oc.CentroCosto,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, it.Sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = it.Sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = it.Sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,

                    //Precio = precio, //int.Parse(it.Precio),
                    //SubTotal = (int.Parse(it.Cantidad) * precio)/multiplo,
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }



        //ParearSoloCentroDeCostoConDescripcion
        public static OrdenCompraIntegracion AdapterGenericFormatWithSkuAndDescriptionCencosWithMatchToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencosWithMatch(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = it.Sku;
                if (!OracleDataAccess.ExistProduct(it.Sku))
                {
                    sku = "W102030";
                }
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var cantidad = 1;
                if(int.TryParse(it.Cantidad.Equals("00050")|| it.Cantidad.Equals("La") ? "1" : it.Cantidad, out cantidad))
                {
                    cantidad = 1;
                }
                it.Cantidad = cantidad.ToString();
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = sku.Equals("W102030") ? int.Parse(it.Precio) : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030") ? int.Parse(it.Precio) * int.Parse(it.Cantidad) : int.Parse(it.Cantidad) * precio,
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        /// <summary>
        /// Pareo de Centro de Costo
        /// SKU de PDF
        /// </summary>
        /// <param name="oc"></param>
        /// <returns></returns>
        public static OrdenCompraIntegracion AdapterGenericFormatWithSkuToCompraWithStockW102030Integracion(
            this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = it.Sku.Equals("W102030") ? int.Parse(it.Precio) : precio, //int.Parse(it.Precio),
                    SubTotal = it.Sku.Equals("W102030") ? int.Parse(it.Precio) * int.Parse(it.Cantidad) : int.Parse(it.Cantidad) * precio,
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

    }
}