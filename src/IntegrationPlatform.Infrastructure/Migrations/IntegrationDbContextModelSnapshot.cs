using IntegrationPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
#nullable disable
namespace IntegrationPlatform.Infrastructure.Migrations
{
    [DbContext(typeof(IntegrationDbContext))]
    partial class IntegrationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");
            modelBuilder.Entity("IntegrationPlatform.Domain.Entities.Operation", b =>
                {
                    b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("TEXT");
                    b.Property<DateTime?>("CompletedAt").HasColumnType("TEXT");
                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
                    b.Property<string>("ErrorMessage").HasMaxLength(2000).HasColumnType("TEXT");
                    b.Property<bool>("IsAsync").HasColumnType("INTEGER");
                    b.Property<int>("MaxRetries").HasColumnType("INTEGER");
                    b.Property<string>("Payload").IsRequired().HasMaxLength(4000).HasColumnType("TEXT");
                    b.Property<string>("Result").HasMaxLength(8000).HasColumnType("TEXT");
                    b.Property<int>("RetryCount").HasColumnType("INTEGER");
                    b.Property<DateTime?>("StartedAt").HasColumnType("TEXT");
                    b.Property<int>("Status").HasColumnType("INTEGER");
                    b.Property<int>("TimeoutSeconds").HasColumnType("INTEGER");
                    b.Property<int>("Type").HasColumnType("INTEGER");
                    b.HasKey("Id");
                    b.ToTable("Operations");
                });
            modelBuilder.Entity("IntegrationPlatform.Domain.Entities.OperationHistory", b =>
                {
                    b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("TEXT");
                    b.Property<string>("Message").HasMaxLength(2000).HasColumnType("TEXT");
                    b.Property<Guid>("OperationId").HasColumnType("TEXT");
                    b.Property<int>("Status").HasColumnType("INTEGER");
                    b.Property<DateTime>("Timestamp").HasColumnType("TEXT");
                    b.HasKey("Id");
                    b.HasIndex("OperationId");
                    b.ToTable("OperationHistories");
                });
            modelBuilder.Entity("IntegrationPlatform.Domain.Entities.OperationHistory", b =>
                {
                    b.HasOne("IntegrationPlatform.Domain.Entities.Operation", "Operation").WithMany("History").HasForeignKey("OperationId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                    b.Navigation("Operation");
                });
            modelBuilder.Entity("IntegrationPlatform.Domain.Entities.Operation", b => { b.Navigation("History"); });
#pragma warning restore 612, 618
        }
    }
}