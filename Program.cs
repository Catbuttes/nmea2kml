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
            var fixData = new List<Decoders.IDecoder>();
            using (var textFile = new System.IO.StreamReader(file))
            {
                var dataLine = textFile.ReadLine();
                var decoderFactory = new Decoders.DecoderFactory();
                while (!String.IsNullOrEmpty(dataLine))
                {
                    var fix = decoderFactory.CreateDecoder(dataLine);
                    if(fix != null)
                        fixData.Add(fix);
                    
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

            using (var writeFile = new System.IO.StreamWriter(kmlFile))
            {
                writeFile.Write(output.ToString());
            }

        }
    }
}
