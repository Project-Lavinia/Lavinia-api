# Lavinia JSON Exporter

A lightweight CLI tool to export Norwegian election data as static JSON files.

## Overview

This tool reads election data from CSV files and outputs JSON files that match the Lavinia API contracts. It's designed for scenarios where the data is static and an API server is unnecessary.

## Usage

```bash
dotnet run [output-directory]
```

If no output directory is specified, files are exported to `./output/`.

## Output Files

The tool generates the following JSON files:

- **years.json** - List of all election years (sorted, newest first)
- **parties.json** - Dictionary mapping party codes to party names
- **districts.json** - List of all districts
- **votes.json** - All party votes by district, year, and election type
- **metrics.json** - District metrics (area, population, seats) by year
- **parameters.json** - Election parameters including algorithm settings

## Data Source

The tool reads data from CSV files in `../Api/Data/Countries/NO/`:
- `CountyData.csv` - District metrics
- `PE/*.csv` - Election results by year
- `PE/Elections.csv` - Election parameters

## Building

```bash
dotnet build
```

## Running

From the project directory:

```bash
dotnet run
```

Or after building, from the bin directory:

```bash
./bin/Debug/net9.0/LaviniaJsonExporter
```

## Requirements

- .NET 9.0 SDK
- Access to the Api/Data directory with election data CSV files
