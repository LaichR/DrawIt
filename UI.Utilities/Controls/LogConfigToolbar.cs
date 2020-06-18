using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using HIMS.Services.Core;
using HIMS.Services.Logging;
using Prism.Commands;
using Prism.Mvvm;

namespace Bluebottle.Base.Controls
{
    public class LogConfigToolbar : BindableBase
    {
        const string LogEnabledStringTemplate = "Log enabled in {0}";
        const string LogDisabledString = "Log is disabled";

        Bitmap _logEnableButtonImage = null;
        string _enableDisableLogTooltip = "";
        string _logLocation;
        DelegateCommand _enableDisableLog;
        LogLevel _selectedLogLevel = LogLevel.Keep;
        bool _logIsEnabled = false;

        public LogConfigToolbar(string logLocation)
        {
            _logLocation = logLocation;
            _logIsEnabled = false;
            _enableDisableLog = new DelegateCommand(() => ActionDecorator.DecorateCatchAndShowException("Enable/Disable log", OnEnableDisableLog)());

            LogManager.GetLogManager().RegisterLoggersConfiguration(null, new int[] { (int)SelectedLogLevel });

            ContextMenu cm = new ContextMenu();
            var m = new MenuItem
            {
                Header = "Open log",
                Command = new DelegateCommand(() => System.Diagnostics.Process.Start(_logLocation), () => System.IO.File.Exists(_logLocation)),
            };
            cm.Items.Add(m);
            //m = new MenuItem - Currently not available 
            //{
            //    Header = "Clear log",
            //    Command = new DelegateCommand(() => LogManager.GetLogManager().ClearLog(), () => System.IO.File.Exists(_logLocation)),
            //};
            //cm.Items.Add(m);

            ToolBar = new ToolBar();
            EnableDisableLogButtonImage = Bluebottle.Base.Properties.Resources.LogDisabled;
            EnableDisableLogTooltip = LogDisabledString;
            ToolBar.ContextMenu = cm;
            var bindings = new Dictionary<string, DependencyProperty>();
            bindings.Clear();
            bindings["EnableDisableLogButtonImage"] = Bluebottle.Base.Controls.ToolbarButton.BitmapProperty;
            bindings["EnableDisableLog"] = Bluebottle.Base.Controls.ToolbarButton.CommandProperty;
            bindings["EnableDisableLogTooltip"] = Bluebottle.Base.Controls.ToolbarButton.ToolTipProperty;
            var logEnableDisable = new Bluebottle.Base.Controls.ToolbarButton(this, EnableDisableLogButtonImage, bindings);
            ToolBar.Items.Add(logEnableDisable);

            bindings.Clear();
            var logLevelSelector = new ComboBox();
            logLevelSelector.SetBinding(ComboBox.SelectedValueProperty, "SelectedLogLevel");
            logLevelSelector.SetBinding(ComboBox.ItemsSourceProperty, "AvailableLogLevels");
            logLevelSelector.DataContext = this;
            logLevelSelector.Width = 60;
            ToolBar.Items.Add(logLevelSelector);
        }

        public ToolBar ToolBar { get; private set; }

        public string LogLocation
        {
            get { return _logLocation; }
            //set { _logLocation = value; }
        }

        public Bitmap EnableDisableLogButtonImage
        {
            get
            {
                return _logEnableButtonImage;
            }
            set
            {
                SetProperty<Bitmap>(ref _logEnableButtonImage, value);
            }
        }

        public string EnableDisableLogTooltip
        {
            get
            {
                return _enableDisableLogTooltip;
            }
            private set
            {
                SetProperty<string>(ref _enableDisableLogTooltip, value);
            }
        }

        public DelegateCommand EnableDisableLog
        {
            get { return _enableDisableLog; }
        }

        public LogLevel[] AvailableLogLevels
        {
            get
            {
                return ((LogLevel[])Enum.GetValues(typeof(LogLevel))).OrderBy((x) => (int)x).ToArray();
            }
        }

        public LogLevel SelectedLogLevel
        {
            get
            {
                return _selectedLogLevel;
            }
            set
            {
                SetProperty<LogLevel>(ref _selectedLogLevel, value);
                LogManager.GetLogManager().RegisterLoggersConfiguration(null, new int[] { (int)_selectedLogLevel });
            }
        }

        private void OnEnableDisableLog()
        {
            try
            {
                _logIsEnabled = !_logIsEnabled;

                LogManager.GetLogManager().SetLog(_logIsEnabled, _logLocation);
            }
            finally
            {
                if (_logIsEnabled)
                {
                    EnableDisableLogButtonImage = Bluebottle.Base.Properties.Resources.LogEnabled;
                    EnableDisableLogTooltip = string.Format(LogEnabledStringTemplate, _logLocation);
                }
                else
                {
                    EnableDisableLogButtonImage = Bluebottle.Base.Properties.Resources.LogDisabled;
                    EnableDisableLogTooltip = LogDisabledString;
                }
            }
        }
    }
}
