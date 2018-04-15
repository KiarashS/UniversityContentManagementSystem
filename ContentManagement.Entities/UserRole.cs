﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class UserRole : IEntityType
    {
        public long UserId       { get; set; }
        public int RoleId        { get; set; }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
