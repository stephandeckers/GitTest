using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("META_SPEC_POP_SUBRACK_MAP")]
    public partial class MetaSpecPopSubrackMap
    {
        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }
        [Column("SUBRACK_SPEC_ID", TypeName = "NUMBER(18)")]
        public long SubrackSpecId { get; set; }
        [Column("POP_ASSEMBLY_ID", TypeName = "NUMBER(18)")]
        public long PopAssemblyId { get; set; }
        [Column("HU_FROM", TypeName = "NUMBER(18)")]
        public long? HuFrom { get; set; }
        [Column("HU_TO", TypeName = "NUMBER(18)")]
        public long? HuTo { get; set; }

        [ForeignKey(nameof(PopAssemblyId))]
        [InverseProperty(nameof(MetaSpecPopAssembly.MetaSpecPopSubrackMaps))]
        public virtual MetaSpecPopAssembly PopAssembly { get; set; }
        [ForeignKey(nameof(SubrackSpecId))]
        [InverseProperty(nameof(MetaSpecSubrack.MetaSpecPopSubrackMaps))]
        public virtual MetaSpecSubrack SubrackSpec { get; set; }
    }
}
