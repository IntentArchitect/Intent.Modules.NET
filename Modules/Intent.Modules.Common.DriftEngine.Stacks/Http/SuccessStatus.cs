using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Http;

/// <summary>
/// The success status a handler states (<c>operation.status</c>): what each of its returns builds its response with, a
/// framework's result factory (ASP.NET's <c>Ok</c>, <c>TypedResults.Created</c>, <c>StatusCode(204)</c>; Spring's
/// <c>ResponseEntity.noContent()</c>), followed into the code's own helpers that build it (<c>Http.created(x)</c>), else the
/// typed results its return type names (<c>Results&lt;Created&lt;T&gt;, NotFound&gt;</c>). An error result isn't the success
/// status. A handler that returns its data itself, not a result, is sent with the framework's default 200: a default reading,
/// which a modelled 200 verifies and <c>unmodelled.value</c> doesn't report.
/// </summary>
internal static class SuccessStatus
{
    /// <summary>The factories that build a success response, by name: ASP.NET Core's and Web API 2's, and Spring's ResponseEntity's.</summary>
    private static readonly Dictionary<string, int> Factories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Ok"] = 200, ["Of"] = 200, ["Json"] = 200, ["File"] = 200, ["PhysicalFile"] = 200, ["Stream"] = 200, ["Bytes"] = 200,
        ["Created"] = 201, ["CreatedAtAction"] = 201, ["CreatedAtRoute"] = 201,
        ["Accepted"] = 202, ["AcceptedAtAction"] = 202, ["AcceptedAtRoute"] = 202,
        ["NoContent"] = 204
    };

    /// <summary>The result types a handler may create for a success: ASP.NET Core's action results and Web API 2's.</summary>
    private static readonly Dictionary<string, int> Results = new(StringComparer.Ordinal)
    {
        ["OkResult"] = 200, ["OkObjectResult"] = 200, ["OkNegotiatedContentResult"] = 200, ["JsonResult"] = 200, ["FileContentResult"] = 200,
        ["FileStreamResult"] = 200, ["PhysicalFileResult"] = 200,
        ["CreatedResult"] = 201, ["CreatedAtActionResult"] = 201, ["CreatedAtRouteResult"] = 201, ["CreatedNegotiatedContentResult"] = 201,
        ["CreatedAtRouteNegotiatedContentResult"] = 201,
        ["AcceptedResult"] = 202, ["AcceptedAtActionResult"] = 202, ["AcceptedAtRouteResult"] = 202,
        ["NoContentResult"] = 204
    };

    /// <summary>The factories whose first argument is the status: <c>StatusCode(201)</c>, <c>ResponseEntity.status(HttpStatus.CREATED)</c>, Web API 2's <c>Content(HttpStatusCode.Created, x)</c>.</summary>
    private static readonly HashSet<string> GivenStatus = new(StringComparer.OrdinalIgnoreCase) { "StatusCode", "Status", "CreateResponse", "Content" };

    /// <summary>What a factory is called on: nothing (a controller's own), the framework's result types, or the request.</summary>
    private static readonly HashSet<string> Receivers = new(StringComparer.Ordinal) { "this", "base", "TypedResults", "Results", "ResponseEntity", "Request" };

    /// <summary>Status names, as HttpStatusCode, HttpStatus and StatusCodes write them, without underscores and case aside.</summary>
    private static readonly Dictionary<string, int> Names = new(StringComparer.OrdinalIgnoreCase)
    {
        ["OK"] = 200, ["Created"] = 201, ["Accepted"] = 202, ["NonAuthoritativeInformation"] = 203, ["NoContent"] = 204, ["ResetContent"] = 205,
        ["PartialContent"] = 206, ["MultiStatus"] = 207, ["AlreadyReported"] = 208, ["IMUsed"] = 226,
        ["MovedPermanently"] = 301, ["Found"] = 302, ["Redirect"] = 302, ["SeeOther"] = 303, ["NotModified"] = 304, ["TemporaryRedirect"] = 307,
        ["BadRequest"] = 400, ["Unauthorized"] = 401, ["Forbidden"] = 403, ["NotFound"] = 404, ["MethodNotAllowed"] = 405, ["NotAcceptable"] = 406,
        ["Conflict"] = 409, ["Gone"] = 410, ["PreconditionFailed"] = 412, ["UnprocessableEntity"] = 422, ["TooManyRequests"] = 429,
        ["InternalServerError"] = 500, ["NotImplemented"] = 501, ["BadGateway"] = 502, ["ServiceUnavailable"] = 503, ["GatewayTimeout"] = 504
    };

    /// <summary>A success typed result a return type names, as ASP.NET Core's <c>Microsoft.AspNetCore.Http.HttpResults</c> has them.</summary>
    private static readonly Dictionary<string, int> TypedResults = new(StringComparer.Ordinal)
    {
        ["Ok"] = 200, ["Created"] = 201, ["CreatedAtRoute"] = 201, ["Accepted"] = 202, ["AcceptedAtRoute"] = 202, ["NoContent"] = 204
    };

    private const int MaxDepth = 3;

    /// <param name="stated">A status the framework's annotation on the handler states (Spring's <c>@ResponseStatus</c>), which comes first.</param>
    public static ClaimReading Read(ClaimContext context, MemberNode handler, (int Status, Span At)? stated = null)
    {
        if (stated is { } annotation)
        {
            return ClaimReading.Stated(annotation.Status.ToString(), annotation.At);
        }

        var found = new List<(int Status, Span At)>();
        Span? unreadable = null;
        var declaring = context.Code.Types.FirstOrDefault(x => x.Members.Contains(handler))
                        ?? (context.Owner(context.Element) is { } service ? context.TypeOf(service) : null);
        Returns(context.Code, declaring, handler, null, 0, found, ref unreadable);
        if (found.Count > 0)
        {
            var statuses = found.Select(x => x.Status.ToString()).Distinct().ToList();
            var at = found.FirstOrDefault(x => x.Status.ToString() == context.Expected).At ?? found[0].At;
            return ClaimReading.Compared(string.Join(", ", statuses), at, statuses.Contains(context.Expected));
        }

        if (unreadable is { } where)
        {
            return ClaimReading.CantVerify("unreadable", $"`{handler.Name}` returns a status the code doesn't give as a value, so the rule can't read which.", where);
        }

        var typed = Typed(handler.Type).Distinct().ToList();
        if (typed is [var only])
        {
            return ClaimReading.Stated(only.ToString(), handler.At);
        }

        // A handler that returns its data itself, not a result, has it sent with 200 by ASP.NET Core, Web API 2 and Spring
        // alike: the framework's default, which a modelled 200 verifies. Intent's controllers send 201 for a POST that states
        // no status, so a model must state the 200 such a POST sends.
        return Data(context.Code, handler.Type) is { } data
            ? ClaimReading.Defaulted("200", handler.At, $"it returns `{data.Text}` itself")
            : ClaimReading.Absent(handler.At);
    }

    /// <summary>The framework results a handler may return, by name: anything else it returns is its data.</summary>
    private static readonly HashSet<string> FrameworkResults = new(StringComparer.Ordinal)
    {
        "IActionResult", "ActionResult", "IHttpActionResult", "IResult", "IConvertToActionResult", "HttpResponseMessage", "ResponseEntity", "Results"
    };

    /// <summary>
    /// What a handler returns when it is its data: its return type, under <c>Task</c> or <c>ValueTask</c>, and ASP.NET Core's
    /// <c>ActionResult&lt;T&gt;</c>, whose <c>T</c> it may return itself, unless that is nothing or a framework result
    /// (<see cref="FrameworkResults"/>, a typed result, or any other <c>…Result</c> the code doesn't declare). Null otherwise.
    /// Only reached when no return builds a success, so an <c>ActionResult&lt;T&gt;</c> action's factories are read first.
    /// </summary>
    private static TypeRefNode? Data(CodeGraph code, TypeRefNode? type)
    {
        while (type is { Name: "Task" or "ValueTask" or "ActionResult", Arguments: [var inner] })
        {
            type = inner;
        }

        if (type is null or { Name: "void" or "Void" or "Task" or "ValueTask" or "Unit" } || FrameworkResults.Contains(type.Name))
        {
            return null;
        }

        var own = code.Types.Any(x => x.Name == type.Name);
        return !own && (TypedResults.ContainsKey(type.Name) || type.Name.EndsWith("Result", StringComparison.Ordinal)) ? null : type;
    }

    /// <summary>
    /// The success statuses each return of the method builds its response with, in the code's own methods it returns too,
    /// an interface's implementations included. The handler's own returns are where a finding points; a helper's are its call's.
    /// </summary>
    private static void Returns(CodeGraph code, TypeNode? type, MemberNode method, Span? at, int depth, List<(int Status, Span At)> found, ref Span? unreadable)
    {
        if (method.Body is not { } body)
        {
            return;
        }

        foreach (var value in body.Returns)
        {
            Value(code, type, body, value, at ?? value.At, depth, found, ref unreadable);
        }
    }

    /// <summary>The status a value builds the response with: a result it creates, a factory it calls, or what a method of the code's own returns.</summary>
    private static void Value(CodeGraph code, TypeNode? type, FunctionNode body, ValueNode value, Span at, int depth, List<(int Status, Span At)> found, ref Span? unreadable)
    {
        if (depth > MaxDepth)
        {
            return;
        }

        foreach (var creation in value.Creations)
        {
            if (Results.TryGetValue(creation.Type.Name, out var result))
            {
                found.Add((result, at));
                return;
            }

            if (creation.Type.Name is "StatusCodeResult" or "HttpResponseMessage" && creation.Arguments.Count > 0)
            {
                Given(StatusIn(creation.Arguments[0]), at, found, ref unreadable);
                return;
            }

            // Web API 2's ResponseMessageResult sends the response it is given: new ResponseMessageResult(request.CreateResponse(HttpStatusCode.OK)).
            if (creation.Type.Name == "ResponseMessageResult" && creation.Arguments.Count > 0)
            {
                Value(code, type, body, creation.Arguments[0], at, depth + 1, found, ref unreadable);
                return;
            }
        }

        var returned = value.Call ?? Outermost(body, value.At) ?? value.Given.Select(x => x.Call).FirstOrDefault(x => x is not null);
        if (returned is null && value.Given.Count > 0)
        {
            // A local: what it is given (var response = request.CreateResponse(HttpStatusCode.OK)).
            foreach (var given in value.Given)
            {
                Value(code, type, body, given, at, depth + 1, found, ref unreadable);
            }

            return;
        }

        for (var call = returned; call is not null; call = call.On)
        {
            if (FromResult(call) is { } wrapped)
            {
                Value(code, type, body, wrapped, at, depth + 1, found, ref unreadable);
                return;
            }

            // Web API 2's ResponseMessage(response) sends the response it is given, as new ResponseMessageResult(response) does.
            if (call is { Name: "ResponseMessage", Arguments: [var message] } && IsFramework(call))
            {
                Value(code, type, body, message, at, depth + 1, found, ref unreadable);
                return;
            }

            if (GivenStatus.Contains(call.Name) && (IsFramework(call) || call.Name == "CreateResponse"))
            {
                if (call.Name == "Content" && StatusIn(call.Arguments.FirstOrDefault()) is { Known: false } && !IsStatusCode(call.Arguments.FirstOrDefault()))
                {
                    // ASP.NET Core's Content(text) is a 200; Web API 2's Content(status, value) gives one, even one this can't read.
                    found.Add((200, at));
                }
                else
                {
                    Given(StatusIn(call.Arguments.FirstOrDefault()), at, found, ref unreadable);
                }

                return;
            }

            if (Factories.TryGetValue(call.Name, out var factory) && IsFramework(call))
            {
                found.Add((factory, at));
                return;
            }

            // A method of the code's own that builds the response, a helper (Http.created(body)) or a service behind its
            // interface (documents.SearchAsync(request)).
            IReadOnlyList<(TypeNode Type, MemberNode Member)> bodies = type is null ? [] : Messaging.MessageCode.Bodies(code, type, call);
            if (bodies.Count > 0)
            {
                foreach (var (owner, member) in bodies)
                {
                    Returns(code, owner, member, at, depth + 1, found, ref unreadable);
                }

                return;
            }
        }
    }

    /// <summary>A status given as a value: a success is found, an error isn't the success status, and one the code doesn't give as a value can't be read.</summary>
    private static void Given((bool Known, int Status) status, Span at, List<(int Status, Span At)> found, ref Span? unreadable)
    {
        if (!status.Known)
        {
            unreadable ??= at;
        }
        else if (status.Status is >= 200 and < 300)
        {
            found.Add((status.Status, at));
        }
    }

    private static bool IsFramework(CallNode call) => call.Target is null && (call.Receiver is null || Receivers.Contains(call.Receiver));

    /// <summary>What <c>Task.FromResult(x)</c> or <c>ValueTask.FromResult(x)</c> wraps: a result handed back as a finished task, returned or awaited.</summary>
    public static ValueNode? FromResult(CallNode call) =>
        call is { Name: "FromResult", Target: null, Arguments: [var wrapped], Receiver: "Task" or "ValueTask" or "System.Threading.Tasks.Task" or "System.Threading.Tasks.ValueTask" }
            ? wrapped
            : null;

    /// <summary>The status a value names, if it names one the code gives: <c>204</c>, <c>HttpStatusCode.NoContent</c>, <c>(int)HttpStatusCode.Created</c>, <c>StatusCodes.Status201Created</c>, <c>HttpStatus.NO_CONTENT</c>.</summary>
    private static (bool Known, int Status) StatusIn(ValueNode? value) =>
        StatusOf(value?.Literal?.Value ?? value?.Text) is { } status ? (true, status) : (false, 0);

    /// <summary>The status a text names, when it names one as the code gives it; null for anything else, such as a variable.</summary>
    public static int? StatusOf(string? text)
    {
        if (text is null)
        {
            return null;
        }

        text = Regex.Replace(text, @"^\((int|Integer|(System\.Net\.)?HttpStatusCode)\)\s*", "").Trim();
        if (int.TryParse(text, out var number))
        {
            return number;
        }

        if (Regex.Match(text, @"\bStatus(\d{3})[A-Za-z]*$") is { Success: true } constant)
        {
            return int.Parse(constant.Groups[1].Value);
        }

        if (!Regex.IsMatch(text, @"^(System\.Net\.|org\.springframework\.http\.)?(HttpStatusCode|HttpStatus)\."))
        {
            return null;
        }

        var name = text.Split('.')[^1].Replace("_", "");
        if (Names.TryGetValue(name, out var named))
        {
            return named;
        }

        // Any other status .NET names (HttpStatusCode.ExpectationFailed), and Spring's HttpStatus, mostly the same words.
        // A name only: Enum.TryParse would read digits, or names joined by commas, as a value too.
        return Regex.IsMatch(name, "^[A-Za-z]+$") && Enum.TryParse<System.Net.HttpStatusCode>(name, ignoreCase: true, out var status) ? (int)status : null;
    }

    /// <summary>
    /// A value of .NET's <c>HttpStatusCode</c>: one of its members (<c>HttpStatusCode.ExpectationFailed</c>), a cast to it, or a
    /// value the code types as one. Web API 2's <c>Content(status, value)</c> takes one first, ASP.NET Core's <c>Content(text)</c> a string.
    /// </summary>
    public static bool IsStatusCode(ValueNode? value) =>
        value is not null && (Regex.IsMatch(value.Text, @"^\(?(System\.Net\.)?HttpStatusCode[.)]") || value.Types.Any(x => x.Name == "HttpStatusCode"));

    /// <summary>The success typed results a return type names, under <c>Task</c> and <c>Results&lt;...&gt;</c>.</summary>
    private static IEnumerable<int> Typed(TypeRefNode? type)
    {
        while (type is { Name: "Task" or "ValueTask", Arguments: [var inner] })
        {
            type = inner;
        }

        var results = type is { Name: "Results" } ? type.Arguments : type is null ? [] : [type];
        return results.Select(x => TypedResults.TryGetValue(x.Name, out var status) ? status : 0).Where(x => x != 0);
    }

    /// <summary>The call a value is, under an <c>await</c> or a cast: the outermost call within it, the last of a chain.</summary>
    private static CallNode? Outermost(FunctionNode body, Span span) =>
        body.Calls.Where(x => !x.IsReference && span.Contains(x.At))
            .OrderBy(x => (x.At.Line, x.At.Column ?? 0))
            .ThenByDescending(x => (x.At.EndLine ?? x.At.Line, x.At.EndColumn ?? 0))
            .FirstOrDefault();
}
