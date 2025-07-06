using System;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.BSG.Services;

/// <summary>
/// Used to cache responses for faster handling
/// </summary>
public class ResponseService
{
    public static ResponseService Instance => instance.Value;
    private static readonly Lazy<ResponseService> instance = new(() => new ResponseService());

    private ResponseService()
    {
        EmptyJsonResponse = Json.Stringify(new ResponseBody<object>
        {
            data = null
        });
        EmptyJsonArrayResponse = Json.Stringify(new ResponseBody<object[]>
        {
            data = []
        });
    }

    /// <summary>
    /// An empty Json response
    /// </summary>
    public string EmptyJsonResponse { get; }
    /// <summary>
    /// An empty Json array response
    /// </summary>
    public string EmptyJsonArrayResponse { get; }
}