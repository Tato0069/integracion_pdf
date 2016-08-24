using System;
using System.Linq;
using LecturaMail.Utils.Oracle.DataAccess;
using LecturaMail.Utils.OrdenCompra.Integracion;

namespace LecturaMail.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter
{
    public static class DataAdapterGeneric
    {
       public static OrdenCompraIntegracion TraspasoUltimateIntegracion(this OrdenCompra oc)
        {
            var cencos = oc.CentroCosto.Replace("-", " ");
            switch (oc.TipoPareoCentroCosto)
            {
                case TipoPareoCentroCosto.SinPareo:
                    cencos = oc.CentroCosto;
                    break;
                case TipoPareoCentroCosto.PareoDescripcionMatch:
                    cencos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencosWithMatch(oc.Rut, cencos);
                    break;
                case TipoPareoCentroCosto.PareoDescripcionExacta:
                    cencos = OracleDataAccess.GetCenCosFromRutCliente(oc.Rut, cencos);
                    break;
                case TipoPareoCentroCosto.PareoDescripcionLike:
                    cencos = OracleDataAccess.GetCenCosFromRutClienteAndDescCencos(oc.Rut, cencos,true);
                    break;
            }
            var ret = new OrdenCompraIntegracion
            {
                NumPed = OracleDataAccess.GetNumPed(),
                RutCli = int.Parse(oc.Rut),
                OcCliente = oc.NumeroCompra,
                Observaciones = oc.Observaciones,
                CenCos = cencos,
                Direccion = oc.Direccion
            };

            foreach (var it in oc.Items)
            {
                var sku = it.Sku.ToUpper().DeleteSymbol();
                switch (it.TipoPareoProducto)
                {
                    case TipoPareoProducto.SinPareo:
                        sku = it.Sku;
                        break;
                    case TipoPareoProducto.PareoCodigoCliente:
                        sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku,true);
                        break;
                    case TipoPareoProducto.PareoDescripcionTelemarketing:
                        sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription(it.Descripcion);
                        break;
                    case TipoPareoProducto.PareoDescripcionCliente:
                        sku = OracleDataAccess.GetSkuWithMatchClientProductDescription(oc.Rut, it.Descripcion);
                        break;
                    case TipoPareoProducto.PareoSkuClienteDescripcionTelemarketing:
                        sku = OracleDataAccess.GetSkuDimercFromCodCliente(oc.NumeroCompra, oc.Rut, it.Sku,false);
                        if (sku.Equals("W102030"))
                        {
                            sku = OracleDataAccess.GetSkuWithMatcthDimercProductDescription(it.Descripcion);
                            if (!sku.Equals("W102030"))
                            {
                                OracleDataAccess.InsertIntoReCodCli(oc.Rut, sku, it.Sku,it.Descripcion);
                            }
                        }
                        break;
                }
                sku = sku.Replace(".", "");
                if (!OracleDataAccess.ExistProduct(sku))
                {
                    sku = "W102030";
                }
                //if (sku.Equals("R397109"))
                //    sku = "R381114";
                var pConv = OracleDataAccess.GetPrecioConvenio(oc.Rut, ret.CenCos, sku, it.Precio);
                var precio = int.Parse(pConv);
                var multiplo = OracleDataAccess.GetMultiploFromRutClienteCodPro(oc.Rut, sku);
                var dt = new DetalleOrdenCompraIntegracion
                //if (! int.TryParse(it.Cantidad))
                {
                    NumPed = ret.NumPed,
                    Cantidad = int.Parse(it.Cantidad) / multiplo,
                    Precio = sku.Equals("W102030")
                        ? int.Parse(it.Precio)
                        : precio == 0 ?
                         int.Parse(it.Precio)
                         : precio, //int.Parse(it.Precio),
                    SubTotal = sku.Equals("W102030")
                        ? (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : precio == 0 ? 
                        (int.Parse(it.Precio) * int.Parse(it.Cantidad))
                        : (int.Parse(it.Cantidad) * precio) / multiplo,
                    SkuDimerc = sku
                };
                ret.AddDetalleCompra(dt);
            }
            return ret;
        }



    }
}