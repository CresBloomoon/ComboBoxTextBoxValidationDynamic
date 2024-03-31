using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace コンボボックスの選択値によって動的に検証ルールを変更させる {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private DeviceModel _selectedDeviceModel;
        public DeviceModel SelectedDeviceModel {
            get { return _selectedDeviceModel; }
            set {
                _selectedDeviceModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDeviceModel)));
                SetValidationRule();
            }
        }

        private string _deviceNumber;
        public string DeviceNumber {
            get { return _deviceNumber; }
            set {
                _deviceNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceNumber)));
                ValidateDeviceNumber();
            }
        }

        public ReactiveCollection<DeviceModel> DeviceModels { get; set; }

        public ReactiveCommand ExecuteCommand { get; private set; }
        public ReactiveProperty<bool> CanExecute { get; private set; }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            // Initialize device models
            DeviceModels = new ReactiveCollection<DeviceModel>
            {
                new DeviceModel("A", "^AA[0-9]{6}$"),
                new DeviceModel("B", "^BB[0-9]{6}$")
            };

            // Initialize commands and properties
            CanExecute = new ReactiveProperty<bool>(false);
            ExecuteCommand = CanExecute.ToReactiveCommand();
            ExecuteCommand.Subscribe(_ => Execute());

            // Subscribe to text change events
            Observable.FromEventPattern(deviceComboBox, nameof(deviceComboBox.SelectionChanged))
                .Merge(Observable.FromEventPattern(deviceNumberTextBox, nameof(deviceNumberTextBox.TextChanged)))
                .Subscribe(_ => SetValidationRule());
        }

        private void SetValidationRule() {
            if (SelectedDeviceModel != null) {
                // Set validation rule for device number textbox based on selected device model
                var regexPattern = SelectedDeviceModel.ValidationPattern;
                deviceNumberTextBox.Text = ""; // Clear the textbox
                deviceNumberTextBox.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("DeviceNumber") {
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    Converter = new ValidationConverter(regexPattern)
                });
            }
        }

        private void ValidateDeviceNumber() {
            // Check if device number matches the validation pattern
            CanExecute.Value = SelectedDeviceModel != null && System.Text.RegularExpressions.Regex.IsMatch(DeviceNumber, SelectedDeviceModel.ValidationPattern);
        }

        private void Execute() {
            // Execute your logic here
            MessageBox.Show("Executing with device number: " + DeviceNumber);
        }
    }
}
