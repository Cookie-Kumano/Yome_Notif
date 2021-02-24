using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace YomeNotif
{
    class AutoSelectBehaviors : FreezableCollection<Behavior>
    {
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
           "Enabled",
           typeof(bool),
           typeof(AutoSelectBehaviors),
           new UIPropertyMetadata(false, Enabled_Changed));

        public static bool GetEnabled(DependencyObject obj) { return (bool)obj.GetValue(EnabledProperty); }
        public static void SetEnabled(DependencyObject obj, bool value) { obj.SetValue(EnabledProperty, value); }

        private static void Enabled_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var attachEvents = (bool)e.NewValue;
            var targetUiElement = (UIElement)sender;

            if (attachEvents)
            {
                targetUiElement.IsKeyboardFocusWithinChanged += TargetUiElement_IsKeyboardFocusWithinChanged;
            }
            else
            {
                targetUiElement.IsKeyboardFocusWithinChanged -= TargetUiElement_IsKeyboardFocusWithinChanged;
            }
        }

        static void TargetUiElement_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var targetUiElement = (UIElement)sender;

            if (targetUiElement.IsKeyboardFocusWithin)
            {
                Selector.SetIsSelected(targetUiElement, true);
            }
        }
    }
}
