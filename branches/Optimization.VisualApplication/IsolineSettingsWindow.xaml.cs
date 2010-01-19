using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;

namespace Optimization.VisualApplication
{
    /// <summary>
    /// Interaction logic for IsolineSettings.xaml
    /// </summary>
    public partial class IsolineSettingsWindow : Window
    {
        public IsolineSettingsWindow()
        {
            InitializeComponent();
        }

        internal IsolineSetting IsolineSetting
        {
            get { return (IsolineSetting)this.DataContext; }
            set { this.DataContext = value; }
        }

        void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog box canceled
            this.DialogResult = false;
        }

        void okButton_Click(object sender, RoutedEventArgs e)
        {
            // Don't accept the dialog box if there is invalid data
            if (!IsValid(this)) return;

            // Dialog box accepted
            this.DialogResult = true;
        }

        // Validate all dependency objects in a window
        bool IsValid(DependencyObject node)
        {
            // Check if dependency object was passed
            if (node != null)
            {
                // Check if dependency object is valid.
                // NOTE: Validation.GetHasError works for controls that have validation rules attached 
                bool isValid = !Validation.GetHasError(node);
                if (!isValid)
                {
                    // If the dependency object is invalid, and it can receive the focus,
                    // set the focus
                    if (node is IInputElement) Keyboard.Focus((IInputElement)node);
                    return false;
                }
            }

            // If this dependency object is valid, check all child dependency objects
            foreach (object subnode in LogicalTreeHelper.GetChildren(node))
            {
                if (subnode is DependencyObject)
                {
                    // If a child dependency object is invalid, return false immediately,
                    // otherwise keep checking
                    if (IsValid((DependencyObject)subnode) == false) return false;
                }
            }

            // All dependency objects are valid
            return true;
        }
    }

    public class IsolineSettingValidationRule : ValidationRule
    {
        double minValue;
        double maxValue;

        public double MinValue
        {
            get { return this.minValue; }
            set { this.minValue = value; }
        }

        public double MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int inputValue;

            // Is a number?
            if (!int.TryParse((string)value, out inputValue))
            {
                return new ValidationResult(false, "Not a integer.");
            }

            // Is in range?
            if ((inputValue < this.minValue) || (inputValue > this.maxValue))
            {
                string msg = string.Format("Value must be between {0} and {1}.", this.minValue, this.maxValue);
                return new ValidationResult(false, msg);
            }

            // Number is valid
            return new ValidationResult(true, null);
        }
    }
}
