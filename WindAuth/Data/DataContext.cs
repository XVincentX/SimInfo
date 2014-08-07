using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WindAuth.Models;

namespace WindAuth.Data
{
    public class DataContext : DbContext
    {
        public DbSet<LoggedUser> LoggedUsers { get; set; }
        public DbSet<NotificationUri> NotificationUris { get; set; }
        public DbSet<PayingUser> PayingUsers { get; set; }
    }
}