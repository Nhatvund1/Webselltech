using Microsoft.VisualBasic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace ECommerce.API.DataAccess
{
    public class MyDatabase
    {
        SqlConnection _connection;
        SqlCommand _command;
        SqlDataAdapter _adapter;
        SqlDataReader _reader;
        private readonly string dbconnection;
        private readonly IConfiguration configuration;
        public MyDatabase(IConfiguration configuration)
        {
            this.configuration = configuration;
            dbconnection = this.configuration["ConnectionStrings:DB"];
            _connection = new SqlConnection(dbconnection);
        }

        public DataTable GetDataTable(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Open();

                _command = new SqlCommand();
                _command.Connection = _connection;
                _command.CommandType = commandType;
                _command.CommandText = sql;
                if (parameters != null)
                {
                    foreach (SqlParameter param in parameters)
                    {
                        _command.Parameters.Add(param);
                    }
                }

                _adapter = new SqlDataAdapter(_command);
                _adapter.Fill(dt);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _connection.Close();
            }
            return dt;
        }


        public int MyExcuteNonQuery(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            int result = -1;
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Open();

                _command = new SqlCommand();
                _command.Connection = _connection;
                _command.CommandType = commandType;
                _command.CommandText = sql;
                _command.CommandTimeout = 60;
                if (parameters != null)
                {
                    foreach (SqlParameter param in parameters)
                    {
                        _command.Parameters.Add(param);
                    }
                }

                result = _command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _connection.Close();
            }
            return result;
        }

        public object MyExcuteScalar(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            object result = null;
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Open();

                _command = new SqlCommand();
                _command.Connection = _connection;
                _command.CommandType = commandType;
                _command.CommandText = sql;
                if (parameters != null)
                {
                    foreach (SqlParameter param in parameters)
                    {
                        _command.Parameters.Add(param);
                    }
                }

                result = _command.ExecuteScalar();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _connection.Close();
            }
            return result;
        }

        public SqlDataReader MyExcuteReader(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlDataReader result = null;
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Open();

                _command = new SqlCommand();
                _command.Connection = _connection;
                _command.CommandType = commandType;
                _command.CommandText = sql;
                if (parameters != null)
                {
                    foreach (SqlParameter param in parameters)
                    {
                        _command.Parameters.Add(param);
                    }
                }

                result = _command.ExecuteReader();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return result;
        }
    }
}
