### Version 1.1.11

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.1.10

- Improvement: Service proxies will now generate within any folders they are modelled within.

### Version 1.1.9

- Improvement: Updated referenced packages.

### Version 1.1.8

- Improvement: Removed internal shared project.

### Version 1.1.7

- Improvement: Updated to use improved common infrastructure shared with other service proxy modules.

### Version 1.1.6

- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 1.1.5

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 1.1.4

- Fixed: `Create` method on `DtoContracts` will now used the default values from the original `Command/Query`.

### Version 1.1.3

- Improvement: Included module help topic.

### Version 1.1.2

- Improvement: Small updated to module internal code

### Version 1.1.1

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.1.0

- Improvement: Updated according to changes made in `Intent.Eventing.MassTransit` module.

### Version 1.0.0

- New Feature: Add `Message Triggered` stereotypes to `Command`/`Query` elements to expose them as Consumers for MassTransit. Setup Service Proxies on the client side to interact with those endpoints as though they were HTTP based.
