using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


// Make into a factory patern one for GPX XML markup and another for a different type of markup
namespace MountainBikeTracker_WP8.Models
{
    public class GPXMarkupWriter
    {
        public string GetGPXMarkup(GeoCoordinateCollection geos, List<DateTime> times, string description, string name)
        {
            GPXMarkup m = new GPXMarkup(geos, times, description, name);

            XmlSerializer ser = new XmlSerializer(typeof(GPXMarkup));
            string xml;
            using (StringWriter s = new StringWriter())
            {
                ser.Serialize(s, m);
                StringBuilder sb = s.GetStringBuilder();
                sb.Remove(0, 41);
                xml = sb.ToString();
            }

            return xml;
        }

        [XmlRoot("gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
        internal class GPXMarkup
        {
            [XmlElement("trk")]
            internal GPXTrack Track { get; set; }

            internal GPXMarkup(GeoCoordinateCollection geos, List<DateTime> times, string description, string name)
            {
                this.Track = new GPXTrack();
                this.Track.Description = description;
                this.Track.Name = name;

                this.Track.TrackSegment = new GPXPoint[geos.Count];
                for (int i = 0; i < geos.Count; i++)
                {
                    GPXPoint p = new GPXPoint()
                    {
                        Latitude = geos[i].Latitude,
                        Longitude = geos[i].Longitude,
                        Elevation = geos[i].Altitude,
                        Time = times[i]
                    };

                    this.Track.TrackSegment[i] = p;
                }
            }
        }

        internal class GPXTrack
        {
            [XmlElement("name")]
            internal string Name { get; set; }
            
            [XmlElement("desc")]
            internal string Description { get; set; }
            
            [XmlArray("trkseg")]
            [XmlArrayItem("trkpt")]
            internal GPXPoint[] TrackSegment { get; set; }
        }

        internal class GPXPoint
        {
            [XmlAttribute("lat")]
            internal double Latitude { get; set; }
            
            [XmlAttribute("lon")]
            internal double Longitude { get; set; }
            
            [XmlElement("ele")]
            internal double Elevation { get; set; }
            
            [XmlElement("time")]
            internal DateTime Time { get; set; }
        }
    }
}
