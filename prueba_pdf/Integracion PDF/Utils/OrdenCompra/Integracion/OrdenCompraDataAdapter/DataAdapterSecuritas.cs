using System;
using System.Collections.Generic;
using System.Linq;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Securitas;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterSecuritas
    {
        public static List<OrdenCompraIntegracion> AdapterSecuritasFormatToCompraIntegracion
            (this OrdenCompraSecuritas oc)
        {
            var ret = new List<OrdenCompraIntegracion>();
            foreach (var o in oc.GetSecuritasAdapterOrdenCompra())
            {
                var aux = new OrdenCompraIntegracion
                {
                    NumPed = OracleDataAccess.GetNumPed(),
                    RutCli = int.Parse(o.Rut),
                    OcCliente = o.NumeroCompra,
                    Observaciones = o.Observaciones,
                    CenCos = OracleDataAccess.GetCenCosFromRutCliente(o.NumeroCompra,o.Rut, o.CentroCosto),
                    Direccion = o.Direccion
                };
                foreach (var dt in o.ItemsSecuritas.Select(it => new DetalleOrdenCompraIntegracion
                {
                    NumPed = aux.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = int.Parse(it.Precio),
                    SubTotal = int.Parse(it.Cantidad)*int.Parse(it.Precio),
                    SkuDimerc = it.Sku
                }))
                {
                    aux.AddDetalleCompra(dt);
                }
                ret.Add(aux);
            }

            return ret;
        }

        public static List<OrdenCompraIntegracion> AdapterSecuritasAustralFormatToCompraIntegracion(
            this OrdenCompraSecuritas oc)
        {
            var ret = new List<OrdenCompraIntegracion>();
            foreach (var o in oc.GetSecuritasAdapterOrdenCompra())
            {
                var aux = new OrdenCompraIntegracion
                {
                    NumPed = OracleDataAccess.GetNumPed(),
                    RutCli = int.Parse(o.Rut),
                    OcCliente = o.NumeroCompra,
                    Observaciones = o.Observaciones,
                    CenCos = OracleDataAccess.GetCenCosFromRutCliente(o.NumeroCompra,o.Rut, o.CentroCosto),
                    Direccion = o.Direccion
                };

                foreach (var it in o.ItemsSecuritas)
                {
                    var precio = int.Parse(it.Precio);
                    if (!it.Sku.Equals("W102030"))
                    {
                        var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, aux.CenCos, it.Sku, it.Precio);
                        precio = int.Parse(pConv);
                    }
                    else
                    {
                        continue;
                    }
                    var dt = new DetalleOrdenCompraIntegracion
                    {
                        NumPed = aux.NumPed,
                        Cantidad = int.Parse(it.Cantidad),
                        Precio = precio, //int.Parse(it.Precio),
                        SubTotal = int.Parse(it.Cantidad)*precio,
                        SkuDimerc = it.Sku
                    };
                    aux.AddDetalleCompra(dt);
                }
                ret.Add(aux);
            }
            return ret;
        }

        public static List<OrdenCompraIntegracion> AdapterSecuritasAustralFormatToCompraIntegracionWithBodega(
            this OrdenCompraSecuritas oc)
        {
            var ret = new List<OrdenCompraIntegracion>();

            foreach (var o in oc.GetSecuritasAdapterOrdenCompra())
            {
                var cencos = OracleDataAccess.GetCenCosFromRutCliente(o.NumeroCompra,o.Rut, o.CentroCosto);
                var antofa = OracleDataAccess.TieneBodegaAntofagasta(o.Rut, cencos);
                if (antofa)
                {
                    var itemsAntofa = new List<DetalleOrdenCompraIntegracion>();
                    var itemsSantiago = new List<DetalleOrdenCompraIntegracion>();
                    foreach (var it in o.ItemsSecuritas)
                    {
                        var stock = OracleDataAccess.GetStockAntofagasta(it.Sku);
                        var antofaItem = (stock > int.Parse(it.Cantidad));
                        var precio = int.Parse(it.Precio);
                        if (!it.Sku.Equals("W102030"))
                        {
                            var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, cencos, it.Sku, it.Precio);
                            precio = int.Parse(pConv);
                        }
                        else
                        {
                            continue;
                        }
                        var dt = new DetalleOrdenCompraIntegracion
                        {
                            Cantidad = int.Parse(it.Cantidad),
                            Precio = precio, //int.Parse(it.Precio),
                            SubTotal = int.Parse(it.Cantidad) * precio,
                            SkuDimerc = it.Sku,
                            CodigoBodega = antofaItem ? 66 : 1
                        };
                        if (antofaItem) itemsAntofa.Add(dt);
                        else itemsSantiago.Add(dt);
                    }
                    if (itemsAntofa.Count > 0)
                    {
                        var num = OracleDataAccess.GetNumPed();
                        var aux = new OrdenCompraIntegracion
                        {
                            NumPed = num,
                            RutCli = int.Parse(o.Rut),
                            OcCliente = o.NumeroCompra,
                            Observaciones = o.Observaciones,
                            CenCos = OracleDataAccess.GetCenCosFromRutCliente(o.NumeroCompra,o.Rut, o.CentroCosto),
                            Direccion = o.Direccion
                        };
                        foreach (var d in itemsAntofa)
                        {
                            d.NumPed = num;
                            aux.AddDetalleCompra(d);
                        }
                        ret.Add(aux);
                    }
                    if (itemsSantiago.Count > 0)
                    {
                        var num = OracleDataAccess.GetNumPed();
                        var aux = new OrdenCompraIntegracion
                        {
                            NumPed = num,
                            RutCli = int.Parse(o.Rut),
                            OcCliente = o.NumeroCompra,
                            Observaciones = o.Observaciones,
                            CenCos = OracleDataAccess.GetCenCosFromRutCliente(o.NumeroCompra, o.Rut, o.CentroCosto),
                            Direccion = o.Direccion
                        };
                        foreach (var d in itemsSantiago)
                        {
                            d.NumPed = num;
                            aux.AddDetalleCompra(d);
                        }
                        ret.Add(aux);
                    }
                }
                else
                {
                    var aux = new OrdenCompraIntegracion
                    {
                        NumPed = OracleDataAccess.GetNumPed(),
                        RutCli = int.Parse(o.Rut),
                        OcCliente = o.NumeroCompra,
                        Observaciones = o.Observaciones,
                        CenCos = OracleDataAccess.GetCenCosFromRutCliente(o.NumeroCompra,o.Rut, o.CentroCosto),
                        Direccion = o.Direccion
                    };
                    foreach (var it in o.ItemsSecuritas)
                    {
                        var precio = int.Parse(it.Precio);
                        if (!it.Sku.Equals("W102030"))
                        {
                            var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, cencos, it.Sku, it.Precio);
                            precio = int.Parse(pConv);
                        }
                        else
                        {
                            continue;
                        }
                        var dt = new DetalleOrdenCompraIntegracion
                        {
                            NumPed = aux.NumPed,
                            Cantidad = int.Parse(it.Cantidad),
                            Precio = precio, //int.Parse(it.Precio),
                            SubTotal = int.Parse(it.Cantidad) * precio,
                            SkuDimerc = it.Sku
                        };
                        aux.AddDetalleCompra(dt);
                    }
                    ret.Add(aux);
                }
            }
            return ret;
        }
    }


}