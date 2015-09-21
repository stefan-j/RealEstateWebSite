﻿using Microsoft.AspNet.Identity.EntityFramework;

namespace TestSite.Models
{
    
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    /// <summary>
    /// Drunk and drugs
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
    }

    /// <summary>
    /// Insert comments like this yah
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}