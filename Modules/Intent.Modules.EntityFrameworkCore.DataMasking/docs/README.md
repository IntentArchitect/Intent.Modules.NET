# Intent.EntityFrameworkCore.DataMasking

This modules adds support for a variety of `Data Masking` options on string entity attributes.

## Configure Data Masking

To configure a class attribute as having data masking, in the `Domain Designer`, apply the `Data Masking` stereotype to the attribute.

### Temporal Table annotation

Once applied, the attribute will be annotated with an icon to indicate it has been configured with _data masking_ (EmailAddress in the below screen shot):

![Annotated](images/data-masking-annotation.png)

### Configuration Properties

There are a number of properties available for customization of the data masking. Leaving the default values will still provide default data masking behavior.

![Properties](images/data-masking-configuration.png)

- **Data Masking Type**: The type of data masking to use.

  - **Set Length:** Data is always masked to the length specified (the default)
  - **Variable Length:** Data is always masked to the length of the original value
  - **Partial Mask:** Data is partially masked based on the configured lengths
- **Mask Character:** The character to use for masking (defaults to `*`)
- **Set Length:** The length of the masked data (only available and used when _Data Masking Type_ is _Set Length_)
- **Unmasked Prefix Length:** The length at the beginning of original value which will remain unmasked
- **Unmasked Prefix Length:** The length at the end of original value which will remain unmasked
- **Roles:** A comma-separated list of security roles that will bypass the data masking process. Users with these roles will have access to unmasked data. (Only available if a `Security Configuration` has not been added - otherwise the  `Security Roles` configuration field is used. The `Security Configuration` option is available by installing the [Intent.Metadata.Security](https://docs.intentarchitect.com/articles/modules-common/intent-metadata-security/intent-metadata-security.html) module)
- **Policies:** A comma-separated list of policies that will bypass the data masking process. Users with these policies will have access to unmasked data. (Only available if a `Security Configuration` has not been added - otherwise the  `Security Roles` configuration field is used. The `Security Configuration` option is available by installing the [Intent.Metadata.Security](https://docs.intentarchitect.com/articles/modules-common/intent-metadata-security/intent-metadata-security.html) module)
- **Security Roles:** A selection of security roles that will bypass the data masking process. Users with these roles will have access to unmasked data. (Only available if a `Security Configuration` has been added - only roles configured will be available for selection)
- **Security Policies:** A selection of policies that will bypass the data masking process. Users with these policies will have access to unmasked data. (Only available if a `Security Configuration` has been added - only policies configured will be available for selection)

## Updating Masked Data

Users which only have access to the masked data for a specific attribute, will not be able to persist updates back to the data store for that attribute - any updates will be ignored. Only users which have permission to the unmasked data, are permitted to updated the data store tables.