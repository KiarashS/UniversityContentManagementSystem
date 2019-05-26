using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class ContentAudioViewModel
    {
        public long Id { get; set; }
        public string Caption { get; set; }
        public string Audioname { get; set; }
        public bool EnableControls { get; set; } = true;
        public bool EnableAutoplay { get; set; }
        public int? Priority { get; set; }


        public string GetMediaType
        {
            get
            {
                if (string.IsNullOrEmpty(Audioname))
                {
                    return "application/octet-stream";
                }

                if (Audioname.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "audio/mpeg";
                }
                else if (Audioname.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "audio/wav";
                }
                else if (Audioname.EndsWith(".ogg", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "audio/ogg";
                }
                else
                {
                    return "application/octet-stream";
                }
            }
        }
    }
}
