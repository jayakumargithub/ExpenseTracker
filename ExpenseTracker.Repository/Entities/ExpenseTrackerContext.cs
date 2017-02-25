﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Entities
{
    public partial class ExpenseTrackerContext : DbContext
    {
        public ExpenseTrackerContext()
            : base("name=ExpenseTrackerContext")
        {
            
        }

        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpenseGroup> ExpenseGroups { get; set; }
        public virtual DbSet<ExpenseGroupStatus> ExpenseGroupStatusses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ExpenseGroup>()
                .HasMany(e => e.Expenses)
                .WithRequired(e => e.ExpenseGroup).WillCascadeOnDelete(); 

            modelBuilder.Entity<ExpenseGroupStatus>()
                .HasMany(e => e.ExpenseGroups)
                .WithRequired(e => e.ExpenseGroupStatus)
                .HasForeignKey(e => e.ExpenseGroupStatusId)
                .WillCascadeOnDelete(false);

        }
    }
}
