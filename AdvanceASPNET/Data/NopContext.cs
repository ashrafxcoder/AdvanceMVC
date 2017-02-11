using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web;
using AdvanceASPNET.Domain;

namespace AdvanceASPNET.Data
{
    public class NopContext : DbContext
    {
        //protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        //{
        //    return base.ValidateEntity(entityEntry, items);
        //}


        //protected override bool ShouldValidateEntity(DbEntityEntry entityEntry)
        //{
        //    return base.ShouldValidateEntity(entityEntry);
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<BlogPost>().ToTable("tbl_blog_post");

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !String.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                               && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            foreach (var type in typesToRegister)
            {
                dynamic configInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configInstance);

            }


            //modelBuilder.ComplexType<BillingAddress>()
            //            .Property(p => p.CreditCardNumber)
            //            .HasColumnName("CardNumber");
            //modelBuilder.ComplexType<Address>()
            //            .Property(p => p.StreetAddress)
            //            .HasColumnName("StreetAddress");
                                    


            ConfigurationRegistrar registrar = modelBuilder.Configurations;

            ConventionsConfiguration conventions = modelBuilder.Conventions;

            DbModel dbModel = modelBuilder.Build(this.Database.Connection);
            EdmModel edmModel = dbModel.ConceptualModel;
            EntityContainerMapping containerMapping = dbModel.ConceptualToStoreMapping;
            DbProviderInfo dbProviderInfo = dbModel.ProviderInfo;
            DbProviderManifest dbProviderManifest = dbModel.ProviderManifest;
            EdmModel dbModelStoreModel = dbModel.StoreModel;
            DbCompiledModel dbCompiledModel = dbModel.Compile();


            PropertyConventionConfiguration propertyConvention = modelBuilder.Properties();
            TypeConventionConfiguration typeConvention = modelBuilder.Types();
            //modelBuilder.RegisterEntityType();

            //registrar.AddFromAssembly()
            //conventions.AddFromAssembly();


            //propertyConvention
            //    .Where(info => info.GetType() is typeof(string))
            //    .Configure(cfg => cfg.HasMaxLength(200));
            //typeConvention.Configure(cfg => cfg.MapToStoredProcedures());


            base.OnModelCreating(modelBuilder);
        }


        public string CreateDatabseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }


        public void Test()
        {
            IObjectContextAdapter contextAdapter = this;
            // ReSharper disable once ExpressionIsAlwaysNull
            ObjectContext objectContext = contextAdapter as ObjectContext;
            // ReSharper disable once PossibleNullReferenceException
            objectContext.ExecuteStoreQueryAsync<BlogPost>("SELECT * FROM dbo.Posts WHERE Author = @p0", (string)null);

            DbCompiledModel compiledModel;

        }



    }

    [ComplexType]
    public class Address
    {
        public int AddressId { get; set; }
        [MaxLength(150)]
        [Column("StreetAddress")]
        public string StreetAddress { get; set; }
        [Column("City")]
        public string City { get; set; }
        [Column("State")]
        public string State { get; set; }
        [Column("ZipCode")]
        public string ZipCode { get; set; }
    }


    public class BillingAddress
    {
        public string StreatAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public string CreditCardType { get; set; }
        public string CreditCardNumber { get; set; }
    }


}