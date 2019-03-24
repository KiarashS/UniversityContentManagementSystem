using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class User : IEntityType
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public long            Id                              { get; set; }
        public string          Email                           { get; set; }
        public string          Username                        { get; set; }
        public string          Password                        { get; set; }
        public string          DisplayName                     { get; set; }
        public bool            IsActive                        { get; set; }
        public DateTimeOffset? LastLogIn                       { get; set; }
        public string          LastIp                          { get; set; }
        public int?            PortalId                        { get; set; }
        public Portal          Portal                          { get; set; }
        public virtual         ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// every time the user changes his Password,
        /// or an admin changes his Roles or stat/IsActive,
        /// create a new `SerialNumber` GUID and store it in the DB.
        /// </summary>
        public string          SerialNumber                    { get; set; }
    }
}
