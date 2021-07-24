using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kasboekje
{
    static class SQLiteLib
    {
        public static bool createTables(string filename)
        {
            /*
             * TransactionType
             * Name - VARCHAR(255) - PRIMARY KEY
             * Type - VARCHAR(255) - NOT NULL (inkomsten/uitgaven
             * SubType - VARCHAR(255) - NOT NULL (huishoudelijk/etc)
             * 
             * Transaction
             * Id - INT - PRIMARY KEY
             * TypeName - VARCHAR(255) - FOREIGN KEY -> TransactionType.Name
             * Date - DATETIME - NOT NULL
             */

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source = {filename}; Version = 3; New = true; Compress = True; Pooling = True; Max Pool Size = 100;"))
            {
                string query1 = "CREATE TABLE 'TransactionTypes' (" +
                    "'Name' VARCHAR(255) NOT NULL, " +
                    "'Type' VARCHAR(255) NOT NULL, " +
                    "'SubType' VARCHAR(255) NOT NULL UNIQUE, " +
                    "PRIMARY KEY('Name'));";

                string query2 = "BEGIN;" +
                    "CREATE TABLE 'Transactions'(" +
                        "'Id' INT NOT NULL," +
                        "'TypeName' VARCHAR(255) NOT NULL," +
                        "'Date' DATETIME NOT NULL," +
                        "PRIMARY KEY('Id'));" +
                    "CREATE INDEX transaction_idx ON Transactions(TypeName, Date);" +
                    "COMMIT;";


                conn.Open();

                SQLiteCommand sqlCommand;

                // create TransactionType table
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = query1;
                sqlCommand.ExecuteNonQuery();

                // create Transactions table
                sqlCommand.CommandText = query2;
                sqlCommand.ExecuteNonQuery();

                return checkForDefaultTables(filename);
            }
        }

        // checks whether the default tables are present -- does not check for correctness
        public static bool checkForDefaultTables(string filename)
        {
            bool succes = true;

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source = default.db; Version = 3; New = false; Compress = True; Pooling = True; Max Pool Size = 100;"))
            {
                string query1 = "SELECT name " +
                    "FROM sqlite_master " +
                    "WHERE type='table' " +
                    "AND name='TransactionTypes';";

                string query2 = "SELECT name " +
                    "FROM sqlite_master " +
                    "WHERE type='table' " +
                    "AND name='Transactions';";

                SQLiteCommand sqlCommand;

                // create TransactionType table
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = query1;

                conn.Open();

                using (SQLiteDataReader sqlReader = sqlCommand.ExecuteReader())
                {
                    // Always call Read before accessing data.
                    while (sqlReader.Read())
                    {
                        // table exists
                        if (!sqlReader.HasRows)
                        {
                            // something wrong
                            succes = false;
                        }
                    }

                }

                sqlCommand.CommandText = query2;
                using (SQLiteDataReader sqlReader = sqlCommand.ExecuteReader())
                {
                    // Always call Read before accessing data.
                    while (sqlReader.Read())
                    {
                        // table exists
                        if (!sqlReader.HasRows)
                        {
                            // something wrong
                            succes = false;
                        }
                    }

                }
            }

            return succes;
        }
    }
}
