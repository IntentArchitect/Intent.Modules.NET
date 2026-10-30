// Excerpt of Basislink's WebCommon context (2026-10-19): three of the four entities the authority audit found
// wrong. Their tables are named by convention, from the DbSet properties.
using Basislink.Data.Model.WebCommonEntities;
using Microsoft.EntityFrameworkCore;

namespace Basislink.Data.Model;

public partial class WebCommonDBContext : DbContext
{
    public virtual DbSet<Application_Basislink_Document_Doc> Application_Basislink_Document_Docs { get; set; }

    public virtual DbSet<Application_Basislink_Document_Log> Application_Basislink_Document_Logs { get; set; }

    public virtual DbSet<IntegrationServices_Logs> IntegrationServices_Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application_Basislink_Document_Doc>(entity =>
        {
            entity.Property(e => e.DocumentLink).IsUnicode(false);
            entity.Property(e => e.DocumentName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DocumentUpdated).HasColumnType("datetime");
        });

        modelBuilder.Entity<Application_Basislink_Document_Log>(entity =>
        {
            entity.Property(e => e.DateAccesptedRead).HasColumnType("datetime");
            entity.Property(e => e.MySenwesUser)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IntegrationServices_Logs>(entity =>
        {
            entity.Property(e => e.AuthorizationToken).IsUnicode(false);
            entity.Property(e => e.Machine).IsUnicode(false);
            entity.Property(e => e.RequestContentBody).IsUnicode(false);
            entity.Property(e => e.RequestHeaders).IsUnicode(false);
            entity.Property(e => e.RequestIpAddress).IsUnicode(false);
            entity.Property(e => e.RequestMethod).IsUnicode(false);
            entity.Property(e => e.RequestOrigin).IsUnicode(false);
            entity.Property(e => e.RequestReferrer).IsUnicode(false);
            entity.Property(e => e.RequestRouteData).IsUnicode(false);
            entity.Property(e => e.RequestSystem).IsUnicode(false);
            entity.Property(e => e.RequestTimestamp).HasColumnType("datetime");
            entity.Property(e => e.RequestUri).IsUnicode(false);
            entity.Property(e => e.ResponseContentType).IsUnicode(false);
            entity.Property(e => e.ResponseHeaders).IsUnicode(false);
            entity.Property(e => e.ResponseStatusCode).IsUnicode(false);
            entity.Property(e => e.ResponseTimestamp).IsUnicode(false);
        });
    }
}
