using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shift.DataLayer
{
    public static class SQLiteDBHelpers
    {

        #region For LocalDB's

        public const string DB_DIRECTORY = "Data";

        public static string GetLocalDB(string dbName, bool deleteIfExists = false)
        {
            try
            {
                string outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DB_DIRECTORY);
                string mdfFilename = dbName + ".db";
                string dbFileName = Path.Combine(outputFolder, mdfFilename);

                // Create Data Directory If It Doesn't Already Exist.
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // If the file exists, and we want to delete old data, remove it here and create a new database.
                if (File.Exists(dbFileName) && deleteIfExists)
                {
                    try
                    {
                        File.Delete(dbFileName);
                        CreateDatabase(dbName, dbFileName);
                    }
                    catch (System.IO.IOException ex)
                    {
                        if (!ex.Message.Contains("used by another process"))
                        {
                            throw ex;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                // If the database does not already exist, create it.
                else if (!File.Exists(dbFileName))
                {
                    CreateDatabase(dbName, dbFileName);
                }

                // Open newly created, or old database.
                string connectionString = String.Format(@"Data Source={0};", dbFileName);
                return connectionString;
            }
            catch
            {
                throw;
            }
        }

        public static bool CreateDatabase(string dbName, string dbFileName)
        {
            try
            {
                string connectionString = String.Format(@"Data Source={0}", dbFileName);
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string sql1 =
                        "CREATE TABLE [Job](" +
                                     "[JobID]       INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                     "[AppID]       VARCHAR(100) NULL, " +
                                     "[UserID]      VARCHAR(100) NULL, " +
                                     "[ProcessID]   VARCHAR(100) NULL, " +
                                     "[JobType]     VARCHAR(50) NULL, " +
                                     "[JobName]     VARCHAR(100) NULL, " +
                                     "[InvokeMeta]  text NULL, " +
                                     "[Parameters]  TEXT NULL, " +
                                     "[Command]     VARCHAR(50) NULL, " +
                                     "[Status]      INTEGER NULL, " +
                                     "[Error]       TEXT NULL, " +
                                     "[Start]       DATETIME        NULL, " +
                                     "[End]         DATETIME        NULL, " +
                                     "[Created]     DATETIME        NULL " +
                                ") ";
                    SQLiteCommand cmd1 = connection.CreateCommand();
                    cmd1.CommandText = sql1;
                    cmd1.ExecuteNonQuery();

                    string sql2 = "CREATE INDEX [IX_ProcessID] ON [Job]([ProcessID] ASC) ";
                    SQLiteCommand cmd2 = connection.CreateCommand();
                    cmd2.CommandText = sql2;
                    cmd2.ExecuteNonQuery();

                    string sql3 = "CREATE TABLE [JobProgress](" +
                                     "[JobID] INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES Job([JobID]) ON DELETE CASCADE ON UPDATE CASCADE, " +
                                     "[Percent] INTEGER NULL," +
                                     "[Note] text NULL," +
                                     "[Data] TEXT NULL) ";
                    SQLiteCommand cmd3 = connection.CreateCommand();
                    cmd3.CommandText = sql3;
                    cmd3.ExecuteNonQuery();

                    string sql4 = "CREATE VIEW [JobView] AS " +
                                     "SELECT Job.*, JobProgress.[Percent], JobProgress.Note, JobProgress.Data " +
                                     "FROM Job " +
                                     "LEFT OUTER JOIN JobProgress ON Job.JobID = JobProgress.JobID ";
                    SQLiteCommand cmd4 = connection.CreateCommand();
                    cmd4.CommandText = sql4;
                    cmd4.ExecuteNonQuery();

                    connection.Close();
                }

                if (File.Exists(dbFileName)) return true;
                else return false;
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }
}
