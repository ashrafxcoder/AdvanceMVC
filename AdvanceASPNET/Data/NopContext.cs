using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
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


    public class Note
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        /*A common scenario people ask about is preventing developers from modifying a particular
        property (e.g., PersonId) in code by setting its setter to private or internal.
         */
        public int NoteId { get; private set; }
        public string Text { get; set; }

        public class NoteConfiguration : EntityTypeConfiguration<Note>
        {
            public NoteConfiguration()
            {
                this.HasKey(note => note.NoteId);
                this.Property(note => note.Text).HasMaxLength(200);

                /*
                 * Custom Descriminator coulumn for Table Per Hierarchy Relationships for 
                 * different subclasses of a base type
                 * only possible through Fluent API
                 */

                //this.Map(m =>
                //{
                //    m.Requires("Text").HasValue("Standard");
                //});
            }
        }
    }

    public class CustomConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(
        string nameOrConnectionString)
        {
            var name = nameOrConnectionString
            .Split('.').Last()
            .Replace("Context", string.Empty);
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @".\SQLEXPRESS",
                InitialCatalog = name,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true
            };
            return new SqlConnection(builder.ToString());
        }
    }

    public class PromptForDropCreateDatabaseWhenModelChages<TContext> : IDatabaseInitializer<TContext>
                                                                                where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            // If the database exists and matches the model
            // there is nothing to do
            var exists = context.Database.Exists();
            if (exists && context.Database.CompatibleWithModel(true))
            {
                return;
            }
            // If the database exists and doesn't match the model
            // then prompt for input
            if (exists)
            {
                Console.WriteLine
                ("Existing database doesn't match the model!");
                Console.Write
                ("Do you want to drop and create the database? (Y/N): ");
                var res = Console.ReadKey();
                Console.WriteLine();
                if (!String.Equals(
                "Y",
                res.KeyChar.ToString(),
                StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                context.Database.Delete();
            }
            // Database either didn't exist or it didn't match
            // the model and the user chose to delete it
            context.Database.Create();
        }
    }

}