# Changelog

All notable changes to this project are documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/), versioning
follows [Semantic Versioning](https://semver.org/).
## [1.2.0] - 2026-11-23

### Documentation

- Update CHANGELOG

### Features

- Upgrade to .NET 10 and publish single-file executables

### Miscellaneous Tasks

- Fix release pipeline PR step
- Adopt reusable workflows architecture
- Remove obsolete deploy.yml placeholder
- Remove npm tooling and simplify CI pipeline
- Fix path to example file
- Implement reusable workflows
## [1.1.2] - 2026-04-27

### Other

- Implement automated release workflow with git-cliff and husky
## [1.1.1] - 2026-04-27

### Documentation

- Update CHANGELOG
- Modified CHANGELOG.md
- Update CHANGELOG
- Update CHANGELOG for v1.1.1

### Miscellaneous Tasks

- Align development environment
- Sync package-lock.json after dependency update
- Removing package-lock.json file after removing npm dependencies
- Changed the project name from Tests.Unit to Tests for development consistency

### Other

- Lock .NET SDK version to 8.0.200 in global.json
- Optimize dev workflow and relax commitlint rules
## [1.1.0] - 2026-03-20

### Features

- Testing automated release
## [1.0.1] - 2026-03-20

### Miscellaneous Tasks

- Add github actions workflow for automated build and test
- Update workflow triggers to target main branch

### Testing

- Trigger CI pipeline and verify branch naming
- Trigger CI pipeline and verify branch naming
## [1.0.0] - 2026-03-16

### Bug Fixes

- Serialize enum as numeric value instead of name in output (#2)
- Handle potential null context in CSV parsing exception

### Features

- Add default output path fallback if unspecified (#1)
- Implement robust error handling for CSV parsing and CLI feedback
