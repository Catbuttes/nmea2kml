using System;
using System.Collections.Generic;
namespace nmea2kml
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Use me correctly");
                return;
            }

            var file = args[0];
            var kmlFile = args[1];
            var fixData = new List<GGADecoder>();
            using (var textFile = new System.IO.StreamReader(file))
            {
                var dataLine = textFile.ReadLine();

                while (!String.IsNullOrEmpty(dataLine))
                {
                    if (dataLine.StartsWith("$GPGGA"))
                    {
                        var fix = new GGADecoder(dataLine);
                        fixData.Add(fix);
                    }

                    dataLine = textFile.ReadLine();
                }
            }
            var output = new System.Text.StringBuilder();

            output.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            output.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            output.AppendLine("<Document>");

            foreach (var fix in fixData)
            {
                output.Append(fix.KmlPlacemark());
            }

            output.AppendLine("</Document>");
            output.AppendLine("</kml>");

            using(var writeFile = new System.IO.StreamWriter(kmlFile))
            {
                writeFile.Write(output.ToString());
            }

        }
    }
}
