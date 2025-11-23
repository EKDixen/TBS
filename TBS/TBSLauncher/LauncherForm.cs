using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace TBSLauncher
{
    public partial class LauncherForm : Form
    {
        private const string GITHUB_API_URL = "https://api.github.com/repos/EKDixen/TBS/releases/latest";
        private const string VERSION_FILE = "version.txt";
        private const string GAME_EXE = "TBS.exe";
        private const string TEMP_DOWNLOAD = "update_temp.zip";

        private Label statusLabel;
        private ProgressBar progressBar;
        private Button launchButton;

        public LauncherForm()
        {
            InitializeComponent();
            this.Load += LauncherForm_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "TBS Launcher";
            this.Size = new System.Drawing.Size(500, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            statusLabel = new Label
            {
                Text = "Checking for updates...",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(450, 30),
                Font = new System.Drawing.Font("Segoe UI", 12F),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(statusLabel);

            progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(50, 80),
                Size = new System.Drawing.Size(400, 30),
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(progressBar);

            launchButton = new Button
            {
                Text = "Launch Game",
                Location = new System.Drawing.Point(175, 140),
                Size = new System.Drawing.Size(150, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Visible = false
            };
            launchButton.Click += LaunchButton_Click;
            this.Controls.Add(launchButton);
        }

        private async void LauncherForm_Load(object? sender, EventArgs e)
        {
            await CheckAndUpdate();
        }

        private async Task CheckAndUpdate()
        {
            try
            {
                statusLabel.Text = "Checking for updates...";
                progressBar.Value = 0;

                string currentVersion = GetCurrentVersion();
                
                string latestVersion = await GetLatestVersionFromGitHub();
                string downloadUrl = await GetDownloadUrlFromGitHub();

                if (string.IsNullOrEmpty(latestVersion) || string.IsNullOrEmpty(downloadUrl))
                {
                    statusLabel.Text = "Could not check for updates. Launching game...";
                    await Task.Delay(1000);
                    LaunchGame();
                    return;
                }

                // Compare versions
                if (currentVersion == latestVersion)
                {
                    statusLabel.Text = "Game is up to date! Launching...";
                    progressBar.Value = 100;
                    await Task.Delay(1000);
                    LaunchGame();
                }
                else
                {
                    statusLabel.Text = $"Updating from v{currentVersion} to v{latestVersion}...";
                    await DownloadAndInstallUpdate(downloadUrl, latestVersion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during update check: {ex.Message}\n\nLaunching game anyway...", 
                    "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LaunchGame();
            }
        }

        private string GetCurrentVersion()
        {
            try
            {
                if (File.Exists(VERSION_FILE))
                {
                    return File.ReadAllText(VERSION_FILE).Trim();
                }
            }
            catch { }
            return "0.0.0";
        }

        private async Task<string> GetLatestVersionFromGitHub()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TBS-Launcher");
                    string response = await client.GetStringAsync(GITHUB_API_URL);
                    JObject json = JObject.Parse(response);
                    string? tagName = json["tag_name"]?.ToString();
                    return tagName?.TrimStart('v') ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        private async Task<string> GetDownloadUrlFromGitHub()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TBS-Launcher");
                    string response = await client.GetStringAsync(GITHUB_API_URL);
                    JObject json = JObject.Parse(response);
                    
                    // Look for a .zip asset in the release
                    var assets = json["assets"];
                    if (assets != null)
                    {
                        foreach (var asset in assets)
                        {
                            string? name = asset["name"]?.ToString();
                            if (name != null && name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                            {
                                return asset["browser_download_url"]?.ToString() ?? "";
                            }
                        }
                    }
                    
                    // Fallback to zipball_url if no asset found
                    return json["zipball_url"]?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        private async Task DownloadAndInstallUpdate(string downloadUrl, string newVersion)
        {
            try
            {
                statusLabel.Text = "Downloading update...";
                
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TBS-Launcher");
                    
                    using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        
                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(TEMP_DOWNLOAD, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            var buffer = new byte[8192];
                            long totalRead = 0;
                            int bytesRead;
                            
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;
                                
                                if (totalBytes > 0)
                                {
                                    progressBar.Value = (int)((totalRead * 100) / totalBytes);
                                }
                            }
                        }
                    }
                }

                statusLabel.Text = "Installing update...";
                progressBar.Value = 90;

                string tempExtractPath = "temp_extract";
                if (Directory.Exists(tempExtractPath))
                    Directory.Delete(tempExtractPath, true);
                
                ZipFile.ExtractToDirectory(TEMP_DOWNLOAD, tempExtractPath);

                string gameFilesPath = FindGameFilesPath(tempExtractPath);
                
                if (!string.IsNullOrEmpty(gameFilesPath))
                {
                    CopyGameFiles(gameFilesPath, Directory.GetCurrentDirectory());
                }

                File.WriteAllText(VERSION_FILE, newVersion);

                File.Delete(TEMP_DOWNLOAD);
                Directory.Delete(tempExtractPath, true);

                statusLabel.Text = "Update complete! Launching game...";
                progressBar.Value = 100;
                await Task.Delay(1000);
                
                LaunchGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error installing update: {ex.Message}\n\nTrying to launch game anyway...", 
                    "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LaunchGame();
            }
        }

        private string FindGameFilesPath(string extractPath)
        {
            if (File.Exists(Path.Combine(extractPath, GAME_EXE)))
                return extractPath;

            foreach (var dir in Directory.GetDirectories(extractPath, "*", SearchOption.AllDirectories))
            {
                if (File.Exists(Path.Combine(dir, GAME_EXE)))
                    return dir;
            }

            return extractPath; // Return root if not found
        }

        private void CopyGameFiles(string sourceDir, string targetDir)
        {
            foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(sourceDir, file);
                
                if (relativePath.Contains("Launcher", StringComparison.OrdinalIgnoreCase))
                    continue;

                string targetPath = Path.Combine(targetDir, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                
                File.Copy(file, targetPath, true);
            }
        }

        private void LaunchGame()
        {
            try
            {
                if (File.Exists(GAME_EXE))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "conhost.exe",
                        Arguments = $"\"{Path.Combine(Directory.GetCurrentDirectory(), GAME_EXE)}\"",
                        UseShellExecute = false,
                        WorkingDirectory = Directory.GetCurrentDirectory()
                    });
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    MessageBox.Show($"Game executable not found: {GAME_EXE}\n\nPlease reinstall the game.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching game: {ex.Message}",
                    "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
        }

        private void LaunchButton_Click(object? sender, EventArgs e)
        {
            LaunchGame();
        }
    }
}
