using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterFlores
    {
        public static OrdenCompraIntegracion AdapterFloresToCompraIntegracion(this OrdenCompra oc)
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
                var existSku = OracleDataAccess.ExistProduct(it.Sku);
                var precio = int.Parse(it.Precio);
                var sku = it.Sku;
                if (existSku)
                {
                    var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, it.Sku, it.Precio);
                    precio = int.Parse(pConv);
                }
                else
                {
                    sku = "W102030";
                }
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad),
                    Precio = precio, //int.Parse(it.Precio),
                    SubTotal = int.Parse(it.Cantidad) * precio,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }
    }
}