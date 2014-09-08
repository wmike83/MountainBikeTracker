using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MountainBikeTracker_WP8.Helpers
{
    [XmlRoot("gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GPXMarkup
    {
        [XmlElement("trk")]
        public GPXTrack Track { get; set; }

        public GPXMarkup(GeoCoordinateCollection geos, List<DateTime> dates, string name)
        {
            this.Track = new GPXTrack()
            {
                Name = name,
                //<![CDATA[Apr 21, 2013  7:12 pm]]>
                Description = "<![CDATA[" + DateTime.Now.ToShortDateString() + "]]>",
                TrackSegment = new GPXPoint[geos.Count]
            };

            for(int i = 0; i < geos.Count; i++ )
            {
                GPXPoint p = new GPXPoint()
                {
                    Latitude = geos[i].Latitude,
                    Longitude = geos[i].Longitude,
                    Elevation = geos[i].Altitude,
                    Time = dates[i]
                };
                this.Track.TrackSegment[i] = p;
            }
        }

        public string ToXML()
        {
            XmlSerializer ser = new XmlSerializer(typeof(GPXMarkup));
            string xml;
            using (StringWriter s = new StringWriter())
            {
                ser.Serialize(s, this);
                StringBuilder sb = s.GetStringBuilder();
                sb.Remove(0, 41);
                xml = sb.ToString();
            }
            return xml;
        }
    }

    private class GPXTrack
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("desc")]
        public string Description { get; set; }
        [XmlArray("trkseg")]
        [XmlArrayItem("trkpt")]
        public GPXPoint[] TrackSegment { get; set; }
    }

    private class GPXPoint
    {
        [XmlAttribute("lat")]
        public double Latitude { get; set; }
        [XmlAttribute("lon")]
        public double Longitude { get; set; }
        [XmlElement("ele")]
        public double Elevation { get; set; }
        [XmlElement("time")]
        public DateTime Time { get; set; }
    }
}
