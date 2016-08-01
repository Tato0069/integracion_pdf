using System.Collections.Generic;
using System.Linq;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterClinicaDavila
    {
        public static OrdenCompraIntegracion AdapterClinicaDavilaFormatToCompraIntegracion(this OrdenCompraClinicaDavila oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.CentroCosto,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencos(oc.Rut, oc.CentroCosto, true),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.ItemsClinicaDavila)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCencosud(oc.NumeroCompra, oc.Rut, it.Sku);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = precio,
                    SubTotal = int.Parse(it.Cantidad) * precio,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        public static OrdenCompraIntegracion AdapterClinicaDavilaFormatToCompraIntegracionWithMatchCencos(this OrdenCompraClinicaDavila oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.CentroCosto,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencosWithMatch(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.ItemsClinicaDavila)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCencosud(oc.NumeroCompra, oc.Rut, it.Sku);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = precio,
                    SubTotal = int.Parse(it.Cantidad) * precio,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }
    }
}