using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX
{
    public class Excel
    {

        public static string ExtractTextInOneLine(string excelPath)
        {
            var con =
                $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={excelPath};" +
                @"Extended Properties='Excel 8.0;HDR=Yes;'";
            using (var connection = new OleDbConnection(con))
            {
                connection.Open();
                var command = new OleDbCommand("select * from [Hoja1$]", connection);
                using (var dr = command.ExecuteReader())
                {
                    var x = new List<string>();
                    while (dr.Read())
                    {
                        var ret = "";
                        for (var i = 0; i < dr.FieldCount; i++)
                        {
                            ret += $"{dr[i]} ";
                        }
                        x.Add(ret);
                        //$"Depth: {dr.Depth} , Field Count : {dr.FieldCount}, Visible Field Count: {dr.VisibleFieldCount}");
                    }
                    var retu = x.Aggregate("", (current, y) => current + y);
                    return retu;
                }
            }
        }
    }
}