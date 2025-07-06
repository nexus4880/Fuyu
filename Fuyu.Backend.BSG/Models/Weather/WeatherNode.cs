using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Weather;

[DataContract]
public class WeatherNode
{
    [DataMember(Name = "timestamp")]
    public long Timestamp { get; set; }

    [DataMember(Name = "time")]
    public string Time { get; set; }

    [DataMember(Name = "cloud")]
    public float Cloud { get; set; }

    [DataMember(Name = "wind_speed")]
    public float WindSpeed { get; set; }

    [DataMember(Name = "wind_direction")]
    public int WindDirection { get; set; }

    [DataMember(Name = "wind_gustiness")]
    public float WindGustiness { get; set; }

    [DataMember(Name = "rain")]
    public float Rain { get; set; }

    [DataMember(Name = "rain_intensity")]
    public float RainIntensity { get; set; }

    [DataMember(Name = "fog")]
    public float Fog { get; set; }

    [DataMember(Name = "temp")]
    public float Temperature { get; set; }

    [DataMember(Name = "pressure")]
    public float Pressure { get; set; }

    [DataMember(Name = "date")]
    public string Date { get; set; }
}