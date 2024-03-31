namespace コンボボックスの選択値によって動的に検証ルールを変更させる {
    public sealed class DeviceModel {
        public string Name { get; }
        public string ValidationPattern { get; }

        public DeviceModel(string name, string validationPattern) {
            Name = name;
            ValidationPattern = validationPattern;
        }
    }
}
