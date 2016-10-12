using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Avm.Forms;
using Avm.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Microsoft.Win32;

namespace Avm
{
    internal static class EntryPoint
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainApplication());
        }

        private sealed class MainApplication : ApplicationContext
        {
            private readonly AutomaticMixer _automaticMixer;
            private AudioSessionWindow _audioSessionWindow;
            private ConfigurationManager _configurationManager;
            private SaveFileDialog _exportDialog;
            private OpenFileDialog _importDialog;
            private SettingsWindow _settingsWindow;
            private NotifyIcon _trayIcon;
            private ContextMenu _trayMenuStrip;

            public MainApplication()
            {
                InitializeInterface();
                
                _automaticMixer = new AutomaticMixer();
                if (!string.IsNullOrWhiteSpace(Settings.Default.Behaviours))
                {
                    try
                    {
                        _automaticMixer.SetBehavioursFromString(Settings.Default.Behaviours);
                    }
                    catch
                    {
                        // Settings were invalid, TODO info box
                        MessageBox.Show("Failed to load events from the configuration file, they will be lost.",
                            "Automatic Volume Manager", MessageBoxButtons.OK);
                    }
                }
                _automaticMixer.BehavioursEnabled = !Settings.Default.DisableBehaviours;

                Application.ApplicationExit += OnApplicationExit;
            }

            private void InitializeInterface()
            {
                _trayIcon = new NotifyIcon();
                _trayMenuStrip = new ContextMenu();

                _trayIcon.Icon = Resources.trayicon;
                _trayIcon.Text = "Automatic Volume Mixer";
                _trayIcon.ContextMenu = _trayMenuStrip;
                _trayIcon.Visible = true;
                _trayIcon.DoubleClick += OpenConfigManager;

                _importDialog = new OpenFileDialog
                {
                    Multiselect = false,
                    CheckFileExists = true,
                    Title = "Import AVM events...",
                    Filter = "XML files|*.xml",
                    DefaultExt = "xml"
                };
                _exportDialog = new SaveFileDialog
                {
                    ValidateNames = true,
                    Title = "Export AVM events...",
                    Filter = "XML files|*.xml",
                    DefaultExt = "xml"
                };

                var disableBehaviours = new MenuItem("Disable all events");
                disableBehaviours.Click += (sender, args) =>
                {
                    disableBehaviours.Checked = !disableBehaviours.Checked;
                    _automaticMixer.BehavioursEnabled = !disableBehaviours.Checked;
                };
                _trayMenuStrip.Popup += (sender, args) => disableBehaviours.Checked = !_automaticMixer.BehavioursEnabled;

                _trayMenuStrip.MenuItems.Add(new MenuItem("Automatic Volume Mixer") { Enabled = false });
                _trayMenuStrip.MenuItems.Add("-");
                _trayMenuStrip.MenuItems.Add("Open event manager", OpenConfigManager);
                //_trayMenuStrip.MenuItems.Add(new MenuItem("Open normalization manager", OpenConfigManager));
                _trayMenuStrip.MenuItems.Add("View audio sessions", OpenSessionPreview);
                _trayMenuStrip.MenuItems.Add("-");
                _trayMenuStrip.MenuItems.Add(disableBehaviours);

                var startOnBootMenuItem = new MenuItem("Start AVM on boot") { Checked = false };
                using (var key = Registry.LocalMachine
                    .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    startOnBootMenuItem.Checked = key.GetValue("AutomaticVolumeMixer") != null;
                }
                startOnBootMenuItem.Click += (sender, args) =>
                {
                    startOnBootMenuItem.Checked = !startOnBootMenuItem.Checked;
                    using (var key = Registry.LocalMachine
                        .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (startOnBootMenuItem.Checked)
                            key.SetValue("AutomaticVolumeMixer", $"\"{Assembly.GetExecutingAssembly().Location}\"");
                        else
                            key.DeleteValue("AutomaticVolumeMixer");
                    }
                };
                _trayMenuStrip.MenuItems.Add(startOnBootMenuItem);
                _trayMenuStrip.MenuItems.Add("Reset audio session volumes",
                    (sender, args) => _automaticMixer.ResetSessionVolumes());
                _trayMenuStrip.MenuItems.Add("Options", OpenSettings);
                _trayMenuStrip.MenuItems.Add("-");
                //var linkMenu = new MenuItem("Open website...") { Enabled = false };
                //linkMenu.MenuItems.Add(new MenuItem("Homepage"));
                //linkMenu.MenuItems.Add(new MenuItem("Rate this app"));
                _trayMenuStrip.MenuItems.Add("Send feedback", (x, y) => PremadeDialogs.StartProcessSafely(
                    @"http://klocmansoftware.weebly.com/feedback--contact.html"));
                _trayMenuStrip.MenuItems.Add("Donate", (x, y) => PremadeDialogs.StartProcessSafely(
                    @"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=TB9DA2P8KQX52"));
                //_trayMenuStrip.MenuItems.Add(linkMenu);
                _trayMenuStrip.MenuItems.Add(new MenuItem("Help") { Enabled = false });
                _trayMenuStrip.MenuItems.Add("-");
                _trayMenuStrip.MenuItems.Add("Shut down AVM", (sender, args) => Application.Exit());
            }

            private void OpenSessionPreview(object sender, EventArgs e)
            {
                if (_audioSessionWindow == null || _audioSessionWindow.IsDisposed)
                    _audioSessionWindow = new AudioSessionWindow(_automaticMixer);

                _audioSessionWindow.ShowAndMoveToTop();
            }

            private void OpenConfigManager(object sender, EventArgs eventArgs)
            {
                if (_configurationManager == null || _configurationManager.IsDisposed)
                {
                    _configurationManager = new ConfigurationManager(_automaticMixer);
                    _configurationManager.ViewSessionsClick += OpenSessionPreview;
                    _configurationManager.SettingsClick += OpenSettings;
                    _configurationManager.ExportClick += OpenExport;
                    _configurationManager.ImportClick += OpenImport;
                }

                _configurationManager.ShowAndMoveToTop();
            }

            private void OpenImport(object sender, EventArgs e)
            {
                if (_importDialog.ShowDialog() == DialogResult.OK
                    && _importDialog.FileName != null
                    && File.Exists(_importDialog.FileName))
                {
                    var text = File.ReadAllText(_importDialog.FileName);
                    try
                    {
                        _automaticMixer.SetBehavioursFromString(text, false);
                    }
                    catch (Exception ex)
                    {
                        PremadeDialogs.GenericError(ex);
                    }
                }
            }

            private void OpenExport(object sender, EventArgs e)
            {
                if (_exportDialog.ShowDialog() == DialogResult.OK
                    && !string.IsNullOrEmpty(_exportDialog.FileName))
                {
                    File.WriteAllText(_exportDialog.FileName, _automaticMixer.GetBehavioursAsString(false));
                }
            }

            private void OpenSettings(object sender, EventArgs e)
            {
                if (_settingsWindow == null || _settingsWindow.IsDisposed)
                    _settingsWindow = new SettingsWindow();

                _settingsWindow.ShowAndMoveToTop();
            }

            private void OnApplicationExit(object sender, EventArgs eventArgs)
            {
                _trayIcon.Dispose();
                _trayMenuStrip.Dispose();

                _audioSessionWindow?.Dispose();
                _configurationManager?.Dispose();

                if (Settings.Default.RestoreSessionSettingsOnExit)
                    _automaticMixer.ResetSessionVolumes();

                Settings.Default.Behaviours = _automaticMixer.GetBehavioursAsString(true);
                Settings.Default.DisableBehaviours = !_automaticMixer.BehavioursEnabled;
                Settings.Default.Save();

                _automaticMixer.Dispose();
            }
        }
    }
}