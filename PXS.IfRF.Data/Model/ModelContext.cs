using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    public partial class ModelContext : DbContext
    {
        private readonly DbCommandInterceptor _dbCommandInterceptor;

        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }
        public ModelContext(DbCommandInterceptor dbCommandInterceptor)
        {
            _dbCommandInterceptor = dbCommandInterceptor;
        }

        public ModelContext(DbContextOptions<ModelContext> options, DbCommandInterceptor dbCommandInterceptor)
            : base(options)
        {
            _dbCommandInterceptor = dbCommandInterceptor;
        }

        public virtual DbSet<Connection>                Connections					{ get; set; }
        public virtual DbSet<MetaSpecPopAssembly>       MetaSpecPopAssemblies		{ get; set; }
        public virtual DbSet<MetaSpecPopSubrackMap>     MetaSpecPopSubrackMaps		{ get; set; }
        public virtual DbSet<MetaSpecSrPosGroup>        MetaSpecSrPosGroups			{ get; set; }
        public virtual DbSet<MetaSpecSrPosition>        MetaSpecSrPositions			{ get; set; }
        public virtual DbSet<MetaSpecSubrack>           MetaSpecSubracks			{ get; set; }
        public virtual DbSet<Pop>                       Pops						{ get; set; }
        public virtual DbSet<RackSpace>                 RackSpaces					{ get; set; }
        public virtual DbSet<RefEqPortType>             RefEqPortTypes				{ get; set; }
        public virtual DbSet<RefPlConnectionType>       RefPlConnectionTypes		{ get; set; }
        public virtual DbSet<RefPlConnectorType>        RefPlConnectorTypes			{ get; set; }
        public virtual DbSet<RefPlFrameType>            RefPlFrameTypes				{ get; set; }
        public virtual DbSet<RefPlLifecycleStatus>      RefPlLifecycleStatuses		{ get; set; }
        public virtual DbSet<RefPlOperationalStatus>    RefPlOperationalStatuses	{ get; set; }
        public virtual DbSet<RefPlOrderStatus>          RefPlOrderStatuses			{ get; set; }
        public virtual DbSet<RefPlOrderType>            RefPlOrderTypes				{ get; set; }
        public virtual DbSet<RefPlOwner>                RefPlOwners					{ get; set; }
        public virtual DbSet<RefPlPopModel>             RefPlPopModels				{ get; set; }
        public virtual DbSet<RefPlPopStatus>            RefPlPopStatuses			{ get; set; }
		public virtual DbSet<RefPlRoute>				RefPlRoutes					{ get; set; }
        public virtual DbSet<RefPlPopType>              RefPlPopTypes				{ get; set; }
        public virtual DbSet<RefPlPositionType>         RefPlPositionTypes			{ get; set; }
        public virtual DbSet<RefPlSubrackType>          RefPlSubrackTypes			{ get; set; }
        public virtual DbSet<RefZone>                   RefZones					{ get; set; }
        public virtual DbSet<ResourceCharacteristic>    ResourceCharacteristics		{ get; set; }
        public virtual DbSet<ResourceOrder>             ResourceOrders				{ get; set; }
        public virtual DbSet<RfArea>                    RfAreas						{ get; set; }
        public virtual DbSet<Resource>					RsResources					{ get; set; }
        public virtual DbSet<SrPosition>                SrPositions					{ get; set; }
        public virtual DbSet<Subrack>                   Subracks					{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("IFRF_SCHEMA")
                .HasAnnotation("Relational:Collation", "USING_NLS_COMP");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_dbCommandInterceptor);
        }
    }
}
