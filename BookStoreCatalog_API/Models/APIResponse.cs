﻿using System.Net;

namespace BookStoreCatalog_API.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }

        public int TotalPages { get; set; }
        
        public APIResponse() 
        { 
            ErrorMessages = new List<string>();
        }


    }
}
