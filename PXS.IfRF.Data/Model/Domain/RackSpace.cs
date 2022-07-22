using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("RACKSPACE")]
    public partial class RackSpace: SpecificResource
    {
        public RackSpace()
        {
            Subracks = new HashSet<Subrack>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }

        [Column("FRAME_ID")]
        [StringLength(255)]
        public string FrameId { get; set; }

        [Column("NR_HU", TypeName = "NUMBER(18)")]
        public long? NrHu { get; set; }

        [Column("FRAME_HU_T", TypeName = "NUMBER(18)")]
        public long? FrameHuT { get; set; }

        [Column("FRAME_HU_B", TypeName = "NUMBER(18)")]
        public long? FrameHuB { get; set; }

        [Required]
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

        [Column("POP_ID", TypeName = "NUMBER(18)")]
        public long PopId { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual Resource Resource { get; set; }

        [ForeignKey(nameof(FrameType))]
        public virtual RefPlFrameType RefFrameType { get; set; }

        //[ForeignKey(nameof(Id))]
        //[InverseProperty(nameof(Resource.RackSpace))]
        //public virtual Resource IdNavigation { get; set; }

        [ForeignKey(nameof(PopId))]
        public virtual Pop Pop { get; set; }

        [InverseProperty(nameof(Subrack.RackSpace))]
        public virtual ICollection<Subrack> Subracks { get; set; }
    }
}
