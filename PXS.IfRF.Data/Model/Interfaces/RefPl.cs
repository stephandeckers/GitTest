using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXS.IfRF.Data.Model
{
    public class RefPl
    {
        [Key]
        [Column("KEY")]
        [StringLength(4)]
        public string Key { get; set; }

        [Column("RANK", TypeName = "NUMBER(18)")]
        public long? Rank { get; set; }

        [Required]
        [Column("DISPLAY_NAME")]
        [StringLength(20)]
        public string DisplayName { get; set; }

        [Required]
        [Column("DESCRIPTION")]
        [StringLength(255)]
        public string Description { get; set; }

        [Column("STATUS", TypeName = "NUMBER(18)")]
        public long? Status { get; set; }

        [Column("OWNER")]
        [StringLength(4)]
        public virtual string Owner { get; set; }

        [Column("EXTERNAL_REF")]
        [StringLength(20)]
        public virtual string ExternalRef { get; set; }
    }
}
