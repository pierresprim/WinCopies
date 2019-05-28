using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WinCopies.IO;

namespace WinCopies.GUI
{
    [ValueConversion(typeof(IBrowsableObjectInfo), typeof(ImageSource))]
    public class IBrowsableObjectInfoToImageSourceConverter : Util.Data.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((IBrowsableObjectInfo)value).SmallBitmapSource;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
