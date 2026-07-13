# Homework 2: Customer Support Ticketing API

**Student**: Vlad Radchenko ([@Vladkee](https://github.com/Vladkee))

## Overview

REST API for customer support ticket management with multi-format import (CSV/JSON/XML), auto-classification engine, and comprehensive test coverage.

## Tech Stack

- **Framework**: ASP.NET Core 8 (.NET 8)
- **Storage**: In-memory (ConcurrentDictionary)
- **Testing**: xUnit + FluentAssertions + Coverlet
- **Import Libraries**: CsvHelper, System.Text.Json, System.Xml.Linq

## Features

- ✅ Full CRUD operations for support tickets
- ✅ Multi-format bulk import (CSV, JSON, XML)
- ✅ Auto-classification with confidence scoring
- ✅ Advanced filtering (status, priority, category, date range)
- ✅ >85% test coverage with 56 tests

## Quick Start

See [HOWTORUN.md](HOWTORUN.md) for detailed setup and demo instructions.

## Documentation

- [API Reference](API_REFERENCE.md) — All endpoints with request/response examples
- [Architecture](ARCHITECTURE.md) — System design and data flow diagrams
- [Testing Guide](TESTING_GUIDE.md) — Test strategy and coverage report

## AI Session Log

See [AI-CONVERSATION.md](AI-CONVERSATION.md) for all AI-assisted development sessions.
