using System; 
using System.Linq; 
using System.Windows;

namespace WPF_MVVM_Template
{
    public static class StorePositionBehavior
    {
        public static readonly DependencyProperty StorageKeyProperty = DependencyProperty.RegisterAttached(
            "StorageKey",
            typeof(string),
            typeof(Window),
            new UIPropertyMetadata("", OnStorageKeyChanged));

        public static string GetStorageKey(Window window)
            => (string)window.GetValue(StorageKeyProperty);

        public static void SetStorageKey(Window window, string value)
            => window.SetValue(StorageKeyProperty, value);

        static void OnStorageKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Window window) || !(e.NewValue is string property))
                return;

            var settingsType = (from type in window.GetType().Assembly.GetTypes()
                                where type.Name == "Settings"
                                where type.Namespace.EndsWith("Properties")
                                select type).FirstOrDefault();

            var instanceField = settingsType.GetProperty("Default");
            var instance = instanceField.GetValue(null);

            var save = settingsType.GetMethod("Save");
            var field = settingsType.GetProperty(property);

            if (field.GetValue(instance) is string data && DeserializedSetting(data, out var left, out var top, out var w, out var h))
            {
                window.Left = left;
                window.Top = top;
                window.Width = w;
                window.Height = h;
            }

            AdjustForScreenChanges(window);

            window.Closing += (_sender, _e) =>
            {
                field.SetValue(instance, SerializedSetting(window));
                save.Invoke(instance, null);
            };
        }

        static void AdjustForScreenChanges(Window window)
        {
            double Clamp(double val, double min, double max) => Math.Max(min, Math.Min(val, max));



            window.Left = Clamp(window.Left, SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth - window.Width);
            window.Top = Clamp(window.Top, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight - window.Height);
        }

        static string SerializedSetting(Window window)
            => $"{window.Left:0},{window.Top:0},{window.Width:0},{window.Height:0}";

        static bool DeserializedSetting(string entry, out int left, out int top, out int w, out int h)
        {
            left = top = w = h = 0;
            try
            {
                var data = entry.Split(',');
                left = int.Parse(data[0]);
                top = int.Parse(data[1]);
                w = int.Parse(data[2]);
                h = int.Parse(data[3]);



                return true;
            }
            catch { }

            return false;
        }
    }
}
