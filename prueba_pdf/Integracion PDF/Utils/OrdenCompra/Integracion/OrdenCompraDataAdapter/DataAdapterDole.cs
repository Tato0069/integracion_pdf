namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterDole
    {
        public static OrdenCompraIntegracion AdapterDoleFormatToCompraIntegracion(this OrdenCompra oc)
        {
            return oc.AdapterGenericFormatToCompraIntegracion();
        }
    }
}