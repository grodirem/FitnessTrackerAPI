﻿using System.Net;

namespace BLL.DTOs;

public class ErrorResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}
