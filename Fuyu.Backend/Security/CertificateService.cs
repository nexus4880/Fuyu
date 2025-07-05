using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Fuyu.Common.IO;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.Security;

public class CertificateService : ICertificateService
{
    private readonly ILogger<CertificateService> _logger;

    public CertificateService(ILogger<CertificateService> logger)
    {
        _logger = logger;
    }

    public X509Certificate2 GetOrCreateCertificate(string certificatePath, string certificatePassword)
    {
        if (certificatePath != null && VFS.Exists(certificatePath))
        {
            return LoadExistingCertificate(certificatePath, certificatePassword);
        }

        return CreateAndSaveCertificate(certificatePath, certificatePassword);
    }

    private X509Certificate2 LoadExistingCertificate(string certificatePath, string certificatePassword)
    {
        var certificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword);

        if (DateTime.UtcNow > certificate.NotAfter)
        {
            throw new InvalidOperationException("Certificate has expired");
        }

        _logger.LogInformation("Loaded certificate {CertificateName}", certificate.SubjectName.Name);
        return certificate;
    }

    private X509Certificate2 CreateAndSaveCertificate(string certificatePath, string certificatePassword)
    {
        var certificate = GenerateSelfSignedCertificate(certificatePassword, out var certificateBytes);

        if (certificatePath != null)
        {
            VFS.WriteBytes(certificatePath, certificateBytes);
            _logger.LogInformation("Wrote certificate to {CertificatePath}", certificatePath);
        }

        return certificate;
    }

    private static X509Certificate2 GenerateSelfSignedCertificate(string password, out byte[] certificateBytes)
    {

        using var rsa = RSA.Create();
        var request = new CertificateRequest("cn=Fuyu", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var certificate = request.CreateSelfSigned(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30));

        certificateBytes = certificate.Export(X509ContentType.Pfx, password);
        return X509CertificateLoader.LoadPkcs12(certificateBytes, password);
    }
}