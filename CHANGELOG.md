# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## Unreleased
### Added
- New first-argument settings `csharp_alignator_align_first_argument_in_constructor`, `csharp_alignator_align_first_argument_in_function` and `csharp_alignator_align_first_argument_in_attribute` (`FirstArgumentPosition`, default `do_not_change`): `space_before_parenthese` pads before the opening parenthesis so the parentheses (and with them the first arguments) share one column, `space_after_parenthese` pads after it so the first arguments share one column. The settings are independent of the comma alignment settings (`csharp_alignator_align_arguments_in_*`) and can be enabled at the same time - the first-argument setting pads the parenthesis gap, the comma setting pads the comma gaps.
- New setting `Align argument names` (`csharp_alignator_align_argument_names`, default off): aligns the values of named arguments within one multi-line call by padding after the `name:` colon, and after the `Name =` assignment for attribute arguments. Applies to invocations, object creations and attributes; single-line calls stay untouched.

### Changed
- The comma alignment rules (`csharp_alignator_align_arguments_in_constructor`, `csharp_alignator_align_arguments_in_function`, `csharp_alignator_align_arguments_in_attribute`) now stand back for argument lists that use named arguments, including mixed lists, so they no longer stack with `csharp_alignator_align_argument_names` and pad the same lines twice.

## 1.0.0
- Initial version
