# csv-to-yealink-cfg

> A lightweight .NET tool to convert custom CSV contact lists into Yealink expansion module configuration files (.cfg).

## Features

* **Type-safe parsing:** Converts CSV data into structured Yealink configuration.
* **CLI Arguments:** Flexible input/output paths using `CommandLineParser`.
* **Standard Compliant:** Generates `.cfg` files compatible with Yealink T4x/T5x series expansion modules.

---

## 🛠 Installation

1. **Prerequisites:**
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or newer.


2. **Clone the repository:**
```bash
git clone https://github.com/rcorra68/csv-to-yealink-cfg.git
cd csv-to-yealink-cfg

```


3. **Build the project:**
```bash
dotnet build -c Release

```



---

## 🚀 Usage

Run the tool by specifying the source CSV file. If no output is specified, it defaults to `directory.cfg`.

```bash
dotnet run -- --source contacts.csv --output my_phone_config.cfg

```

### Options:

| Flag | Long Name | Required | Description |
| --- | --- | --- | --- |
| `-s` | `--source` | **Yes** | Path to the source CSV file. |
| `-o` | `--output` | No | Path for the generated .cfg (Default: `directory.cfg`). |
| `-t` | `--template` | No | (Coming soon) Path to a custom .tpl file. |
| `-v` | `--verbose` | No | Set output to verbose messages. |
| `-e` | `--env` | No | Target environment (Development, Staging, Production). |

---

## 📄 Data Mapping

### Input (CSV)

The tool expects a CSV with the following header: `key,label,line,type,value`

```csv
key,label,line,type,value
1,Segreteria - Affari generali,1,16,402
2,SOUP Postazione VVF,,2,00718064319

```

### Input (Excel)

A `.xlsx` source is read directly, no CSV needed. The first sheet mirrors the module: each row holds two keys side by side.

| A | B | C | D | E | F | G | H | I |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| key | label | type | value | | label | type | value | key |

* Rows whose key cell is not a number (e.g. `PAGINA 1`) are ignored.
* Keys with an empty label are skipped.
* Spaces, dashes, dots and slashes are stripped from numbers (`0765 123456` → `0765123456`).
* BLF keys use `line = 1`.
* Format the number columns as **Text** in Excel, or leading zeros are lost.

```bash
CsvToYealink -s Schema_Estensione.xlsx -o module1.cfg
```

### Supported Types (`type` column)

| Value | Yealink Type | Description |
| --- | --- | --- |
| `2` | **Forward** | Speed dial or call forwarding. |
| `16` | **BLF** | Busy Lamp Field (Monitoring). |

### Output (.cfg)

```ini
expansion_module.1.key.1.label = Segreteria
expansion_module.1.key.1.type = 16
expansion_module.1.key.1.line = 1
expansion_module.1.key.1.value = 402

```

---

## 🏗 Project Structure

* `Program.cs`: Application entry point and orchestration.
* `Options.cs`: CLI argument definitions.
* `CsvParser.cs`: Logic for reading and validating CSV data.
* `YealinkConfigGenerator.cs`: Logic for generating the final .cfg string.

---

## 📝 Roadmap / TODO

* [ ] Implement external templates via `--template`.
* [ ] Add Unit Tests for CSV validation.
