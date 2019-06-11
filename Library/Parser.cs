using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using File = Library.Torrents.File;
using Library.Torrents;

namespace Library
{
    public class Parser
    {
        int blockSize;
        Dictionary<int?, Forum> ForumDictionary = new Dictionary<int?, Forum>();

        public Parser(int blockSize = 1000)
        {
            this.blockSize = blockSize;
        }

        public void Parse(string torrentPatch)
        {
            Console.WriteLine("-----------------------------------------------------Are we going----------------------------------------------------------");
            List<Torrent> torrents = new List<Torrent>();
            List<File> files = new List<File>();
            List<Forum> forums = new List<Forum>();
            int i = 0;
            Console.WriteLine("We start");

            foreach (var torrent in GetTorrent(torrentPatch))
            {
                i++;
                torrents.Add(torrent);
                foreach (var file in torrent.Files)
                {
                    files.Add(file);
                }
                if (torrent.Forum != null)
                {
                    forums.Add(torrent.Forum);
                }

                if (i % blockSize == 0)
                {
                    Load(files, torrents, forums, i);
                    Console.WriteLine("We added " + i + " torrents");
                    torrents = new List<Torrent>();
                    files = new List<File>();
                    forums = new List<Forum>();
                }
            }
            Load(files, torrents, forums , i);
        }
        void Load(IList<File> files, IList<Torrent> torrents, IList<Forum> forums , int i)
        {
            Console.WriteLine("We added " + i + " torrents");
            using (Repo context = new Repo())
            {
                context.Insert(forums);
                context.Insert(torrents, blockSize);
                context.Insert(files, blockSize * 5);
            }
        }
        IEnumerable<Torrent> GetTorrent(string torrentPatch)
        {
            Torrent torrent = new Torrent();
            bool torrentSwitch = true;
            using (var FileStream = new FileStream(torrentPatch, FileMode.Open))
            using (var gZipStream = new GZipStream(FileStream, CompressionMode.Decompress))
            using (var xmlReader = XmlReader.Create(gZipStream))
            {
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xmlReader.Name == "torrent")
                            {
                                if (torrentSwitch)
                                {
                                    torrent.TorrentId = int.Parse(xmlReader.GetAttribute("id"));
                                    torrent.RegistredAt = DateTime.Parse(xmlReader.GetAttribute("registred_at"));
                                    torrent.Size = xmlReader.GetAttribute("size");
                                    torrentSwitch = false;
                                    break;

                                }
                                torrent.Hash = xmlReader.GetAttribute("hash");
                                torrent.TrackerId = int.Parse(xmlReader.GetAttribute("tracker_id"));
                                torrentSwitch = true;
                                break;
                            }
                            if (xmlReader.Name == "title")
                            {
                                torrent.Title = xmlReader.ReadElementContentAsString();
                                break;
                            }
                            if (xmlReader.Name == "content")
                            {
                                torrent.Content = xmlReader.ReadElementContentAsString();
                                break;
                            }
                            if (xmlReader.Name == "forum")
                            {
                                torrent.ForumId = int.Parse(xmlReader.GetAttribute("id"));
                                if (ForumDictionary.ContainsKey(torrent.ForumId))
                                    break;
                                ForumDictionary.Add(torrent.ForumId, torrent.Forum);
                                torrent.Forum = new Forum()
                                {
                                    Id = torrent.ForumId.Value,
                                    Value = xmlReader.ReadElementContentAsString()
                                };
                                break;
                            }
                            if (xmlReader.Name == "del")
                            {
                                torrent.Del = true;
                                break;
                            }
                            if (xmlReader.Name == "file")
                            {
                                torrent.Files.Add(new File { Name = xmlReader.GetAttribute("name"), Size = xmlReader.GetAttribute("size"), TorrentId = torrent.TorrentId });
                                break;
                            }
                            if (xmlReader.Name == "dir")
                                torrent.Dir = xmlReader.GetAttribute("name");
                            break;


                        case XmlNodeType.EndElement:
                            if (xmlReader.Name == "torrent")
                            {
                                yield return torrent;
                                torrent = new Torrent();
                            }
                            break;
                    }
                }
            }
        }
    }
}
