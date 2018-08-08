using System;
using System.Text;

namespace nmea2kml.Decoders
{
    class GGADecoder : IDecoder
    {
        private DateTime SentanceTime;
        private String LatitudeDegrees;
        private String LatitudeMinutes;
        private bool North;
        private String LongitudeDegrees;
        private String LongitudeMinutes;
        private bool East;
        private double Altitude;
        private int Satellites;
        public GGADecoder(String GGAString)
        {
            var data = GGAString.Split(',');

            //Sanity check
            if (!data[0].EndsWith("GGA"))
            {
                throw new NotSupportedException("Unsupported NMEA 0183 Sentence passed to GGA decoder");
            }

            var hour = int.Parse(data[1].Substring(0, 2));
            var minute = int.Parse(data[1].Substring(2, 2));
            var second = int.Parse(data[1].Substring(4, 2));
            this.SentanceTime = new DateTime(1970, 01, 01, hour, minute, second);

            this.LatitudeDegrees = data[2].Substring(0, 2);
            this.LatitudeMinutes = data[2].Substring(2);
            this.North = (data[3] == "N");
            while(this.LatitudeDegrees.StartsWith("0"))
            {
                //Trim off leading zeros
                this.LatitudeDegrees = this.LatitudeDegrees.Substring(1);
            }

            this.LongitudeDegrees = data[4].Substring(0, 3);
            this.LongitudeMinutes = data[4].Substring(3);
            while(this.LongitudeDegrees.StartsWith("0"))
            {
                //Trim off leading zeros
                this.LongitudeDegrees = this.LongitudeDegrees.Substring(1);
            }
            this.East = (data[5] == "E");


            double.TryParse(data[9], out this.Altitude);
            int.TryParse(data[7], out this.Satellites);
        }

        private static string ToDecimalDegrees(string sDegs, string sMins, bool positive)
        {
            var dmins = float.Parse(sMins);
            var ddegs = dmins / 60;
            var degs = float.Parse(sDegs);
            return String.Format("{0}{1}", positive?"":"-", degs+ddegs);
        }

        public String KmlPlacemark()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<Placemark>");
            sb.AppendLine("<name>NMEA0183 GGADecoder Fix</name>");
            sb.AppendLine("<description>");
            sb.AppendLine("<![CDATA[");
            sb.AppendFormat("Fix at {0}\nWith {1} Satellites\n", this.SentanceTime.TimeOfDay.ToString(), this.Satellites);
            if(this.Altitude > 0)
            {
                sb.AppendFormat("At {0}M\n", this.Altitude);
            }
            sb.AppendLine("]]>");
            sb.AppendLine("</description>");
            sb.AppendLine("<Point>");
            sb.AppendFormat("<coordinates>{0},", ToDecimalDegrees(this.LatitudeDegrees, this.LatitudeMinutes, this.North));
            sb.AppendFormat("{0},{1}</coordinates>\n", ToDecimalDegrees(this.LongitudeDegrees, this.LongitudeMinutes, this.East), this.Altitude);
            sb.AppendLine("</Point>");
            sb.AppendLine("</Placemark>");

            return sb.ToString();
        }
    }
}