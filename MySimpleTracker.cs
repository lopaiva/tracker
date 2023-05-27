using MonoTorrent.TorrentWatcher;
using MonoTorrent.Tracker.Listeners;
using MonoTorrent.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MonoTorrent;

namespace ArbdTracker
{
    public class MySimpleTracker
    {
        readonly TrackerServer tracker;
        TorrentFolderWatcher watcher;
        const string TORRENT_DIR = "Torrents";

        // Start the Tracker. Start Watching the TORRENT_DIR Directory for new Torrents.
        public MySimpleTracker()
        {
            tracker = new TrackerServer();
            tracker.AllowUnregisteredTorrents = true;

            var localIp = System.Net.IPAddress.Parse("192.168.1.69");

            var httpEndpoint = new System.Net.IPEndPoint(localIp, 10000);
            var udpEndpoint = new System.Net.IPEndPoint(localIp, 10001);
            Console.WriteLine("Listening for HTTP requests at: {0}", httpEndpoint);
            Console.WriteLine("Listening for UDP requests at: {0}", udpEndpoint);

            var listeners = new[] {
                TrackerListenerFactory.CreateHttp (httpEndpoint),
                TrackerListenerFactory.CreateUdp (udpEndpoint)
            };
            foreach (var listener in listeners)
            {
                tracker.RegisterListener(listener);
                listener.Start();
            }

            SetupTorrentWatcher();

            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        private void SetupTorrentWatcher()
        {
            watcher = new TorrentFolderWatcher(Path.GetFullPath(TORRENT_DIR), "*.torrent");
            watcher.TorrentFound += delegate (object? sender, TorrentWatcherEventArgs e)
            {
                try
                {
                    // This is a hack to work around the issue where a file triggers the event
                    // before it has finished copying. As the filesystem still has an exclusive lock
                    // on the file, monotorrent can't access the file and throws an exception.
                    // The best way to handle this depends on the actual application. 
                    // Generally the solution is: Wait a few hundred milliseconds
                    // then try load the file.
                    Thread.Sleep(500);

                    Torrent t = Torrent.Load(e.TorrentPath);

                    // There is also a predefined 'InfoHashTrackable' MonoTorrent.Tracker which
                    // just stores the infohash and name of the torrent. This is all that the tracker
                    // needs to run. So if you want an ITrackable that "just works", then use InfoHashTrackable.

                    //ITrackable trackable = new InfoHashTrackable(t);
                    ITrackable trackable = new CustomITrackable(t);

                    // The lock is here because the TorrentFound event is asyncronous and I have
                    // to ensure that only 1 thread access the tracker at the same time.
                    lock (tracker)
                        tracker.Add(trackable);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error loading torrent from disk: {0}", ex.Message);
                    Debug.WriteLine("Stacktrace: {0}", ex.ToString());
                }
            };

            watcher.Start();
            watcher.ForceScan();
        }

        public static void StartTracker()
        {
            new MySimpleTracker();
        }
    }
}
