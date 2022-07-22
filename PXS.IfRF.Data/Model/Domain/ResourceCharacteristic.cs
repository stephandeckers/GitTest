using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("RESOURCE_CHARACTERISTIC")]
    public partial class ResourceCharacteristic
    {
        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }
        [Column("RESOURCE_ID", TypeName = "NUMBER(18)")]
        public long ResourceId { get; set; }
        [Column("NAME")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("VALUE")]
        [StringLength(255)]
        public string Value { get; set; }
        [Column("VALUE_TYPE")]
        [StringLength(255)]
        public string ValueType { get; set; }

        [ForeignKey(nameof(ResourceId))]
        [InverseProperty( nameof( Model.Resource.ResourceCharacteristics))]
        public virtual Resource Resource { get; set; }
    }
}
