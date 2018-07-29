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

            var hour = int.Parse(data[1].Substring(0,2));
            var minute = int.Parse(data[1].Substring(2,2));
            var second = int.Parse(data[1].Substring(4,2));
            this.SentanceTime = new DateTime(1970, 01, 01, hour, minute, second);  

            this.Latitude = String.Format("{0} {1}", data[2].Substring(0, 2), data[2].Substring(2));
            this.North = (data[3] == "N");
            this.Longitude = String.Format("{0} {1}", data[4].Substring(0, 2), data[4].Substring(2));
            this.East = (data[5] == "E");

            if(data[9].Length > 1)
                this.Altitude = int.Parse(data[9]);
            else
                this.Altitude = -1;

            this.Satellites = int.Parse(data[7]);
        }

        public String PrettyPrint()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Fix at:     {0}\n", this.SentanceTime.TimeOfDay.ToString());
            sb.AppendFormat("Latitude:   {0} {1}\n", this.Latitude, this.North?"N":"S");
            sb.AppendFormat("Longitude:  {0} {1}\n", this.Longitude, this.East?"E":"W");
            if(this.Altitude > 0)
            {
                sb.AppendFormat("Altitude:   {0}", this.Altitude);
            }

            if(this.Satellites > 0)
            {
                sb.AppendFormat("Satellites: {0}", this.Satellites);
            }
            return sb.ToString();
        }
    }
}