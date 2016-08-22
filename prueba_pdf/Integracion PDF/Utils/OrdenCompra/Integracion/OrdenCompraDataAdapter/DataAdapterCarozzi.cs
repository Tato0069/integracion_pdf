using System;
using System.Collections.Generic;
using System.Linq;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;

namespace IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterCarozzi
    {
        private static readonly Dictionary<string, string> CodCliPriceSku = new Dictionary<string, string>
        {
            {"80199", "2118:D105633;1794:A185101"},
            {"87361", "410:Z410448;534:Z380104;731:Z311104;682:Z320004"},
            {"1305748", "600:Z108085;583:Z107124"}
        };
        public static OrdenCompraIntegracion AdapterCarozziFormatToCompraIntegracion(this OrdenCompraCarozzi oc)
        {
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, oc.CentroCosto),
                Direccion = oc.Direccion
            };

            foreach (var it in oc.ItemsCarozzi)
            {
                var num = 0;
                int.TryParse(it.Sku, out num);
                var rules = "";
                foreach (var codPrice 
                    in CodCliPriceSku
                        .Where(codPrice =>
                            codPrice.Key == num.ToString()))
                {
                    rules = codPrice.Value;
                }
                var skuCarozzi = it.Sku;
                var sku = "";
                if (!rules.Equals(""))
                {
                    if(rules.Contains(";"))
                        foreach (var sk in from price 
                                in CodCliPriceSku[num.ToString()].Split(';')
                                select price.Split(':') into aux
                                let preci = aux[0]
                                let sk = aux[1]
                                where it.Precio.Equals(preci)
                                select sk)
                        {
                            sku = sk;
                            break;
                        }
                    
                }
                if (sku.Equals(""))
                    sku = (int.TryParse(it.Sku, out num))
                        ? OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, num.ToString(),true)
                        : OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku, true);
                sku = sku.ToUpper();
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut,ret.CenCos, sku, it.Precio);

                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodCli(oc.Rut, int.Parse(it.Sku).ToString());
                
                var precio = int.Parse(pConv);

                var subtotal = sku.Equals("W102030")
                    ? int.Parse(it.Cantidad)*int.Parse(it.Precio)
                    : int.Parse(it.Cantidad)*precio;
                subtotal = subtotal/multiplo;

                //Console.WriteLine($"Sku: {sku}, Multiplo: {multiplo}, SubtotalPDF: {it.SubTotal}, SubtotalMultiplo: {subtotal}");
                if (int.Parse(it.SubTotal) < subtotal)
                {
                    if (it.SubTotal.Length < subtotal.ToString().Length)
                        Log.SaveProblemaConversionUnidades(oc.NumeroCompra, oc.Rut, skuCarozzi, sku, it.SubTotal, subtotal);
                }
                var dt = new DetalleOrdenCompraIntegracion
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad)/ multiplo,
                    Precio = sku.Equals("W102030") ? int.Parse(it.Precio) : precio,
                    SubTotal = subtotal,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }
    }
}