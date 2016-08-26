using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterUnab
    {
        public static OrdenCompraIntegracion AdapterUnabFormatToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutCliente(oc.NumeroCompra, oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                it.Sku = it.Sku.Equals("") ? "W102030" : it.Sku;
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut,ret.CenCos, it.Sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = it.Sku.Equals("W102030") ? int.Parse(it.Precio) : precio,
                    SubTotal =
                        it.Sku.Equals("W102030")
                            ? int.Parse(it.Cantidad)*int.Parse(it.Precio)
                            : int.Parse(it.Cantidad)*precio,
                    SkuDimerc = it.Sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        public static OrdenCompraIntegracion AdapterClinicaAlemanaFormatToCompraIntegracion(this OrdenCompra oc)
        {
            return oc.AdapterGenericFormatToCompraIntegracion();
        }
    }
}