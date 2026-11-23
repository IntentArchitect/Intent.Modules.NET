using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.AspNetCore;

/// <summary>
/// What an action or handler returns when its return type doesn't say: <c>IActionResult</c>, <c>ActionResult</c>, Web API 2's
/// <c>IHttpActionResult</c> or Minimal APIs' <c>IResult</c>, under <c>Task</c>. Each return is read for the success result it
/// builds, a controller's factory (<c>Ok(data)</c>, an MVC view's <c>View(model)</c>), <c>Results</c>' or <c>TypedResults</c>',
/// or a result it creates (<c>new OkObjectResult(data)</c>), through locals, conditionals and the code's own methods that build
/// it, its controller extension methods included (<c>this.LoadingPage(...)</c>); what that result carries is what the
/// operation returns: <c>Ok(data)</c> and <c>View(data)</c> are data's type, <c>NoContent()</c> and <c>View()</c> nothing, which
/// the model states with no type or with the action's own result type (a GET must have one in the Services designer). An
/// error result isn't modelled, and an action that only redirects returns nothing. A body whose type the code doesn't say is the action's declared response (<c>[ResponseType]</c>,
/// <c>[ProducesResponseType]</c>); a result this doesn't read, or an untyped body with no declared response, is can't verify, never a guess.
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

    /// <summary>The factories this reads the success of, besides the errors.</summary>
    private static readonly HashSet<string> Factories =
    [
        "Ok", "Json", "NoContent", "Created", "Accepted", "CreatedAtAction", "CreatedAtRoute", "AcceptedAtAction", "AcceptedAtRoute", "StatusCode", "Content",
        "File", "Stream", "PhysicalFile", "VirtualFile", "Bytes", "View", "PartialView"
    ];

    /// <summary>The results that send the user somewhere else: an MVC action that only returns these returns nothing.</summary>
    private static readonly HashSet<string> Navigations =
    [
        "Redirect", "RedirectPermanent", "RedirectToAction", "RedirectToRoute", "RedirectToPage", "LocalRedirect", "Challenge", "SignOut",
        "RedirectResult", "RedirectToActionResult", "RedirectToRouteResult", "LocalRedirectResult", "ChallengeResult", "SignOutResult"
    ];

    /// <summary>The types a controller extension method of the code's own extends: <c>this Controller controller</c>.</summary>
    private static readonly HashSet<string> ControllerBases = ["Controller", "ControllerBase"];

    private static readonly TypeRefNode Stream = new() { Text = "Stream", Name = "Stream", QualifiedName = "System.IO.Stream" };

    // An array's element is its one type argument, as the comparer reads it.
    private static readonly TypeRefNode Bytes = new()
    {
        Text = "byte[]", Name = "byte", QualifiedName = "byte", IsArray = true,
        Arguments = [new TypeRefNode { Text = "byte", Name = "byte", QualifiedName = "byte", IsPredefinedValueType = true }]
    };

    private static readonly TypeRefNode Text = new() { Text = "string", Name = "string", QualifiedName = "string" };

    private const int MaxDepth = 3;

    private static bool IsFile(TypeRefNode body) => ReferenceEquals(body, Stream) || ReferenceEquals(body, Bytes);

    /// <summary>A return type that doesn't state what it succeeds with, under <c>Task</c> or <c>ValueTask</c>.</summary>
    public static bool IsUntyped(TypeRefNode type) =>
        (type is { Name: "Task" or "ValueTask", Arguments: [var awaited] } ? awaited : type) is { Arguments.Count: 0, IsArray: false } result && Untyped.Contains(result.Name);

    public static ClaimReading Read(ClaimContext context, MemberNode member)
    {
        var returns = member.Type!;

        // A success with no body is the model's nothing, or the action's own result type: the Services designer requires a GET
        // to return a type, so an MVC GET that renders View() or only redirects can only be modelled as IActionResult.
        var result = returns is { Name: "Task" or "ValueTask", Arguments: [var awaited] } ? awaited : returns;
        var bodiless = context.ExpectedValue is not ITypeReference stated || context.TypeMatches(stated, result);

        // An action that only redirects, challenges or signs out renders nothing of its own, as Intent's MVC module models
        // RedirectToAction: it returns nothing.
        if (member.Body is { Returns: [var first, ..] } navigating && navigating.Returns.All(Navigates))
        {
            return ClaimReading.Compared($"{returns.Text} with no body", first.At, bodiless);
        }

        var successes = new List<(TypeRefNode? Body, Span At, bool View)>();
        var untyped = new List<Unread>();
        foreach (var value in member.Body?.Returns ?? [])
        {
            if (Value(context.Code, member.Body!, value, value.At, 0, successes, Receivers) is not { } unread)
            {
                continue;
            }

            // A success whose body the code doesn't type may be the one the action declares; any other unread result can't be.
            if (unread.Reason != "unresolved")
            {
                return ClaimReading.CantVerify(unread.Reason, unread.Message, unread.At);
            }

            untyped.Add(unread);
        }

        // The body comes first. Only when it builds no success, as a stub that only throws, or builds one whose body the code
        // doesn't type (Content(HttpStatusCode.OK, x == "y")), does the action's declared response say: [ResponseType(typeof(bool))]
        // is the code's own statement of its success, and a body that is typed is still compared against the model as it is.
        if (successes.Count == 0 || untyped.Count > 0)
        {
            var declared = new List<(TypeRefNode? Body, Span At, bool View)>();
            Declared(member, declared);
            if (untyped.Count > 0 && declared.Count == 0)
            {
                return ClaimReading.CantVerify(untyped[0].Reason, untyped[0].Message, untyped[0].At);
            }

            // Each untyped success is the declared response, at the return that builds it.
            successes.AddRange(untyped.Count > 0 ? untyped.SelectMany(x => declared.Select(d => (d.Body, x.At, false))) : declared);
        }

        if (successes.Count == 0)
        {
            return ClaimReading.CantVerify("unreadable", member.Body is null
                ? $"`{member.Name}` returns `{returns.Text}`, which doesn't state the type of its body, and it has no body to read."
                : $"`{member.Name}` returns `{returns.Text}`, which doesn't state the type of its body, and it builds no success result.", member.At);
        }

        // A success with no body beside ones with a body (a fallback Ok()) is the operation returning nothing of its type.
        if (successes.Any(x => x.Body is not null))
        {
            successes.RemoveAll(x => x.Body is null);
        }

        // A file is a file, whether the code sends its bytes or a stream: either is the model's Stream or binary.
        bool Matches(ITypeReference model, TypeRefNode body) =>
            context.TypeMatches(model, body) || IsFile(body) && (context.TypeMatches(model, Stream) || context.TypeMatches(model, Bytes));

        bool Same((TypeRefNode? Body, Span At, bool View) success) =>
            success.Body is null ? bodiless : context.ExpectedValue is ITypeReference model && Matches(model, success.Body);

        // An MVC action may render several pages: its own, and others such as a redirect page (Identity's LoadingPage). The model
        // states one return type, so a view model it renders is what the model may give; an API's bodies must all be the one.
        var same = successes.All(x => x.View) ? successes.Any(Same) : successes.All(Same);
        var actual = string.Join(" or ", successes.Select(x => x.Body?.Text ?? "no body").Distinct());
        return ClaimReading.Compared($"{returns.Text} with {actual}", successes[0].At, same);
    }

    private sealed record Unread(string Reason, string Message, Span At);

    /// <summary>
    /// The success a returned value builds, added to <paramref name="successes"/>; why it can't be read, when it can't.
    /// <paramref name="controllers"/> are what a factory is called on: the controller itself, or the parameter a controller
    /// extension method of the code's own is called on.
    /// </summary>
    private static Unread? Value(CodeGraph code, FunctionNode body, ValueNode value, Span at, int depth, List<(TypeRefNode? Body, Span At, bool View)> successes,
        IReadOnlySet<string> controllers)
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
                if (Value(code, body, given, given.At, depth + 1, successes, controllers) is { } unread)
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

        // return await Task.FromResult(Ok(data)): the result it hands back as a finished task.
        if (Http.SuccessStatus.FromResult(call) is { } wrapped)
        {
            return Value(code, body, wrapped, at, depth + 1, successes, controllers);
        }

        var onController = call.Receiver is null || controllers.Contains(call.Receiver);

        // A controller extension method of the code's own that builds the result: return this.LoadingPage("Redirect", url),
        // whose body calls the factory on the controller it extends, controller.View(viewName, model).
        if (call.Target is null && onController && !Factories.Contains(call.Name) && !Errors.Contains(call.Name)
            && Extension(code, call) is { Body: { } extension, Parameters: [var extended, ..] })
        {
            return Helper(code, extension, at, depth, successes, new HashSet<string>(Receivers) { extended.Name });
        }

        if (call.Target is null && onController)
        {
            return Factory(body, call, at, successes);
        }

        // A method of the code's own that builds the result: return HandleResult(data).
        if (call.Target is { Member: { Type: { } type, Body: { } helper } } && IsUntyped(type))
        {
            return Helper(code, helper, at, depth, successes, Receivers);
        }

        return Unreadable(value, at);
    }

    /// <summary>The successes each return of a method of the code's own builds.</summary>
    private static Unread? Helper(CodeGraph code, FunctionNode helper, Span at, int depth, List<(TypeRefNode? Body, Span At, bool View)> successes, IReadOnlySet<string> controllers)
    {
        foreach (var returned in helper.Returns)
        {
            if (Value(code, helper, returned, at, depth + 1, successes, controllers) is { } unread)
            {
                return unread;
            }
        }

        return null;
    }

    /// <summary>
    /// The one static method of the code's own a call on the controller reaches as an extension method: of the call's name,
    /// taking a <c>Controller</c> or <c>ControllerBase</c> and then the call's arguments, and returning an untyped result.
    /// </summary>
    private static MemberNode? Extension(CodeGraph code, CallNode call) =>
        code.Types.SelectMany(x => x.Members)
            .Where(x => x is { Kind: "method", IsStatic: true, Type: { } type } && x.Name == call.Name && IsUntyped(type)
                        && x.Parameters.Count == call.Arguments.Count + 1 && x.Parameters[0].Type is { } extended && ControllerBases.Contains(extended.Name))
            .Take(2).ToList() is [var only] ? only : null;

    /// <summary>A return that only sends the user elsewhere: <c>RedirectToAction("Index")</c>, <c>Challenge(props, scheme)</c>.</summary>
    private static bool Navigates(ValueNode value) =>
        value is { Call: { } call } ? Navigations.Contains(call.Name) && (call.Receiver is null || Receivers.Contains(call.Receiver))
            : value.Creations is [var creation] && Navigations.Contains(creation.Type.Name);

    /// <summary><c>View(viewName)</c>'s name: text, not the model the view renders.</summary>
    private static bool IsViewName(ValueNode value) =>
        value.Literal is not null || value is { IsKnown: true, Types: [{ Name: "string" or "String" }] };

    /// <summary>A controller's, <c>Results</c>' or <c>TypedResults</c>' factory, and the body its success carries.</summary>
    private static Unread? Factory(FunctionNode body, CallNode call, Span at, List<(TypeRefNode? Body, Span At, bool View)> successes)
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
            case "View" or "PartialView":
                // View(), View(viewName), View(model) or View(viewName, model): the model the view renders is what the action returns,
                // as Intent's MVC module models it, and with none it returns nothing.
                return Success(body, arguments.Count > 1 ? arguments[1] : arguments is [var model] && !IsViewName(model) ? model : null, at, successes, view: true);
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
            case "Content" when Http.SuccessStatus.IsStatusCode(arguments.FirstOrDefault()):
                // Web API 2's Content(status, value) with a status this can't read is never ASP.NET Core's Content(text).
                return new Unread("unreadable", $"`{call.Text}` gives a status the code doesn't give as a value, so whether it succeeds isn't known.", at);
            case "Content":
                // ASP.NET Core's Content(text) sends the text.
                successes.Add((Text, at, false));
                return null;
            case "File" or "Stream" or "PhysicalFile" or "VirtualFile" or "Bytes":
                successes.Add((call.Name == "Bytes" || arguments.FirstOrDefault() is { IsKnown: true, Types: [{ IsArray: true, Name: "byte" }] } ? Bytes : Stream, at, false));
                return null;
            default:
                return new Unread("unreadable", $"`{call.Text}` builds a result this extension doesn't read, so what it returns isn't known.", at);
        }
    }

    /// <summary>A result the action creates, and the body its success carries.</summary>
    private static Unread? Created(FunctionNode body, CreationNode creation, Span at, List<(TypeRefNode? Body, Span At, bool View)> successes)
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
                successes.Add((Bytes, at, false));
                return null;
            case "FileStreamResult" or "PhysicalFileResult" or "VirtualFileResult":
                successes.Add((Stream, at, false));
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
    private static Unread? Success(FunctionNode body, ValueNode? payload, Span at, List<(TypeRefNode? Body, Span At, bool View)> successes, bool view = false)
    {
        if (payload is null)
        {
            successes.Add((null, at, view));
            return null;
        }

        if (MinimalApiClaims.TypeOf(body, payload, 0) is not { } type)
        {
            return new Unread("unresolved", $"The code doesn't say what `{payload.Text}` is, so what the action returns isn't known.", at);
        }

        successes.Add((type, at, view));
        return null;
    }

    /// <summary>
    /// The success responses the action declares: Web API 2's <c>[ResponseType(typeof(T))]</c>, and ASP.NET Core's
    /// <c>[ProducesResponseType(typeof(T), 200)]</c>, <c>[ProducesResponseType(200, Type = typeof(T))]</c> and
    /// <c>[ProducesResponseType&lt;T&gt;(200)]</c>, of a 2xx status (200 when it gives none). One with no type, or
    /// <c>typeof(void)</c>, has no body.
    /// </summary>
    private static void Declared(MemberNode member, List<(TypeRefNode? Body, Span At, bool View)> successes)
    {
        foreach (var attribute in member.Attributes)
        {
            if (attribute is { Name: "ResponseType", Arguments: [var type] })
            {
                successes.Add((TypeOfType(type), attribute.At, false));
            }
            else if (attribute.Name == "ProducesResponseType")
            {
                var body = attribute.TypeArguments is [var generic]
                    ? generic
                    : attribute.Arguments.Select(TypeOfType).FirstOrDefault(x => x is not null) ?? (attribute.Named.TryGetValue("Type", out var named) ? TypeOfType(named) : null);
                var status = attribute.Arguments.Select(Http.SuccessStatus.StatusOf).FirstOrDefault(x => x is not null)
                             ?? (attribute.Named.TryGetValue("StatusCode", out var code) ? Http.SuccessStatus.StatusOf(code) : 200);
                if (status is >= 200 and < 300)
                {
                    successes.Add((body, attribute.At, false));
                }
            }
        }
    }

    /// <summary>The type a <c>typeof(T)</c> argument names; null for anything else, and for <c>typeof(void)</c>.</summary>
    private static TypeRefNode? TypeOfType(string argument) =>
        SyntaxFactory.ParseExpression(argument) is TypeOfExpressionSyntax { Type: var type } && type is not PredefinedTypeSyntax { Keyword.Text: "void" }
            ? CSharpReader.TypeRef(type)
            : null;

    /// <summary><c>Accepted(uri)</c>'s location: a string or a <c>Uri</c>, not a body.</summary>
    private static bool IsLocation(ValueNode value) =>
        value.Literal is not null || value is { IsKnown: true, Types: [{ Name: "string" or "String" or "Uri" }] };

    private static int? SuccessStatus(ValueNode value) => Http.SuccessStatus.StatusOf(value.Literal?.Value ?? value.Text);

    private static Unread Unreadable(ValueNode value, Span at) =>
        new("unreadable", $"The action returns `{value.Text}`, which isn't a result this extension reads, so what it returns isn't known.", at);
}
