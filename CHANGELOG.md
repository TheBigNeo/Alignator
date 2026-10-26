# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## Unreleased
### Added
- New setting `Align argument names` (`csharp_alignator_align_argument_names`, default off): aligns the values of named arguments within one multi-line call by padding after the `name:` colon, and after the `Name =` assignment for attribute arguments. Applies to invocations, object creations and attributes; single-line calls stay untouched.

### Changed
- The comma alignment rules (`csharp_alignator_align_arguments_in_constructor`, `csharp_alignator_align_arguments_in_function`, `csharp_alignator_align_arguments_in_attribute`) now stand back for argument lists that use named arguments, including mixed lists, so they no longer stack with `csharp_alignator_align_argument_names` and pad the same lines twice.

## 1.0.0
- Initial version
