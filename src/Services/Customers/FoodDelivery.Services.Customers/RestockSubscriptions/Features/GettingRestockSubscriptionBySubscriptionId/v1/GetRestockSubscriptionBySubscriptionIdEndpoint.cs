using AutoMapper;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Customers.RestockSubscriptions.Dtos.v1;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodDelivery.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionBySubscriptionId.v1;

internal class GetRestockSubscriptionBySubscriptionIdEndpoint
    : IQueryMinimalEndpoint<
        GetRestockSubscriptionBySubscriptionIdRequestParameters,
        Ok<GetRestockSubscriptionBySubscriptionIdResponse>,
        ValidationProblem,
        NotFoundHttpProblemResult,
        UnAuthorizedHttpProblemResult
    >
{
    public string GroupName => RestockSubscriptionsConfigs.Tag;
    public string PrefixRoute => RestockSubscriptionsConfigs.RestockSubscriptionsUrl;
    public double Version => 1.0;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapGet("/{restockSubscriptionId}", HandleAsync)
            .RequireAuthorization(CustomersConstants.Role.Admin)
            // .Produces<GetRestockSubscriptionBySubscriptionIdResponse>(StatusCodes.Status200OK)
            // .Produces<StatusCodeProblemDetails>(StatusCodes.Status401Unauthorized)
            // .Produces<StatusCodeProblemDetails>(StatusCodes.Status400BadRequest)
            // .Produces<StatusCodeProblemDetails>(StatusCodes.Status404NotFound)
            .WithName(nameof(GetRestockSubscriptionBySubscriptionId))
            .WithDisplayName(nameof(GetRestockSubscriptionBySubscriptionId).Humanize())
            .WithSummaryAndDescription(
                nameof(GetRestockSubscriptionBySubscriptionId).Humanize(),
                nameof(GetRestockSubscriptionBySubscriptionId).Humanize()
            );
    }

    public async Task<
        Results<
            Ok<GetRestockSubscriptionBySubscriptionIdResponse>,
            ValidationProblem,
            NotFoundHttpProblemResult,
            UnAuthorizedHttpProblemResult
        >
    > HandleAsync([AsParameters] GetRestockSubscriptionBySubscriptionIdRequestParameters requestParameters)
    {
        var (restockSubscriptionId, _, queryProcessor, mapper, cancellationToken) = requestParameters;
        var result = await queryProcessor.SendAsync(
            GetRestockSubscriptionBySubscriptionId.Of(restockSubscriptionId),
            cancellationToken
        );

        var response = new GetRestockSubscriptionBySubscriptionIdResponse(result.RestockSubscription);

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
        return TypedResults.Ok(response);
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetRestockSubscriptionBySubscriptionIdRequestParameters(
    [FromRoute] long RestockSubscriptionId,
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;

public record GetRestockSubscriptionBySubscriptionIdResponse(RestockSubscriptionDto RestockSubscription);
