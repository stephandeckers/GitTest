/**
 * @Name Response.cs
 * @Purpose 
 * @Date 28 September 2021, 07:06:46
 * @Author S.Deckers
 * @Description 

	Suppose we have the following network :

			                        -- 100 -- o 3690 01 | --- 2000 --- o 4000 (ONTP / Customer endpoint)
	    		                    -- 101 -- o 3691 02 | --- 2001 --- o 4001 (ONTP / Customer endpoint)
1062         898          368	    -- 102 -- o 3692 03 |
  o --- 24 --- o --- 25 --- o ----- -- 103 -- o 3693 04 | SPLO
GPON         PP          SPLI       -- 104 -- o 3694 05 |
			                        -- 105 -- o 3695 06 |
			                        -- 106 -- o 3696 07 |

ONTPPositionConnectionSearchResponse will hold the following information for ONTP 4000:

	ONTPPosition	: 4000
	ConnectionInfo	: 2000
	SPLOPosition	: 3690
	PPPosition		: 25
	OLTPPosition	: 1062

 */

namespace PXS.IfRF.Data.Model.ONTPPositionConnectionSearch
{
	#region -- Using directives --
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class ONTPPositionConnectionSearchResponse
	{
		public ResponseSrPosition	ONTPPosition			{ get; set; }
		public ResponseConnection	ConnectionInfo			{ get; set; }
		public ResponseSrPosition	SPLOPosition			{ get; set; }
		public ResponseSrPosition	OLTPPosition			{ get; set; }
		//public ResponseConnection	FeedingConnectionInfo	{ get; set; }
		public ResponseSrPosition	PPPosition				{ get; set; }
		//public ResponseConnection	PPPosition				{ get; set; }

		public static ResponseSrPosition Clone( SrPosition o, Resource r)
		{ 
			ResponseSrPosition item = new ResponseSrPosition( )
			{
				Id					= o.Id
			,	PositionId			= o.PositionId
			,	Type				= o.Type
			,	PosGroup			= o.PosGroup
			,	Utac				= o.Utac
			,	LineId				= o.LineId
			,	Comments			= o.Comments
			,	ConnectorType		= o.ConnectorType
			,	PxsId				= o.PxsId
			,	SeqNr				= o.SeqNr
			,	GaId				= o.GaId
			,	SubrackId			= o.SubrackId
			,	SpecId				= o.SpecId
			,	PositionName		= o.PositionName
			,	LifecycleStatus		= r.LifecycleStatus
			,	OperationalStatus	= r.OperationalStatus
			,	InternalOrderId		= r?.ResourceOrder?.Id
			,	OrderId				= r?.ResourceOrder?.OrderId
			,	ConnectionFroms		= o.ConnectionFroms
			,	ConnectionTos		= o.ConnectionTos
			,	SubrackInfo			= new SubrackInfo
				{
					Id  = o.SubrackId
				}
			};
			return( item);
		}

		public static ResponseConnection Clone( Connection o, Resource r, string positionName)
		{ 
			ResponseConnection item = new ResponseConnection( )
			{
				Id					= o.Id
			,	Attentuation		= o.Attentuation
			,	Comments			= o.Comments
			,	FromId				= o.FromId
			,	FromPopName			= o.FromPopName
			,	FromSrName			= o.FromSrName
			,	LineId				= o.LineId
			,	Nr					= o.Nr
			,	ToId				= o.ToId
			,	ToPopName			= o.ToPopName
			,	ToSrName			= o.ToSrName			
			,	LifecycleStatus		= r.LifecycleStatus			
			,	OperationalStatus	= r.OperationalStatus
			,	InternalOrderId		= r?.ResourceOrder?.Id
			,	OrderId				= r?.ResourceOrder?.OrderId
			,	PositionName		= positionName
			};
			return( item);
		}
	}
}