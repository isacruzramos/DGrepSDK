using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Monitoring.DGrep.DataContracts.External;
using Microsoft.Azure.Monitoring.DGrep.SDK;

using DGrepLibrary;
using Newtonsoft.Json;
using System.Data;

namespace DgrepQuery
{
    internal class Program
    {
        /// <summary>
        /// TableName/Event to query
        /// </summary>
        private static string TableName;

        private static DateTime QueryStartTime;

        private static DateTime QueryEndTime;

        private static string ServerQuery = String.Empty;

        private static string MdsNamespace = string.Empty;

        static void Main(string[] args)
        {
            MdsNamespace = "XSyncEvents";
            TableName = "Ifx3Operation";
            QueryStartTime = DateTime.Parse("2023/05/10T16:10:00Z");
            QueryEndTime = QueryStartTime.AddSeconds(30);
            //ServerQuery = "where operationName == \\\\\\\"WorkFrameworkActionExecution\\\" where env_cloud_roleInstance.containsi(\\\"afsppwus01dpvms\\\") where ServiceName == \\\"Kailani.SF.SyncWorker\\\" let QueueLatencyMs = Convert.ToInt32(Regex.Match(Context, \\\"‖PickupTimeMs=(.*?)‖\\\").Groups[1].Value) \r\n groupby env_time.RoundDown(\\\"PT30S\\\"), QueueLatencyMs";
           // ServerQuery = ""
            try
            {

                var resultDataSet = QueryHelper.ExecuteQuery(MdsNamespace, TableName, QueryStartTime, QueryEndTime, ServerQuery);

                PrintRowSet(resultDataSet);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private static void PrintRowSet(DataSet dataSet)
        {
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}
