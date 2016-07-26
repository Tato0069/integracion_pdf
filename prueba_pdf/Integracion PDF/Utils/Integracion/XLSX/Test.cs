using System;
using System.Data.OleDb;

//using Excel;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX
{
    public static  class Test
    {

        public static void TestingExcel()
        {
            var con =
               @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\Procesar\Integrar\Excel\Flores\Tienda Escuela Militar.xls;" +
               @"Extended Properties='Excel 8.0;HDR=Yes;'";
            using (var connection = new OleDbConnection(con))
            {
                connection.Open();
                var command = new OleDbCommand("select * from [Hoja1$]", connection);
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        for (var i = 0; i < dr.FieldCount; i++)
                        {
                            Console.Write($", i: {i}, VALUE: {dr[i]}");
                        }
                        Console.WriteLine(@"\n");
                        //$"Depth: {dr.Depth} , Field Count : {dr.FieldCount}, Visible Field Count: {dr.VisibleFieldCount}");
                    }
                }
            }
        }
    }

    
}