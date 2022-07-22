using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PXS.IfRF.Data.Model
{
	[Table("SR_POSITION")]
	public partial class SrPosition: SpecificResource
	{
		public SrPosition()
		{
			ConnectionFroms = new HashSet<Connection>();
			ConnectionTos = new HashSet<Connection>();
		}

		[Key]
		[Column("ID", TypeName = "NUMBER(18)")]
		public long Id { get; set; }

		[Column("POSITION_ID")]
		[StringLength(255)]
		public string PositionId { get; set; }

		[Column("TYPE")]
		[StringLength(4)]
		public string Type { get; set; }

		[Column("POS_GROUP")]
		[StringLength(255)]
		public string PosGroup { get; set; }

		[Column("UTAC")]
		[StringLength(255)]
		public string Utac { get; set; }

		[Column("LINE_ID")]
		[StringLength(255)]
		public string LineId { get; set; }

		[Column("COMMENTS")]
		[StringLength(255)]
		public string Comments { get; set; }

		[Column("OPTICS")]
		[StringLength(4)]
		public string ConnectorType { get; set; }

		[Column("PXS_ID")]
		[StringLength(255)]
		public string PxsId { get; set; }

		[ Column("SEQ_NR")]
		[ StringLength(10)]
		public string SeqNr { get; set; }

		[Column("GA_ID", TypeName = "NUMBER(18)")]
		public long? GaId { get; set; }

		[Column("SUBRACK_ID", TypeName = "NUMBER(18)")]
		public long? SubrackId { get; set; }

		[Column("SPEC_ID", TypeName = "NUMBER(18)")]
		public long? SpecId { get; set; }

		[ NotMapped()]
		public string PositionName { get; set; }

		[ForeignKey(nameof(Id))]
		public virtual Resource Resource { get; set; }

		[ForeignKey(nameof(SpecId))]
		[InverseProperty(nameof(MetaSpecSrPosition.SrPositions))]
		public virtual MetaSpecSrPosition Spec { get; set; }

		[ForeignKey(nameof(SubrackId))]
		[InverseProperty("SrPositions")]
		public virtual Subrack Subrack { get; set; }

		[ForeignKey(nameof(Type))]
		public virtual RefPlPositionType RefPositionType { get; set; }

		[ForeignKey(nameof(ConnectorType))]
		public virtual RefPlConnectorType RefConnectorType { get; set; }

		[InverseProperty(nameof(Connection.From))]
		public virtual ICollection<Connection> ConnectionFroms { get; set; }

		[InverseProperty(nameof(Connection.To))]
		public virtual ICollection<Connection> ConnectionTos { get; set; }
	}
}