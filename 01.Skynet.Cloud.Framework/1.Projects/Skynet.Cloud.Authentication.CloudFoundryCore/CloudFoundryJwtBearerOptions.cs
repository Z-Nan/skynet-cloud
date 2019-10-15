﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UWay.Skynet.Cloud.Authentication.CloudFoundry
{
    public class CloudFoundryJwtBearerOptions : JwtBearerOptions
    {
        public CloudFoundryJwtBearerOptions()
        {
            string authURL = "http://" + CloudFoundryDefaults.OAuthServiceUrl;
            ClaimsIssuer = CloudFoundryDefaults.AuthenticationScheme;
            JwtKeyUrl = authURL + CloudFoundryDefaults.JwtTokenUri;
            SaveToken = true;
            TokenValidationParameters.ValidateAudience = false;
            TokenValidationParameters.ValidateIssuer = true;
            TokenValidationParameters.ValidateLifetime = true;
        }

        public string JwtKeyUrl { get; set; }

        public bool Validate_Certificates { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating whether to validate auth server certificate
        /// </summary>
        public bool ValidateCertificates
        {
            get { return Validate_Certificates; }
            set { Validate_Certificates = value; }
        }
    }
}
