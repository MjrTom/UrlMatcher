# Change Log

## Current Version

v3.0.2

- Performance optimizations: ~30% faster with reduced memory allocations
- Security fix: `Parts` property now returns a copy to prevent external mutation of internal state
- Improved query string and fragment stripping logic using `IndexOf` instead of `Split`
- Optimized `ExtractParameter` method to return parameter names without braces directly
- Enhanced XML documentation clarifying case-sensitivity, URL-encoding behavior, and parameter handling
- Modern C# syntax improvements (expression-bodied members, `StringComparison.Ordinal`)
- Removed unnecessary array allocations in `Split` calls
- Fixed logic inconsistency: `NameValueCollection` is now empty (not null) on match failure

## Previous Versions

v3.0.1

- XML documentation file generation
- Retargeting updates

v3.0.0

- Refactor to support both static methods and instances
- Instances take either a `Uri` or `string` (URL)

v2.0.x

- Migrate to `NameValueCollection` instead of `Dictionary`

v1.0.x

- Initial release