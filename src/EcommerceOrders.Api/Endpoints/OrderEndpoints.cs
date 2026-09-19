using EcommerceOrders.Application.DTOs.Orders;
using EcommerceOrders.Application.Services.Orders;
using EcommerceOrders.Domain.Enums;

namespace EcommerceOrders.Api.Endpoints
{
    public static class OrderEndpoints
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/orders")
                .WithTags("Orders");

            group.MapPost(
                    "/",
                    async (
                        CreateOrderRequest request,
                        CreateOrderService service,
                        CancellationToken cancellationToken) =>
                    {
                        var order = await service.ExecuteAsync(
                            request,
                            cancellationToken);

                        return Results.Created(
                            $"/api/v1/orders/{order.Id}",
                            order);
                    })
                .WithName("CreateOrder")
                .Produces<OrderResponse>(
                    StatusCodes.Status201Created)
                .ProducesProblem(
                    StatusCodes.Status400BadRequest);

            group.MapGet(
                    "/",
                    async (
                        OrderStatus? status,
                        ListOrdersService service,
                        CancellationToken cancellationToken) =>
                    {
                        var orders = await service.ExecuteAsync(
                            status,
                            cancellationToken);

                        return Results.Ok(orders);
                    })
                .WithName("ListOrders")
                .Produces<IReadOnlyList<OrderResponse>>(
                    StatusCodes.Status200OK);

            group.MapGet(
                    "/{id:int}",
                    async (
                        int id,
                        GetOrderService service,
                        CancellationToken cancellationToken) =>
                    {
                        var order = await service.ExecuteAsync(
                            id,
                            cancellationToken);

                        return Results.Ok(order);
                    })
                .WithName("GetOrderById")
                .Produces<OrderResponse>(
                    StatusCodes.Status200OK)
                .ProducesProblem(
                    StatusCodes.Status404NotFound);

            group.MapPut(
                    "/{id:int}",
                    async (
                        int id,
                        UpdateOrderRequest request,
                        UpdateOrderService service,
                        CancellationToken cancellationToken) =>
                    {
                        var order = await service.ExecuteAsync(
                            id,
                            request,
                            cancellationToken);

                        return Results.Ok(order);
                    })
                .WithName("UpdateOrder")
                .Produces<OrderResponse>(
                    StatusCodes.Status200OK)
                .ProducesProblem(
                    StatusCodes.Status400BadRequest)
                .ProducesProblem(
                    StatusCodes.Status404NotFound)
                .ProducesProblem(
                    StatusCodes.Status409Conflict);

            group.MapPost(
                    "/{id:int}/cancel",
                    async (
                        int id,
                        CancelOrderService service,
                        CancellationToken cancellationToken) =>
                    {
                        var order = await service.ExecuteAsync(
                            id,
                            cancellationToken);

                        return Results.Ok(order);
                    })
                .WithName("CancelOrder")
                .Produces<OrderResponse>(
                    StatusCodes.Status200OK)
                .ProducesProblem(
                    StatusCodes.Status404NotFound)
                .ProducesProblem(
                    StatusCodes.Status409Conflict);

            group.MapDelete(
                    "/{id:int}",
                    async (
                        int id,
                        DeleteOrderService service,
                        CancellationToken cancellationToken) =>
                    {
                        await service.ExecuteAsync(
                            id,
                            cancellationToken);

                        return Results.NoContent();
                    })
                .WithName("DeleteOrder")
                .Produces(
                    StatusCodes.Status204NoContent)
                .ProducesProblem(
                    StatusCodes.Status404NotFound);

            return app;
        }
    }
}
