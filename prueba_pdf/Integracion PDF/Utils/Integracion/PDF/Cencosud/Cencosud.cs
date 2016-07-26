using System.Collections.Generic;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Cencosud
{
    public abstract class Cencosud
    {
        //abstract public OrdenCompra.OrdenCompra GetOrdenCompra(string[] pdfLines);
        abstract protected List<Item> GetItems(string[] pdfLines, int firstIndex);
        abstract protected string GetCentroCosto(string str);
        abstract protected string GetSku(string str);
        abstract protected string GetCantidad(string str);
        abstract protected string GetPrecio(string str);
        abstract protected string GetOrdenCompra(string str);
        abstract protected string GetRut(string str);
        public abstract OrdenCompra.OrdenCompra GetOrdenCompra();
    }
}