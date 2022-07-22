using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("META_SPEC_SR_POS_GROUP")]
    public partial class MetaSpecSrPosGroup
    {
        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }
        [Column("SR_TYPE")]
        [StringLength(4)]
        public string SrType { get; set; }
        [Column("SR_SPEC_ID", TypeName = "NUMBER(18)")]
        public long? SrSpecId { get; set; }
        [Column("SR_POS_TYPE")]
        [StringLength(4)]
        public string SrPosType { get; set; }
        [Column("SR_POS_SPEC_ID", TypeName = "NUMBER(18)")]
        public long? SrPosSpecId { get; set; }

        [ForeignKey(nameof(SrPosSpecId))]
        [InverseProperty(nameof(MetaSpecSrPosition.MetaSpecSrPosGroups))]
        public virtual MetaSpecSrPosition SrPosSpec { get; set; }

        [ForeignKey(nameof(SrPosType))]
        public virtual RefPlPositionType RefSrPosType { get; set; }

        [ForeignKey(nameof(SrSpecId))]
        [InverseProperty(nameof(MetaSpecSubrack.MetaSpecSrPosGroups))]
        public virtual MetaSpecSubrack SrSpec { get; set; }

        [ForeignKey(nameof(SrType))]
        public virtual RefPlSubrackType RefSrType { get; set; }
    }
}
