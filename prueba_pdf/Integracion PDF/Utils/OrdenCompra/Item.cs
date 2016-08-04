using System.Runtime.Serialization;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra
{
    public class Item
    {
        public Item()
        {
            Sku = "";
            Descripcion = "";
            Cantidad = "";
            Precio = "";
            TipoPareoProducto = TipoPareoProducto.SinPareo;
        }

        public TipoPareoProducto TipoPareoProducto { get; set; }

        public string Sku { get; set; }

        public string Descripcion { get; set; }

        public string Cantidad
        {
            get { return _cantidad.Replace(".", ""); }
            set { _cantidad = value; }
        }
        private string _cantidad;

        public string Precio
        {
            get { return _precio.Replace(".", "").Split(',')[0]; }
            set { _precio = value; }
        }
        
        private string _precio;
             
        public override string ToString()
        {
            return $"Sku: {Sku}, Cantidad: {Cantidad}, Precio: {Precio}, Descripción: {Descripcion}";
        }
    }

    /// <summary>
    /// Tipo de Pareo de Producto
    /// </summary>
    public enum TipoPareoProducto
    {
        SinPareo = 0,
        PareoCodigoCliente = 1,
        PareoDescripcionTelemarketing = 2,
        PareoDescripcionCliente = 3        
    }

    public class ItemCarozzi : Item
    {
        public double PrecioDecimal { get; set; }
        public string SubTotal { get; set; }
        public override string ToString()
        {
            return base.ToString() + $", SubTotal: {SubTotal}";
        }

    }
    
    public class ItemSecuritas : Item
    {
        public string CodigoProyectoSecuritas { get; set; }
        public override string ToString()
        {
            return base.ToString() + $", Codigo Proyecto: {CodigoProyectoSecuritas}";
        }

    }
    
    public class ItemDavila : Item
    {
        public ItemDavila() : base()
        {
            Descripcion = "";

        }
        
        public string Descripcion
        {
            get { return _descripcion.Replace("'", "''"); }
            set { _descripcion = value; }
        }

        private string _descripcion;
        public override string ToString()
        {
            return base.ToString() + $", Descripción: {Descripcion}";
        }
    }
}