namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterEsach
    {
        public static OrdenCompraIntegracion AdapterEsachFormatToCompraIntegracion(this Integracion_PDF.Utils.OrdenCompra.OrdenCompra oc)
        {
            return oc.AdapterGenericFormatToCompraIntegracion();
        }
    }
}