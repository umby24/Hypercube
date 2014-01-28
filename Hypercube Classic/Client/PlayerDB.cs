using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Hypercube_Classic.Client {
    /// <summary>
    /// Class for interaction with a SQLite Database holding critical user information. Some of this information (Such as IP) is used for logging purposes,
    /// and may be used for looking up bans.
    /// </summary>
    public class PlayerDB {
        const string DatabaseName = "PlayerDB.s3db";

        public PlayerDB() {
            
            if (!File.Exists("Settings/" + DatabaseName)) {
                // -- We need to create the PlayerDB.
                SQLiteConnection.CreateFile(Path.GetFullPath("Settings/" + DatabaseName));

                // -- Now we need to connect and create the table.
                var Connection = new SQLiteConnection("Data Source=" + Path.GetFullPath("Settings/" + DatabaseName));
                Connection.Open();

                var Command = new SQLiteCommand("CREATE TABLE PlayerDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Rank INTEGER, BoundBlock INTEGER, RankChangedBy TEXT, LoginCounter INTEGER, KickCounter INTEGER, Ontime INTEGER, LastOnline INTEGER, IP TEXT, Stopped BOOLEAN, StoppedBy TEXT, Banned BOOLEAN, Vanished BOOLEAN, BannedBy STRING, BannedUntil INTEGER, Global BOOLEAN, Time_Muted INTEGER, BanMessage TEXT, KickMessage TEXT, MuteMessage TEXT, RankMessage TEXT, StopMessage TEXT)", Connection);
                Command.ExecuteNonQuery();

                Connection.Close(); // -- All done.
            }
        }

        /// <summary>
        /// Creates a new entry in the PlayerDB for a player.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IP"></param>
        public void CreatePlayer(string Name, string IP) {
            var myValues = new Dictionary<string, string>();
            myValues.Add("Name", Name);
            myValues.Add("IP", IP);

            Insert("PlayerDB", myValues);
        }

        /// <summary>
        /// Checks to see if a player by this name already exists for this server.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ContainsPlayer(string Name) {
            var dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + Name + "'");

            if (dt.Rows.Count > 0) 
                return true;
             else 
                return false;
            
        }

        /// <summary>
        /// Retreives a boolean value from the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public bool GetDatabaseBool(string Playername, string Field) {
            var dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + Playername + "' LIMIT 1");

            try {
                return (bool)dt.Rows[0][Field];
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Retreives an integer value from the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public int GetDatabaseInt(string Playername, string Field) {
            var dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + Playername + "' LIMIT 1");

            try {
                return (int)dt.Rows[0][Field];
            } catch {
                return -1;
            }
        }

        /// <summary>
        /// Retreives a string value from the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public string GetDatabaseString(string Playername, string Field) {
            var dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + Playername + "' LIMIT 1");

            try {
                return (string)dt.Rows[0][Field];
            } catch {
                return "";
            }
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        public void SetDatabase(string Playername, string Field, bool Value) {
            var Values = new Dictionary<string, string>();
            Values.Add(Field, Value.ToString());

            Update("PlayerDB", Values, "Name='" + Playername + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        public void SetDatabase(string Playername, string Field, int Value) {
            var Values = new Dictionary<string, string>();
            Values.Add(Field, Value.ToString());

            Update("PlayerDB", Values, "Name='" + Playername + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        public void SetDatabase(string Playername, string Field, string Value) {
            var Values = new Dictionary<string, string>();
            Values.Add(Field, Value);

            Update("PlayerDB", Values, "Name='" + Playername + "'");
        }

        #region Get Functions
        //public int GetPlayerNumber(string Name) {

        //}

        //public int GetPlayerRank(string Name) {

        //}

        //public string GetPlayerRankChanger(string Name) {

        //}

        //public int GetPlayerBoundBlock(string Name) {

        //}

        //public int GetPlayerLogins(string Name) {

        //}

        //public int GetPlayerKicks(string Name) {

        //}

        //public TimeSpan GetPlayerOntime(string Name) {

        //}

        //public DateTime GetPlayerLastOnline(string Name) {

        //}

        //public string GetPlayerIP(string Name) {

        //}

        //public bool IsPlayerStopped(string Name) {

        //}

        //public string GetPlayerStoppedby(string Name) {

        //}

        //public bool IsPlayerVanished(string Name) {

        //}

        //public bool IsPlayerBanned(string Name) {

        //}

        //public string GetPlayerBannedBy(string Name) {

        //}

        //public DateTime GetPlayerTempban(string Name) {

        //}

        //public bool GetPlayerGlobalChat(string Name) {

        //}

        //public int GetPlayerMuted(string Name) {

        //}

        //public string GetPlayerBanMessage(string Name) {

        //}

        //public string GetPlayerKickMessage(string Name) {

        //}

        //public string GetPlayerMuteMessage(string Name) {

        //}

        //public string GetPlayerRankMessage(string Name) {

        //}

        //public string GetPlayerStopMessage(string Name) {

        //}
        //#endregion
        //#region Set Functions
        //public void SetPlayerNumber(string Name) {

        //}

        //public void SetPlayerRank(string Name) {

        //}

        //public void SetPlayerRankChanger(string Name) {

        //}

        //public void SetPlayerBoundBlock(string Name) {

        //}

        //public void SetPlayerLogins(string Name) {

        //}

        //public void SetPlayerKicks(string Name) {

        //}

        //public void SetPlayerOntime(string Name) {

        //}

        //public void SetPlayerLastOnline(string Name) {

        //}

        //public void SetPlayerIP(string Name) {

        //}

        //public void SetPlayerStopped(string Name) {

        //}

        //public void SetPlayerStoppedby(string Name) {

        //}

        //public void SetPlayerVanished(string Name) {

        //}

        //public void SetPlayerBanned(string Name) {

        //}

        //public void SetPlayerBannedBy(string Name) {

        //}

        //public void SetPlayerTempban(string Name) {

        //}

        //public void SetPlayerGlobalChat(string Name) {

        //}

        //public void SetPlayerMuted(string Name) {

        //}

        //public void SetPlayerBanMessage(string Name) {

        //}

        //public void SetPlayerKickMessage(string Name) {

        //}

        //public void SetPlayerMuteMessage(string Name) {

        //}

        //public void SetPlayerRankMessage(string Name) {

        //}

        //public void SetPlayerStopMessage(string Name) {

        //}
        #endregion
        #region Basic DB Interaction
        // -- Taken / Inspired from.
        // -- http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/#/

        public DataTable GetDataTable(string sql) {
            var dt = new DataTable();

            try {
                var cnn = new SQLiteConnection("Data Source=" + Path.GetFullPath("Settings/" + DatabaseName));
                cnn.Open();

                var command = new SQLiteCommand(cnn);
                command.CommandText = sql;

                var reader = command.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            } catch (Exception e) {
                throw new Exception(e.Message);
            }

            return dt;
        }

        public int ExecuteNonQuery(string sql) {
            var cnn = new SQLiteConnection("Data Source=" + Path.GetFullPath("Settings/" + DatabaseName));
            cnn.Open();

            var command = new SQLiteCommand(cnn);
            command.CommandText = sql;

            var rowsUpdated = command.ExecuteNonQuery();
            cnn.Close();

            return rowsUpdated;
        }

        public bool Update(String tableName, Dictionary<String, String> data, String where) {
            String vals = "";
            Boolean returnCode = true;

            if (data.Count >= 1) {
                foreach (KeyValuePair<String, String> val in data) {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }

            try {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
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
            Boolean returnCode = true;

            try {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
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
            String columns = "";
            String values = "";
            Boolean returnCode = true;

            foreach (KeyValuePair<String, String> val in data) {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }

            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);

            try {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
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
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");

                foreach (DataRow table in tables.Rows) {
                    this.ClearTable(table["NAME"].ToString());
                }

                return true;
            } catch {
                return false;
            }
        }

        public bool ClearTable(String table) {
            try {
                this.ExecuteNonQuery(string.Format("delete from {0};", table));
                return true;
            } catch {
                return false;
            }
        }

        #endregion
    }
}
