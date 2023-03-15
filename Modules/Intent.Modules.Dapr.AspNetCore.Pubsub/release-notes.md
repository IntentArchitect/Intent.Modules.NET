### Version 1.0.1

- Fixed: Event handlers would be generated into a sub-folder with the same name as the message which depending on the message name would cause conflicts between the fully qualified message type and the event handler's namespace.
- Fixed: `IntentManaged` for the `Body` of event handlers classes was set to `Fully` instead of `Merge`.
