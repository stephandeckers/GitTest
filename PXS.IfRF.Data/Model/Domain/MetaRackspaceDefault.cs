using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("META_RACKSPACE_DEFAULTS")]
    public partial class MetaRackspaceDefault
    {
        [Key]
        [Column("FRAME_TYPE")]
        [StringLength(4)]
        public string FrameType { get; set; }
        
        [Column("POWER_NOMINAL", TypeName = "NUMBER(18)")]
        public long? PowerNominal { get; set; }
        
        [Column("POWER_PEAK", TypeName = "NUMBER(18)")]
        public long? PowerPeak { get; set; }
        
        [Column("LOUDNESS_NOMINAL", TypeName = "NUMBER(18)")]
        public long? LoudnessNominal { get; set; }
        
        [Column("LOUDNESS_PEAK", TypeName = "NUMBER(18)")]
        public long? LoudnessPeak { get; set; }

        [ForeignKey(nameof(FrameType))]
        public virtual RefPlFrameType RefFrameType { get; set; }
    }
}
