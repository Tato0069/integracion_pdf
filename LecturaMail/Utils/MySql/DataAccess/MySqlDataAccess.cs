using MySql.Data.MySqlClient;//MySQL
using System;

namespace LecturaMail.Utils.MySql.DataAccess
{
    public static class MySqlDataAccess
    {
        private static MySqlConnection _instance;
        private static MySqlConnection InstanceMySqlEduardo => _instance ??
                                                    (_instance =
                                                        new MySqlConnection(string.Format("server={0};port={1};user id={2}; password={3}; " +
                 "database={4}; pooling=false;" +
                 "Allow Zero Datetime=False;Convert Zero Datetime=True",
                 InternalVariables.GetHostMySql(), InternalVariables.GetPuertoMySql(), InternalVariables.GetUserMySql(), InternalVariables.GetUserMySql(),
                 InternalVariables.GetBaseDatosMySql())));

        public static bool TestConection()
        {
            try
            {
                InstanceMySqlEduardo.Open();//se abre la conexion
                Console.WriteLine("Conectado a la base de datos [{0}]", InternalVariables.GetBaseDatosMySql());
                InstanceMySqlEduardo.Close();//Se cierra la conexion
                Console.WriteLine("La conexion a terminado...");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            return false;
        }


        public static void InsertLecturaXml(string asunto, string cuerpo)
        {
            var query = $"insert into lecturacorreo(asunto, cuerpo) values('{asunto}', '{cuerpo}')";
            try
            {
                InstanceMySqlEduardo.Open();
                var adapter = new MySqlDataAdapter(query, InstanceMySqlEduardo);
                adapter.InsertCommand = new MySqlCommand(query);
                adapter.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;                
                adapter.InsertCommand.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

    }

    //
}
