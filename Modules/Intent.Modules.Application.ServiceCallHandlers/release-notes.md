### Version 4.3.5

- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 4.3.4

- Improvement: Updated referenced packages versions

### Version 4.3.3

- Improvement: Updated module icon

### Version 4.3.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.3.1

- Improvement: Application Client Dto type using directives also to be resolved now in Service implementations.

### Version 4.3.0

- Improvement: Removed its own Service Implementation template (in favor of the existing module's implementation) and converted the SCH template into the File Builder in order for other modules to inject code into the implementation class.

### Version 4.2.1

- New Feature: Support for XmlDocComments on Operation Parameters.

### Version 4.2.0

- Improvement: Support CancellationTokens to align with interface.

### Version 4.1.1

- Improvement: Removed various compiler warnings.

### Version 4.0.2

- Improvement: Updated supported client version to [3.3.0-pre.0, 5.0.0-a).
