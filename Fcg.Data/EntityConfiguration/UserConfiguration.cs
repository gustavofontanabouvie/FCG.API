using Fcg.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Email).HasColumnType("varchar(200)");
        builder.Property(u => u.Name).HasColumnType("varchar(200)");
        builder.Property(u => u.Password).HasColumnType("varchar(200)");

        builder.HasData(new User
        {
            Id = 1,
            Email = "admin@user.com",
            //User@1234
            Password = "$2a$12$4v7rJsBJeZZ.Zivh/iTab.SGuxcNFWOOzKqZ34scDghOlmw3ImV3S",
            Name = "admin",
            Role = UserRole.Admin
        });
    }
}
