using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using WinCopies.GUI.Explorer;

namespace WinCopiesGUIWizard
{
    public class ViewStyleConverter : WinCopies.Util.Data.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)

        {

            Debug.Write("Convert method: ");

            Debug.Write(value);

            Debug.Write("\n");

            return value != null ? Application.Current.Resources[((ViewStyles)value).ToString()] : null;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Debug.Write("ConvertBack method: ");

            Debug.Write(value);

            Debug.Write("\n");

            if (value != null)

                foreach (DictionaryEntry resource in Application.Current.Resources)

                    if (resource.Value.GetType() == typeof(string) && (string)resource.Value == (string)value)

                        return Enum.Parse( typeof(ViewStyles),     (string) resource.Key)    ; 

            return null;

        }
    }
}
