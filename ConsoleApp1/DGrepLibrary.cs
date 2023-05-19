using Microsoft.Azure.Monitoring.DGrep.DataContracts.External;
using Microsoft.Azure.Monitoring.DGrep.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DGrepLibrary
{
    public class QueryHelper
    {
        /// <summary>
        /// DGrep endpoint to query - this should remain unchanged for all XSync accounts
        /// </summary>
        private static readonly Uri DGrepEndpoint = new Uri("https://dgrepv2-frontend-prod.trafficmanager.net");

        /// <summary>
        /// Production MdsEndpoint
        /// </summary>
        private static readonly Uri MdsEndpoint = new Uri("https://production.diagnostics.monitoring.core.windows.net/");

        // Certificate store
        private const StoreLocation ClientCertificateStoreLocation = StoreLocation.CurrentUser;

        public static DataSet ExecuteQuery(string tableName, DateTime startTimeLocal, DateTime endTimeLocal, string serverQueryString)
        {
            return ExecuteQuery("KailaniSvc", tableName, startTimeLocal, endTimeLocal, serverQueryString);
        }

        public static DataSet ExecuteQuery(string mdsNamespace, string tableName, DateTime startTimeLocal, DateTime endTimeLocal, string serverQueryString)
        {
            QueryInput dgrepQuery = GetQuery(mdsNamespace, tableName, startTimeLocal, endTimeLocal, serverQueryString);

            using (var client = new DGrepUserAuthClient(DGrepEndpoint))
            {
                var result = client.GetRowSetResultAsync(dgrepQuery, CancellationToken.None).GetAwaiter().GetResult();
                return ConvertToDataSet(result.RowSet);
            }
        }

        private static QueryInput GetQuery(string mdsNamespace, string tableName, DateTime startTimeLocal, DateTime endTimeLocal, string serverQueryString)
        {
            return new QueryInput
            {
                MdsEndpoint = MdsEndpoint,
                EventFilters = new List<EventFilter>
                {
                    new EventFilter { NamespaceRegex = "^XSyncEvents$", NameRegex = "^(Ifx3Operation)$" },
                },
                //IdentityColumns = new Dictionary<string, List<string>> { { "Role", new List<string> { "Scheduler", "EnumerationWorker" } } },
                StartTime = startTimeLocal,
                EndTime = endTimeLocal,
                ServerQuery = serverQueryString,
            };
        }

        private static DataSet ConvertToDataSet(RowSet rowSet)
        {
            var dataSet = new DataSet("DGrepResult");
            var table = new DataTable();

            var columnNames = rowSet.ColumnDefinitions.Keys;
            foreach (var column in columnNames)
            {
                DataColumn dataColumn = new DataColumn(column);
                table.Columns.Add(dataColumn);
            }

            foreach (var row in rowSet.Rows)
            {
                DataRow dataRow = table.NewRow();
                foreach (var column in columnNames)
                {
                    dataRow[column] = row[column];
                }
                table.Rows.Add(dataRow);
            }

            dataSet.Tables.Add(table);

            return dataSet;
        }

    }
}
