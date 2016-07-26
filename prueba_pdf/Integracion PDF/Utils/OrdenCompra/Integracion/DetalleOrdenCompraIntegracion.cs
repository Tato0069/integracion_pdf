namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion
{
    public class DetalleOrdenCompraIntegracion
    {
        public int ID { get; set; }
        public string NumPed { get; set; }

        public string SkuDimerc { get; set; }

        public int Cantidad { get; set; }

        public int Precio { get; set; }
        public int SubTotal { get; set; }

        public int CodigoBodega = 1;

        public override string ToString()
        {
            return $"NumPed: {NumPed}, SKU: {SkuDimerc}, Cantidad: {Cantidad}, Precio: {Precio}, SubTotal: {SubTotal}, Bodega: {CodigoBodega}";
        }
    }
}