using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("POP")]
    public partial class Pop: SpecificResource
    {
        public Pop()
        {
            InverseCentralPop = new HashSet<Pop>();
            RackSpaces = new HashSet<RackSpace>();
            Subracks = new HashSet<Subrack>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }

        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("TYPE")]
        [StringLength(4)]
        public string Type { get; set; }

        [Column("MODEL")]
        [StringLength(255)]
        public string Model { get; set; }

        [Column("COMMENTS")]
        [StringLength(255)]
        public string Comments { get; set; }

        [Column("STATUS")]
        [StringLength(4)]
        public string Status { get; set; }

        [Column("CENTRAL_POP_ID", TypeName = "NUMBER(18)")]
        public long? CentralPopId { get; set; }

        [Column("AP_CP_LENGTH", TypeName = "NUMBER(18)")]
        public long? ApCpLength { get; set; }

        [Column("NR_OF_LINES", TypeName = "NUMBER(18)")]
        public long? NrOfLines { get; set; }

        [Column("NODE")]
        [StringLength(255)]
        public string Node { get; set; }

        [Column("RF_AREA_ID", TypeName = "NUMBER(18)")]
        public long? RfAreaId { get; set; }

        [Column("POP_ASSEMBLY_ID", TypeName = "NUMBER(18)")]
        public long? PopAssemblyId { get; set; }

        [Column("LOCATION_NAME")]
        [StringLength(255)]
        public string LocationName { get; set; }
        
		[Column("STREET")]
        [StringLength(255)]
        public string Street { get; set; }
        
		[Column("NR")]
        [StringLength(255)]
        public string Nr { get; set; }

        [Column("SUFFIX")]
        [StringLength(255)]        
		public string Suffix { get; set; }

        [Column("SUBADDRESS")]
        [StringLength(255)]        
		public string Subaddress { get; set; }

        [Column("POSTCODE")]
        [StringLength(255)]        
		public string Postcode { get; set; }

        [Column("MUNICIPALITY")]
        [StringLength(255)]
        public string Municipality { get; set; }

        [Column("COOR_X", TypeName = "NUMBER(38)")]
        public decimal? CoorX { get; set; }

        [Column("COOR_Y", TypeName = "NUMBER(38)")]
        public decimal? CoorY { get; set; }

        [Column("GA_ID", TypeName = "NUMBER(18)")]
        public long? GaId { get; set; }
		
		[ForeignKey(nameof(Id))]
		public virtual Resource Resource { get; set; }

        [ForeignKey(nameof(CentralPopId))]
        [InverseProperty(nameof(Pop.InverseCentralPop))]
        public virtual Pop CentralPop { get; set; }

        [ForeignKey(nameof(Model))]
        public virtual RefPlPopModel RefPopModel { get; set; }

        [ForeignKey(nameof(PopAssemblyId))]
        [InverseProperty(nameof(MetaSpecPopAssembly.Pops))]
        public virtual MetaSpecPopAssembly PopAssembly { get; set; }

        [ForeignKey(nameof(RfAreaId))]
        [InverseProperty("Pops")]
        public virtual RfArea RfArea { get; set; }

        [ForeignKey(nameof(Status))]
        public virtual RefPlPopStatus RefStatus { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual RefPlPopType RefPopType { get; set; }

        [InverseProperty(nameof(Pop.CentralPop))]
        public virtual ICollection<Pop> InverseCentralPop { get; set; }

        [InverseProperty(nameof(RackSpace.Pop))]
        public virtual ICollection<RackSpace> RackSpaces { get; set; }

        [InverseProperty(nameof(Subrack.Pop))]
        public virtual ICollection<Subrack> Subracks { get; set; }

		[Column("POP_STATUS_MODIFIED")]
        public DateTime? PopStatusModified { get; set; }

        [Column("POP_STATUS_MODIFIED_BY")]
        [StringLength(200)]
        public string PopStatusModifiedBy { get; set; }
    }
}
