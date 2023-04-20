using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdinoeOkno_program
{
    internal static class OurDatabase
    {
        public const string dBServer = "26.137.232.44";
        public const string dBPort = "5432";
        public const string dBUser = "Danil";
        public const string dBPassword = "1";
        public const string dBDatabase = "EdinoeOkno";
        public const string dBSchema = "dev1";

        public static string CONNECTION_STRING = $"Server={dBServer};Port={dBPort};User id={dBUser};Password={dBPassword};Database={dBDatabase}";

        public static List<string[]> facultyNamesList = new List<string[]>();
        public static List<string[]> requestNamesList = new List<string[]>();
        public static List<string[]> statusNamesList = new List<string[]>();

        public static NpgsqlConnection GetConnection()
        {
            NpgsqlConnection dBconnection = new NpgsqlConnection(OurDatabase.CONNECTION_STRING);
            try
            {
                dBconnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            return dBconnection;
        }

        public static void GetDBItems()
        {

            NpgsqlConnection dBconnection = GetConnection();
            if (dBconnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    facultyNamesList.Clear();
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.faculty_codes", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    facultyNamesList.Add(new string[3] { (string)dB["faculty_code"], (string)dB["faculty_short_name"], (string)dB["faculty_name"] });
                                }
                            }
                        }
                    }
                    requestNamesList.Clear();
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.request_codes", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    requestNamesList.Add(new string[2] { (string)dB["request_code"], (string)dB["request_name"] });
                                }
                            }
                        }
                    }

                    statusNamesList.Clear();
                    using (NpgsqlCommand cmd =
                    new NpgsqlCommand($@"SELECT * FROM {OurDatabase.dBSchema}.status_codes", dBconnection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                foreach (DbDataRecord dB in reader)
                                {
                                    statusNamesList.Add(new string[3] { (string)dB["status_code"], (string)dB["status_short_name"], (string)dB["status_name"] });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
