using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("RF_AREA")]
    public partial class RfArea: SpecificResource
    {
        public RfArea()
        {
            Pops = new HashSet<Pop>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }

        [Column("CODE")]
        [StringLength(255)]
        public string Code { get; set; }

        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("OWNER")]
        [StringLength(4)]
        public string Owner { get; set; }

        [Column("ZONE_ID", TypeName = "NUMBER(18)")]
        public long? ZoneId { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual Resource Resource { get; set; }

        [ ForeignKey(nameof(Owner))]
        public virtual RefPlOwner RefOwner { get; set; }

        [ ForeignKey(nameof(ZoneId))]
        [ InverseProperty(nameof(RefZone.RfAreas))]
        public virtual RefZone Zone { get; set; }

        [ InverseProperty(nameof(Pop.RfArea))]
        public virtual ICollection<Pop> Pops { get; set; }
    }
}
