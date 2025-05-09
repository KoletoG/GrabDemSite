﻿using GrabDemSite.Models.DataModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserDataModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WithdrawDataModel> WithdrawDatas { get; set; }
        public DbSet<DepositDataModel> DepositDatas { get; set; }
        public DbSet<TaskDataModel> TaskDatas { get; set; } 
    }
}