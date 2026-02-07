using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DentalNova.Movil.Helpers
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Caso 1: Si es un entero (ej. conteo de lista), devuelve TRUE si es 0
            if (value is int count)
                return count == 0;

            // Caso 2: Si es booleano, invierte el valor
            if (value is bool booleanValue)
                return !booleanValue;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
