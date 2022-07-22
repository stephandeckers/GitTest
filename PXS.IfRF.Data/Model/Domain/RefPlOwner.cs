using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
    [Table("REF_PL_OWNER")]
    public partial class RefPlOwner : RefPl
    {
        [NotMapped]
        public override string Owner { get; set; }

        [ Column		( "SPARE_POSITIONS")]
        [ StringLength	( 20)]
        public string SparePositions { get; set; }
    }
}
