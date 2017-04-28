using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shift.DataLayer
{
    public static class SqlDBHelpers
    {

        #region For LocalDB's

        // From https://social.msdn.microsoft.com/Forums/en-US/268c3411-102a-4272-b305-b14e29604313/localdb-create-connect-to-database-programmatically-?forum=sqlsetupandupgrade

        public const string DB_DIRECTORY = "Data";

        public static string GetLocalDB(string dbName, bool deleteIfExists = false) //SqlConnection GetLocalDB(string dbName, bool deleteIfExists = false)
        {
            try
            {
                string outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DB_DIRECTORY);
                string mdfFilename = dbName + ".mdf";
                string dbFileName = Path.Combine(outputFolder, mdfFilename);
                string logFileName = Path.Combine(outputFolder, String.Format("{0}_log.ldf", dbName));

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
                        if (File.Exists(logFileName)) File.Delete(logFileName);
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
                string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", dbName, dbFileName);
                //SqlConnection connection = new SqlConnection(connectionString);
                //connection.Open();
                //return connection;
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
                //Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\projects\Github\Shift\Shift.UnitTest\testdatabase.mdf;Integrated Security=True;Connect Timeout=30
                //Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\projects\Github\Shift\Shift.UnitTest\bin\Debug\Data\testdatabase.mdf;Integrated Security=True;Connect Timeout=30

                string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();


                    DetachDatabase(dbName);

                    cmd.CommandText = String.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", dbName, dbFileName);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                string connectionString2 = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", dbName, dbFileName);
                using (var connection2 = new SqlConnection(connectionString2))
                {
                    connection2.Open();

                    string sql1 =
                        "CREATE TABLE [dbo].[Job](" +
                                     "[JobID][int]  IDENTITY(1, 1)    NOT NULL, " +
                                     "[AppID]       [varchar](100)    NULL, " +
                                     "[UserID]      [varchar](100)    NULL, " +
                                     "[ProcessID]   [varchar](100)    NULL, " +
                                     "[JobType]     [varchar](50)     NULL, " +
                                     "[JobName]     [varchar](100)    NULL, " +
                                     "[InvokeMeta]  [varchar](max)    NULL, " +
                                     "[Parameters]  [varchar](max)    NULL, " +
                                     "[Command]     [varchar](50)     NULL, " +
                                     "[Status]      [int]             NULL, " +
                                     "[Error]       [varchar](max)    NULL, " +
                                     "[Start]       [datetime]        NULL, " +
                                     "[End]         [datetime]        NULL, " +
                                     "[Created]     [datetime]        NULL, " +
                                     "CONSTRAINT[PK_Job] PRIMARY KEY CLUSTERED ([JobID] ASC) " +
                                ") ";
                    SqlCommand cmd1 = connection2.CreateCommand();
                    cmd1.CommandText = sql1;
                    cmd1.ExecuteNonQuery();

                    string sql2 = "CREATE NONCLUSTERED INDEX [IX_ProcessID] ON [dbo].[Job]([ProcessID] ASC) ";
                    SqlCommand cmd2 = connection2.CreateCommand();
                    cmd2.CommandText = sql2;
                    cmd2.ExecuteNonQuery();

                    string sql3 = "CREATE TABLE [dbo].[JobProgress](" +
                                     "[JobID]   [int]          NOT NULL," +
                                     "[Percent] [int]          NULL," +
                                     "[Note]    [varchar](max) NULL," +
                                     "[Data]    [varchar](max) NULL," +
                                     "CONSTRAINT [PK_JobProgress] PRIMARY KEY CLUSTERED([JobID] ASC)) ";
                    SqlCommand cmd3 = connection2.CreateCommand();
                    cmd3.CommandText = sql3;
                    cmd3.ExecuteNonQuery();

                    string sql4 = "CREATE VIEW [dbo].[JobView] AS " +
                                     "SELECT dbo.Job.*, dbo.JobProgress.[Percent], dbo.JobProgress.Note, dbo.JobProgress.Data " +
                                     "FROM dbo.Job " +
                                     "LEFT OUTER JOIN dbo.JobProgress ON dbo.Job.JobID = dbo.JobProgress.JobID ";
                    SqlCommand cmd4 = connection2.CreateCommand();
                    cmd4.CommandText = sql4;
                    cmd4.ExecuteNonQuery();

                    connection2.Close();
                }

                if (File.Exists(dbFileName)) return true;
                else return false;
            }
            catch
            {
                throw;
            }
        }

        public static bool DetachDatabase(string dbName)
        {
            try
            {
                string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = String.Format("exec sp_detach_db '{0}'", dbName);
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
