using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Proto.Utils
{
    public static class DataExtensions
    {
        public static string ToStringDefault(this float value)
        {
            return value.ToString("0.000");
        }

        public static float ToFloat(this string value)
        {
            return float.Parse(value);
        }

        public static object GetValue(this object value)
        {
            if (value == null)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        return false;
                    case TypeCode.Byte:
                        return null;
                    case TypeCode.Char:
                        return '\0';
                    case TypeCode.DateTime:
                        throw new NotImplementedException();
                    case TypeCode.DBNull:
                        throw new NotImplementedException();
                    case TypeCode.Empty:
                        throw new NotImplementedException();
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Object:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.String:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return 0;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return value;
            }
        }

    }
}
