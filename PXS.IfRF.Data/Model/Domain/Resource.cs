using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("RS_RESOURCE")]
    public partial class Resource: SpecificResource
    {
        public Resource()
        {
            ResourceCharacteristics = new HashSet<ResourceCharacteristic>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }

        [Column("ORDER_ID", TypeName = "NUMBER(18)")]
        public long? OrderId { get; set; }

        [Column("TYPE")]
        [StringLength(255)]
        public string Type { get; set; }

        [Column("LIFECYCLE_STATUS")]
        [StringLength(4)]
        public string LifecycleStatus { get; set; }

        [Column("OPERATIONAL_STATUS")]
        [StringLength(4)]
        public string OperationalStatus { get; set; }

        [Column("CREATED_DATE", TypeName = "DATE")]
        public DateTime CreatedDate { get; set; }

        [Column("MODIFIED_DATE", TypeName = "DATE")]
        public DateTime ModifiedDate { get; set; }

        [Column("CREATED_BY")]
        [StringLength(200)]
        public string CreatedBy { get; set; }

        [Column("MODIFIED_BY")]
        [StringLength(200)]
        public string ModifiedBy { get; set; }

        [ForeignKey(nameof(LifecycleStatus))]
        public virtual RefPlLifecycleStatus RefLifecycleStatus { get; set; }

        [ForeignKey(nameof(OperationalStatus))]
        public virtual RefPlOperationalStatus RefOperationalStatus { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual ResourceOrder ResourceOrder { get; set; }

        [InverseProperty(nameof(ResourceCharacteristic.Resource))]
        public virtual ICollection<ResourceCharacteristic> ResourceCharacteristics { get; set; }

        public void SetCreated( string in_createdBy, DateTime in_createdDate)
        {
            CreatedBy		= in_createdBy;
            CreatedDate		= in_createdDate;
        }
        
        public void SetModified (string in_modifiedBy, DateTime in_modifiedDate)
        {
            ModifiedBy		= in_modifiedBy;
            ModifiedDate	= in_modifiedDate;
        }
    }
}
