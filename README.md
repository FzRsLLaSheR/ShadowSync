![C#](https://img.shields.io/badge/C%23-.NET%208.0-blue?logo=csharp)
![Windows](https://img.shields.io/badge/Windows-NT-blue?logo=windows)
![Research](https://img.shields.io/badge/purpose-security%20research-red)
![License](https://img.shields.io/badge/license-MIT-green)

# ⚡ ShadowSync – File Collection & Transfer Research Tool (.NET)

**ShadowSync** is a **C#** tool designed for **educational purposes** and **security research**. It simulates file enumeration, filtering, and transfer behaviors that resemble data exfiltration techniques often used by malware.

---

## 🚀 What it Does

- Scans directories for files with specific extensions and sizes (e.g., `.docx`, `.xlsx`, `.pdf`, `.zip`, etc.)
- Transfers selected files to a remote server using **WebDAV** (simulating data exfiltration)
- Uses simple obfuscation techniques (ROT13 + XOR) to encode sensitive configuration values like URLs and credentials
- Demonstrates basic network communication patterns for research into data transfer over the internet

---

## ⚙️ How it Works

1. **File Enumeration**: Recursively scans a specified directory and filters files based on extension and size.
2.  **File Transfer via WebDAV**: Files are uploaded to a remote WebDAV server using HTTP PUT requests, a common method for remote file storage and transfer.
3. **Obfuscation**: Sensitive data like URLs and credentials are obfuscated using ROT13 and XOR.
4. **Network Simulation**: Mimics exfiltration patterns by sending files over HTTP(S) using WebDAV.

---

## 🧳 Anti-VM & Anti-Debugging Features

To avoid detection in **virtual machines** or **debugger environments**, **ShadowSync** implements several defensive techniques:

### 1. **Anti-Low Memory Check**
The program performs a check to determine if the system has at least **2GB of RAM**. This prevents the tool from running in environments with insufficient resources. If the system fails this check, the program will terminate automatically with the following message:
[!] Low memory detected! Exiting...

### 2. **Anti-Debug**
- The program checks for active debuggers using common techniques, such as:
  - **PEB (Process Environment Block)** checks
  - **IsDebuggerPresent** API calls

  ### 3. **Blacklisted process**
- The program checks if any blacklisted processes are running on the system. These processes are often associated with monitoring or anti-malware tools that could interfere with or flag the behavior of ShadowSync. The program will terminate if such a process is detected, with the following message:
 [!] Blacklisted process detected! Exiting...
  
If these conditions are detected, the program either terminates itself or alters its behavior to avoid being analyzed.

---

## 📦 Requirements

- **.NET Framework 4.8 or later**
  
---

## ⚠️ Disclaimer

This software is provided strictly for **educational and defensive research purposes only**.

The author does not condone or support unauthorized data access or misuse of this code outside controlled environments.

Any unauthorized use of this software is strictly discouraged, and the author assumes no responsibility for misuse.

---

## 📜 License

MIT License
