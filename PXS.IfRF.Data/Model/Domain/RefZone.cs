using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("REF_ZONES")]
    public partial class RefZone
    {
        public RefZone()
        {
            RfAreas = new HashSet<RfArea>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }

        [Column("ZOM_ID", TypeName = "NUMBER(18)")]
        public long? ZomId { get; set; }

        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }

        [InverseProperty(nameof(RfArea.Zone))]
        public virtual ICollection<RfArea> RfAreas { get; set; }
    }
}
