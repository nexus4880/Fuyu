namespace Fuyu.Backend.Configuration;

public class FuyuConfiguration
{
    public const string SectionName = "Fuyu";

    public string CertificatePath { get; set; }
    public string CertificatePassword { get; set; }
}