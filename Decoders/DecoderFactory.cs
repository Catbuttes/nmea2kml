using System;
namespace nmea2kml.Decoders
{
    class DecoderFactory
    {
        public DecoderFactory()
        {

        }

        public IDecoder CreateDecoder(String nmeaSentence)
        {
            var nmeaType = nmeaSentence.Substring(3, 3);
            if(nmeaType =="GGA")
                return new GGADecoder(nmeaSentence);

            if(nmeaType =="GLL")
                return new GLLDecoder(nmeaSentence);

            return null;
        }
    }
}
