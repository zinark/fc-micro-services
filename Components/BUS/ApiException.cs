﻿using FCMicroservices.Extensions;

namespace FCMicroservices.Components.BUS
{
    public class ApiException : Exception
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public string ErrorCode { get; private set; }
        public int StatusCode { get; private set; } = 500;

        /// <summary>
        /// 500
        /// </summary>
        public static ApiException CreateCritical(string message, object data = null, Exception exception = null)
        {
            return new ApiException(message, data, exception).WithStatusCode(500);
        }
        /// <summary>
        /// 400
        /// </summary>
        public static ApiException CreateBadRequest(string message, object data = null, Exception exception = null)
        {
            return new ApiException(message, data, exception).WithStatusCode(400);
        }

        /// <summary>
        /// 403
        /// </summary>
        public static ApiException CreateNotAllowed(string message, object data = null, Exception exception = null)
        {
            return new ApiException(message, data, exception).WithStatusCode(403);
        }


        public ApiException(string message, object data = null, Exception innerException = null) : base(string.Format(message, GetArgs(data)), innerException)
        {
            ErrorCode = string.Format("{0:X}", message.GetHashCode());
            Message = string.Format(message, GetArgs(data));
            Data = data;

        }

        private static object[] GetArgs(object data)
        {
            if (data is string) return new[] { data };
            if (data is int) return new[] { data };

            var result = new List<string>();
            if (data == null) return result.ToArray();

            var props = data.GetType().GetProperties();
            foreach (var prop in props)
            {
                var key = prop.Name.ToString();
                var value = prop.GetValue(data)?.ToJson();

                result.Add($"{key}={value}");
            }

            return result.ToArray();
        }
        public ApiException WithStatusCode(int statuscode)
        {
            StatusCode = statuscode;
            return this;
        }
    }
}
