﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string FilePath { get; set; } // or URI if using cloud storage
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsProfilePicture { get; set; }

        public string? RabbitId { get; set; }
        public Rabbit? Rabbit { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public Photo()
        {
            UploadDate = DateTime.Now;
        }
    }
}