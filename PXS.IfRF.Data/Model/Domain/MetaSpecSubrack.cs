using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("META_SPEC_SUBRACK")]
    public partial class MetaSpecSubrack
    {
        public MetaSpecSubrack()
        {
            MetaSpecPopSubrackMaps = new HashSet<MetaSpecPopSubrackMap>();
            MetaSpecSrPosGroups = new HashSet<MetaSpecSrPosGroup>();
            MetaSpecSrPositions = new HashSet<MetaSpecSrPosition>();
            Subracks = new HashSet<Subrack>();
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
        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("STOCK_CODE")]
        [StringLength(255)]
        public string StockCode { get; set; }
        [Column("OWNER")]
        [StringLength(4)]
        public string Owner { get; set; }
        [Column("HEIGHT", TypeName = "NUMBER(18)")]
        public long? Height { get; set; }
        [Column("RANK", TypeName = "NUMBER(18)")]
        public long? Rank { get; set; }
        [Column("STATUS")]
        [StringLength(4)]
        public string Status { get; set; }

        [ForeignKey(nameof(Owner))]
        public virtual RefPlOwner RefOwner { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual RefPlSubrackType RefSubrackType { get; set; }

        [InverseProperty(nameof(MetaSpecPopSubrackMap.SubrackSpec))]
        public virtual ICollection<MetaSpecPopSubrackMap> MetaSpecPopSubrackMaps { get; set; }

        [InverseProperty(nameof(MetaSpecSrPosGroup.SrSpec))]
        public virtual ICollection<MetaSpecSrPosGroup> MetaSpecSrPosGroups { get; set; }

        [InverseProperty(nameof(MetaSpecSrPosition.SrSpec))]
        public virtual ICollection<MetaSpecSrPosition> MetaSpecSrPositions { get; set; }

        [InverseProperty(nameof(Subrack.Spec))]
        public virtual ICollection<Subrack> Subracks { get; set; }
    }
}
