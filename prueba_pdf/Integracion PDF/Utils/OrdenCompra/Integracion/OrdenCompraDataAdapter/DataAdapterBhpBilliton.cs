namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterBhpBilliton
    {
        public static OrdenCompraIntegracion AdapterBhpBillitonFormatToCompraIntegracion(this Integracion_PDF.Utils.OrdenCompra.OrdenCompra oc)
        {
            //var ret = new OrdenCompraIntegracion
            //{
            //    NumPed = OracleDataAccess.GetNumPed(),
            //    RutCli = int.Parse(oc.Rut),
            //    OcCliente = oc.NumeroCompra,
            //    Observaciones = oc.Observaciones,
            //    CenCos = oc.CentroCosto,
            //    Direccion = oc.Direccion
            //};

            //foreach (var dt in oc.Items.Select(it => new DetalleOrdenCompraIntegracion
            //{
            //    NumPed = ret.NumPed,
            //    Cantidad = int.Parse(it.Cantidad),
            //    Precio = int.Parse(it.Precio),
            //    SubTotal = int.Parse(it.Cantidad) * int.Parse(it.Precio),
            //    SkuDimerc = OracleDataAccess.GetSkuDimercFromCencosud(oc.Rut, it.Sku)
            //}))
            //{
            //    ret.AddDetalleCompra(dt);
            //}
            return oc.AdapterGenericFormatToCompraIntegracion();

        }
    }
}