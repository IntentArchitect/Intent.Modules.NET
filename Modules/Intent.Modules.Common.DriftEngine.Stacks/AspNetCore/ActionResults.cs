using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// What an action or handler returns when its return type doesn't say: <c>IActionResult</c>, <c>ActionResult</c>, Web API 2's
/// <c>IHttpActionResult</c> or Minimal APIs' <c>IResult</c>, under <c>Task</c>. Each return is read for the success result it
/// builds, a controller's factory (<c>Ok(data)</c>), <c>Results</c>' or <c>TypedResults</c>', or a result it creates
/// (<c>new OkObjectResult(data)</c>), through locals, conditionals and the code's own methods that build it; what that result
/// carries is what the operation returns: <c>Ok(data)</c> is data's type, <c>NoContent()</c> nothing. An error result isn't
/// modelled. A result this doesn't read, or a body whose type the code doesn't say, is can't verify, never a guess.
/// </summary>
internal static class ActionResults
{
    private static readonly HashSet<string> Untyped = ["IActionResult", "ActionResult", "IHttpActionResult", "IResult"];

    /// <summary>What a factory or a result is called on: nothing (a controller's own), or Minimal APIs' result types.</summary>
    private static readonly HashSet<string> Receivers = ["this", "base", "Results", "TypedResults"];

    /// <summary>The factories of an error or a redirect, and the results they create, which the model doesn't state.</summary>
    private static readonly HashSet<string> Errors =
    [
        "NotFound", "BadRequest", "Unauthorized", "Forbid", "Conflict", "Problem", "ValidationProblem", "UnprocessableEntity", "Challenge",
        "InternalServerError", "Redirect", "RedirectPermanent", "RedirectToAction", "RedirectToRoute", "RedirectToPage", "LocalRedirect", "SignOut",
        "NotFoundResult", "NotFoundObjectResult", "BadRequestResult", "BadRequestObjectResult", "BadRequestErrorMessageResult", "UnauthorizedResult",
        "UnauthorizedObjectResult", "ForbidResult", "ConflictResult", "ConflictObjectResult", "UnprocessableEntityResult", "UnprocessableEntityObjectResult",
        "ChallengeResult", "InternalServerErrorResult", "ExceptionResult", "InvalidModelStateResult", "RedirectResult", "RedirectToActionResult",
        "RedirectToRouteResult", "LocalRedirectResult", "SignOutResult"
    ];

    private static readonly TypeRefNode Stream = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" };

    private static readonly TypeRefNode Bytes = new() { Text = "byte[]", Name = "byte", QualifiedName = "byte", IsArray = true };

    private static readonly TypeRefNode Text = new() { Text = "string", Name = "string", QualifiedName = "string" };

    private const int MaxDepth = 3;

    /// <summary>A return type that doesn't state what it succeeds with, under <c>Task</c> or <c>ValueTask</c>.</summary>
    public static bool IsUntyped(TypeRefNode type) =>
        (type is { Name: "Task" or "ValueTask", Arguments: [var awaited] } ? awaited : type) is { Arguments.Count: 0, IsArray: false } result && Untyped.Contains(result.Name);

    public static ClaimReading Read(ClaimContext context, MemberNode member)
    {
        var returns = member.Type!;
        if (member.Body is not { } body)
        {
            return ClaimReading.CantVerify("unreadable", $"`{member.Name}` returns `{returns.Text}`, which doesn't state the type of its body, and it has no body to read.", member.At);
        }

        var successes = new List<(TypeRefNode? Body, Span At)>();
        foreach (var value in body.Returns)
        {
            if (Value(body, value, value.At, 0, successes) is { } unread)
            {
                return ClaimReading.CantVerify(unread.Reason, unread.Message, unread.At);
            }
        }

        if (successes.Count == 0)
        {
            return ClaimReading.CantVerify("unreadable", $"`{member.Name}` returns `{returns.Text}`, which doesn't state the type of its body, and it builds no success result.", member.At);
        }

        var same = successes.All(x => context.ExpectedValue is ITypeReference model ? x.Body is not null && context.TypeMatches(model, x.Body) : x.Body is null);
        var actual = string.Join(" or ", successes.Select(x => x.Body?.Text ?? "no body").Distinct());
        return ClaimReading.Compared($"{returns.Text} with {actual}", successes[0].At, same);
    }

    private sealed record Unread(string Reason, string Message, Span At);

    /// <summary>The success a returned value builds, added to <paramref name="successes"/>; why it can't be read, when it can't.</summary>
    private static Unread? Value(FunctionNode body, ValueNode value, Span at, int depth, List<(TypeRefNode? Body, Span At)> successes)
    {
        if (depth > MaxDepth)
        {
            return Unreadable(value, at);
        }

        // A local, or a conditional: each value it is given (var result = Ok(x); cond ? NotFound() : Ok(x)).
        if (value.Call is null && value.Creations.Count == 0 && value.Given.Count > 0)
        {
            foreach (var given in value.Given)
            {
                if (Value(body, given, given.At, depth + 1, successes) is { } unread)
                {
                    return unread;
                }
            }

            return null;
        }

        if (value.Call is null && value.Creations is [var creation])
        {
            return Created(body, creation, at, successes);
        }

        if ((value.Call ?? MinimalApiClaims.Outermost(body, value.At)) is not { } call)
        {
            return Unreadable(value, at);
        }

        if (call.Target is null && (call.Receiver is null || Receivers.Contains(call.Receiver)))
        {
            return Factory(body, call, at, successes);
        }

        // A method of the code's own that builds the result: return HandleResult(data).
        if (call.Target is { Member: { Type: { } type, Body: { } helper } } && IsUntyped(type))
        {
            foreach (var returned in helper.Returns)
            {
                if (Value(helper, returned, at, depth + 1, successes) is { } unread)
                {
                    return unread;
                }
            }

            return null;
        }

        return Unreadable(value, at);
    }

    /// <summary>A controller's, <c>Results</c>' or <c>TypedResults</c>' factory, and the body its success carries.</summary>
    private static Unread? Factory(FunctionNode body, CallNode call, Span at, List<(TypeRefNode? Body, Span At)> successes)
    {
        var arguments = call.Arguments;
        if (Errors.Contains(call.Name))
        {
            return null;
        }

        switch (call.Name)
        {
            case "Ok" or "Json":
                return Success(body, arguments.FirstOrDefault(), at, successes);
            case "NoContent":
                return Success(body, null, at, successes);
            case "Created":
                // Created(uri, value); Created() and Created(uri) have none.
                return Success(body, arguments.Count > 1 ? arguments[1] : null, at, successes);
            case "Accepted":
                // Accepted(value) or Accepted(uri, value); Accepted(uri) has none.
                return Success(body, arguments.Count > 1 ? arguments[1] : arguments is [var only] && !IsLocation(only) ? only : null, at, successes);
            case "CreatedAtAction" or "CreatedAtRoute" or "AcceptedAtAction" or "AcceptedAtRoute":
                // The value is last: (action, value), (action, routeValues, value), (action, controller, routeValues, value).
                return Success(body, arguments.Count > 1 && arguments[^1].Literal is null ? arguments[^1] : null, at, successes);
            case "StatusCode" or "Content" when arguments.Count > 0 && SuccessStatus(arguments[0]) is { } status:
                // StatusCode(201, value), Web API 2's Content(HttpStatusCode.Created, value).
                return status is >= 200 and < 300 ? Success(body, arguments.Count > 1 ? arguments[1] : null, at, successes) : null;
            case "StatusCode":
                return new Unread("unreadable", $"`{call.Text}` gives a status the code doesn't give as a value, so whether it succeeds isn't known.", at);
            case "Content":
                // ASP.NET Core's Content(text) sends the text.
                successes.Add((Text, at));
                return null;
            case "File" or "Stream" or "PhysicalFile" or "VirtualFile" or "Bytes":
                successes.Add((call.Name == "Bytes" || arguments.FirstOrDefault() is { IsKnown: true, Types: [{ IsArray: true, Name: "byte" }] } ? Bytes : Stream, at));
                return null;
            default:
                return new Unread("unreadable", $"`{call.Text}` builds a result this extension doesn't read, so what it returns isn't known.", at);
        }
    }

    /// <summary>A result the action creates, and the body its success carries.</summary>
    private static Unread? Created(FunctionNode body, CreationNode creation, Span at, List<(TypeRefNode? Body, Span At)> successes)
    {
        var arguments = creation.Arguments;
        var name = creation.Type.Name;
        if (Errors.Contains(name))
        {
            return null;
        }

        switch (name)
        {
            case "OkObjectResult" or "JsonResult" or "OkNegotiatedContentResult":
                return Success(body, arguments.FirstOrDefault(), at, successes);
            case "OkResult" or "NoContentResult":
                return Success(body, null, at, successes);
            case "CreatedResult" or "AcceptedResult" or "CreatedNegotiatedContentResult":
                return Success(body, arguments.Count > 1 ? arguments[1] : null, at, successes);
            case "CreatedAtActionResult" or "CreatedAtRouteResult" or "AcceptedAtActionResult" or "AcceptedAtRouteResult" or "CreatedAtRouteNegotiatedContentResult":
                return Success(body, arguments.Count > 1 ? arguments[^1] : null, at, successes);
            case "FileContentResult":
                successes.Add((Bytes, at));
                return null;
            case "FileStreamResult" or "PhysicalFileResult" or "VirtualFileResult":
                successes.Add((Stream, at));
                return null;
            case "StatusCodeResult" when arguments.Count > 0 && SuccessStatus(arguments[0]) is { } status:
                return status is >= 200 and < 300 ? Success(body, null, at, successes) : null;
            case "ObjectResult":
            {
                // An ObjectResult succeeds with its value unless its StatusCode says otherwise.
                var given = creation.Initializers.FirstOrDefault(x => x.Member == "StatusCode")?.Value;
                var status = given is null ? 200 : SuccessStatus(given);
                return status switch
                {
                    null => new Unread("unreadable", $"`{creation.Text}` gives a status the code doesn't give as a value, so whether it succeeds isn't known.", at),
                    >= 200 and < 300 => Success(body, arguments.FirstOrDefault(), at, successes),
                    _ => null
                };
            }
            default:
                return new Unread("unreadable", $"`{creation.Text}` creates a result this extension doesn't read, so what it returns isn't known.", at);
        }
    }

    /// <summary>A success with the body given, or none; a body whose type the code doesn't say can't be read.</summary>
    private static Unread? Success(FunctionNode body, ValueNode? payload, Span at, List<(TypeRefNode? Body, Span At)> successes)
    {
        if (payload is null)
        {
            successes.Add((null, at));
            return null;
        }

        if (MinimalApiClaims.TypeOf(body, payload, 0) is not { } type)
        {
            return new Unread("unresolved", $"The code doesn't say what `{payload.Text}` is, so what the action returns isn't known.", at);
        }

        successes.Add((type, at));
        return null;
    }

    /// <summary><c>Accepted(uri)</c>'s location: a string or a <c>Uri</c>, not a body.</summary>
    private static bool IsLocation(ValueNode value) =>
        value.Literal is not null || value is { IsKnown: true, Types: [{ Name: "string" or "String" or "Uri" }] };

    private static int? SuccessStatus(ValueNode value) => Http.SuccessStatus.StatusOf(value.Literal?.Value ?? value.Text);

    private static Unread Unreadable(ValueNode value, Span at) =>
        new("unreadable", $"The action returns `{value.Text}`, which isn't a result this extension reads, so what it returns isn't known.", at);
}
