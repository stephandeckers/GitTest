using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("REF_PL_SUBRACK_TYPE")]
    public partial class RefPlSubrackType : RefPl
    {
        [NotMapped]
        public override string ExternalRef { get; set; }

        [NotMapped]
        public override string Owner { get; set; }
    }
}
