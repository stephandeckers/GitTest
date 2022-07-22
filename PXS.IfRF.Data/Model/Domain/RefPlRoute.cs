/**
 * @Name RefPlOrderRoute.cs
 * @Purpose 
 * @Date 17 May 2022, 11:22:40
 * @Author S.Deckers
 * @Description 
 */

#nullable disable

namespace PXS.IfRF.Data.Model
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Microsoft.EntityFrameworkCore;
	#endregion

    [ Table("REF_PL_ROUTE")]
    public partial class RefPlRoute : RefPl
    {
        [ NotMapped()]
        public override string Owner { get; set; }
    }
}
