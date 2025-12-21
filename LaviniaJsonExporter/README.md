# Lavinia JSON Exporter

A lightweight CLI tool to export Norwegian election data as static JSON files with AOT compilation for fast startup.

## Overview

This tool reads election data from CSV files and outputs JSON files that match the Lavinia API contracts. It's designed for scenarios where the data is static and an API server is unnecessary.

## Usage

```bash
# Basic usage (uses default directories)
dotnet run

# Specify output directory
dotnet run -- -o /path/to/output

# Specify both output and data directories
dotnet run -- -o /path/to/output -d /path/to/data

# Generate a summary report for CI/CD review
dotnet run -- -r

# Combine parameters
dotnet run -- -o /path/to/output -d /path/to/data -r

# Show help
dotnet run -- --help
```

### Parameters

- **-o, --output-dir** (optional) - Output directory for JSON files (default: `output`)
- **-d, --data-dir** (optional) - Data directory containing CSV files (default: `../Api/Data/Countries/NO`)
- **-r, --report** (optional) - Generate a summary report for CI/CD review (default: `false`)

### Summary Report

When the `-r` or `--report` flag is used, the tool generates a comprehensive summary report (`export-report.txt`) alongside the JSON files. This report includes:

- **Overview**: Export timestamp and output directory
- **Election Years**: Total count, year range, and list of all years
- **Political Parties**: Total count and top 5 parties by vote count
- **Districts**: Total count and complete list
- **Vote Data**: Total records, total votes cast, and votes by year
- **District Metrics**: Total records and latest year statistics (population, area, seats)
- **Election Parameters**: Latest election configuration (algorithm, threshold, seats)
- **Output Files**: Individual and total file sizes

The report is designed for CI/CD pipelines to review data changes, track historical trends, and validate exports.

## Building and Running

### Development Build

```bash
dotnet build
dotnet run
```

### AOT (Ahead-Of-Time) Compilation

For production use with fast startup times:

```bash
dotnet publish -c Release
./bin/Release/net9.0/linux-x64/publish/LaviniaJsonExporter
```

AOT compilation provides:
- Near-instant startup time
- Reduced memory footprint
- No JIT compilation overhead
- Single native executable

## Output Files

The tool generates the following JSON files:

- **years.json** - List of all election years (sorted, newest first)
- **parties.json** - Dictionary mapping party codes to party names
- **districts.json** - List of all districts
- **votes.json** - All party votes by district, year, and election type
- **metrics.json** - District metrics (area, population, seats) by year
- **parameters.json** - Election parameters including algorithm settings

## Expected Data File Structure

The data directory must have the following structure:

```
Data/Countries/NO/
├── CountyData.csv          # District metrics for all years
└── PE/                     # Parliamentary Election data
    ├── Elections.csv       # Election parameters for all years
    ├── 1945.csv           # Election results for 1945
    ├── 1949.csv           # Election results for 1949
    ├── ...                # One file per election year
    └── 2025.csv           # Election results for 2025
```

## CSV File Formats

### CountyData.csv

Semicolon-delimited file containing district metrics.

**Format:** `Year;County;Area;Population;Seats`

**Fields:**
- **Year** (int) - Election year
- **County** (string) - District name
- **Area** (double) - Geographic area in km²
- **Population** (int) - Population count
- **Seats** (int) - Number of predetermined district seats

**Example:**
```csv
Year;County;Area;Population;Seats
2025;Akershus;5895;728803;0
2025;Oslo;454;717710;0
```

**Notes:**
- Header line is required
- Lines starting with `#` are ignored

### PE/Elections.csv

Semicolon-delimited file containing election parameters.

**Format:** `Year;Algorithm;FirstDivisor;Threshold;AreaFactor;Seats;LevelingSeats`

**Fields:**
- **Year** (int) - Election year
- **Algorithm** (string) - Algorithm name (e.g., "Sainte Laguës (modified)", "d'Hondt")
- **FirstDivisor** (double) - First divisor for modified algorithms (empty for standard algorithms)
- **Threshold** (double) - Minimum percentage threshold for leveling seats
- **AreaFactor** (double) - Multiplier for area when distributing district seats
- **Seats** (int) - Total number of district seats
- **LevelingSeats** (int) - Number of leveling seats to distribute

**Example:**
```csv
Year;Algorithm;FirstDivisor;Threshold;AreaFactor;Seats;LevelingSeats
2025;Sainte Laguës (modified);1.4;4.0;1.8;150;19
2021;Sainte Laguës (modified);1.4;4.0;1.8;150;19
```

### PE/{year}.csv

Semicolon-delimited files containing election results per year (e.g., `2021.csv`, `2017.csv`).

**Format:** 18 fields per line (fields 0-17)

**Key Fields Used:**
- **Field 1** - Fylkenavn (District name)
- **Field 6** - Partikode (Party code)
- **Field 7** - Partinavn (Party name)
- **Field 12** - Antall stemmer totalt (Total votes)

**Example:**
```csv
Fylkenummer;Fylkenavn;Kommunenummer;Kommunenavn;Stemmekretsnummer;Stemmekretsnavn;Partikode;Partinavn;Oppslutning prosentvis;Antall stemmeberettigede;Antall forhåndsstemmer;Antall valgtingstemmer;Antall stemmer totalt;Endring % siste tilsvarende valg;Endring % siste ekvivalente valg;Antall mandater;Antall utjevningsmandater;
01;Østfold;;;;;A;Arbeiderpartiet;30.47944;223945;24817;24528;49345;-1.59066;3.00272;3;0;
01;Østfold;;;;;SV;Sosialistisk Venstreparti;6.07798;223945;5525;4315;9840;1.71448;1.87632;1;1;
```

**Notes:**
- Header line is required
- Lines starting with `#` are ignored
- Empty fields between semicolons are allowed

## Requirements

- .NET 9.0 SDK
- Access to CSV data files with the structure described above

## Technical Details

- Uses ConsoleAppFramework for parameter handling and help generation
- Supports Native AOT compilation for fast startup
- Simple semicolon-delimited CSV parsing
- Pretty-printed JSON output with proper Unicode encoding
