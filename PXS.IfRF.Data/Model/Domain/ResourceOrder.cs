using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("RESOURCE_ORDER")]
    public partial class ResourceOrder
    {
        public ResourceOrder()
        {
            RsResources = new HashSet<Resource>();
        }

        [Key]
        [Column("ID", TypeName = "NUMBER(18)")]
        public long Id { get; set; }
        [Column("TYPE")]
        [StringLength(4)]
        public string Type { get; set; }
        [Column("STATUS")]
        [StringLength(4)]
        public string Status { get; set; }
        [Column("ORDER_ID")]
        [StringLength(255)]
        public string OrderId { get; set; }
        [Column("CREATION_DATE")]
        public DateTime? CreationDate { get; set; }
        [Column("CLOSURE_DATE")]
        public DateTime? ClosureDate { get; set; }

        [ForeignKey(nameof(Status))]
        public virtual RefPlOrderStatus RefOrderStatus { get; set; }

        [ForeignKey(nameof(Type))]
        public virtual RefPlOrderType RefOrderType { get; set; }

        [InverseProperty(nameof(Resource.ResourceOrder))]
        public virtual ICollection<Resource> RsResources { get; set; }
    }
}
