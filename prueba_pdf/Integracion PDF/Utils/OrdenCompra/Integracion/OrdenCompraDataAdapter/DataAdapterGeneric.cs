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
                var sku = OracleDataAccess.GetSkuDimercFromCencosud(oc.NumeroCompra, oc.Rut, it.Sku);
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
                it.Sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription(it.Descripcion);
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
                var sku = OracleDataAccess.GetSkuDimercFromCencosud(oc.NumeroCompra, oc.Rut, it.Sku);
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
                var sku = OracleDataAccess.GetSkuDimercFromCencosud(oc.NumeroCompra, oc.Rut, it.Sku);
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
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
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