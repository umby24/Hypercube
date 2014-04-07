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
    public class Database {
        const string DatabaseName = "Database.s3db";

        public Database() {
            
            if (!File.Exists("Settings/" + DatabaseName)) {
                // -- We need to create the PlayerDB.
                SQLiteConnection.CreateFile(Path.GetFullPath("Settings/" + DatabaseName));

                // -- Now we need to connect and create the table.
                var Connection = new SQLiteConnection("Data Source=" + Path.GetFullPath("Settings/" + DatabaseName));
                Connection.Open();

                var Command = new SQLiteCommand("CREATE TABLE PlayerDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Rank TEXT, RankStep TEXT, BoundBlock INTEGER, RankChangedBy TEXT, LoginCounter INTEGER, KickCounter INTEGER, Ontime INTEGER, LastOnline INTEGER, IP TEXT, Stopped INTEGER, StoppedBy TEXT, Banned INTEGER, Vanished INTEGER, BannedBy STRING, BannedUntil INTEGER, Global INTEGER, Time_Muted INTEGER, BanMessage TEXT, KickMessage TEXT, MuteMessage TEXT, RankMessage TEXT, StopMessage TEXT)", Connection);
                Command.ExecuteNonQuery();

                Command.CommandText = "CREATE TABLE RankDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Prefix TEXT, Suffix TEXT, Next TEXT, RGroup TEXT, Points INTEGER, Op INTEGER)";
                Command.ExecuteNonQuery();

                Command.CommandText = "CREATE TABLE BlockDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, OnClient INTEGER, PlaceRank STRING, DeleteRank STRING, Physics INTEGER, PhysicsDelay INTEGER, PhysicsRandom INTEGER, PhysicsPlugin TEXT, Kills INTEGER, Color INTEGER, CPELevel INTEGER, CPEReplace INTEGER, Special INTEGER, ReplaceOnLoad INTEGER)";
                Command.ExecuteNonQuery();

                Connection.Close(); // -- All done.
            }
        }

        /// <summary>
        /// Creates a new entry in the PlayerDB for a player.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IP"></param>
        public void CreatePlayer(string Name, string IP, Hypercube Core) {
            var myValues = new Dictionary<string, string>();
            myValues.Add("Name", Name);
            myValues.Add("IP", IP);
            myValues.Add("Rank", Core.DefaultRank.ID.ToString());
            myValues.Add("RankStep", "0");
            myValues.Add("Global", "1");
            myValues.Add("Banned", "0");
            myValues.Add("Stopped", "0");
            myValues.Add("Vanished", "0");
            myValues.Add("BoundBlock", "1");

            Insert("PlayerDB", myValues);
        }

        /// <summary>
        /// Checks to see if a player by this name already exists for this server.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ContainsPlayer(string Name) {
            var dt = GetDataTable("SELECT * FROM PlayerDB"); //WHERE Name='" + Name + "' AND Service='" + Service + "'");

            foreach (DataRow c in dt.Rows) {
                if (((string)c["Name"]).ToLower() == Name.ToLower())
                    return true;
            }

            return false;
            
        }

        /// <summary>
        /// Returns the Case-correct version of a player's name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetPlayerName(string Name) {
            var dt = GetDataTable("SELECT * FROM PlayerDB"); //WHERE Name='" + Name + "' AND Service='" + Service + "'");

            foreach (DataRow c in dt.Rows) {
                if (((string)c["Name"]).ToLower() == Name.ToLower())
                    return (string)c["Name"];
            }

            return "";
        }

        public void BanPlayer(string Name, string Reason, string BannedBy) {
            Name = GetPlayerName(Name);
            SetDatabase(Name, "PlayerDB", "Banned", 1);
            SetDatabase(Name, "PlayerDB", "BanMessage", Reason);
            SetDatabase(Name, "PlayerDB", "BannedBy", BannedBy);
        }

        public void UnbanPlayer(string Name) {
            Name = GetPlayerName(Name);
            SetDatabase(Name, "PlayerDB", "Banned", 0);
        }

        public void StopPlayer(string Name, string Reason, string StoppedBy) {
            Name = GetPlayerName(Name);
            SetDatabase(Name, "PlayerDB", "Stopped", 1);
            SetDatabase(Name, "PlayerDB", "StopMessage", Reason);
            SetDatabase(Name, "PlayerDB", "StoppedBy", StoppedBy);
        }

        public void UnstopPlayer(string Name) {
            Name = GetPlayerName(Name);
            SetDatabase(Name, "PlayerDB", "Stopped", 0);
        }

        public void MutePlayer(string Name, int Minutes, string Reason) {
            Name = GetPlayerName(Name);
            SetDatabase(Name, "PlayerDB", "MuteMessage", Reason);
            SetDatabase(Name, "PlayerDB", "Time_Muted", Minutes);
        }

        public void UnmutePlayer(string Name) {
            Name = GetPlayerName(Name);
            SetDatabase(Name, "PlayerDB", "Time_Muted", 0);
        }

        /// <summary>
        /// Creates a new rank in the database.
        /// </summary>
        /// <param name="Rankname"></param>
        /// <param name="RankGroup"></param>
        /// <param name="RankPrefix"></param>
        /// <param name="RankSuffix"></param>
        /// <param name="IsOp"></param>
        /// <param name="PointsInRank"></param>
        /// <param name="NextRank"></param>
        public void CreateRank(string Rankname, string RankGroup, string RankPrefix = "", string RankSuffix = "", byte IsOp = 0, int PointsInRank = 10, string NextRank = "") {
            var myValues = new Dictionary<string, string>();
            myValues.Add("Name", Rankname);
            myValues.Add("Prefix", RankPrefix);
            myValues.Add("Suffix", RankSuffix);
            myValues.Add("Next", NextRank);
            myValues.Add("RGroup", RankGroup);
            myValues.Add("Points", PointsInRank.ToString());
            myValues.Add("Op", IsOp.ToString());

            Insert("RankDB", myValues);
        }

        /// <summary>
        /// Checks to see if a rank by this name already exists.
        /// </summary>
        /// <param name="Rankname"></param>
        /// <returns></returns>
        public bool ContainsRank(string Rankname) {
            var dt = GetDataTable("SELECT * FROM RankDB WHERE Name='" + Rankname + "'");

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a new block in the block database.
        /// </summary>
        /// <param name="Blockname">The name of the block</param>
        /// <param name="OnClient">The BlockID that will be sent to clients.</param>
        /// <param name="RanksPlace">A comma seperated list of the IDs of each rank that can place this block.</param>
        /// <param name="RanksDelete">A comma seperated list of the IDs of each rank that can delete this block.</param>
        /// <param name="Physics">The type of physics (if any) this block will use.</param>
        /// <param name="PhysicsPlugin">The lua plugin that will process this block's physics.</param>
        /// <param name="Kills">True if this block kills players on contact.</param>
        /// <param name="Color">The isomap color for this block.</param>
        /// <param name="CPELevel">The CPE level that implements this block.</param>
        /// <param name="CPEReplace">The block this block should be replaced with should the client not support the required CustomBlocks level.</param>
        /// <param name="Special">if true, the block will appear in /materials. </param>
        /// <param name="ReplaceOnLoad">-1 for none. The block that this block will be replaced with upon a map load.</param>
        public void CreateBlock(string Blockname, byte OnClient, string RanksPlace, string RanksDelete, int Physics, int PhysicsDelay, int PhysicsRandom,
            string PhysicsPlugin, bool Kills, int Color, int CPELevel, int CPEReplace, bool Special, int ReplaceOnLoad) {

                var myValues = new Dictionary<string, string>();
                myValues.Add("Name", Blockname);
                myValues.Add("OnClient", OnClient.ToString());
                myValues.Add("PlaceRank", RanksPlace);
                myValues.Add("DeleteRank", RanksDelete);
                myValues.Add("Physics", Physics.ToString());
                myValues.Add("PhysicsDelay", PhysicsDelay.ToString());
                myValues.Add("PhysicsRandom", PhysicsRandom.ToString());
                myValues.Add("PhysicsPlugin", PhysicsPlugin);
                myValues.Add("Kills", Kills.ToString());
                myValues.Add("Color", Color.ToString());
                myValues.Add("CPELevel", CPELevel.ToString());
                myValues.Add("CPEReplace", CPEReplace.ToString());
                myValues.Add("Special", Special.ToString());
                myValues.Add("ReplaceOnLoad", ReplaceOnLoad.ToString());

                Insert("BlockDB", myValues);
        }

        /// <summary>
        /// Searches the database to determine if a block already exists with the given name.
        /// </summary>
        /// <param name="Blockname"></param>
        /// <returns></returns>
        public bool ContainsBlock(string Blockname) {
            var dt = GetDataTable("SELECT * FROM BlockDB WHERE Name='" + Blockname + "'");

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Retreives an integer value from the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public int GetDatabaseInt(string Name, string Table, string Field) {
            var dt = GetDataTable("SELECT * FROM " + Table + " WHERE Name='" + Name + "' LIMIT 1");

            try {
                return Convert.ToInt32(dt.Rows[0][Field]);
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
        public string GetDatabaseString(string Name, string Table, string Field) {
            var dt = GetDataTable("SELECT * FROM " + Table + " WHERE Name='" + Name + "' LIMIT 1");

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
        public void SetDatabase(string Name, string Table, string Field, bool Value) {
            var Values = new Dictionary<string, string>();
            Values.Add(Field, Value.ToString());

            Update(Table, Values, "Name='" + Name + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        public void SetDatabase(string Name, string Table, string Field, int Value) {
            var Values = new Dictionary<string, string>();
            Values.Add(Field, Value.ToString());

            Update(Table, Values, "Name='" + Name + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="Playername"></param>
        /// <param name="Field"></param>
        /// <param name="Value"></param>
        public void SetDatabase(string Name, string Table, string Field, string Value) {
            var Values = new Dictionary<string, string>();
            Values.Add(Field, Value);

            Update(Table, Values, "Name='" + Name + "'");
        }

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
