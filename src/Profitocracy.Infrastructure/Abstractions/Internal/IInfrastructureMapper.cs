namespace Profitocracy.Infrastructure.Abstractions.Internal;

internal interface IInfrastructureMapper<TDomain, TInfrastructure>
{
    public TDomain MapToDomain(TInfrastructure model);
    public TInfrastructure MapToModel(TDomain entity);
}