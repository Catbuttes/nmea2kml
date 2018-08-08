using System;
namespace nmea2kml.Decoders
{
    public interface IDecoder
    {
        String KmlPlacemark();
    }
}
