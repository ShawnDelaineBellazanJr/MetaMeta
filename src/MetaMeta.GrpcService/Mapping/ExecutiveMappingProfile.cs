using AutoMapper;
using MetaMeta.GrpcService.Protos;
using MetaMeta.Orchestration;

namespace MetaMeta.GrpcService.Mapping
{
    /// <summary>
    /// AutoMapper profile that defines mappings between gRPC request/response objects
    /// and their corresponding domain entities for the Executive Agent service.
    /// </summary>
    public class ExecutiveMappingProfile: Profile
    {
        /// <summary>
        /// Initializes a new instance of the ExecutiveMappingProfile class 
        /// with configured mappings for Executive Agent operations.
        /// </summary>
        public ExecutiveMappingProfile()
        {
            // Map from gRPC ExecutiveRequest to domain NorthStarDirective
            CreateMap<ExecutiveRequest, NorthStarDirective>();
        }
    }
} 