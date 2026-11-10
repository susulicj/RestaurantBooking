using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Application.Exceptions;

namespace RestaurantBooking.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Not found");
            await Write(context, StatusCodes.Status404NotFound, "Nije pronađeno", ex.Message);
        }
        catch (ConflictException ex)
        {
            logger.LogWarning(ex, "Conflict");
            await Write(context, StatusCodes.Status409Conflict, "Konflikt", ex.Message);
        }
        catch (BadRequestException ex)
        {
            logger.LogWarning(ex, "Bad request");
            await Write(context, StatusCodes.Status400BadRequest, "Neispravan zahtev", ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            logger.LogWarning(ex, "Unauthorized");
            await Write(context, StatusCodes.Status401Unauthorized, "Neautorizovan pristup", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await Write(context, StatusCodes.Status500InternalServerError,
                "Greška na serveru", "Došlo je do neočekivane greške.");
        }
    }

    private static Task Write(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        return context.Response.WriteAsJsonAsync(problem);
    }
}