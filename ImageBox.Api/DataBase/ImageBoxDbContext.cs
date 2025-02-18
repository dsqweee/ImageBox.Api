﻿using ImageBox.Api.DataBase.Entity;
using Microsoft.EntityFrameworkCore;

namespace ImageBox.Api.DataBase;

public class ImageBoxDbContext(DbContextOptions<ImageBoxDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
                    .Property(x => x.Id)
                    .UseIdentityColumn(); // Increment users

        modelBuilder.Entity<ImageEntity>()
                    .Property(x=>x.Id)
                    .UseIdentityColumn(); // Increment images
    }

    public DbSet<UserEntity> users { get; set; }
    public DbSet<ImageEntity> images { get; set; }
}
