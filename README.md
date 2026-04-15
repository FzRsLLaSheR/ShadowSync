![C#](https://img.shields.io/badge/C%23-.NET%208.0-blue?logo=csharp)
![Windows](https://img.shields.io/badge/Windows-NT-blue?logo=windows)
![Security Research](https://img.shields.io/badge/purpose-detection%20engineering-red)
![Network](https://img.shields.io/badge/network-HTTP%20analysis-purple)
![License](https://img.shields.io/badge/license-MIT-green)

# ⚡ ShadowSync – File Collection & Transfer Research Tool (.NET)

> ⚠️ Security research & defensive analysis only

As of 2026-04-15, this project is designed for studying **file collection patterns, sensitive data exposure risks, and network transfer behaviors** in controlled environments for defensive cybersecurity research.

---

## 🚀 Purpose

**ShadowSync** is a security research tool built in **C#** aimed at analyzing **file enumeration** and **remote data transfer behaviors** commonly associated with data exfiltration and malicious software.

This project focuses on:

- **Simulating sensitive file collection**: Identifying and categorizing specific file types (documents, credentials, archives).
- **File transfer simulation**: Mimicking data exfiltration over HTTP(S) protocols (like WebDAV) to a remote endpoint.
- **Obfuscation techniques**: Demonstrating how malicious tools hide sensitive configuration data using ROT13 and XOR encoding to evade static analysis.
- **Detecting data exfiltration**: Helping security teams identify suspicious file transfer activities.

---

## 🧠 Research Goals

This tool is designed for:

- **Endpoint Detection & Response (EDR) research**
- **Data Loss Prevention (DLP) strategy validation**
- **Malware behavior simulation** (safe lab environments)
- **Network telemetry analysis**
- **Threat hunting rule development**

---

## ⚙️ Observed Behaviors (Research Model)

### 1. File System Enumeration
The tool recursively scans directories and identifies files based on:
- File extensions (documents, credentials, archives, etc.)
- File size thresholds (to avoid large payloads)
- Sensitive data categorization (e.g., passwords, email contents)

---

### 2. File Classification Model
Files are grouped into categories such as:
- **Office documents** (`.docx`, `.xlsx`, `.pdf`)
- **Archives** (`.zip`, `.bak`)
- **Credential stores** (`.kdbx`, `.pem`)
- **Communication exports** (`.msg`, `.eml`)

---

### 3. Network Transfer Simulation
The project models **HTTP-based file transfer behavior**:
- Uses **PUT requests** to simulate file uploads
- Implements **retry logic** and **long-running connections**
- Demonstrates **Basic Authentication** for communication

---

### 4. Configuration Obfuscation
Sensitive configuration values such as URLs and credentials are obfuscated using:
- **ROT13 transformation**
- **XOR encoding** (with a static key `0x09`)

These techniques simulate common evasion tactics used by malware to avoid static detection.

---

## 🌐 Security Research Context

This repository is relevant for:
- **Malware behavior analysis**
- **Detection engineering** (Sigma/YARA rules)
- **Incident response investigation patterns**
- **Endpoint telemetry correlation**
- **Data exfiltration simulation in lab environments**

---

## 🧪 Detection Opportunities

This tool can be used to design detections for:

- **Unusual bulk file reads** from user directories
- **Suspicious HTTP PUT requests** (large file uploads)
- **Encoded configuration decoding at runtime**
- **Repeated directory traversal patterns**
- **Suspicious file access** based on extension/type

---

## 🛑 Safe Usage Scope

This project should only be used in:

- **Isolated virtual machines**
- **Malware analysis sandboxes**
- **Authorized security research environments**
- **Academic cybersecurity labs**

---

## ⚠️ Disclaimer

This software is provided strictly for **educational and defensive research purposes only**.

The author does not condone or support unauthorized data access or misuse of this code outside controlled environments.

Any unauthorized use of this software is strictly discouraged, and the author assumes no responsibility for misuse.

---

## 📜 License

MIT License
