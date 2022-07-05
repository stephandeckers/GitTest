using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using PXS.IfRF.Supporting;
using PXS.IfRF.Services;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.BusinessLogic
{
	/// <summary>
	/// If connection type is/changes to 'CL-C' and has a lyfecyclestatus of 'A'
	/// Then
	/// Ensure that the operationalstatus of the 2 positions being connected is 'O'
	/// </summary>
	public class SubrackLogic : BusinessLogic
	{
		private readonly ISettingsService _settingsService = null;

		public SubrackLogic
		(
			ModelContext		ifrfContext
		,	ILoggerManager		logger
		,	ISettingsService	settingsService
		) : base(ifrfContext, logger)
		{
			this._settingsService = settingsService;
		}

		public override BusinessLogicResult AfterCreate(SpecificResource specificResource)
		{
			assignUniqueSeqNrPerPop( specificResource as Subrack);
			return ( null);
		}

		/// <summary date="27-09-2021, 13:44:18" author="S.Deckers">
		/// Assigns the unique seq nr per pop.
		/// </summary>
		/// <param name="subRack">The sub rack.</param>
		private void assignUniqueSeqNrPerPop( Subrack subRack)
		{
			string prefix = this._settingsService.GetSeqNrPrefix( Enum.SequenceNumbers.Subrack);

			Subrack item = this._ifrfContext.Subracks.Where( x => x.PopId == subRack.PopId && x.SeqNr != null).OrderByDescending( x => x.Id).FirstOrDefault();

			if( item == null)
			{ 
				subRack.SeqNr = $"{ prefix }1";
				return;
			}

			try
			{
				string[] parts = item.SeqNr.Split( prefix);

				long nextNum = System.Convert.ToInt64( parts[ parts.Length - 1]);
				nextNum++;

				subRack.SeqNr = $"{ prefix }{ nextNum}";
			}
			catch( System.Exception)
			{ 
			};
		}

		public override BusinessLogicResult AfterDelete(SpecificResource resource)
		{
			Subrack subRack = resource as Subrack;

			foreach (SrPosition position in subRack.SrPositions)
			{
				// first remove the internal connections
				IEnumerable<long> ConnectionFromIds = position.ConnectionFroms.Where(c => c.Type.Equals("INT")).Select(c => c.Id);
				IEnumerable<long> ConnectionToIds = position.ConnectionTos.Where(c => c.Type.Equals("INT")).Select(c => c.Id);
				_ifrfContext.RemoveRange(_ifrfContext.Connections.Where(c=> ConnectionFromIds.Contains(c.Id) || ConnectionToIds.Contains(c.Id)));
				_ifrfContext.RemoveRange(_ifrfContext.RsResources.Where(c => ConnectionFromIds.Contains(c.Id) || ConnectionToIds.Contains(c.Id)));

				// then remove the owned positions
				long positionId = position.Id;
				_ifrfContext.SrPositions.Remove(position);
				_ifrfContext.RsResources.Remove(_ifrfContext.RsResources.SingleOrDefault(r => r.Id == positionId));
			}

			return null;
		}

		public override BusinessLogicResult AfterUpdate(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return null;
		}

		public override BusinessLogicResult ValidateCreate( SpecificResource newEntity )
		{
			return null;
		}

		public override BusinessLogicResult ValidateDelete(SpecificResource resource)
		{
			BusinessLogicResult result = new BusinessLogicResult();

			Subrack subRack = resource as Subrack;

			foreach (SrPosition position in subRack.SrPositions)
			{
				if (position.ConnectionFroms.Any(c => c.Type != "INT")
					|| position.ConnectionTos.Any(c => c.Type != "INT"))
				{
					result.ErrorMessages.Add("One or more positions are still connected");
					return result;
				}
			}

			return result;
		}

		public override BusinessLogicResult ValidateEdit(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return null;
		}

		/// <summary date="15-09-2021, 11:04:48" author="S.Deckers">
		/// Creates the positions according to definitions found in metadata
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="spec_id">The spec identifier.</param>
		internal void CreateMetadataPositions
		( 
			long	subrack_id
		,	long	spec_id
		,	string	modified_by
		)
		{
			DbConnection connection = _ifrfContext.GetOpenConnection();
			DbCommand cmd = connection.CreateCommand();

			string sql =
@"
declare
 	tBuf	varchar( 1000);

	/**
	 * @name create_sr_positions
	 * @description Create positions according to metadata-definitions
	 */
 	
	procedure create_sr_positions
	(
		v_sr_id			in subrack.id%type
	, 	v_spec_id 		in subrack.spec_id%type
	,	v_created_by 	in rs_resource.created_by%type
	)
	as
		/**
		 * @name create_positions
		 * @description 
		 */

		procedure create_positions
		(
			v_sr_id1		in subrack.id%type
		, 	v_spec_id1 		in subrack.spec_id%type
		,	v_created_by1 	in rs_resource.created_by%type
		)
		as
			seq_nr		sr_position.seq_nr%type := 1;
			pos_type	sr_position%rowtype;
			
			cursor c is
				select	meta_spec_sr_position.id
					,	meta_spec_sr_position.sr_spec_id
					,	meta_spec_sr_position.type	sr_type			/* store in sr_position.type */
					,	meta_spec_sr_position.nr_positions			/* # positions to create */
					,	meta_spec_sr_position.pos_group				/* group to assign */
					,	meta_spec_sr_position.start_from			/* not to use */
					,	meta_spec_sr_position.rule					/* ? */
					,	meta_spec_sr_position.mask					/* ? */
					,	meta_spec_sr_position.prefix				/* not to use */
					,	meta_spec_sr_position.suffix				/* not to use */
					,	meta_spec_sr_position.rank					/* order to be used in FE */
					,	meta_spec_sr_position.status				/* use it or not */
					,	meta_spec_sr_position.optics				/* copy to sr_position.optics */
					,	meta_spec_sr_position.pxs_start_from		/* not to use */
					,	meta_spec_sr_position.pxs_rule				/* not to use */
					,	meta_spec_sr_position.pxs_mask				/* not to use */
					,	meta_spec_sr_position.pxs_prefix			/* not to use */
					,	meta_spec_sr_position.pxs_suffix			/* not to use */
					,	meta_spec_sr_position.position_list_sr		/* for each value, store in sr_position.position_id */
					,	meta_spec_sr_position.pxs_position_list_sr	/* for each value, store in sr_position.pxs_id */
					,	REGEXP_COUNT( meta_spec_sr_position.position_list_sr, ',') + 1 cnt1
					,	REGEXP_COUNT( meta_spec_sr_position.pxs_position_list_sr, ',') + 1 cnt2
					,	meta_spec_sr_pos_group.id			sr_ps_group_id
					,	meta_spec_sr_pos_group.sr_type 		sr_pos_group_sr_type		/* Filtering FE */
					,	meta_spec_sr_pos_group.sr_spec_id	sr_pos_group_spec			/* FK , use to retrieve */
					,	meta_spec_sr_pos_group.sr_pos_type								/* Filtering FE */
					,	meta_spec_sr_pos_group.sr_pos_spec_id							/* Filtering FE */
					,	meta_spec_sr_pos_group.group_prefix				/* group to assign if meta_spec_sr_position.pos_group is empty */
				from meta_spec_sr_position
					--left outer join meta_spec_sr_pos_group 	on meta_spec_sr_pos_group.sr_spec_id = v_spec_id1 and meta_spec_sr_pos_group.sr_pos_spec_id = meta_spec_sr_position.id
					inner join meta_spec_sr_pos_group 	on meta_spec_sr_pos_group.sr_spec_id = v_spec_id1 and meta_spec_sr_pos_group.sr_pos_spec_id = meta_spec_sr_position.id
				where 1=1
					and meta_spec_sr_position.sr_spec_id = v_spec_id1
				and 2=2
				order by meta_spec_sr_position.rank; --- fix2: more readable
				--order by rank;

				cursor c2( p1 varchar, p2 varchar) is
					with t as
					(
						select 	p1,	p2 from dual
					) select regexp_substr( p1,'[^,]+', 1, level) r1
						,	 regexp_substr( p2,'[^,]+', 1, level) r2
						from t
						connect by regexp_substr( p1, '[^,]+', 1, level) is not null;

				/**
				 * @name create_position
				 * @description create a single position
				 */

				procedure create_position
				(
					v_pos			in sr_position%rowtype
				,	v_created_by2 	in rs_resource.created_by%type
				)
				as
					pk 	integer;
				begin
					insert into rs_resource( type, 			operational_status, created_date, 	modified_date, 	created_by, 		modified_by)
									values ( 'Position',	'O',				sysdate,		sysdate,		v_created_by2,		v_created_by2)
									returning id into pk;

					insert into sr_position( id,	position_id,		type, 		pos_group, 		 utac, 			line_id, 		comments, 		optics, 		pxs_id, 		seq_nr, 		ga_id, 			subrack_id, 		spec_id)
									values ( pk,	v_pos.position_id,	v_pos.type,	v_pos.pos_group, v_pos.utac, 	v_pos.line_id, 	v_pos.comments, v_pos.optics, 	v_pos.pxs_id, 	v_pos.seq_nr, 	v_pos.ga_id, 	v_pos.subrack_id, 	v_pos.spec_id);
				end;
		begin

			for it in c
			loop
				tBuf := utl_lms.format_message( 'sr_id=%s', to_char( v_sr_id));
				tBuf := utl_lms.format_message( '%s, sr_spec_id=%s', 	tBuf, to_char( it.sr_spec_id));
				tBuf := utl_lms.format_message( '%s, id=%s', 			tBuf, to_char( it.id));
				--tBuf := utl_lms.format_message( '%s, spec_code=%s', 			tBuf, to_char( it.spec_code));
				tBuf := utl_lms.format_message( '%s, sr_type=%s', 			tBuf, to_char( it.sr_type));
				tBuf := utl_lms.format_message( '%s, pos_group=%s', 			tBuf, to_char( it.pos_group));
				tBuf := utl_lms.format_message( '%s, optics=%s', 			tBuf, to_char( it.optics));
				--tBuf := utl_lms.format_message( '%s, sr_type=%s', 			tBuf, to_char( it.sr_type));
				tBuf := utl_lms.format_message( '%s, sr_pos_type=%s', 			tBuf, to_char( it.sr_pos_type));
				tBuf := utl_lms.format_message( '%s, sr_pos_spec_id=%s', 			tBuf, to_char( it.sr_pos_spec_id));
				tBuf := utl_lms.format_message( '%s, group_prefix=%s', 			tBuf, to_char( it.group_prefix));
				--dbms_output.put_line( tBuf);
				--continue;
				
				if( it.cnt1 != it.cnt2) then
					raise_application_error( -20101, 'Unequal positions detected');
				end if;

				tBuf := utl_lms.format_message( 'position_list_sr=%s', to_char( it.position_list_sr));
				tBuf := utl_lms.format_message( '%s, pxs_position_list_sr=%s', 	tBuf, to_char( it.pxs_position_list_sr));
				--dbms_output.put_line( tBuf);
				
				--continue;
				
				for it2 in c2( p1 => it.position_list_sr, p2 => it.pxs_position_list_sr)
				loop
					pos_type.pos_group := null; --- fix1
					pos_type.type 	:= it.sr_type;
					
					--pos_type.pos_group		:= it.pos_group;

					if( it.pos_group is not null) then
						pos_type.pos_group	:= it.pos_group;
					end if;
					
					-- If there is data in meta_spec_sr_pos_group it has precedence (20210927 SDE)
					 
					if( it.group_prefix is not null) then
						pos_type.pos_group	:= it.group_prefix;
					end if;

					tBuf := utl_lms.format_message( 'seq_nr=%s', to_char( seq_nr));
					--tBuf := utl_lms.format_message( '%s, r1=%s', 	tBuf, to_char( it2.r1));
					--tBuf := utl_lms.format_message( '%s, r2=%s', 	tBuf, to_char( it2.r2));
					tBuf := utl_lms.format_message( '%s, type=%s', 	tBuf, to_char( pos_type.type));
					tBuf := utl_lms.format_message( '%s, pos_group=%s', 	tBuf, to_char( pos_type.pos_group));
					--dbms_output.put_line( tBuf);
					
					pos_type.subrack_id 	:= v_sr_id1;
					pos_type.spec_id 		:= it.id;
					pos_type.optics			:= it.optics;
					
					if( it.group_prefix is not null) then
						pos_type.position_id	:= it.group_prefix || '/' || it2.r1;
					else
						pos_type.position_id	:= it2.r1;
					end if;
					
					pos_type.pxs_id			:= it2.r2;
					pos_type.seq_nr			:= 'P' || seq_nr;
					
					create_position( v_pos => pos_type,	v_created_by2 => v_created_by1);

					seq_nr:=seq_nr+1;
				end loop;
				
			end loop;
		end;

		/**
		 * @name Create connections for splitters
		 * @description 
		 */

		procedure create_splitter_connections
		(
			v_sr_id1		in subrack.id%type
		,	v_created_by1 	in rs_resource.created_by%type
		)
		as
			i int := 1;
			
			cursor c is
				select 	i.id 		in_id
					,	i.type		in_type
					,	i.pos_group	in_group
					,	o.id		out_id
					,	o.type		out_type
					,	o.pos_group	out_group
					from sr_position i
						inner join sr_position o on o.subrack_id = v_sr_id1 
							and o.type = 'SPLO'
							and ( ( o.pos_group = i.pos_group) or ( o.pos_group is null and i.pos_group is null) )
				where 1=1
					and i.subrack_id = v_sr_id1
					and i.type = 'SPLI'
				and 2=2
				order by i.pos_group;

			/**
			 * @name create single connection 
			 * @description 
			 */
			 
			procedure create_splitter_connection
			(
				v_from 			in connection.from_id%type
			,	v_to 			in connection.from_id%type 
			,	v_created_by2 	in rs_resource.created_by%type
			)
			as
				pk 	integer;
			begin
				insert into rs_resource( type, 			operational_status, created_date, 	modified_date, 	created_by, 		modified_by)
								values ( 'Connection',	'O',				sysdate,		sysdate,		v_created_by2,		v_created_by2)
								returning id into pk;
				insert into connection ( id, 	from_id, 	to_id, 	type)
								values ( pk,	v_from,		v_to,	'INT');
			end;
		begin

			for it in c
			loop
				tBuf := utl_lms.format_message( 'i=%s', to_char( i));
				tBuf := utl_lms.format_message( '%s, in_id=%s', 	tBuf, to_char( it.in_id));
				tBuf := utl_lms.format_message( '%s, in_type=%s', 	tBuf, to_char( it.in_type));
				tBuf := utl_lms.format_message( '%s, in_group=%s', 	tBuf, to_char( it.in_group));
				tBuf := utl_lms.format_message( '%s, out_id=%s', 	tBuf, to_char( it.out_id));
				tBuf := utl_lms.format_message( '%s, out_type=%s', 		tBuf, to_char( it.out_type));
				tBuf := utl_lms.format_message( '%s, out_group=%s', 	tBuf, to_char( it.out_group));
				--dbms_output.put_line( tBuf);
			
				create_splitter_connection( v_from => it.in_id, v_to => it.out_id, v_created_by2 => v_created_by1);
				
				i := i + 1;
			end loop;
		end;
		
	begin
		create_positions			( v_sr_id1 => v_sr_id, v_spec_id1 => v_spec_id, v_created_by1 => v_created_by);
		create_splitter_connections	( v_sr_id1 => v_sr_id, v_created_by1 => v_created_by);
	end;
begin
	create_sr_positions(v_sr_id => :v_sr_id, v_spec_id => :v_spec_id, v_created_by => :v_created_by);
	
/*
	declare
		spec_id int;
	begin
		--spec_id := 12; 
		--spec_id := 13;
		--spec_id := 14;	-- OLT group defined in meta_spec_sr_pos_group
		--spec_id := 15;
		spec_id := 16; -- Splitter group defined in meta_spec_sr_position
		--spec_id := 17; -- Splitter, no group
		spec_id := 16;
		create_sr_positions( v_sr_id => 20, v_spec_id => spec_id,  v_created_by => 'SDE');
	end;
*/
end;
";

			cmd.CommandText = sql;

			cmd.Parameters.Add(new OracleParameter() { ParameterName = "v_sr_id", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = subrack_id });
			cmd.Parameters.Add(new OracleParameter() { ParameterName = "v_spec_id", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = spec_id });
			cmd.Parameters.Add(new OracleParameter() { ParameterName = "v_created_by", DbType = DbType.String, Direction = ParameterDirection.Input, Value = modified_by });

			cmd.ExecuteNonQuery();
		}

		/// <summary date="15-09-2021, 11:04:48" author="S.Deckers">
		/// Creates the positions according to definitions found in metadata
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="spec_id">The spec identifier.</param>
		internal void CreateCustomPositions
		( 
			long	v_sr_id
		,	long	v_pos_spec_id
		,	string	v_group
		,	string	v_operational_status
		,	string	v_created_by
		)
		{
			DbConnection connection = _ifrfContext.GetOpenConnection();
			DbCommand cmd = connection.CreateCommand();

			string sql =
@"
declare
 	tBuf	varchar( 1000);

	/**
	 * @name create_sr_positions
	 * @description Create positions according to metadata-definitions
	 */
 	
	procedure create_sr_positions
	(
		v_sr_id					in subrack.id%type
	, 	v_spec_id 				in subrack.spec_id%type
	, 	v_group 				in varchar
	, 	v_operational_status 	in rs_resource.operational_status%type
	,	v_created_by 			in rs_resource.created_by%type
	)
	as
		/**
		 * @name create_positions
		 * @description 
		 */

		procedure create_positions
		(
			v_sr_id1				in subrack.id%type
		, 	v_spec_id1 				in sr_position.spec_id%type
		, 	v_group1 				in varchar
		, 	v_operational_status1 	in rs_resource.operational_status%type
		,	v_created_by1 			in rs_resource.created_by%type
		)
		as
			seq_nr		sr_position.seq_nr%type := 1;
			pos_type	sr_position%rowtype;
			
			cursor c is
				select	meta_spec_sr_position.id
					,	meta_spec_sr_position.spec_code				/* spec_code shown by FE */	
					,	meta_spec_sr_position.type	sr_type			/* store in sr_position.type */
					,	meta_spec_sr_position.nr_positions			/* # positions to create */
					,	meta_spec_sr_position.pos_group				/* group to assign */
					,	meta_spec_sr_position.start_from			/* not to use */
					,	meta_spec_sr_position.rule					/* ? */
					,	meta_spec_sr_position.mask					/* ? */
					,	meta_spec_sr_position.prefix				/* not to use */
					,	meta_spec_sr_position.suffix				/* not to use */
					,	meta_spec_sr_position.rank					/* order to be used in FE */
					,	meta_spec_sr_position.status				/* use it or not */
					,	meta_spec_sr_position.optics				/* copy to sr_position.optics */
					,	meta_spec_sr_position.pxs_start_from		/* not to use */
					,	meta_spec_sr_position.pxs_rule				/* not to use */
					,	meta_spec_sr_position.pxs_mask				/* not to use */
					,	meta_spec_sr_position.pxs_prefix			/* not to use */
					,	meta_spec_sr_position.pxs_suffix			/* not to use */
					,	meta_spec_sr_position.position_list_sr		/* for each value, store in sr_position.position_id */
					,	meta_spec_sr_position.pxs_position_list_sr	/* for each value, store in sr_position.pxs_id */
					,	REGEXP_COUNT( meta_spec_sr_position.position_list_sr, ',') + 1 cnt1
					,	REGEXP_COUNT( meta_spec_sr_position.pxs_position_list_sr, ',') + 1 cnt2
					,	meta_spec_sr_position.sr_spec_id
				from meta_spec_sr_position
				where 1=1
					and meta_spec_sr_position.type not in ( 'SPLI', 'SPLO')
					and meta_spec_sr_position.id = v_spec_id1
				and 2=2
				order by rank;
	
				cursor c2( p1 varchar, p2 varchar) is
					with t as
					(
						select 	p1,	p2 from dual
					) select regexp_substr( p1,'[^,]+', 1, level) r1
						,	 regexp_substr( p2,'[^,]+', 1, level) r2
						from t
						connect by regexp_substr( p1, '[^,]+', 1, level) is not null;

				/**
				 * @name create_position
				 * @description create a single position
				 */

				procedure create_position
				(
					v_pos					in sr_position%rowtype
				,	v_operational_status2	in rs_resource.operational_status%type
				,	v_created_by2 			in rs_resource.created_by%type
				)
				as
					pk 	integer;
				begin
					insert into rs_resource( type, 			operational_status, 	created_date, 	modified_date, 	created_by, 		modified_by)
									values ( 'Position',	v_operational_status2,	sysdate,		sysdate,		v_created_by2,		v_created_by2)
									returning id into pk;

					insert into sr_position( id,	position_id,		type, 		pos_group, 		 utac, 			line_id, 		comments, 		optics, 		pxs_id, 		seq_nr, 		ga_id, 			subrack_id, 		spec_id)
									values ( pk,	v_pos.position_id,	v_pos.type,	v_pos.pos_group, v_pos.utac, 	v_pos.line_id, 	v_pos.comments, v_pos.optics, 	v_pos.pxs_id, 	v_pos.seq_nr, 	v_pos.ga_id, 	v_pos.subrack_id, 	v_pos.spec_id);
				end;
		begin

			for it in c
			loop
				tBuf := utl_lms.format_message( 'id=%s', 						to_char( it.id));
				tBuf := utl_lms.format_message( '%s, spec_code=[%s]', 			tBuf, to_char( it.spec_code));
				tBuf := utl_lms.format_message( '%s, grp=[%s]', 				tBuf, to_char( v_group1));				
				tBuf := utl_lms.format_message( '%s, id=%s', 					tBuf, to_char( it.id));
				tBuf := utl_lms.format_message( '%s, sr_type=%s', 				tBuf, to_char( it.sr_type));
				tBuf := utl_lms.format_message( '%s, nr_positions=%s', 			tBuf, to_char( it.nr_positions));
				tBuf := utl_lms.format_message( '%s, pos_group=%s', 			tBuf, to_char( it.pos_group));
				tBuf := utl_lms.format_message( '%s, optics=%s', 				tBuf, to_char( it.optics));					
				tBuf := utl_lms.format_message( '%s, start_from=%s', 			tBuf, to_char( it.start_from));
				tBuf := utl_lms.format_message( '%s, rule=%s', 					tBuf, to_char( it.rule));
				tBuf := utl_lms.format_message( '%s, mask=%s', 					tBuf, to_char( it.mask));
				tBuf := utl_lms.format_message( '%s, prefix=%s', 				tBuf, to_char( it.prefix));
				tBuf := utl_lms.format_message( '%s, suffix=%s', 				tBuf, to_char( it.suffix));
				tBuf := utl_lms.format_message( '%s, rank=%s', 					tBuf, to_char( it.rank));
				tBuf := utl_lms.format_message( '%s, status=%s', 				tBuf, to_char( it.status));
				tBuf := utl_lms.format_message( '%s, optics=%s', 				tBuf, to_char( it.optics));
				tBuf := utl_lms.format_message( '%s, pxs_start_from=%s', 		tBuf, to_char( it.pxs_start_from));
				tBuf := utl_lms.format_message( '%s, pxs_rule=%s', 				tBuf, to_char( it.pxs_rule));
				tBuf := utl_lms.format_message( '%s, pxs_mask=%s', 				tBuf, to_char( it.pxs_mask));
				tBuf := utl_lms.format_message( '%s, pxs_prefix=%s', 			tBuf, to_char( it.pxs_prefix));
				tBuf := utl_lms.format_message( '%s, pxs_suffix=%s', 			tBuf, to_char( it.pxs_suffix));
				tBuf := utl_lms.format_message( '%s, position_list_sr=[%s]', 	tBuf, to_char( it.position_list_sr));
				tBuf := utl_lms.format_message( '%s, pxs_position_list_sr=[%s]',tBuf, to_char( it.pxs_position_list_sr));
				tBuf := utl_lms.format_message( '%s, cnt1=%s', 					tBuf, to_char( it.cnt1));
				tBuf := utl_lms.format_message( '%s, cnt2=%s', 					tBuf, to_char( it.cnt2));
				tBuf := utl_lms.format_message( '%s, sr_spec_id=%s', 			tBuf, to_char( it.sr_spec_id));				
				--dbms_output.put_line( tBuf);
				--continue;
				
				if( it.cnt1 != it.cnt2) then
					raise_application_error( -20101, 'Unequal positions detected');
				end if;

				tBuf := utl_lms.format_message( 'position_list_sr=%s', to_char( it.position_list_sr));
				tBuf := utl_lms.format_message( '%s, pxs_position_list_sr=%s', 	tBuf, to_char( it.pxs_position_list_sr));
				--dbms_output.put_line( tBuf);
				
				--continue;
				
				for it2 in c2( p1 => it.position_list_sr, p2 => it.pxs_position_list_sr)
				loop
					pos_type.type 	:= it.sr_type;				

					tBuf := utl_lms.format_message( 'seq_nr=%s', to_char( seq_nr));
					tBuf := utl_lms.format_message( '%s, p1=%s', 	tBuf, to_char( it2.r1));
					tBuf := utl_lms.format_message( '%s, p2=%s', 	tBuf, to_char( it2.r2));
					tBuf := utl_lms.format_message( '%s, type=%s', 	tBuf, to_char( pos_type.type));
					--tBuf := utl_lms.format_message( '%s, pos_group=%s', 	tBuf, to_char( pos_type.pos_group));
					dbms_output.put_line( tBuf);
					
					pos_type.subrack_id 	:= v_sr_id1;
					pos_type.spec_id 		:= it.id;
					pos_type.optics			:= it.optics;
					
					pos_type.pxs_id			:= it2.r2;
					--pos_type.seq_nr			:= seq_nr;
					pos_type.seq_nr			:= 'P' || seq_nr;
					pos_type.pos_group		:= v_group;
					--pos_type.position_id	:= it2.r1;
					if( v_group1 is not null) then
						pos_type.position_id	:= v_group1 || '/' || it2.r1;
					else
						pos_type.position_id	:= it2.r1;
					end if;	
					
					create_position( v_pos => pos_type,	v_operational_status2 => v_operational_status1, v_created_by2 => v_created_by1);

					seq_nr:=seq_nr+1;
				end loop;
				
			end loop;
		end;
		
	begin
		create_positions( v_sr_id1 => v_sr_id, v_spec_id1 => v_spec_id, v_group1 => v_group, v_operational_status1 => v_operational_status, v_created_by1 => v_created_by);
	end create_sr_positions;
begin
	create_sr_positions(v_sr_id => :v_sr_id, v_spec_id => :v_spec_id, v_group => :v_group, v_operational_status => :v_operational_status, v_created_by => :v_created_by);
	
	/*
	declare
		spec_id int;
	begin
		spec_id := 31; -- 31 OLT_LT16_GPON
		create_sr_positions( v_sr_id => 20, v_spec_id => spec_id, v_group => 'GPON', v_operational_status => 'O', v_created_by => 'SDE');
	end;
	*/
end;
";

			cmd.CommandText = sql;
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "v_sr_id",				DbType = DbType.Int64,	Direction = ParameterDirection.Input, Value = v_sr_id });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "v_spec_id",			DbType = DbType.Int64,	Direction = ParameterDirection.Input, Value = v_pos_spec_id });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "v_group",				DbType = DbType.String, Direction = ParameterDirection.Input, Value = v_group });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "v_operational_status",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = v_operational_status });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "v_created_by",			DbType = DbType.String, Direction = ParameterDirection.Input, Value = v_created_by });

			cmd.ExecuteNonQuery();
		}
	}
}
