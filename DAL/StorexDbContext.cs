using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnnotations;
using DAL.Interfaces;
using Domain.Contacts;
using Domain.Orders;
using Domain.Storage;
using Domain.Identity;
using NLog;
using DAL.EFConfiguration;
using DAL.Helpers;
using Domain;

namespace DAL
{
    public class StorexDbContext : DbContext, IDbContext
    {
        private readonly ILogger _logger;
        private readonly string _instanceId = Guid.NewGuid().ToString();
        private readonly IUserNameResolver _userNameResolver;


        public StorexDbContext(IUserNameResolver userNameResolver, ILogger logger) : base("StorexDbConnection")
        {
            _logger = logger;
            _userNameResolver = userNameResolver;

            _logger.Debug("Instance id: " + _instanceId);

            Database.SetInitializer(new DbInitializer());

#if DEBUG
            this.Database.Log = s => Trace.Write(s);
#endif
            this.Database.Log =
                s =>
                    _logger.Info((s.Contains("SELECT") || s.Contains("UPDATE") || s.Contains("DELETE") ||
                                  s.Contains("INSERT"))
                        ? "\n" + s.Trim()
                        : s.Trim());
        }

        //hack for mvc scaffolding, paramaterles constructor is required
        public StorexDbContext()
            : this(new UserNameResolver(() => "Anonymous"), NLog.LogManager.GetCurrentClassLogger())
        {

        }


        #region Identity tables

        public IDbSet<Role> Roles { get; set; }
        public IDbSet<UserClaim> UserClaims { get; set; }
        public IDbSet<UserLogin> UserLogins { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<UserRole> UserRoles { get; set; }

        #endregion

        #region Contacts tables

        public IDbSet<Contact> Contacts { get; set; }
        public IDbSet<ContactType> ContactTypes { get; set; }
        public IDbSet<Person> Persons { get; set; }
        public IDbSet<PersonType> PersonTypes { get; set; }

        #endregion

        #region Orders tables

        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderType> OrderTypes { get; set; }

        #endregion

        #region Storage tables

        public IDbSet<Product> Products { get; set; }
        public IDbSet<ProductCategory> ProductCategories { get; set; }
        public IDbSet<StoredProduct> StoredProducts { get; set; }

        #endregion


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            // remove tablename pluralizing
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // remove cascade delete
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Identity, PK - int 
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserClaimMap());
            modelBuilder.Configurations.Add(new UserLoginMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserRoleMap());

            Precision.ConfigureModelBuilder(modelBuilder);

            // convert all datetime and datetime? properties to datetime2 in ms sql
            // ms sql datetime has min value of 1753-01-01 00:00:00.000
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            // use Date type in ms sql, where [DataType(DataType.Date)] attribute is used
            modelBuilder.Properties<DateTime>()
                .Where(x => x.GetCustomAttributes(false).OfType<DataTypeAttribute>()
                    .Any(a => a.DataType == DataType.Date))
                .Configure(c => c.HasColumnType("date"));
        }

        public override int SaveChanges()
        {

            // Update metafields in entitys, that implement IBaseEntity - CreatedAtDT, CreatedBy, etc
            var entities =
                ChangeTracker.Entries()
                    .Where(
                        x =>
                            x.Entity is IBaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IBaseEntity)entity.Entity).CreatedAtDT = DateTime.Now;
                    ((IBaseEntity)entity.Entity).CreatedBy = _userNameResolver.CurrentUserName;
                }

                ((IBaseEntity)entity.Entity).ModifiedAtDT = DateTime.Now;
                ((IBaseEntity)entity.Entity).ModifiedBy = _userNameResolver.CurrentUserName;
            }

            // Custom exception - gives much more details why EF Validation failed
            // or watch this inside exception ((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                throw newException;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _logger.Info("Disposing: " + disposing + " _instanceId: " + _instanceId);
            base.Dispose(disposing);
        }
    }
}
