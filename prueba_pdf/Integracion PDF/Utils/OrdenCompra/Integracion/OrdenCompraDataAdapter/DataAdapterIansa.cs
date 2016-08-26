using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterIansa
    {
        public static OrdenCompraIntegracion AdapterIansaFormatToCompraIntegracion(this OrdenCompra oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencos(oc.NumeroCompra,oc.Rut,
                    oc.CentroCosto.Replace(".", "").ToUpper().DeleteAcent(), true),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra,oc.Rut, it.Sku, true);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030") ? int.Parse(it.Precio) : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }
    }
}