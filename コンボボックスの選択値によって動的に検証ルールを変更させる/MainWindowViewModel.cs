using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace コンボボックスの選択値によって動的に検証ルールを変更させる {
    public class MainWindowViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReactiveCollection<DeviceModel> DeviceModels { get; }
        public ReactivePropertySlim<DeviceModel> SelectedDeviceModel { get; }
        public ReactiveProperty<string> DeviceNumber { get; }
        public ReactiveCommand ExecuteCommand { get; }
        public ReadOnlyReactiveProperty<bool> CanExecute { get; }

        public MainWindowViewModel() {
            DeviceModels = new ReactiveCollection<DeviceModel>
            {
                new DeviceModel("A", "^AA[0-9]{6}$"),
                new DeviceModel("B", "^BB[0-9]{6}$")
            };

            SelectedDeviceModel = new ReactivePropertySlim<DeviceModel>();
            DeviceNumber = new ReactiveProperty<string>().SetValidateNotifyError(x => ValidateDeviceNumber(x));
            CanExecute = SelectedDeviceModel.Select(_ => DeviceNumber.Value != null && DeviceNumber.Value != "").ToReadOnlyReactiveProperty();

            ExecuteCommand = CanExecute.ToReactiveCommand().WithSubscribe(Execute);

            // Subscribe to selection change events
            SelectedDeviceModel.Subscribe(_ => DeviceNumber.Value = "");

            // Subscribe to text change events
            DeviceNumber.Subscribe(_ => DeviceNumber.ForceValidate());
        }

        private string ValidateDeviceNumber(string input) {
            if (SelectedDeviceModel.Value != null && !string.IsNullOrEmpty(input)) {
                if (!System.Text.RegularExpressions.Regex.IsMatch(input, SelectedDeviceModel.Value.ValidationPattern)) {
                    return "Invalid device number";
                }
            }
            return null;
        }

        private void Execute() {
            // Execute your logic here
            Console.WriteLine("Executing with device number: " + DeviceNumber.Value);
        }

        private IDisposable Disposable { get; } = new System.Reactive.Disposables.CompositeDisposable();

        public void Dispose() => Disposable.Dispose();
    }
}
