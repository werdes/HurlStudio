using Avalonia.Controls;
using Avalonia.Media;
using HurlStudio.Collections.Attributes;
using HurlStudio.Collections.Settings;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.Model.Enums;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.UiState;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingEnabledChangedEventArgs>? SettingEnabledChanged;
        public event EventHandler<SettingOrderChangedEventArgs>? SettingOrderChanged;
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingKeyChanged;
        public event EventHandler<SettingCollapsedChangedEventArgs>? SettingCollapsedChanged;
        public event EventHandler<SettingChangedEventArgs>? SettingChanged;

        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _isReadOnly;
        private bool _collapsed;
        private bool _overwritten;
        private bool _canMove;
        private bool _isEnabled;
        private BaseSetting _setting;
        private IEditorDocument? _document;
        private HurlSettingSection _settingSection;
        private EnableType _enableType;

        public HurlSettingContainer(IEditorDocument? document, HurlSettingSection settingSection, BaseSetting setting, bool isReadOnly, bool canMove, EnableType enableType)
        {
            _isReadOnly = isReadOnly;
            _collapsed = false;
            _canMove = canMove;
            _setting = setting;
            _document = document;
            _settingSection = settingSection;
            _isEnabled = true;
            _enableType = enableType;

            _setting.PropertyChanged += this.On_Setting_PropertyChanged;
            _setting.SettingPropertyChanged += this.On_Setting_SettingPropertyChanged;
        }

        /// <summary>
        /// Listen for any notified changes on the underlying setting and propagate it to the section
        /// Compared to the PropertyChanged event of INotifyPropertyChanged, this only indicates that something has changed
        /// It's also thrown on setting properties of type ObservableCollection having their items updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Setting_SettingPropertyChanged(object? sender, Collections.Model.EventArgs.SettingPropertyChangedEventArgs e)
        {
            SettingChanged?.Invoke(this, new SettingChangedEventArgs(_setting));
        }

        /// <summary>
        /// Listen for Property changes on the underlying setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Setting_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyInfo? propertyInfo = sender?.GetType().GetProperty(e.PropertyName ?? string.Empty);

            if (propertyInfo != null)
            {
                if (propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(HurlSettingKeyAttribute)))
                {
                    this.SettingKeyChanged?.Invoke(this, new SettingEvaluationChangedEventArgs(this));
                }
                if (propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(HurlSettingDisplayStringAttribute)))
                {
                    this.Setting.RefreshDisplayString();
                }
            }

            // Reevaluate, when IsEnabled setting is changed
            if (e.PropertyName == nameof(_setting.IsEnabled))
            {
                this.SettingEnabledChanged?.Invoke(this, new SettingEnabledChangedEventArgs(_setting.IsEnabled, _settingSection.SectionType));
                this.Notify(nameof(this.DisplayOpacity)); 
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                this.Notify();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                this.Notify();
                this.Notify(nameof(this.DisplayOpacity));
                this.SettingEnabledChanged?.Invoke(this, new SettingEnabledChangedEventArgs(_isEnabled, _settingSection.SectionType));
            }
        }

        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                _collapsed = value;
                this.Notify();
                this.SettingCollapsedChanged?.Invoke(this, new SettingCollapsedChangedEventArgs(_collapsed));
            }
        }

        public bool Overwritten
        {
            get => _overwritten;
            set
            {
                _overwritten = value;
                this.Notify();
                this.Notify(nameof(this.DisplayOpacity));
                this.Notify(nameof(this.DisplayTextDecoration));
            }
        }

        public bool CanMove
        {
            get => _canMove;
        }

        public EnableType EnableType
        {
            get => _enableType;
        }

        public bool ChangeEnabledStateInContainer
        {
            get => _enableType == EnableType.Container;
        }

        public bool IsFileSettingSection
        {
            get => _settingSection.SectionType == Enums.HurlSettingSectionType.File;
        }

        public BaseSetting Setting
        {
            get => _setting;
            set
            {
                _setting = value;
                this.Notify();
            }
        }

        public HurlSettingSection SettingSection
        {
            get => _settingSection;
        }

        public IEditorDocument Document
        {
            get => _document;
        }

        public double DisplayOpacity
        {
            // Either overwritten or disabled -> 0.5 opacity
            get => ((_enableType == EnableType.Container) ? (!_isEnabled || !_setting.IsEnabled) : !_setting.IsEnabled) || _overwritten ? 0.5D : 1D;
        }

        public bool UnderlyingSettingDisabled
        {
            get => ((_enableType == EnableType.Container) && !_setting.IsEnabled);
        }

        public bool IsInFileDocument
        {
            get => _document is FileDocumentViewModel;
        }

        public TextDecorationCollection? DisplayTextDecoration
        {
            // Either overwritten or the setting being disabled despite being enabled/disabled by its container -> strikethrough
            get => _overwritten || ( _enableType == EnableType.Container && !_setting.IsEnabled ) ? TextDecorations.Strikethrough : null;
        }

        public bool IsRedefineInFileSettingsVisible
        {
            get => this.IsInFileDocument && !this.IsFileSettingSection;
        }
        public bool IsDuplicateVisible
        {
            get => this.IsInFileDocument && this.IsFileSettingSection;
        }

        public void MoveUp() => this.SettingOrderChanged?.Invoke(this, new SettingOrderChangedEventArgs(this, Enums.MoveDirection.Up));
        public void MoveDown() => this.SettingOrderChanged?.Invoke(this, new SettingOrderChangedEventArgs(this, Enums.MoveDirection.Down));
        public void MoveToTop() => this.SettingOrderChanged?.Invoke(this, new SettingOrderChangedEventArgs(this, Enums.MoveDirection.ToTop));
        public void MoveToBottom() => this.SettingOrderChanged?.Invoke(this, new SettingOrderChangedEventArgs(this, Enums.MoveDirection.ToBottom));

        public string GetId()
        {
            if (_settingSection == null) throw new ArgumentNullException(nameof(this.SettingSection));
            if (_settingSection.CollectionComponent == null) throw new ArgumentNullException(nameof(this.SettingSection.CollectionComponent));
            if (!_settingSection.SettingContainers.Contains(this)) throw new InvalidOperationException($"{this} not in setting containers");

            string id = _document?.GetId() + "#" +
                        _settingSection.CollectionComponent.GetId() + "#" +
                        _settingSection.SettingContainers.IndexOf(this);
            return id.ToSha256Hash();
        }
    }
}
