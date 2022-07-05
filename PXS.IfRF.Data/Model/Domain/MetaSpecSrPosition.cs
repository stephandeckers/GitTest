using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("META_SPEC_SR_POSITION")]
    public partial class MetaSpecSrPosition
    {
        public MetaSpecSrPosition()
        {
            MetaSpecSrPosGroups = new HashSet<MetaSpecSrPosGroup>();
            SrPositions = new HashSet<SrPosition>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }
        
        [Column("SPEC_CODE")]
        [StringLength(255)]
        public string SpecCode { get; set; }
        
        [Column("TYPE")]
        [StringLength(4)]
        public string Type { get; set; }

        [Column("OPTICS")]
        [StringLength(4)]
        public string ConnectorType { get; set; }

        [Column("POS_GROUP")]
        [StringLength(255)]
        public string PosGroup { get; set; }
        [Column("NR_POSITIONS", TypeName = "NUMBER(18)")]
        public long? NrPositions { get; set; }
        [Column("START_FROM")]
        [StringLength(255)]
        public string StartFrom { get; set; }
        [Column("RULE")]
        [StringLength(255)]
        public string Rule { get; set; }
        [Column("MASK")]
        [StringLength(255)]
        public string Mask { get; set; }
        [Column("PREFIX")]
        [StringLength(255)]
        public string Prefix { get; set; }
        [Column("SUFFIX")]
        [StringLength(255)]
        public string Suffix { get; set; }
        [Column("PXS_START_FROM")]
        [StringLength(255)]
        public string PxsStartFrom { get; set; }
        [Column("PXS_RULE")]
        [StringLength(255)]
        public string PxsRule { get; set; }
        [Column("PXS_MASK")]
        [StringLength(255)]
        public string PxsMask { get; set; }
        [Column("PXS_PREFIX")]
        [StringLength(255)]
        public string PxsPrefix { get; set; }
        [Column("PXS_SUFFIX")]
        [StringLength(255)]
        public string PxsSuffix { get; set; }
        [Column("POSITION_LIST_SR")]
        [StringLength(255)]
        public string PositionListSr { get; set; }
        [Column("PXS_POSITION_LIST_SR")]
        [StringLength(255)]
        public string PxsPositionListSr { get; set; }
        [Column("RANK", TypeName = "NUMBER(18)")]
        public long? Rank { get; set; }
        [Column("STATUS")]
        [StringLength(4)]
        public string Status { get; set; }
        [Column("SR_SPEC_ID", TypeName = "NUMBER(18)")]
        public long SrSpecId { get; set; }

        [ForeignKey(nameof(SrSpecId))]
        [InverseProperty(nameof(MetaSpecSubrack.MetaSpecSrPositions))]
        public virtual MetaSpecSubrack SrSpec { get; set; }

        [ForeignKey(nameof(Status))]
        public virtual RefPlOperationalStatus RefStatus { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual RefPlPositionType RefSrPosType { get; set; }

        [ForeignKey(nameof(ConnectorType))]
        public virtual RefPlConnectorType RefConnectorType { get; set; }

        [InverseProperty(nameof(MetaSpecSrPosGroup.SrPosSpec))]
        public virtual ICollection<MetaSpecSrPosGroup> MetaSpecSrPosGroups { get; set; }

        [InverseProperty(nameof(SrPosition.Spec))]
        public virtual ICollection<SrPosition> SrPositions { get; set; }
    }
}
