﻿using MonoTorrent.Tracker;
using MonoTorrent;

namespace ArbdTracker
{
    public class CustomITrackable : ITrackable
    {
        // I just want to keep the TorrentFiles in memory when i'm tracking the torrent, so i store
        // a reference to them in the ITrackable. This allows me to display information about the
        // files in a GUI without having to keep the entire (really really large) Torrent instance in memory.

        // We require the infohash and the name of the torrent so the tracker can work correctly

        public CustomITrackable(Torrent t)
        {
            // Note: I'm just storing the files, infohash and name. A typical Torrent instance
            // is ~100kB in memory. A typical CustomITrackable will be ~100 bytes.
            Files = t.Files;
            InfoHash = t.InfoHash;
            Name = t.Name;
        }

        /// <summary>
        /// The files in the torrent
        /// </summary>
        public IList<TorrentFile> Files { get; }

        /// <summary>
        /// The infohash of the torrent
        /// </summary>
        public InfoHash InfoHash { get; }

        /// <summary>
        /// The name of the torrent
        /// </summary>
        public string Name { get; }
    }
}
