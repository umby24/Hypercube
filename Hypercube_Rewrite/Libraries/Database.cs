using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Hypercube.Libraries {
    /// <summary>
    /// Class for interaction with a SQLite Database holding critical user information. Some of this information (Such as IP) is used for logging purposes,
    /// and may be used for looking up bans.
    /// </summary>
    public class Database {
        const string DatabaseName = "Database.s3db";
        public SQLiteConnection DBConnection;
        readonly object _dbLock = new object();

        public Database() {
            if (!File.Exists("Settings/" + DatabaseName)) {
                // -- We need to create the PlayerDB.
                SQLiteConnection.CreateFile(Path.GetFullPath("Settings/" + DatabaseName));

                // -- Now we need to connect and create the table.
                lock (_dbLock) {
                    var connection = new SQLiteConnection("Data Source=" + Path.GetFullPath("Settings/" + DatabaseName));
                    connection.Open();

                    var command = new SQLiteCommand("CREATE TABLE PlayerDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Rank TEXT, RankStep TEXT, BoundBlock INTEGER, RankChangedBy TEXT, LoginCounter INTEGER, KickCounter INTEGER, Ontime INTEGER, LastOnline INTEGER, IP TEXT, Stopped INTEGER, StoppedBy TEXT, Banned INTEGER, Vanished INTEGER, BannedBy STRING, BannedUntil INTEGER, Global INTEGER, Time_Muted INTEGER, BanMessage TEXT, KickMessage TEXT, MuteMessage TEXT, RankMessage TEXT, StopMessage TEXT)", connection);
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX PlayerDB_Index ON PlayerDB (Name COLLATE NOCASE)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE IPBanDB (Number INTEGER PRIMARY KEY, IP TEXT UNIQUE, Reason TEXT, BannedBy TEXT)";
                    command.ExecuteNonQuery();

                    DBConnection = connection; // -- All done.
                }
            } else {
                DBConnection = new SQLiteConnection("Data Source=" + Path.GetFullPath("Settings/" + DatabaseName));
                DBConnection.Open();
            }
        }

        /// <summary>
        /// Creates a new entry in the PlayerDB for a player.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="core"></param>
        public void CreatePlayer(string name, string ip, Hypercube core) {
            var myValues = new Dictionary<string, string>
            {
                {"Name", name},
                {"IP", ip},
                {"Rank", core.DefaultRank.ID.ToString()},
                {"RankStep", "0"},
                {"Global", "1"},
                {"Banned", "0"},
                {"Stopped", "0"},
                {"Vanished", "0"},
                {"BoundBlock", "1"}
            };

            Insert("PlayerDB", myValues);
        }

        /// <summary>
        /// Checks to see if a player by this name already exists for this server.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsPlayer(string name) {
            var dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + name + "'");

            foreach (DataRow c in dt.Rows)
                if (((string) c["Name"]).ToLower() == name.ToLower()) return true;

            return false;
        }

        /// <summary>
        /// Returns the Case-correct version of a player's name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetPlayerName(string name) {
            var dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + name + "'");

            foreach (DataRow c in dt.Rows)
                if (((string) c["Name"]).ToLower() == name.ToLower()) return (string) c["Name"];
            

            return "";
        }

        public void BanPlayer(string name, string reason, string bannedBy) {
            name = GetPlayerName(name);
            SetDatabase(name, "PlayerDB", "Banned", 1);
            SetDatabase(name, "PlayerDB", "BanMessage", reason);
            SetDatabase(name, "PlayerDB", "BannedBy", bannedBy);
        }

        public void UnbanPlayer(string name) {
            name = GetPlayerName(name);
            SetDatabase(name, "PlayerDB", "Banned", 0);
        }

        public void StopPlayer(string name, string reason, string stoppedBy) {
            name = GetPlayerName(name);
            SetDatabase(name, "PlayerDB", "Stopped", 1);
            SetDatabase(name, "PlayerDB", "StopMessage", reason);
            SetDatabase(name, "PlayerDB", "StoppedBy", stoppedBy);
        }

        public void UnstopPlayer(string name) {
            name = GetPlayerName(name);
            SetDatabase(name, "PlayerDB", "Stopped", 0);
        }

        public void MutePlayer(string name, int minutes, string reason) {
            name = GetPlayerName(name);
            SetDatabase(name, "PlayerDB", "MuteMessage", reason);
            SetDatabase(name, "PlayerDB", "Time_Muted", minutes);
        }

        public void UnmutePlayer(string name) {
            name = GetPlayerName(name);
            SetDatabase(name, "PlayerDB", "Time_Muted", 0);
        }

        public void IpBan(string ip, string reason, string bannedby) {
            var values = new Dictionary<string, string> {{"IP", ip}, {"Reason", reason}, {"BannedBy", bannedby}};
            Insert("IPBanDB", values);
        }

        public void UnIpBan(string ip) {
            if (!IsIpBanned(ip))
                return;

            Delete("IPBanDB", "IP='" + ip + "'");
        }

        public bool IsIpBanned(string ip) {
            var dt = GetDataTable("SELECT * FROM IPBanDB WHERE IP=='" + ip + "'");
            return dt.Rows.Cast<DataRow>().Any(c => ((string) c["IP"]) == ip);
        }

        /// <summary>
        /// Retreives an integer value from the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetDatabaseInt(string name, string table, string field) {
            var dt = GetDataTable("SELECT * FROM " + table + " WHERE Name='" + name + "' LIMIT 1");

            try {
                return Convert.ToInt32(dt.Rows[0][field]);
            } catch {
                return -1;
            }
        }

        /// <summary>
        /// Retreives a string value from the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetDatabaseString(string name, string table, string field) {
            var dt = GetDataTable("SELECT * FROM " + table + " WHERE Name='" + name + "' LIMIT 1");

            try {
                return (string)dt.Rows[0][field];
            } catch {
                return "";
            }
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetDatabase(string name, string table, string field, bool value) {
            var values = new Dictionary<string, string> {{field, value.ToString()}};

            Update(table, values, "Name='" + name + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetDatabase(string name, string table, string field, int value) {
            var values = new Dictionary<string, string> {{field, value.ToString()}};
            Update(table, values, "Name='" + name + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetDatabase(string name, string table, string field, string value) {
            var values = new Dictionary<string, string> {{field, value}};
            Update(table, values, "Name='" + name + "'");
        }

        #region Basic DB Interaction
        // -- Taken / Inspired from.
        // -- http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/#/

        public DataTable GetDataTable(string sql) {
            var dt = new DataTable();

            try {
                lock (_dbLock) {
                    var command = new SQLiteCommand(DBConnection) {CommandText = sql};

                    var reader = command.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
            } catch (Exception e) {
                throw new Exception(e.Message);
            }

            return dt;
        }

        public int ExecuteNonQuery(string sql) {
            lock (_dbLock) {
                var command = new SQLiteCommand(DBConnection) {CommandText = sql};

                var rowsUpdated = command.ExecuteNonQuery();

                return rowsUpdated;
            }
        }

        public bool Update(String tableName, Dictionary<String, String> data, String where) {
            var vals = "";
            var returnCode = true;

            if (data.Count >= 1) {
                foreach (var val in data) 
                    vals += String.Format(" {0} = '{1}',", val.Key, val.Value);
                
                vals = vals.Substring(0, vals.Length - 1);
            }

            try {
                ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            } catch {
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where) {
            var returnCode = true;

            try {
                ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            } catch {
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data) {
            var columns = "";
            var values = "";
            var returnCode = true;

            foreach (var val in data) {
                columns += String.Format(" {0},", val.Key);
                values += String.Format(" '{0}',", val.Value);
            }

            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);

            try {
                ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            } catch {
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB() {
            DataTable tables;

            try {
                tables = GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");

                foreach (DataRow table in tables.Rows) 
                    ClearTable(table["NAME"].ToString());

                return true;
            } catch {
                return false;
            }
        }

        public bool ClearTable(String table) {
            try {
                ExecuteNonQuery(string.Format("delete from {0};", table));
                return true;
            } catch {
                return false;
            }
        }

        #endregion
    }
}
