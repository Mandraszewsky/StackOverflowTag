using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflow.Domain.Entities;

namespace StackOverflow.Infrastructure.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Count)
            .IsRequired();

        builder.Property(x => x.Percentage)
            .HasPrecision(10, 6);

        builder.Property(x => x.FetchedAt)
            .IsRequired();
    }
}
