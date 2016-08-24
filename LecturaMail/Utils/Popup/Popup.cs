using System;
using System.Collections.Generic;
using System.Linq;

namespace LecturaMail.Utils.Popup
{
    public class Popup
    {
        public string RutUsuario { get; set; }

        public List<DetallePopup> Detalles { get; set; }

        public Popup()
        {
            Detalles = new List<DetallePopup>();
        }

        public void AddDetallePopup(DetallePopup det)
        {
            var exist = false;
            //Console.WriteLine($"Detalle a AGREGAR:: {det.ToString()}");
            foreach (var d in Detalles.Where(d => d.RutCli.Equals(det.RutCli)))
            {
                d.CantidadOrdenes++;
                exist = true;
            }
            //Console.WriteLine($"EXISTE: {exist}");
            if (!exist)
            {
                //Console.WriteLine($"Agregado: {det.ToString()}");
                Detalles.Add(det);

            }
        }

        public string DetalleToString
        {
            get
            {
                var ret = "";
                var count = 1;
                var length = Detalles.Count;
                foreach (var det in Detalles)
                {
                    if (count < length)
                        ret += det.CantidadOrdenes > 1
                            ? $"{det.CantidadOrdenes} Ordenes Procesadas de: {det.Razon}\n"
                            : $"{det.CantidadOrdenes} Orden Procesada de: {det.Razon}\n";
                    else
                        ret += det.CantidadOrdenes > 1
                            ? $"{det.CantidadOrdenes} Ordenes Procesadas de: {det.Razon}"
                            : $"{det.CantidadOrdenes} Orden Procesada de: {det.Razon}";
                    count++;
                }
                return ret;
            }
        }

        public override string ToString()
        {
            return
                $"Rut Usuario: {RutUsuario}\n" +
                $"{Detalles.Aggregate("", (current, de) => current + $"{de.ToString()}\n")}";
        }
    }

    public class DetallePopup
    {
        public string RutCli { get; set; }

        public string Razon { get; set; }

        public int CantidadOrdenes { get; set; }

        public override string ToString()
        {
            return $"Rut Cliente: {RutCli}, Razon: {Razon}, Cantidad: {CantidadOrdenes}";
        }
    }
}