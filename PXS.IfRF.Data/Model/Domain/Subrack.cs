using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("SUBRACK")]
    public partial class Subrack: SpecificResource
    {
        public Subrack()
        {
            SrPositions = new HashSet<SrPosition>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }

		[ Column("SEQ_NR")]
		[ StringLength(10)]
		public string SeqNr { get; set; }

        [Column("SUBRACK_ID")]
        [StringLength(255)]
        public string SubrackId { get; set; }

        [Column("TYPE")]
        [StringLength(4)]
        public string Type { get; set; }

        [Column("COMMENTS")]
        [StringLength(255)]
        public string Comments { get; set; }

        [Column("PXS_ID")]
        [StringLength(255)]
        public string PxsId { get; set; }

        [Column("ASB_MATERIAL_CODE")]
        [StringLength(255)]
        public string AsbMaterialCode { get; set; }

        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("POP_ID", TypeName = "NUMBER(18)")]
        public long? PopId { get; set; }

        [Column("SPEC_ID", TypeName = "NUMBER(18)")]
        public long? SpecId { get; set; }

        [Column("RACKSPACE_ID", TypeName = "NUMBER(18)")]
        public long? RackSpaceId { get; set; }

        [Column("FROM_HU", TypeName = "NUMBER(18)")]
        public long? FromHu { get; set; }

        [Column("TO_HU", TypeName = "NUMBER(18)")]
        public long? ToHu { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual Resource Resource { get; set; }

        [ForeignKey(nameof(PopId))]
        [InverseProperty("Subracks")]
        public virtual Pop Pop { get; set; }

        [ForeignKey(nameof(RackSpaceId))]
        [InverseProperty("Subracks")]
        public virtual RackSpace RackSpace { get; set; }

        [ForeignKey(nameof(SpecId))]
        [InverseProperty(nameof(MetaSpecSubrack.Subracks))]
        public virtual MetaSpecSubrack Spec { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual RefPlSubrackType RefSubrackType { get; set; }

        [InverseProperty(nameof(SrPosition.Subrack))]
        public virtual ICollection<SrPosition> SrPositions { get; set; }
    }
}
