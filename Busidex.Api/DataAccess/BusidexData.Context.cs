﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Busidex.Api.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class busidexEntities : DbContext
    {
        public busidexEntities()
            : base("name=busidexEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardAddress> CardAddresses { get; set; }
        public virtual DbSet<ExternalLink> ExternalLinks { get; set; }
    }
}
