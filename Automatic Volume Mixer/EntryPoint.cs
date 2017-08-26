using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avm.Forms;
using Avm.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;

namespace Avm
{
    internal static class EntryPoint
    {
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            Application.ThreadException += NBug.Handler.ThreadException;
            TaskScheduler.UnobservedTaskException += NBug.Handler.UnobservedTaskException;

            //NBug.Settings.

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
            private VariableViewWindow _variableViewWindow;

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
                        MessageBox.Show("Failed to load behaviours from the configuration file, they will be lost.",
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

                var disableBehaviours = new MenuItem("Disable AVM");
                disableBehaviours.Click += (sender, args) =>
                {
                    disableBehaviours.Checked = !disableBehaviours.Checked;
                    _automaticMixer.BehavioursEnabled = !disableBehaviours.Checked;
                };
                //_trayMenuStrip.Popup += (sender, args) => disableBehaviours.Checked = !_automaticMixer.BehavioursEnabled;

                _trayMenuStrip.MenuItems.Add(new MenuItem("Automatic Volume Mixer") { Enabled = false });
                _trayMenuStrip.MenuItems.Add("-");
                var openMain = new MenuItem("Open event manager") { DefaultItem = true };
                openMain.Click += OpenConfigManager;
                _trayMenuStrip.MenuItems.Add(openMain);
                //_trayMenuStrip.MenuItems.Add(new MenuItem("Open normalization manager", OpenConfigManager));
                _trayMenuStrip.MenuItems.Add("View audio sessions", OpenSessionPreview);
                _trayMenuStrip.MenuItems.Add("View variables", OpenVariablePreview);
                _trayMenuStrip.MenuItems.Add("-");
                //TODO _trayMenuStrip.MenuItems.Add(disableBehaviours);

                _trayMenuStrip.MenuItems.Add("Reset volumes of running audio sessions",
                    (sender, args) => _automaticMixer.ResetSessionVolumes());
                _trayMenuStrip.MenuItems.Add("Settings", OpenSettings);
                _trayMenuStrip.MenuItems.Add("-");

                var linkMenu = new MenuItem("Links");
                _trayMenuStrip.MenuItems.Add(linkMenu);
                linkMenu.MenuItems.Add("Homepage", (x, y) => PremadeDialogs.StartProcessSafely(@"https://github.com/Klocman/Automatic-Volume-Mixer"));
                linkMenu.MenuItems.Add("Send feedback", (x, y) => PremadeDialogs.StartProcessSafely(@"http://klocmansoftware.weebly.com/feedback--contact.html"));
                linkMenu.MenuItems.Add("Bug reports", (x, y) => PremadeDialogs.StartProcessSafely(@"https://github.com/Klocman/Automatic-Volume-Mixer/issues"));
                linkMenu.MenuItems.Add("Help", (x, y) => PremadeDialogs.StartProcessSafely(@"https://github.com/Klocman/Automatic-Volume-Mixer"));

                _trayMenuStrip.MenuItems.Add("Donate", (x, y) => PremadeDialogs.StartProcessSafely(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=TB9DA2P8KQX52"));
                _trayMenuStrip.MenuItems.Add("About", (x, y) => new AboutBox().Show());
                //_trayMenuStrip.MenuItems.Add(linkMenu);
                _trayMenuStrip.MenuItems.Add("-");
                _trayMenuStrip.MenuItems.Add("Shut down Automatic Volume Mixer", (sender, args) => Application.Exit());
            }

            private void OpenSessionPreview(object sender, EventArgs e)
            {
                if (_audioSessionWindow == null || _audioSessionWindow.IsDisposed)
                    _audioSessionWindow = new AudioSessionWindow(_automaticMixer);

                _audioSessionWindow.ShowAndMoveToTop();
            }

            private void OpenVariablePreview(object sender, EventArgs e)
            {
                if (_variableViewWindow == null || _variableViewWindow.IsDisposed)
                    _variableViewWindow = new VariableViewWindow(_automaticMixer);

                _variableViewWindow.ShowAndMoveToTop();
            }

            private void OpenConfigManager(object sender, EventArgs eventArgs)
            {
                if (_configurationManager == null || _configurationManager.IsDisposed)
                {
                    _configurationManager = new ConfigurationManager(_automaticMixer);
                    _configurationManager.ViewSessionsClick += OpenSessionPreview;
                    _configurationManager.ViewVariablesClick += OpenVariablePreview;
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
                //TODO Settings.Default.DisableBehaviours = !_automaticMixer.BehavioursEnabled;
                Settings.Default.Save();

                _automaticMixer.Dispose();
            }
        }
    }
}