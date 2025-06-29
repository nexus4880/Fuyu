using System.Security.Cryptography.X509Certificates;

namespace Fuyu.Backend.Security;

public interface ICertificateService
{
    X509Certificate2 GetOrCreateCertificate(string certificatePath, string certificatePassword);
}