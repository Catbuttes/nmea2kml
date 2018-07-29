using System;
using System.Text;

namespace nmea2kml
{
    class GGADecoder
    {
        private DateTime SentanceTime;
        private String Latitude;
        private bool North;
        private String Longitude;
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

            this.Latitude = String.Format("{0} {1}", data[2].Substring(0, 2), data[2].Substring(2));
            this.North = (data[3] == "N");
            this.Longitude = String.Format("{0} {1}", data[4].Substring(0, 3), data[4].Substring(3));
            if(this.Longitude.StartsWith("0"))
            {
                this.Longitude = this.Longitude.Substring(1);
            }
            this.East = (data[5] == "E");

            if (data[9].Length > 1)
            {
                this.Altitude = int.Parse(data[9]);
            }
            else
            {
                this.Altitude = -1;
            }

            this.Satellites = int.Parse(data[7]);
        }

        private static string ToDecimalDegrees(string Value, bool positive)
        {
            var data = Value.Split(' ');
            var dmins = float.Parse(data[1]);
            var ddegs = dmins / 60;
            var degs = float.Parse(data[0]);
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
            sb.AppendFormat("<coordinates>{0},", ToDecimalDegrees(this.Latitude, this.North));
            sb.AppendFormat("{0}</coordinates>\n", ToDecimalDegrees(this.Longitude, this.East));
            sb.AppendLine("</Point>");
            sb.AppendLine("</Placemark>");

            return sb.ToString();
        }
    }
}