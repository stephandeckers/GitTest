using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("REF_EQ_PORT_TYPES")]
    public partial class RefEqPortType: RefPl
    {
        [NotMapped]
        public override string ExternalRef { get; set; }

        [NotMapped]
        public override string Owner { get; set; }
    }
}
