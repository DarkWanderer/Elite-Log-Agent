namespace DW.ELA.Interfaces
{
    using System;
    using System.Collections.Generic;

    public class JournalContext : IEquatable<JournalContext>
    {
        public JournalContext(string commanderName, string gameVersion, string frontierID)
        {
            CommanderName = commanderName;
            GameVersion = gameVersion;
            FrontierID = frontierID;
        }

        public string CommanderName { get; }

        public string GameVersion { get; }

        public string FrontierID { get; }

        public JournalContext WithCommanderName(string commanderName) => new JournalContext(commanderName, GameVersion, FrontierID);

        public JournalContext WithGameVersion(string gameVersion) => new JournalContext(CommanderName, gameVersion, FrontierID);

        public JournalContext WithFrontierID(string frontierID) => new JournalContext(CommanderName, GameVersion, frontierID);

        public override bool Equals(object obj) => Equals(obj as JournalContext);

        public bool Equals(JournalContext other) => other != null && CommanderName == other.CommanderName && GameVersion == other.GameVersion && FrontierID == other.FrontierID;

        public override int GetHashCode()
        {
            var hashCode = -809620204;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CommanderName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GameVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FrontierID);
            return hashCode;
        }
    }
}
