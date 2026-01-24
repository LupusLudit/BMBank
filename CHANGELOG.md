# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

GitHub repository to this project can be found at: [BMBank](https://github.com/LupusLudit/BMBank.git).

## [Unreleased]

### Added on 2026-01-18 by Michal Bielina

- Initial setup for the project structure
- Documentation folder created:
	- Class documentation to the ClassDocumentation.xml file
	- Technical documentation to the TechnicalDocumentation.md file
- Basic functionality - user is capable to connect to the bank server.
	- App (which includes the commands)
		- Commands (includes commands themselves)
	- Common (shared code used - or which could be used - in multiple files)
	- Network (client-server communication)
- Command for displaying the bank code to the user (BC)

### Added on 2026-01-24 by Michal Bielina

- This CHANGELOG.md file to track changes in the project.
- App.config file for configuration settings.
- More documentation to the ClassDocumentation.xml file
- The project database and added the sql script for its creation (provided in the Sql folder)
- Database connection and interaction logic (DatabaseInteraction folder)
	- Connection
	- Core
		- DAO
		- DBEntities
- All compulsory commands:
	- Account create (AC)
	- Account deposit (AD)
	- Account withdraw (AW)
	- Account balance (AB)
	- Account remove (AR)
	- Bank (total) amount (BA)
	- Bank number (of clients) (BN)