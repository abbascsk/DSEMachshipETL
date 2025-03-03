using System.Text.RegularExpressions;
using AutoMapper;

namespace DSEConETL.AutoMapper;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination> UseDestinationNamingConvention<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var destinationProperties = typeof(TDestination).GetProperties();
        
        foreach (var sourceProp in sourceProperties)
        {
            var underscoreName = ConvertToUnderscore(sourceProp.Name);
            
            var destProp = Array.Find(destinationProperties, p => 
                string.Equals(p.Name, underscoreName, StringComparison.OrdinalIgnoreCase));
                
            if (destProp != null)
            {
                expression.ForMember(destProp.Name, opt => 
                    opt.MapFrom(src => sourceProp.GetValue(src, null)));
            }
        }
        
        return expression;
    }
    
    private static string ConvertToUnderscore(string name)
    {
        var result = Regex.Replace(name, @"(?<=[a-z0-9])([A-Z])", "_$1").ToLower();
        return result;
    }
}