using Microsoft.AspNet.OData;
using PXS.IfRF.Data.Model;
using PXS.IfRF.ErrorHandling;
using PXS.IfRF.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.BusinessLogic;
using PXS.IfRF.Supporting;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.Services
{
    public class ResourceService : IResourceService
    {
        private readonly ModelContext	_ifrfContext	= null;
		private readonly ILoggerManager _logger			= null;
        private readonly ResourceLogic _resourceLogic;
        private readonly HttpContext _httpContext;

        public ResourceService(ModelContext ifrfContext, ILoggerManager logger, IHttpContextAccessor httpContextAccessor)
        {
            _ifrfContext	= ifrfContext;
			_logger         = logger;
            _resourceLogic = new ResourceLogic(_ifrfContext, _logger, httpContextAccessor);
            _httpContext = httpContextAccessor.HttpContext;
        }

		public Resource Get( long id)
		{
			return _ifrfContext.RsResources.SingleOrDefault(a => a.Id == id);
		}

        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<Resource> GetResources()
        {
            return _ifrfContext.RsResources;
        }

        public bool DeleteResource(long id)
        {
            Resource resource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);

            if (resource == null)
            {
                return false;
            }

            string resourceType = resource.Type;

            SpecificResource specificResource = null;
			switch( resourceType)
			{
				// --- RsPop

				case nameof( Pop):
				{
					specificResource = _ifrfContext.Pops.SingleOrDefault( x => x.Id == id );
					break;
				}

				// --- RsRfArea

				case nameof( RfArea):
				{
					specificResource = _ifrfContext.RfAreas.SingleOrDefault( x => x.Id == id );
					break;
				}

				// --- RsSubrack

				case nameof( Subrack ):
				{
					specificResource = _ifrfContext.Subracks.SingleOrDefault( x => x.Id == id );
					break;
				}

				// --- RsConnection

				case nameof( Connection):
				{
					specificResource = _ifrfContext.Connections.SingleOrDefault( x => x.Id == id );
					break;
				}

				// --- RsPosition

				case nameof( SrPosition):
				{
					specificResource = _ifrfContext.SrPositions.SingleOrDefault( x => x.Id == id );
					break;
				}
                case nameof(RackSpace):
                {
                    specificResource = _ifrfContext.RackSpaces.SingleOrDefault(x => x.Id == id);
                    break;
                }

                // --- Unsupported 

                default:
				{
					throw new ServiceException( $"Unknown type: ${resourceType}" );
				}
			}

			if (specificResource == null)
			{
				return false;
			}

			// --- Item found, remove it

			_ifrfContext.Remove(specificResource);
            _ifrfContext.Remove(resource);
            _ifrfContext.SaveChanges();

            return true;
        }

        public Resource CreateNewResource(SpecificResource specificResource)
        {
            Resource resource = new Resource()
            {
                Type = specificResource.GetType().Name
            };

            _resourceLogic.AfterCreate(resource);
            
			return resource;
        }

        public void UpdateResourceMetadataFields(Resource resource)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            //_httpContext.Dump ();

            //foreach( var item in _httpContext.Items)
            //{
            //    d.WriteLine( item.ToString());
            //}
            
            //if( _httpContext.Items["iv-user"] == null)
            //{
            //    return;
            //}

			//_httpContext.Request.Headers[ ];
            //string s = _httpContext.Items["iv-user"].ToString();
            //resource.SetModified((string)(_httpContext.Items["iv-user"]), DateTime.Now);

			string user = _httpContext.Request.Headers.getUsername( );
			resource.SetModified( user, DateTime.Now);
        }

        public Resource PatchResource( long id, Delta<Resource> deltaResource)
        {
            Resource dbResource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);

            if (dbResource == null)
            {
                return null;
            }

            deltaResource.Patch(dbResource);
            _resourceLogic.AfterUpdate(dbResource, deltaResource.GetInstance(), deltaResource.GetChangedPropertyNames());
            
            //dbResource.SetModified((string)(_httpContext.Items["iv-user"]), DateTime.Now);
            string user = _httpContext.Request.Headers.getUsername( );
            dbResource.SetModified( user, DateTime.Now);

            _ifrfContext.SaveChanges();

            return dbResource;
        }

        public IEnumerable<ResourceCharacteristic> AddCharacteristics(long resourceId, IEnumerable<ResourceCharacteristic> newResourceCharacteristics)
        {
            Resource dbResource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == resourceId);

            if (dbResource == null)
            {
                return null;
            }

            foreach (ResourceCharacteristic aCharacteristics in newResourceCharacteristics)
            {
                aCharacteristics.Id = resourceId;
            }

            _ifrfContext.ResourceCharacteristics.AddRange(newResourceCharacteristics.ToArray());

			string user = _httpContext.Request.Headers.getUsername( );
            //dbResource.SetModified((string)(_httpContext.Items["iv-user"]), DateTime.Now);
			dbResource.SetModified( user, DateTime.Now);

            _ifrfContext.SaveChanges();

            IEnumerable<ResourceCharacteristic> allResourceCharacteristics =
                _ifrfContext.ResourceCharacteristics.Where(c => c.Id == resourceId);

            return allResourceCharacteristics;
        }
    }
}
