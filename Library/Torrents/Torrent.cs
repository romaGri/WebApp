using System;
using System.Collections.Generic;

namespace Library.Torrents
{
    public class Torrent : IRootInterface
    {
        public int Id { get; set; }
        public int TorrentId { get; set; }
        public DateTime RegistredAt { get; set; }
        public string Size { get; set; }
        public string Title { get; set; }
        public string Hash { get; set; }
        public int? TrackerId { get; set; }
        public string Content { get; set; }
        public string Dir { get; set; }
        public int? ForumId { get; set; }
        public Forum Forum { get; set; }
        public bool Del { get; set; }
        public ICollection<File> Files { get; set; }


        public Torrent()
        {
            Files = new HashSet<File>();
        }
    }
}
