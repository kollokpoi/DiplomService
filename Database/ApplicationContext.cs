
using DiplomService.Models;
using DiplomService.Models.ChatsFolder;
using DiplomService.Models.EventsFolder.Division;
using DiplomService.Models.OrganizationFolder;
using DiplomService.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace DiplomService.Database
{
    public class ApplicationContext : IdentityDbContext<User>
    {

        public DbSet<EventApplication> EventApplications { get; set; }
        public DbSet<ApplicationData> ApplicationData { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserDeviceTokens> UserDeviceTokens { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Measure> Measures { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationApplication> OrganizationApplications { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<DivisionUsers> DivisionUsers { get; set; }
        public DbSet<MobileUser> MobileUsers { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<WebUser> WebUsers { get; set; }
        public DbSet<OrganizationUsers> OrganizationUsers { get; set; }
        public DbSet<MeasureDivisionsInfo> MeasureDivisionsInfos { get; set; }
        public ApplicationContext() { }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
              .UseLoggerFactory(LoggerFactory.Create(builder =>
              {
                  builder.AddConsole();
                  builder.AddDebug();
              }));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                  .HasMany(c => c.Organizations)
                  .WithMany(s => s.Events)
                  .UsingEntity(j => j.ToTable("EventOrganization"));

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => l.UserId);


            modelBuilder.Entity<Measure>()
                .HasOne(m => m.Event)
                .WithMany(e => e.Measures)
                .HasForeignKey(m => m.EventId);

            modelBuilder.Entity<Division>()
                .HasOne(d => d.Event)
                .WithMany(e => e.Divisions)
                .HasForeignKey(d => d.EventId);

            modelBuilder.Entity<MeasureDivisionsInfo>()
                    .HasOne(mdi => mdi.Division)
                    .WithMany(d => d.MeasureDivisionsInfos)
                    .HasForeignKey(mdi => mdi.DivisionId)
                    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MeasureDivisionsInfo>()
                .HasOne(mdi => mdi.Measure)
                .WithMany(m => m.MeasureDivisionsInfos)
                .HasForeignKey(mdi => mdi.MeasureId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationData>()
                .HasOne(ad => ad.Division)
                .WithMany()
                .HasForeignKey(ad => ad.DivisionId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
