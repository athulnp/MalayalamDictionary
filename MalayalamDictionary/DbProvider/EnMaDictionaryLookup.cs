using MalayalamDictionary.Bots;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;

namespace MalayalamDictionary.DbProvider
{
    public class EnMaDictionaryLookup : IDictionaryLookup
    {
        private SQLiteConnection conn;
        private readonly ILogger logger;
        private static Mutex mut = new Mutex();

        public EnMaDictionaryLookup(ILogger<EnMaDictionaryLookup> _logger)
        {
            this.logger = _logger;
        }

        private void Initialize()
        {
            mut.WaitOne();
            if (this.IsInitialized())
            {
                logger.LogInformation("Mutex protection");
                mut.ReleaseMutex();
                return;
            }
            try
            {
                logger.LogInformation("Initializing sqlite connection");
                string relativePath = @"db.sqlite";
                string connectionString = string.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;", relativePath);
                conn = new SQLiteConnection(connectionString);
                conn.Open();
                logger.LogInformation("Sqlite connection opened successfully");
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        private bool IsInitialized()
        {
            return this.conn != null && this.conn.State == System.Data.ConnectionState.Open;
        }
        public IEnumerable<string> GetMeanings(string word)
        {
            if (!IsInitialized())
                this.Initialize();

            Stopwatch watch = new Stopwatch();
            logger.LogInformation($"Execution started for {word}");
            watch.Start();
            string stm = $@"select ml.word from eng_w_e_ml en join eng_w_ml ml on en.eid = ml.mid where en.word ='{word}' COLLATE NOCASE; ";

            using var cmd = new SQLiteCommand(stm, conn);
            using SQLiteDataReader rdr = cmd.ExecuteReader();
            List<string> results = new List<string>();
            while (rdr.Read())
            {
                results.Add(rdr.GetString(0));
            }
            watch.Stop();
            logger.LogInformation($"Execution ended for {word}");
            logger.LogInformation($"Total time taken to get meaning of {word} : {watch.Elapsed.TotalMilliseconds}");
            if (results.Count == 0)
                throw new WordNotFoundException(word);
            return results;
        }

        public void Dispose()
        {
            conn.Close();
        }
    }
}
