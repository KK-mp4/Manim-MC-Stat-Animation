namespace StatsUI
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;

    public class DataAccess
    {
        public List<Stat> GetStats(int stat_id, DateTime date1, DateTime date2)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("StatsDB")))
            {
                var sqlFormattedDate1 = date1.ToString("yyyy-MM-dd HH:mm:ss");
                var sqlFormattedDate2 = date2.ToString("yyyy-MM-dd HH:mm:ss");
                return connection.Query<Stat>(
                    $"SELECT * FROM archive_materialized " +
                    $"WHERE stat_id = '{ stat_id }' " +
                    $"AND timestamp >= '{ sqlFormattedDate1 }' " +
                    $"AND timestamp <= '{ sqlFormattedDate2 }' " +
                    $"ORDER BY timestamp").ToList();
            }
        }
    }
}
