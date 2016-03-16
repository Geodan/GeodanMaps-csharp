using System;
using Geodan.Cloud.Client.Routing.Models;
using Geodan.Cloud.Client.Routing.RequestParams;

namespace Geodan.Cloud.Client.Routing.Extensions
{
    public static class UnitFormatTypeExtensions
    {
        public static string GetValue(this UnitFormatType type)
        {
            switch (type)
            {
                case UnitFormatType.minkm:
                    return "min-km";
                case UnitFormatType.minm:
                    return "min-m";
                case UnitFormatType.seckm:
                    return "sec-km";
                case UnitFormatType.secm:
                    return "sec-m";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static TimeUnit ToTimeUnit(this UnitFormatType type)
        {
            switch (type)
            {
                case UnitFormatType.minkm:
                    return TimeUnit.Minutes;
                case UnitFormatType.minm:
                    return TimeUnit.Minutes;
                case UnitFormatType.seckm:
                    return TimeUnit.Seconds;
                case UnitFormatType.secm:
                    return TimeUnit.Seconds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static DistanceUnit ToDistanceUnit(this UnitFormatType type)
        {
            switch (type)
            {
                case UnitFormatType.minkm:
                    return DistanceUnit.Kilometers;
                case UnitFormatType.minm:
                    return DistanceUnit.Meters;
                case UnitFormatType.seckm:
                    return DistanceUnit.Kilometers;
                case UnitFormatType.secm:
                    return DistanceUnit.Meters;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
