using System.Text.RegularExpressions;
using AutoMapper;
using DSEConETL.Entities;
using DSEConETL.Models.Xml;

namespace DSEConETL.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ConsignmentItemXmlRequest, ConsignmentItem>()
            .UseDestinationNamingConvention();
        
        CreateMap<ConsignmentXmlRequest, Consignment>()
            .UseDestinationNamingConvention();
    }
}