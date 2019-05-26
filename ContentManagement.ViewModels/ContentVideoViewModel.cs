using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class ContentVideoViewModel
    {
        public long Id { get; set; }
        public string Caption { get; set; }
        public string Videoname { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool EnableControls { get; set; } = true;
        public bool EnableAutoplay { get; set; }
        public int? Priority { get; set; }


        public string GetMediaType
        {
            get
            {
                if (string.IsNullOrEmpty(Videoname))
                {
                    return "application/octet-stream";
                }
                
                if (Videoname.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "video/mp4";
                }
                else if (Videoname.EndsWith(".webm", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "video/webm";
                }
                else if (Videoname.EndsWith(".ogg", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "video/ogg";
                }
                else
                {
                    return "application/octet-stream";
                }
            }
        }
    }
}
