using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;
using System;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterClinicaDavila
    {
        public static OrdenCompraIntegracion AdapterClinicaDavilaFormatToCompraIntegracion(this OrdenCompraClinicaDavila oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.CentroCosto,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencos(oc.NumeroCompra, oc.Rut, oc.CentroCosto.DeleteAcent().Trim(), true),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.ItemsClinicaDavila)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku, true);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }

        public static OrdenCompraIntegracion AdapterClinicaDavilaFormatToCompraIntegracionWithMatchCencos(this OrdenCompraClinicaDavila oc)
        {
            //Console.WriteLine($"CC: {oc.CentroCosto}, ocHX: {oc.CentroCosto.ConvertStringToHex()}\n");
            //oc.CentroCosto = oc.CentroCosto.ConvertStringToHex().Replace("c2", "").ConvertHexToString();
            if(oc.CentroCosto.Contains("RECUPERACION")
                && oc.CentroCosto.Contains("SECUNDARIA")
                && oc.CentroCosto.Contains("4")
                && oc.CentroCosto.Contains("PISO")
                && oc.CentroCosto.Contains("EDIF"))
            {
                oc.CentroCosto = "RECUPERACION SECUNDARIA 4 PISO EDIF G";
            }else if (oc.CentroCosto.Contains("LAB")
                && oc.CentroCosto.Contains("CENTRAL")
                && oc.CentroCosto.Contains("CITROMETRIA")
                && oc.CentroCosto.Contains("FLUJO")
                && oc.CentroCosto.Contains("DE"))
            {
                oc.CentroCosto = "LAB CENTRAL - CITROMETRIA DE FLUJO";
            }

            //LAB CENTRAL - CITROMETRIA DE FLUJO
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.CentroCosto,
                CenCos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencos(oc.NumeroCompra,oc.Rut, oc.CentroCosto.DeleteAcent().Trim(),true),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.ItemsClinicaDavila)
            {
                var sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku, true);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = sku.Equals("W102030") ? int.Parse(it.Cantidad)
                    : int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad)) / multiplo
                        : (int.Parse(it.Cantidad) * precio) / multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }
    }
}