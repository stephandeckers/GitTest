using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
	[Table("CONNECTION")]
	public partial class Connection : SpecificResource
	{
		[Key]
		[Column("ID", TypeName = "NUMBER(18)")]
		public long Id { get; set; }

		[Column("FROM_ID", TypeName = "NUMBER(18)")]
		public long? FromId { get; set; }

		[Column("TO_ID", TypeName = "NUMBER(18)")]
		public long? ToId { get; set; }

		[Column("FROM_POP_NAME")]
		[StringLength(255)]		
		public string FromPopName { get; set; }

		[Column("FROM_SR_NAME")]
		[StringLength(255)]
		public string FromSrName { get; set; }

		[Column("TO_POP_NAME")]
		[StringLength(255)]
		public string ToPopName { get; set; }

		[Column("TO_SR_NAME")]
		[StringLength(255)]
		public string ToSrName { get; set; }

		[Column("LINE_ID")]
		[StringLength(255)]
		public string LineId { get; set; }

		[ Column("NR")]
		[ StringLength(10)]
		public string Nr { get; set; }

		[Column("ATTENTUATION", TypeName = "NUMBER(18)")]
		public long? Attentuation { get; set; }

		[Column("TYPE")]
		[StringLength(4)]
		public string Type { get; set; }

		[Column("COMMENTS")]
		[StringLength(255)]
		public string Comments { get; set; }

		[Column("ROUTE")]
		[StringLength(4)]
		public string Route { get; set; }

		[ForeignKey(nameof(Id))]
		public virtual Resource Resource { get; set; }

		[ForeignKey(nameof(FromId))]
		[InverseProperty(nameof(SrPosition.ConnectionFroms))]
		public virtual SrPosition From { get; set; }

		[ForeignKey(nameof(ToId))]
		[InverseProperty(nameof(SrPosition.ConnectionTos))]
		public virtual SrPosition To { get; set; }

		[ForeignKey(nameof(Type))]
		public virtual RefPlConnectionType RefConnectionType { get; set; }

		[ForeignKey(nameof(Route))]
		public virtual RefPlRoute RefPlRoute { get; set; }

		/// <summary date="18-07-2022, 11:03:10" author="S.Deckers">
		/// Gets or sets the name of the CustomerReference.
		/// </summary>
		/// 
		/// <remarks>Only used by JV2/UniFiber</remarks>
		[NotMapped( )]
		public string CustomerReference { get; set; }

		/// <summary date="18-07-2022, 11:03:10" author="S.Deckers">
		/// Gets or sets the name of the CustomerReference.
		/// </summary>
		/// 
		/// <remarks>Only used by JV2/UniFiber</remarks>
		//[NotMapped()]
		
		//public string CustomerReference
		//{ 
		//	get
		//	{
		//		return( _customerReference);
		//	}
		//	set
		//	{
		//		if( this.From != null)
		//		{
		//			if( this.From.Type == "PP")
		//			{
		//				//conToExportUN.CustomerReference = x.FromPopName + "-" + x.From?.Subrack?.RackSpace?.FrameId + "-" + x.FromSrName + "-" + x.fromPositionId;

		//				_customerReference = $"{this.FromPopName}-{this?.From?.Subrack?.RackSpace?.FrameId}-{this.FromSrName}-{this?.From?.PositionId}";
		//			}
		//		}

		//		if( this.To != null)
		//		{
		//			if( this.To.Type == "PP");
		//			{
		//				//conToExportUN.CustomerReference = x.ToPopName + "-" + x.To?.Subrack?.RackSpace?.FrameId + "-" + x.ToSrName + "-" + x.toPositionId;

		//				_customerReference = $"{this.ToPopName}-{this?.To?.Subrack?.RackSpace?.FrameId}-{this.ToSrName}-{this?.To?.PositionId}";
		//			}
		//		}				
		//	}
	}
}