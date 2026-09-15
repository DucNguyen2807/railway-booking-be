using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public BaseResponse(bool success, string? message = null, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static BaseResponse<T> SuccessResponse(T? data = default, string? message = null) =>
            new BaseResponse<T>(true, message, data);

        public static BaseResponse<T> SuccessResponse(string? message = null) =>
            new BaseResponse<T>(true, message);

        public static BaseResponse<T> FailureResponse(T? data = default, string? message = null) =>
            new BaseResponse<T>(false, message, data);

        public static BaseResponse<T> FailureResponse(string? message = null) =>
            new BaseResponse<T>(false, message);
    }
}
