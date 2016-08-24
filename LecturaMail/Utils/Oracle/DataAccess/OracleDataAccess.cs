using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using LecturaMail.Utils.OrdenCompra.Integracion;
using LecturaMail.View;

#pragma warning disable 618

namespace LecturaMail.Utils.Oracle.DataAccess

{
    public static class OracleDataAccess
    {

        private static OracleConnection _instance;

        private static OracleConnection InstanceTransferWeb => _instance ??
                                                    (_instance =
                                                        new OracleConnection(
                                                            ConfigurationManager.AppSettings.Get("StringConnection")));
        private static OracleConnection _instance2;

        private static OracleConnection InstanceDmVentas => _instance2 ??
                                                    (_instance2 =
                                                        new OracleConnection(
                                                            ConfigurationManager.AppSettings.Get("StringConnectionVentas")));


        public static void CloseConexion()
        {
            InstanceDmVentas?.Close();
            InstanceTransferWeb?.Close();
        }
        public static bool TestConexion()
        {
            if (TestConexion_()) return true;
            Log.Save("Error", "No es posible Conectarse a la Base de Datos, Análisis Cancelado");
            //IntegracionPdf.Instance.ShowBalloon("Error",
            //    "No es posible Conectarse a la Base de Datos, Análisis Cancelado", BalloonIcon.Info);
            Email.Email.SendEmailFromProcesosXmlDimerc(
                InternalVariables.GetMainEmail(),
                null,
                "Fallo Conexión a Base de Datos",
                "No es posible Conectarse a la Base de Datos, Análisis Cancelado");
            return false;
        }

        private static bool TestConexion_()
        {
            try
            {
                InstanceDmVentas.Open();
                const string sql = "SELECT * FROM re_cctocli WHERE rutcli = 99512120 AND cencos = 844";
                var command = new OracleCommand(sql, InstanceDmVentas);
                InstanceTransferWeb.Open();
                var command2 = new OracleCommand(sql,InstanceTransferWeb);
                command.ExecuteReader();
                command2.ExecuteReader();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            finally
            {
                InstanceDmVentas?.Close();
                InstanceTransferWeb?.Close();
            }
        }

        private static string GetPrecioEspecial(string rutCli,string codPro)
        {
            var ret = "";
            try
            {
                InstanceDmVentas.Open();
                var sql =
                    $"select get_cliente_precio_costo(3, {rutCli},'{codPro}','P') precio_especial from dual";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["precio_especial"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        private static string GetNumCotFromNumRelacion(string rutCli)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql =
                    $"Select numcot from en_conveni natural join (select * from re_emprela where numrel = (select numrel from re_emprela where rutcli = {rutCli})) where fecemi <= sysdate and fecven >= sysdate -1";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["numcot"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        /// <summary>
        /// Retorna el Rut del Vendedor de un Cliente
        /// </summary>
        /// <param name="rutCli">Rut Cliente</param>
        /// <returns>Mail Vendedor</returns>
        public static string GetRutUsuarioFromRutCliente(string rutCli)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"SELECT rutusu FROM ma_usuario WHERE userid = (SELECT get_vendedor(3,{rutCli}) FROM dual)";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["rutusu"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        /// <summary>
        /// Retorna el mail del Vendedor de un Cliente
        /// </summary>
        /// <param name="rutCli">Rut Cliente</param>
        /// <returns>Mail Vendedor</returns>
        public static string GetEmailFromRutCliente(string rutCli)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"SELECT mail01 FROM ma_usuario WHERE userid = (SELECT get_vendedor(3,{rutCli}) FROM dual)";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["mail01"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }


        /// <summary>
        /// Función para validar si existe un Producto
        /// </summary>
        /// <param name="sku">Sku</param>
        /// <returns>Existe?</returns>
        public static bool ExistProduct(string sku)
        {
            var ret = "";
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT COUNT(CODPRO) EXIST FROM MA_PRODUCT WHERE CODPRO = '{sku}'";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["EXIST"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret.Equals("1");
        }

        public static bool ExistProductReCodCli(string rutcli, string sku)
        {
            var ret = "";
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT COUNT(CODPRO) EXIST FROM RE_CODCLI WHERE RUTCLI = {rutcli} AND CODPRO = '{sku}'";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["EXIST"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret.Equals("1");
        }


        public static string GetRazonSocial(string rutCli)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"SELECT getrazonsocial(3,{rutCli}) RAZONSOCIAL FROM dual";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["RAZONSOCIAL"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        /// <summary>
        /// Retorna el número del convenio vigente de un cliente
        /// </summary>
        /// <param name="rutCli">Rut de Cliente</param>
        /// <returns>Número de Convenio</returns>
        private static string GetNumCot(string rutCli)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql =
                    $"Select Numcot from en_conveni where rutcli = {rutCli} " +
                    "and fecemi <= sysdate and fecven >= sysdate -1";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["Numcot"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        private static List<object[]> GetRsCabeceraTest()
        {
            var ret = new List<object[]>();
            try
            {
                InstanceTransferWeb.Open();
                const string sql = "SELECT A.*, B.*, GETRAZONSOCIAL (3, RUTCLI) RAZON, GETVENDEDORCLIENTE (3, RUTCLI, CENCOS) VENDEDOR, TO_NUMBER(DECODE(GETPEDSTO(SKU_DIMERC), 'S', 1, DECODE(GETPEDSTO(SKU_DIMERC), 'P', 2, 3))" +
                                   " || GET_PRODUCTO_TIPO(SKU_DIMERC) || DECODE(GETCLIFACLIN(RUTCLI), 'S', GETLINEACLIENTE(RUTCLI, SKU_DIMERC), 1)) TIPROD" +
                                   ", (SELECT DESVAL2 FROM DE_DOMINIO WHERE DESVAL = B.SKU_DIMERC AND CODDOM = 904) RELACIONADO FROM TF_COMPRA_INTEG_PDF A, TF_DETALLE_COMPRA_INTEG_PDF B WHERE A.NUMPED = B.NUMPED" +
                                   " AND A.oc_cliente = 'CHL01-0000112140' " +
                                   " AND A.ESTADO = 1 ORDER BY A.NUMPED, TIPROD ";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                while (data.Read())
                {
                    var ob = new object[data.FieldCount];
                    data.GetOracleValues(ob);
                    ret.Add(ob);
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        private static List<object[]> GetRsCabecera()
        {
            var ret = new List<object[]>();
            try
            {
                InstanceTransferWeb.Open();
                const string sql = "SELECT A.*, B.*, GETRAZONSOCIAL (3, RUTCLI) RAZON, GETVENDEDORCLIENTE (3, RUTCLI, CENCOS) VENDEDOR, TO_NUMBER(DECODE(GETPEDSTO(SKU_DIMERC), 'S', 1, DECODE(GETPEDSTO(SKU_DIMERC), 'P', 2, 3))" +
                                   " || GET_PRODUCTO_TIPO(SKU_DIMERC) || DECODE(GETCLIFACLIN(RUTCLI), 'S', GETLINEACLIENTE(RUTCLI, SKU_DIMERC), 1)) TIPROD" +
                                   ", (SELECT DESVAL2 FROM DE_DOMINIO WHERE DESVAL = B.SKU_DIMERC AND CODDOM = 904) RELACIONADO FROM TF_COMPRA_INTEG_PDF A, TF_DETALLE_COMPRA_INTEG_PDF B WHERE A.NUMPED = B.NUMPED" +
                                   " AND A.ESTADO = 0 ORDER BY A.NUMPED, TIPROD ";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                while(data.Read())
                {
                    var ob = new object[data.FieldCount];
                    data.GetOracleValues(ob);
                    ret.Add(ob);
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        #region UPDATE TELEMARKETING
                public static bool TraspasoTelemarketingPdf()
                {
                    try
                    {
                        var rsCabecera = InternalVariables.IsDebug() ? GetRsCabeceraTest() : GetRsCabecera();
                        if (rsCabecera == null)
                        {
                            return true;
                        }
                        for(var i = 0; i < rsCabecera.Count ; i++)
                        {
                            var rsCab = rsCabecera[i];
                            var finalPedido = 0;
                            var numPed = rsCab[0].ToString();
                            var rutCli = rsCab[1].ToString();
                            var cencos = rsCab[2].ToString();
                            var ocCliente = rsCab[5].ToString();
                            var obs = rsCab[6].ToString();
                            var skuDimerc = rsCab[10].ToString();
                            var cantidad = rsCab[11].ToString();
                            var precio = rsCab[12].ToString();
                            var tipoProd = rsCab[16].ToString();
                            var relacionado = DBNull.Value.Equals(rsCab[17]) ? null : rsCab[17].ToString();
                            var serie = GetNumSerie();
                            var empresa = GetEmpresa(rutCli);
                            var codBod = 66;//GetBodega(rutCli, empresa, cencos, skuDimerc);
                            var claVta = codBod == 1 ? 30 : 41;
                            InsertCabeceraTelemarketing(serie, ocCliente,
                                rutCli, cencos, numPed, obs, codBod.ToString(), empresa, claVta.ToString());
                            //InsertRelacionCentroCosto(rutCli, cencos);
                            Console.WriteLine($"COUNT: {rsCabecera.Count}");

                            while (finalPedido == 0)
                            {
                                InsertDetalleTelemarketing(serie, skuDimerc, cantidad,
                                    precio, empresa, relacionado);
                                if (i + 1 < rsCabecera.Count)
                                {
                                    rsCab = rsCabecera[++i];
                                    Console.WriteLine($"{i}-{rsCabecera.Count}");
                                    var numPedAux = rsCab[0].ToString();
                                    rutCli = rsCab[1].ToString();
                                    skuDimerc = rsCab[10].ToString();
                                    cantidad = rsCab[11].ToString();
                                    precio = rsCab[12].ToString();
                                    var tipoProdAux = rsCab[16].ToString();
                                    relacionado = rsCab[17].ToString();
                                    empresa = GetEmpresa(rutCli);
                                    if (!(numPedAux.Equals(numPed) &&
                                        tipoProdAux.Equals(tipoProd)))
                                    {
                                        finalPedido = 1;
                                        //numPed = numPedAux;
                                        //tipoProd = tipoProdAux;
                                    }
                                    //else finalPedido = 1;
                                }else finalPedido = 1;
                            }
                            UpdateEstadoCompraIntegracion(numPed);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        //Instance?.Close();
                    }
                    return true;
                }

                private static void UpdateEstadoCompraIntegracion(object numPed)
                {
                    if (InternalVariables.IsDebug()) return;
                    using (var command = new OracleCommand())
                    {
                        var sql = $"Update TF_COMPRA_INTEG_PDF set Estado = 1 Where NumPed = {numPed}";

                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        try
                        {
                            InstanceTransferWeb.Open();
                            trans = InstanceTransferWeb.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                        }
                    }
                }

                private static bool InsertDetalleTelemarketing(string serie, string skuDimerc, string cantidad
                    , string precio, int empresa, string relacionado)
                {
                    using (var command = new OracleCommand())
                    {
                        var sql = "Insert Into Wd_notavta (numord, codpro, canpro, prelis, codemp) VALUES " +
                                  $"({serie}, '{skuDimerc}', {cantidad}, {precio}, {empresa})";
                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {

                            //if (!InternalVariables.IsDebug())
                            //{
                                InstanceTransferWeb.Open();
                                trans = InstanceTransferWeb.BeginTransaction();
                                command.Transaction = trans;
                                command.ExecuteNonQuery();
                                trans.Commit();
                            //}
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                            if (skuDimerc.Equals("W421291"))
                            {
                                UpdateNotaVta(serie);
                            }
                            if (!relacionado.Equals("Null"))
                            {
                                InsertDetalleTelemarketing1(serie, cantidad, relacionado);
                                InsertDetalleTelemarketing2(serie, cantidad);
                            }
                        }
                    }
                    return true;
                }

                private static bool InsertDetalleTelemarketing1(string serie, string cantidad
                    , string relacionado)
                {
                    using (var command = new OracleCommand())
                    {
                        var sql = $"Insert Into Wd_notavta (numord, codpro, canpro, prelis, CodEmp) VALUES ({serie}, " +
                                       $"'{relacionado.ToUpper()}', {cantidad}, 1, 3)";

                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {
                            InstanceTransferWeb.Open();
                            trans = InstanceTransferWeb.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                        }
                    }
                    return true;
                }

                private static bool InsertDetalleTelemarketing2(string serie, string cantidad)
                {
            
                    using (var command = new OracleCommand())
                    {
                        var sql = $"Insert Into Wd_notavta(numord, codpro, canpro, prelis, CodEmp) VALUES({serie}, " +
                                       $"'PK10000', 1, {int.Parse(cantidad) * -1}, 3)";

                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {
                            InstanceTransferWeb.Open();
                            trans = InstanceTransferWeb.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                        }
                    }
                    return true;
                }

                private static bool UpdateNotaVta(string serie)
                {
                    using (var command = new OracleCommand())
                    {
                        var sql = $"Update We_NotaVta set ClaVta = 43 where NumOrd = {serie}";
                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {
                            InstanceTransferWeb.Open();
                            trans = InstanceTransferWeb.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            return true;
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                        }
                    }
                }
                private static bool InsertCabeceraTelemarketing(string serie,string ocCliente,
                    string rutCli, string cencos, string numPed, string obs, string codBod,
                    int empresa, string claVta)
                {
                    using (var command = new OracleCommand())
                    {
                        var sql =
                            "Insert Into We_notavta (numord, fecord, facnom, rutcli, cencos, facdir, " +
                            "ordweb, tipweb, observ, descli, codbod, codemp, clavta) VALUES " +
                            $"({serie}, trunc(sysdate), '{ocCliente}', {rutCli}, {cencos}, '0', " +
                            $"{numPed}, 'A', '{obs}', 0, {codBod}, {empresa}, {claVta})";
                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {
                            InstanceTransferWeb.Open();
                            trans = InstanceTransferWeb.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                        }
                    }
                    return true;
                }

                public static bool InsertPopupTelemarketing(Popup.Popup pop)
                {
                    using (var command = new OracleCommand())
                    {
                        var sql = $"INSERT INTO TM_USERMSG(RUTUSU, SNDMSG,CENCOS,TIPMSG,FECHA_CREAC,COLOR) VALUES ({pop.RutUsuario},'{pop.DetalleToString}',NULL,3,SYSDATE,null)";
                        OracleTransaction trans = null;
                        command.Connection = InstanceDmVentas;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {
                            InstanceDmVentas.Open();
                            trans = InstanceDmVentas.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            trans.Commit();
                            return true;
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceDmVentas?.Close();
                        }
                    }
                }


                private static bool InsertRelacionCentroCosto(string rutCli, string cencos)
                {
                    using (var command = new OracleCommand())
                    {
                        var existCencos = ExistCenCosFromRutCliente(rutCli, cencos);
                        if (existCencos) return false;
                        var sql = $"Insert Into Re_cctocli (Rutcli, cencos, ccosto) VALUES ({rutCli}, {cencos}, '{cencos}')";
                        OracleTransaction trans = null;
                        command.Connection = InstanceTransferWeb;
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;
                        Console.WriteLine(sql);
                        if (InternalVariables.IsDebug()) return true;
                        try
                        {
                            InstanceTransferWeb.Open();
                            trans = InstanceTransferWeb.BeginTransaction();
                            command.Transaction = trans;
                            command.ExecuteNonQuery();
                            return true;
                        }
                        catch (SqlException)
                        {
                            trans?.Rollback();
                            return false;
                        }
                        finally
                        {
                            InstanceTransferWeb?.Close();
                        }
                    }
                }

                private static int GetEmpresa(string rut)
                {
                    var ret = 0;
                    try
                    {
                        InstanceTransferWeb.Open();
                        var sql = $"Select nvl(codemp, 3) Empresa from tf_cliente_empresa where rutcli = {rut}";
                        var command = new OracleCommand(sql, InstanceTransferWeb);
                        var data = command.ExecuteReader();
                        ret = !data.Read() ? 3 : int.Parse(data["Empresa"].ToString());
                        data.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        ret = 3;
                    }
                    finally
                    {
                        InstanceTransferWeb?.Close();
                    }
                    return ret;
                }
                private static int GetEjecutivo(int empresa,string rutCli, string cencos)
                {
                    var ret = 0;
                    try
                    {
                        InstanceTransferWeb.Open();
                        var sql = $"Select GETVENDEDORCLIENTE({empresa},{rutCli}, {cencos}) vendedor from dual";
                        var command = new OracleCommand(sql, InstanceTransferWeb);
                        var data = command.ExecuteReader();
                        ret = !data.Read() ? 0 : int.Parse(data["Vendedor"].ToString());
                        data.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        ret = 0;
                    }
                    finally
                    {
                        InstanceTransferWeb?.Close();
                    }
                    return ret;
                }

                #endregion


        private static string GetNumRelacion(string rutCli)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();

                var sql = $"select numrel from re_emprela where rutcli = {rutCli}";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["numrel"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        private static string GetPrecioConvenio(string numCot, string codPro)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"select Precio from de_conveni where numcot = {numCot} and codpro = '{codPro}'";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["Precio"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        public static bool TieneBodegaAntofagasta(string rutcli, string cencos)
        {
            var ret = 0;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT COUNT(RUTCLI) COU FROM RE_CTOCLI_BOD WHERE RUTCLI = {rutcli} AND CENCOS = {cencos}";
                Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    Console.WriteLine(data["COU"]+" - COUNT");
                    ret = int.Parse(data["COU"].ToString());
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret > 0;
        }

        public static int GetStockAntofagasta(string codPro)
        {
            var ret = 0;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT GETSTOCKBODEGA(3,66,'{codPro}') STOCK FROM DUAL";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = int.Parse(data["STOCK"].ToString());
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }
        
        public static string GetPrecioProducto(string rutCli, string cencos, string codpro, string codemp)
        {
            var ret = "0";
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"SELECT GET_PRECIOPROD({rutCli},{cencos},'{codpro}',{codemp}) PRECIO FROM DUAL";

                //Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["PRECIO"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        public static string GetPrecioProductoTest(string rutCli, string cencos, string codpro, string codemp)
        {
            var ret = "0";
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"SELECT GET_PRECIOPROD_BK({rutCli},{cencos},'{codpro}',{codemp}) PRECIO FROM DUAL";

                //Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["PRECIO"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }
        
        public static string GetPrecioConvenio(string rutCli, string cencos, string codPro, string precio)
        {
            var ret =  GetPrecioProductoTest(rutCli, cencos, codPro, "3");
            //var ret2 = GetPrecioProducto(rutCli, cencos, codPro, "3");
            //Console.WriteLine($"ret: {ret}, ret2: {ret2}");
            //return ret.Equals("") ? ret2 : ret;
            return ret;
            //var precioConvenio = "";
            //try
            //{
            //    var numCot = GetNumCot(rutCli);
            //    if (!numCot.Equals(""))
            //    {
            //        var precioCon = GetPrecioConvenio(numCot, codPro);
            //        precioConvenio = !precioCon.Equals("") ? precioCon : GetPrecioLista(rutCli, codPro, precio);
            //    }
            //    else
            //    {
            //        var numCotRel = GetNumCotFromNumRelacion(rutCli);
            //        if (!numCotRel.Equals(""))
            //        {
            //            var precioCon = GetPrecioConvenio(numCotRel, codPro);
            //            if (!precioCon.Equals(""))
            //            {
            //                precioConvenio = precioCon;
            //            }
            //            else
            //            {
            //                precioConvenio = GetPrecioLista(rutCli, codPro, precio);
            //            }
            //        }
            //        else precioConvenio = GetPrecioLista(rutCli, codPro, precio);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            //finally
            //{
            //    InstanceTransferWeb?.Close();
            //}
            //return precioConvenio.Trim().Equals("") ? precio : precioConvenio;
        }

        private static string GetPrecioLista(string rutCli, string codPro, string precio)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql =
                    "SELECT a.codpro, a.despro, a.codlin, getlinea(a.codpro) linea, b.codclasifica categoria, c.codcnl, d.precio " +
                    $"FROM ma_product a, re_claprod b, re_cliente_lista_linea c, re_canprod d WHERE a.codpro = '{codPro}' " +
                    $"and c.rutcli = {rutCli} AND a.codpro = b.codpro and a.codlin = c.codlin and b.codclasifica = c.codcat " +
                    "and a.codpro = d.codpro and c.codcnl = d.codcnl and c.codemp = 3";

                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                ret = data.Read() ? data["precio"].ToString() : precio;
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }


        /// <summary>
        /// Obtiene un Nuevo Numero de Pedido
        /// </summary>
        /// <returns>Número Pedido</returns>
        public static string GetNumPed()
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();

                var sql = "select dm_ventas.SEQ_INTEGRACION.nextval@prod from DUAL";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["nextval"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;

        }

        private static int GetBodega(string rutCli, int codEmp, string cc, string codPro)
        {
            var bodega = GetBodegaAux1(codEmp, rutCli, cc);
            if (bodega != 1)
                bodega = GetBodegaAux2(bodega, codEmp, codPro);
            return bodega;
        }

        private static int GetBodegaAux1(int codEmp, string rutCli, string cc)
        {
            var ret = 1;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT GETRUTCONCESION_NEW({codEmp},{rutCli},{cc}) bodega FROM DUAL";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = int.Parse(data["bodega"].ToString());
                    if (ret == 0) ret = 1;
                }
                else ret = 1;
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas.Close();
            }
            return ret;
        }

        public static string GetNull()
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql = "SELECT (SELECT DESVAL2 FROM DE_DOMINIO WHERE DESVAL = B.SKU_DIMERC AND CODDOM = 904) RELACIONADO FROM TF_COMPRA_INTEG_PDF A, TF_DETALLE_COMPRA_INTEG_PDF B WHERE A.NUMPED = B.NUMPED AND A.ESTADO = 0";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                ret = data.Read() ? data["RELACIONADO"].ToString() : "";
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        private static int GetBodegaAux2(int bodega, int codEmp, string codPro)
        {
            var ret = 1;
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"select *  from re_bodprod_dimerc where codbod = {bodega}  and codemp = {codEmp}  and codpro = '{codPro}'";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                ret = data.Read() ? int.Parse(data["bodega"].ToString()) : 1;
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }


        #region Match Product
        private static int _tamanioMinimoPalabras;
        private const int TamanioMaximoPalabras = 3;
        private static bool ReplaceSymbol;
        private static bool ReplaceNumber;
        private static bool ReplaceSymbolNumber;
        public static string DescPro;

        /// <summary>
        /// Obtiene Producto haciendo Match desde Descripcion Relacionada al Cliente
        /// </summary>
        /// <param name="rutcli">Cliente</param>
        /// <param name="descPro"></param>
        /// <returns></returns>
        public static string GetSkuWithMatchClientProductDescription(string rutcli, string descPro)
        {
            if (descPro == null) return "W102030";
            descPro = descPro.ToUpper().DeleteAcent().DeleteContoniousWhiteSpace();
            var split = descPro.Split(' ');
            var descContainsRow = new List<string>();
            var index = 0;
            var CodProd = "W102030";
            if (_tamanioMinimoPalabras > TamanioMaximoPalabras)
            {
                if (!ReplaceSymbol)
                {
                    _tamanioMinimoPalabras = 0;
                    ReplaceSymbol = true;
                    return GetSkuWithMatchClientProductDescription(rutcli,DescPro.DeleteSymbol());
                }
                if (!ReplaceNumber)
                {
                    _tamanioMinimoPalabras = 0;
                    ReplaceNumber = true;
                    return GetSkuWithMatchClientProductDescription(rutcli, DescPro.DeleteNumber());
                }
                if (!ReplaceSymbolNumber)
                {
                    _tamanioMinimoPalabras = 0;
                    ReplaceSymbolNumber = true;
                    return GetSkuWithMatchClientProductDescription(rutcli, DescPro.DeleteSymbol().DeleteNumber());
                }

                return CodProd;
            }
            foreach (var t in split.Where(t => t.Length > _tamanioMinimoPalabras))
            {
                descContainsRow.Add(t);
                var rows = GetCountProductDescriptionMatchFromClient(rutcli, descContainsRow);
                if (rows == 0)
                {
                    descContainsRow.RemoveAt(index);
                }
                else// if (rows == 1)
                {
                    index++;
                }
                if (rows >= 1)
                {
                    CodProd = MatchProductDescriptionCliente(rutcli, descContainsRow);
                }
            }
            if (CodProd.Equals("W102030"))
            {
                _tamanioMinimoPalabras++;
                return GetSkuWithMatchClientProductDescription(rutcli, descPro);
            }
            Console.WriteLine($"Product Match: {CodProd}");
            return CodProd;
        }

        


        public static string GetSkuWithMatcthDimercProductDescription(string descPro, bool first)
        {
            descPro = descPro.ToUpper().DeleteAcent().DeleteContoniousWhiteSpace();
            if (first) {
                var sku = GetProductExactDescriptionMaestra(descPro);
                if(!sku.Equals("W102030")) return sku;
            }
            var split = descPro.Split(' ');
            var descContainsRow = new List<string>();
            var index = 0;
            var CodProd = "W102030";
            if (_tamanioMinimoPalabras > TamanioMaximoPalabras)
            {
                if (!ReplaceSymbol)
                {
                    _tamanioMinimoPalabras = 0;
                       ReplaceSymbol = true;
                    return GetSkuWithMatcthDimercProductDescription(DescPro.DeleteSymbol(),false);
                }
                if (!ReplaceNumber)
                {
                    _tamanioMinimoPalabras = 0;
                    ReplaceNumber = true;
                    return GetSkuWithMatcthDimercProductDescription(DescPro.DeleteNumber(),false);
                }
                if (!ReplaceSymbolNumber)
                {
                    _tamanioMinimoPalabras = 0;
                    ReplaceSymbolNumber = true;
                    return GetSkuWithMatcthDimercProductDescription(DescPro.DeleteSymbol().DeleteNumber(),false);
                }

                return CodProd;
            }
            foreach (var t in split.Where(t => t.Length > _tamanioMinimoPalabras))
            {
                descContainsRow.Add(t);
                var rows = GetCountDescProdMatch(descContainsRow);
                if (rows == 0)
                {
                    descContainsRow.RemoveAt(index);
                }else// if (rows == 1)
                {
                    index++;
                }
                if (rows >= 1)
                {
                    CodProd = MatchProductDescriptionMaestra(descContainsRow);
                }
            }
            if (CodProd.Equals("W102030"))
            {
                _tamanioMinimoPalabras++;
                return GetSkuWithMatcthDimercProductDescription(descPro,false);
            }
            Console.WriteLine($"Product Match: {CodProd}");
            return CodProd;
        }

        public static string MatchProductDescriptionCliente(string rutCliente, IEnumerable<string> desc)
        {
            var likes = "";
            foreach (var str in desc.Where(str => str.Length > _tamanioMinimoPalabras))
            {
                likes += $"and descripcion LIKE '%{str}%'";
            }
            var ret = "";
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT CODPRO FROM re_codcli WHERE rutcli = {rutCliente} {likes}";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                //Console.WriteLine(sql);
                ret = data.Read() ? data["CODPRO"].ToString() : "";
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        public static string GetProductExactDescriptionMaestra(string desc)
        {
            var ret = "W102030";
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT CODPRO FROM MA_PRODUCT WHERE DESPRO = '{desc}'";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                //Console.WriteLine(sql);
                ret = data.Read() ? data["CODPRO"].ToString() : "W102030";
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }


        public static string MatchProductDescriptionMaestra(IEnumerable<string> desc)
        {
            var likes = "";
            foreach (var str in desc.Where(str => str.Length > _tamanioMinimoPalabras))
            {
                if(likes.Length == 0)
                    likes += $"despro LIKE '%{str}%'";
                else
                    likes += $" AND despro LIKE '%{str}%'";
            }
            var ret = "";
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT CODPRO FROM ma_product WHERE {likes}";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                //Console.WriteLine(sql);
                ret = data.Read() ? data["CODPRO"].ToString() : "";
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        public static int GetCountDescCencosMatch(string rutCli, IEnumerable<string> desc)
        {
            var likes = "";
            foreach (var str in desc.Where(str => str.Length > _tamanioMinimoPalabras))
            {
                if (likes.Length == 0)
                    likes += $"ccosto LIKE '%{str}%'";
                else
                    likes += $" AND ccosto LIKE '%{str}%'";
            }
            var ret = 0;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT cencos FROM re_cctocli WHERE RUTCLI = {rutCli} AND {likes}";
                Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                Console.WriteLine(sql);
                while (data.Read())
                    ret++;
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        private static int GetCountDescProdMatch(IEnumerable<string> desc)
        {
            var likes = "";
            foreach (var str in desc.Where(str => str.Length > _tamanioMinimoPalabras))
            {
                if (likes.Length == 0)
                    likes += $"despro LIKE '%{str}%'";
                else
                    likes += $" AND despro LIKE '%{str}%'";
            }
            var ret = 0;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT CODPRO FROM ma_product WHERE {likes}";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                //Console.WriteLine(sql);
                while (data.Read())
                    ret++;
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }


        private static int GetCountProductDescriptionMatchFromClient(string rutCliente,IEnumerable<string> desc)
        {
            var likes = "";
            foreach (var str in desc.Where(str => str.Length > _tamanioMinimoPalabras))
            {
                likes += $" AND descripcion LIKE '%{str}%'";
            }
            var ret = 0;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"SELECT CODPRO FROM re_codcli WHERE rutcli = {rutCliente} {likes}";
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                //Console.WriteLine(sql);
                while (data.Read())
                    ret++;
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        #endregion


        private static string GetNumSerie()
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();
                var sql = "Select dm_ventas.notaweb_seq.nextval as Serie from dual";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["Serie"].ToString();
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }


        /// <summary>
        /// Obtiene el SKU del producto
        /// </summary>
        /// <param name="ocNumber">Número de Orden</param>
        /// <param name="rutCli">Rut del Cliente</param>
        /// <param name="codCli">Codigo interno de producto del Cliente</param>
        /// <returns></returns>
        public static string GetSkuDimercFromCodCliente(string ocNumber, string rutCli, string codCli, bool mailFaltantes)
        {
            var ret = "";
            try
            {
                InstanceTransferWeb.Open();

                var sql =
                    $"select CODPRO from dm_ventas.RE_CODCLI@prod where RUTCLI = {rutCli} and CODCLI = '{codCli}' and ESTADO = 1";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                ret = data.Read() ? data["CODPRO"].ToString() : "W102030";
                //Console.WriteLine($"SQL:{sql}, \n RET: {ret}");
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
                var razon = GetRazonSocial(rutCli);
                if (ret.Equals("W102030"))
                {
                    if(mailFaltantes == true)
                        Log.SaveItemFaltantes(rutCli,$"{razon} - {rutCli}:",$"Orden N°: {ocNumber}, Producto: {codCli}, no posee pareo de SKU con Dimerc.");
                }
            }
            if (ret.Equals(""))
            {
                //throw new Exception($"No existe Codigo para el producto:\n {codCli}, del cliente {rutCli}");
                Console.WriteLine($"No existe Codigo para el producto:\n {codCli}, del cliente {rutCli}");
            }
            return ret;
        }

        public static int GetMultiploFromRutClienteCodCli(string rutCli, string codPro)
        {
            var ret = 1;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"select multiplo from re_codcli where rutcli = {rutCli} and codcli = '{codPro}'";
                //Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                ret = data.Read() ? int.Parse(data["multiplo"].ToString()) : 1;
                data.Close();
                //Console.WriteLine($"Multiplo: {ret}, SQL: {sql}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        public static int GetMultiploFromRutClienteCodPro(string rutCli, string codPro)
        {
            var ret = 1;
            try
            {
                InstanceDmVentas.Open();
                var sql = $"select multiplo from re_codcli where rutcli = {rutCli} and codpro = '{codPro}'";
                //Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceDmVentas);
                var data = command.ExecuteReader();
                ret = data.Read() ? int.Parse(data["multiplo"].ToString()) : 1;
                data.Close();
                //Console.WriteLine($"Multiplo: {ret}, SQL: {sql}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceDmVentas?.Close();
            }
            return ret;
        }

        /// <summary>
        /// Busca Centro de Costo con Descripción Exacta
        /// </summary>
        /// <param name="rutCli"></param>
        /// <param name="ccosto"></param>
        /// <returns></returns>
        public static string GetCenCosFromRutCliente(string rutCli, string ccosto)
        {
            var ret = "";
            var existCencos = true;
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"select CENCOS from RE_CCTOCLI where RUTCLI = {rutCli} and CCOSTO = '{ccosto}'";
                Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["CENCOS"].ToString();
                }
                else
                {
                    ret = "0";
                    existCencos = false;
                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
                if (!existCencos)
                {
                    Log.SaveCentroCostoFaltantes(rutCli, ccosto);
                    //throw new Exception($"No existe CenCos para el CCOSTO:\n {ccosto}, del cliente {rutCli}");
                    Console.WriteLine($"No existe CenCos para el CCOSTO:\n{ccosto}, del cliente {rutCli}");
                }
            }
            return ret;
        }

        private static bool ExistCenCosFromRutCliente(string rutcli, string cencos)
        {
            var ret = false;
            try
            {
                InstanceTransferWeb.Open();
                var sql = $" SELECT * FROM re_cctocli WHERE rutcli = {rutcli} AND cencos = {cencos}";
                //Console.WriteLine(sql);
                var command = new OracleCommand(sql, InstanceTransferWeb);
                var data = command.ExecuteReader();
                ret = data.Read();
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
            }
            return ret;
        }

        public static string GetCenCosFromRutClienteAndDescCencosWithMatch(string rutCli, string ccosto)
        {
            Console.Write("==================================MATCH====================000");
            var ret1 = GetCenCosFromRutClienteAndDescCencos(rutCli, ccosto, false);
            Console.WriteLine($"ret1: {ret1}");
            if (!ret1.Equals("0")) return ret1;
            var split = ccosto.Split(' ');
            var descContainsRow = new List<string>();
            var index = 0;
            var ret = "0";
            foreach (var t in split.Where(t => t.Length > _tamanioMinimoPalabras))
            {
                descContainsRow.Add(t);
                var rows = GetCountDescCencosMatch(rutCli, descContainsRow);
                if (rows == 0)
                    descContainsRow.RemoveAt(index);
                else
                    index++;
                if (rows >= 1)
                {
                    ret = GetCenCosFromRutClienteAndDescCencos(rutCli,
                        descContainsRow.ToArray().ArrayToString(0, descContainsRow.Count-1), true);

                    Console.WriteLine($"ret2: {ret}");
                }
            }
            Console.WriteLine($"ret3: {ret}");
            return ret;
        }

        /// <summary>
        /// Busca Centro de Costo con Simples Coincidencias utilizando toda la Descripcion del 
        /// </summary>
        /// <param name="rutCli">Rut Cliente</param>
        /// <param name="ccosto">Descripción Centro Costo</param>
        /// <param name="sendEmail">Enviar Mail?</param>
        /// <returns></returns>
        public static string GetCenCosFromRutClienteAndDescCencos(string rutCli, string ccosto, bool sendEmail)
        {
            var num = 0;
            if (int.TryParse(ccosto, out num)) return ccosto;
            var existCencos = true;
            var ret = "";
            var aux = ccosto.NormalizeCentroCostoDavila();
            var split = aux.Split(' ');
            var sufix = split.Where(cc => !cc.Equals("")).Aggregate("", (current, cc) => current + $" and ccosto like '%{cc}%'");
            try
            {
                InstanceTransferWeb.Open();
                var sql = $"select CENCOS from RE_CCTOCLI where RUTCLI = {rutCli} {sufix}";
                var command = new OracleCommand(sql, InstanceTransferWeb);
                Console.WriteLine(sql);
                var data = command.ExecuteReader();
                if (data.Read())
                {
                    ret = data["CENCOS"].ToString();
                }
                else
                {
                    ret = "0";
                    existCencos = false;

                }
                data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                InstanceTransferWeb?.Close();
                if (!existCencos && sendEmail)
                {
                    Log.SaveCentroCostoFaltantes(rutCli, ccosto);
                    Console.WriteLine($"No existe CenCos para el CCOSTO:\n{ccosto}," +
                                      $" del cliente {rutCli}");
                }
            }
            return ret;
        }
        public static void InsertIntoReCodCli(string rutcli, string codpro, string codcli, string descripcionCliente)
        {
            var exist = ExistProductReCodCli(rutcli, codpro);
            if (exist) return;
            using (var command = new OracleCommand())
            {
                var sql = $"INSERT INTO RE_CODCLI(RUTCLI,CODPRO,CODCLI,DESCRIPCION,DESCRIPCION_CLIENTE,ESTADO) VALUES({rutcli},'{codpro}','{codcli}',GETDESCRIPCION('{codpro}'),'{descripcionCliente}',1) ";
                OracleTransaction trans = null;
                command.Connection = InstanceDmVentas;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                Console.WriteLine(sql);
                try
                {
                    InstanceDmVentas.Open();
                    trans = InstanceDmVentas.BeginTransaction();
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                    trans.Commit();
                    InstanceDmVentas?.Close();
                   
                }
                catch (SqlException)
                {
                    trans?.Rollback();
                }
            }
        }

        public static bool InsertOrdenCompraIntegración(OrdenCompraIntegracion oc)
        {
            oc.Observaciones = $"OC Cliente: {oc.OcCliente}, {oc.Observaciones}";
            oc.Observaciones = oc.Observaciones.Length >= 200 ? oc.Observaciones.Substring(0, 199) : oc.Observaciones;
            using (var command = new OracleCommand())
            {
                var sql = "INSERT INTO TF_COMPRA_INTEG_PDF" +
                          "(NUMPED,RUTCLI,CENCOS,FECHA,DIRECCION,OC_CLIENTE,OBS,ESTADO) " +
                          $"VALUES({oc.NumPed},{oc.RutCli},{oc.CenCos},SYSDATE,'{oc.Direccion}'" +
                          $",'{oc.OcCliente}','{oc.Observaciones}',0)";
                OracleTransaction trans = null;
                command.Connection = InstanceTransferWeb;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                Console.WriteLine(sql);
                if (InternalVariables.IsDebug())
                {
                    Console.WriteLine("ocAdapterList:\n" + oc);
                    //Console.WriteLine("BODEGA DETALLEI_NTEG" + oc.DetallesCompra.First().CodigoBodea);
                    return true;
                }
                try
                {
                    InstanceTransferWeb.Open();
                    trans = InstanceTransferWeb.BeginTransaction();
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                    trans.Commit();
                    InstanceTransferWeb?.Close();
                    var cont = 0;
                    if (oc.DetallesCompra.Count == 0) return true;
                    foreach (var detC in oc.DetallesCompra)
                    {
                        if (InsertDetalleOrdenCompraIntegración(detC)) cont++;
                        else break;
                    }
                    if (cont == oc.DetallesCompra.Count) return true;
                    foreach (var detC in oc.DetallesCompra)
                        DeleteDetalleOrdenCompraIntegracion(detC);
                    DeleteOrdenCompraIntegración(oc);
                    Log.Save("Error", "No es posible insertar los datos en la Base de Datos");
                    return false;
                }
                catch (SqlException)
                {
                    trans?.Rollback();
                    Log.Save("Error", "No es posible insertar los datos en la Base de Datos");
                    return false;
                }
            }
        }

        private static void DeleteOrdenCompraIntegración(OrdenCompraIntegracion oc)
        {
            using (var command = new OracleCommand())
            {
                OracleTransaction trans = null;
                command.Connection = InstanceTransferWeb;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    $"DELETE FROM TF_COMPRA_INTEG_PDF WHERE NUMPED = {oc.NumPed}";
                if (InternalVariables.IsDebug()) return;
                try
                {
                    InstanceTransferWeb.Open();
                    trans = InstanceTransferWeb.BeginTransaction();
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (SqlException)
                {
                    trans?.Rollback();
                }
                finally
                {
                    InstanceTransferWeb?.Close();
                }
            }
        }

        private static bool InsertDetalleOrdenCompraIntegración(DetalleOrdenCompraIntegracion det)
        {
            using (var command = new OracleCommand())
            {
                //var existSku = ExistProduct(det.SkuDimerc);
                //var sku = existSku ? det.SkuDimerc : "W102030";
                var sql = "INSERT INTO TF_DETALLE_COMPRA_INTEG_PDF" +
                          "(NUMPED,SKU_DIMERC,CANTIDAD,PRECIO,SUBTOTAL,CODBOD) " +
                          $"VALUES ({det.NumPed},'{det.SkuDimerc}',{det.Cantidad},{det.Precio},{det.SubTotal},{det.CodigoBodega})";
                OracleTransaction trans = null;
                command.Connection = InstanceTransferWeb;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                Console.WriteLine(sql);
                if (InternalVariables.IsDebug()) return true;
                try
                {
                    InstanceTransferWeb.Open();
                    trans = InstanceTransferWeb.BeginTransaction();
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (SqlException)
                {
                    trans?.Rollback();
                    return false;
                }
                finally
                {
                    InstanceTransferWeb?.Close();
                }
            }
            return true;
        }

        private static void DeleteDetalleOrdenCompraIntegracion(DetalleOrdenCompraIntegracion det)
        {
            using (var command = new OracleCommand())
            {
                OracleTransaction trans = null;
                command.Connection = InstanceTransferWeb;
                command.CommandType = CommandType.Text;
                command.CommandText =
                    $"DELETE FROM TF_DETALLE_COMPRA_INTEG_PDF WHERE NUMPED = {det.NumPed}";
                if (InternalVariables.IsDebug()) return;
                try
                {
                    InstanceTransferWeb.Open();
                    trans = InstanceTransferWeb.BeginTransaction();
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (SqlException)
                {
                    trans?.Rollback();
                }
                finally
                {
                    InstanceTransferWeb?.Close();
                }
            }
        }

    }
}
 