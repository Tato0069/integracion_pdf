using System.Collections.Generic;
using System.Linq;

namespace LecturaMail.Utils.OrdenCompra.Integracion
{
    public class OrdenCompraIntegracion
    {
        public OrdenCompraIntegracion()
        {
            DetallesCompra = new List<DetalleOrdenCompraIntegracion>();
        }
        public string NumPed { get; set; }

        public int RutCli { get; set; }

        public string CenCos { get; set; }

        public string Direccion { get; set; }

        public string OcCliente { get; set; }

        public string Observaciones { get; set; }

        public List<DetalleOrdenCompraIntegracion> DetallesCompra { get; set; }

        public string Razon { get; set; }

        public string RutUsuario { get; set; }

        public string EmailEjecutivo { get; set; }

        public void AddDetalleCompra(DetalleOrdenCompraIntegracion det)
        {
            DetallesCompra.Add(det);
        }

        public override string ToString()
        {
            var cont = 1;
            var items = DetallesCompra.Aggregate("", (current, item) => current + $"{cont++}.-{item}\n");
            return $"NumPed: {NumPed},Rut: { RutCli}, N° Compra: { OcCliente}, Centro Costo: { CenCos}, Observaciones: { Observaciones}, Dirección: { Direccion}\nItems:\n{ items}";
        }
    }
}