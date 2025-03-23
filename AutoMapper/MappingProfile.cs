using System.Text.RegularExpressions;
using AutoMapper;
using DSEMachshipETL.Entities;
using DSEMachshipETL.Models.Xml;

namespace DSEMachshipETL.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // CreateMap<ItemXmlRequest, ConsignmentItem>();
            // .UseDestinationNamingConvention();

        // CreateMap<ConsignmentXmlRequest, Consignment>();
            //.UseDestinationNamingConvention();
    }
}