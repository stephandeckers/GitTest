using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("META_SPEC_POP_ASSEMBLY")]
    public partial class MetaSpecPopAssembly
    {
        public MetaSpecPopAssembly()
        {
            MetaSpecPopSubrackMaps = new HashSet<MetaSpecPopSubrackMap>();
            Pops = new HashSet<Pop>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }
        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("OWNER")]
        [StringLength(4)]
        public string Owner { get; set; }
        [Column("POP_TYPE")]
        [StringLength(4)]
        public string PopType { get; set; }
        [Column("RANK", TypeName = "NUMBER(18)")]
        public long? Rank { get; set; }
        [Column("ISDEFAULT", TypeName = "NUMBER(18)")]
        public long? Isdefault { get; set; }

        [ForeignKey(nameof(Owner))]
        public virtual RefPlOwner RefOwner { get; set; }

        [ForeignKey(nameof(PopType))]
        public virtual RefPlPopType RefPopType { get; set; }

        [InverseProperty(nameof(MetaSpecPopSubrackMap.PopAssembly))]
        public virtual ICollection<MetaSpecPopSubrackMap> MetaSpecPopSubrackMaps { get; set; }

        [InverseProperty(nameof(Pop.PopAssembly))]
        public virtual ICollection<Pop> Pops { get; set; }
    }
}
