using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DockingLibrary
{
    /// <summary>
    /// ========================================
    /// .NET Framework 3.0 Custom Control
    /// ========================================
    ///
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DockingLibrary"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DockingLibrary;assembly=DockingLibrary"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file. Note that Intellisense in the
    /// XML editor does not currently work on custom controls and its child elements.
    ///
    ///     <MyNamespace:DockingButton/>
    ///
    /// </summary>
    public class DockingButton : Button
    {
        static DockingButton()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingButton), new FrameworkPropertyMetadata(typeof(DockingButton)));

            DockableContentProperty = DependencyProperty.Register("DockableContent", typeof(DockableContent), typeof(DockingButton));
            DockingButtonGroupProperty = DependencyProperty.Register("DockingButtonGroup", typeof(DockingButtonGroup), typeof(DockingButton));
            DockProperty = DependencyProperty.Register("Dock", typeof(Dock), typeof(DockingButton));
        }

        public static DependencyProperty DockableContentProperty;

        public DockableContent DockableContent
        {
            get
            {
                return GetValue(DockableContentProperty) as DockableContent;
            }
            set
            {
                SetValue(DockableContentProperty, value);
            }
        }

        public static DependencyProperty DockingButtonGroupProperty;

        public DockingButtonGroup DockingButtonGroup
        {
            get
            {
                return GetValue(DockingButtonGroupProperty) as DockingButtonGroup;
            }
            set
            {
                SetValue(DockingButtonGroupProperty, value);
                Dock = value.Dock;
            }
        }

        public static DependencyProperty DockProperty;

        public Dock Dock
        {
            get
            {
                return (Dock)GetValue(DockProperty);
            }
            set
            {
                SetValue(DockProperty, value);
            }
        }

    }
}
