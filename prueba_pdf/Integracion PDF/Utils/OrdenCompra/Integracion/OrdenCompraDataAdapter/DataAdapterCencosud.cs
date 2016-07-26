namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterCencosud
    {
        public static OrdenCompraIntegracion AdapterCencosudFormatToCompraIntegracion(this OrdenCompra oc)
        {
            return oc.AdapterGenericFormatToCompraIntegracion();
        }
    }
}