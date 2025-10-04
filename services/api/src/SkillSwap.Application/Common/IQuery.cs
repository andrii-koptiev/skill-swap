using MediatR;

namespace SkillSwap.Application.Common;

/// <summary>
/// Interface for queries that return a response
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the query</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse> { }
