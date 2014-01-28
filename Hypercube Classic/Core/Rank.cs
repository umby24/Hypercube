using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Libraries;

namespace Hypercube_Classic.Core {
    public struct RankSettings : ISettings {
        public string Filename { get; set; }
        public DateTime LastModified { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public object LoadSettings { get; set; }
    }

    /// <summary>
    /// Contains multiple Rank objects, and contains methods for accessing their information.
    /// </summary>
    public class RankContainer {
        #region Variables
        public List<Rank> Ranks;
        public RankSettings RS;
        #endregion

        public RankContainer(Hypercube Core) {
            Ranks = new List<Rank>();
            RS = new RankSettings();
            RS.Filename = "Ranks.txt";
            RS.LoadSettings = new SettingsReader.LoadSettings(RanksLoaded);
            Core.Settings.ReadSettings(RS);
            Core.Settings.SettingsFiles.Add(RS);
        }

        /// <summary>
        /// Creates a new user rank.
        /// </summary>
        /// <param name="Rankname"></param>
        /// <param name="RankPrefix"></param>
        /// <param name="RankSuffix"></param>
        /// <param name="IsOp"></param>
        /// <param name="PointsInRank"></param>
        /// <param name="NextRank"></param>
        public void AddRank(string Rankname, string RankPrefix = "", string RankSuffix = "", bool IsOp = false, int PointsInRank = 10, string NextRank = "") {
            var NewRank = new Rank();
        }

        /// <summary>
        /// Returns a rank object for modification or reference.
        /// </summary>
        /// <returns></returns>
        public Rank GetRank() {
            return null;
        }

        /// <summary>
        /// Triggers when the ranks file is reloaded. This will reload all ranks into the database.
        /// </summary>
        public void RanksLoaded() {

        }

    }

    public class Rank {
        public string Name, Prefix, Suffix, NextRank;
        public int PointsInRank;
        public bool Op;
        
        /// <summary>
        /// Creates a blank rank object that you can assign values to.
        /// </summary>
        public Rank() {

        }

        /// <summary>
        /// Creates a filled rank object with all fields filled.
        /// </summary>
        /// <param name="RankName">Name of the rank.</param>
        /// <param name="RankPrefix">The Prefix any user with this rank will have on their name.</param>
        /// <param name="RankSuffix">The Suffix any user with this rank will have on their name.</param>
        /// <param name="IsOp">Indicates if this user is considered an Operator or higher.</param>
        /// <param name="RankPoints">Indicates the number of points contained in this rank. When this number has been exceeded, a user will be moved to NextRank.</param>
        /// <param name="NextRankUp">The Next rank the user will acheive if they have received all points possible for this rank. Blank indicates no higher rank.</param>
        public Rank(string RankName, string RankPrefix, string RankSuffix, bool IsOp, int RankPoints, string NextRankUp) {
            Name = RankName;
            Prefix = RankPrefix;
            Suffix = RankSuffix;
            Op = IsOp;
            PointsInRank = RankPoints;
            NextRank = NextRankUp;
        }
    }
}
