# Intent.AspNetCore

This module generates the foundational code for hosting web services on the ASP.NET Core infrastructure.

## How to disable HTTPS Redirect

By default, the `UseHttpsRedirection` middleware is **included** in the application's HTTP request pipeline.  
This can be disabled using the `Enable HTTPS Redirect` setting found on the Application Settings screen:

![HTTPS enabled](images/https-enable.png)

## Endpoint Required Parameters

To control whether *qualifying parameters* on an endpoint are decorated with the `[Required]` attribute, use the `Add Required Attribute to Parameters` setting:

![Required Attribute](images/required-attribute.png)

If **off**, the `[Required]` attribute will **not** be explicitly added to controller method parameters.  
If **on**, the `[Required]` attribute will be added if:

- The parameter source is `Query`, `Header`, or `Form` AND
- The parameter does **not have a default value** AND
- The parameter type is **not nullable**.

### Example

Here’s an example of the generated controller code with `pageNo` and `pageSize` decorated with `[Required]`:

```csharp
[HttpGet("api/customers")]
[ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<PagedResult<CustomerDto>>> GetCustomers(
    [FromQuery][Required] int pageNo,
    [FromQuery][Required] int pageSize,
    [FromQuery] string? orderBy,
    CancellationToken cancellationToken = default)
{
    var result = await _mediator.Send(
        new GetCustomersQuery(pageNo: pageNo, pageSize: pageSize, orderBy: orderBy),
        cancellationToken);

    return Ok(result);
}
```
