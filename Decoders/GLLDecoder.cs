using System;
using System.Text;

namespace nmea2kml.Decoders
{
    class GLLDecoder : IDecoder
    {
        private DateTime SentanceTime;
        private String LatitudeDegrees;
        private String LatitudeMinutes;
        private bool North;
        private String LongitudeDegrees;
        private String LongitudeMinutes;
        private bool East;
        public GLLDecoder(String GLLString)
        {
            var data = GLLString.Split(',');

            //Sanity check
            if (!data[0].EndsWith("GLL"))
            {
                throw new NotSupportedException("Unsupported NMEA 0183 Sentence passed to GLL decoder");
            }

            if(data.Length == 7)
            {
            var hour = int.Parse(data[5].Substring(0, 2));
            var minute = int.Parse(data[5].Substring(2, 2));
            var second = int.Parse(data[5].Substring(4, 2));
            this.SentanceTime = new DateTime(1970, 01, 01, hour, minute, second);
            }

            this.LatitudeDegrees = data[1].Substring(0, 2);
            this.LatitudeMinutes = data[1].Substring(2);
            this.North = (data[2] == "N");
            while(this.LatitudeDegrees.StartsWith("0"))
            {
                //Trim off leading zeros
                this.LatitudeDegrees = this.LatitudeDegrees.Substring(1);
            }

            this.LongitudeDegrees = data[3].Substring(0, 3);
            this.LongitudeMinutes = data[3].Substring(3);
            while(this.LongitudeDegrees.StartsWith("0"))
            {
                //Trim off leading zeros
                this.LongitudeDegrees = this.LongitudeDegrees.Substring(1);
            }
            this.East = (data[4] == "E");

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
            sb.AppendLine("<name>NMEA0183 GLLDecoder Fix</name>");
            sb.AppendLine("<description>");
            sb.AppendLine("<![CDATA[");
            if(this.SentanceTime != null)
            {
                sb.AppendFormat("Fix at {0}\n", this.SentanceTime.TimeOfDay.ToString());
            }
            sb.AppendLine("]]>");
            sb.AppendLine("</description>");
            sb.AppendLine("<Point>");
            sb.AppendFormat("<coordinates>{0},", ToDecimalDegrees(this.LatitudeDegrees, this.LatitudeMinutes, this.North));
            sb.AppendFormat("{0}</coordinates>\n", ToDecimalDegrees(this.LongitudeDegrees, this.LongitudeMinutes, this.East));
            sb.AppendLine("</Point>");
            sb.AppendLine("</Placemark>");

            return sb.ToString();
        }
    }
}