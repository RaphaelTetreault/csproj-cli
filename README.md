# csproj-cli
A command-line interface to update C# .csproj properties.

## General Format

`path action --args`

### Required

1. `<path>`: Required. The path (file or directory) of the .csproj file.
2. `action`: Required. The action to perform.

### Actions

- `modify-property --name="" --value=""` 
- `bump-version-major`
- `bump-version-minor`
- `bump-version-patch`

### Args

* `--name`: The name of the XML item.
* `--value`: The value of the XML item.
