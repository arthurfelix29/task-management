using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskList.Domain.Tasks;

namespace TaskList.Infrastructure.Persistence.Configurations;

internal sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        Guard.Against.Null(builder);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => new TaskId(value))
            .ValueGeneratedNever();

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasConversion(dto => dto.UtcTicks, ticks => new DateTimeOffset(ticks, TimeSpan.Zero))
            .IsRequired();

        builder.HasIndex(t => t.CreatedAt);
    }
}
