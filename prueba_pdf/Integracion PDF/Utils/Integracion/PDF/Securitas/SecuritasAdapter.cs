using System;
using System.Collections.Generic;
using System.Linq;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Securitas
{
    public static class SecuritasAdapter
    {
        public static IEnumerable<OrdenCompraSecuritas> GetSecuritasAdapterOrdenCompra(
            this OrdenCompraSecuritas oc)
        {
            var ordenes = new List<OrdenCompraSecuritas>();
            string[] lastCodProyecto = {""};
            foreach (
                var it in
                    oc.ItemsSecuritas)
            {
                if (!it.CodigoProyectoSecuritas.Equals(lastCodProyecto[0]))
                {
                    lastCodProyecto[0] = it.CodigoProyectoSecuritas;
                    //Console.WriteLine("COD PROY: "+ it.CodigoProyectoSecuritas);
                    var orden = new OrdenCompraSecuritas
                    {
                        NumeroCompra = oc.NumeroCompra,
                        CentroCosto = it.CodigoProyectoSecuritas,
                        Rut = oc.Rut,
                        Direccion = oc.Direccion,
                        Observaciones = $"Código de Proyecto: {it.CodigoProyectoSecuritas}, {oc.Observaciones}"
                    };
                    ordenes.Add(orden);
                }
            }

            foreach (var ord in ordenes)
            {
                foreach (
                    var it in
                        oc.ItemsSecuritas.Where(it => it.CodigoProyectoSecuritas.Equals(ord.CentroCosto)))
                {
                    ord.AddItemSecuritas(it);
                }
            }
            return ordenes;
        }
    }
}