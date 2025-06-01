using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json.Linq;
using Splat;

namespace LoLTracker.Utilities
{
    /// <summary>
    /// Provides helper methods for making API calls using an <see cref="HttpClient"/>.
    /// </summary>
    internal class ApiWorker : IEnableLogger
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHelper"/> class with the specified <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> to be used for making API calls.</param>
        public ApiWorker(HttpClient? client = null)
        {
            this.client = client ?? new();
            this.Log().Info("ApiHelper initialized");
        }

        /// <summary>
        /// Gets the <see cref="HttpClient"/> instance used by this helper.
        /// </summary>
        public HttpClient Client => client;

        /// <summary>
        /// Makes an API call to the specified URL and endpoint, with optional parameters.
        /// </summary>
        /// <param name="url">The base URL for the API call.</param>
        /// <param name="endpoint">The endpoint to be appended to the base URL.</param>
        /// <param name="parameters">Optional parameters to include in the API call.</param>
        /// <returns>A task that represents the asynchronous operation, containing the response as a <see cref="JToken"/>.</returns>
        public async Task<JToken> ApiCall(Uri url, string endpoint, Dictionary<string, string>? parameters = null)
        {
            try
            {
                this.Log().Debug("Making API call to {Url}{Endpoint} with {ParamCount} parameters", url, endpoint, parameters?.Count ?? 0);
                Uri destination = new($"{url}{endpoint}{GetParamsString(parameters)}");
                var result = await BuildRequest(destination).CallAsync();
                this.Log().Debug("API call successfully completed.");
                return result;
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "API call failed to {Url}{Endpoint}", url, endpoint);
                throw;
            }
        }

        /// <summary>
        /// Builds a request using the specified base URL.
        /// </summary>
        /// <param name="url">The base URL for the API call.</param>
        /// <returns>An instance of <see cref="IApiCallBuilder"/> for constructing the API call.</returns>
        public IApiCallBuilder BuildRequest(Uri url)
        {
            this.Log().Debug("Building API request for URL: {Url}", url);
            return new ApiCallBuilder(Client, url);
        }

        /// <summary>
        /// Constructs a query string from the provided parameters.
        /// </summary>
        /// <param name="parameters">A dictionary of parameters to include in the query string.</param>
        /// <returns>A query string formatted as <c>?key=value&amp;key2=value2</c>.</returns>
        private static string GetParamsString(Dictionary<string, string>? parameters)
        {
            StringBuilder paramsList = new();
            if (parameters != null && parameters.Count > 0)
            {
                paramsList.Append('?');
                bool first = true;
                foreach (var parameter in parameters)
                {
                    if (!first) paramsList.Append('&');
                    paramsList.Append(parameter.Key).Append('=').Append(parameter.Value);
                    first = false;
                }
            }

            return paramsList.ToString();
        }

        /// <summary>
        /// A private implementation of <see cref="IApiCallBuilder"/> for constructing API calls.
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the <see cref="ApiCallBuilder"/> class.
        /// </remarks>
        /// <param name="client">The <see cref="HttpClient"/> to use for the API call.</param>
        /// <param name="uri">The base URI for the API call.</param>
        /// <param name="logger">The logger to use for logging API operations.</param>
        private class ApiCallBuilder(HttpClient client, Uri uri) : IApiCallBuilder, IEnableLogger
        {
            /// <summary>
            /// Executes the API call asynchronously and returns the response as a <see cref="JToken"/>.
            /// </summary>
            /// <returns>A task that represents the asynchronous operation, containing the response as a <see cref="JToken"/>.</returns>
            public async Task<JToken> CallAsync()
            {
                try
                {
                    this.Log().Debug("Making HTTP GET request to {Uri}", uri);
                    HttpResponseMessage response;
                    int attempts = 0;
                    do
                    {
                        response = await client.GetAsync(uri);
                        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        {
                            attempts++;
                            if (attempts >= 3)
                            {
                                this.Log().Error("Couldn't process the request because of TooManyRequests");
                                return ThrowHelper.ThrowInvalidOperationException<JToken>("Too many requests. Please, wait a few minutes until sending new requests.");
                            }
                            this.Log().Warn("Received TooManyRequests, throttle for attempt {attempt}/3...", attempts);
                            await Task.Delay(1000);
                            continue;
                        }
                        response.EnsureSuccessStatusCode();
                        break;
                    } while (attempts < 3);
                    var content = await response.Content.ReadAsStringAsync();
                    this.Log().Debug("Received response with status code {StatusCode}", response.StatusCode);
                    return JToken.Parse(content);
                }
                catch (Exception ex)
                {
                    this.Log().Error(ex, "HTTP GET request failed to {Uri}", uri);
                    throw;
                }
            }

            /// <summary>
            /// Sets the endpoint for the API call.
            /// </summary>
            /// <param name="endpoint">The endpoint to be appended to the base URI.</param>
            /// <returns>The current instance of <see cref="IApiCallBuilder"/> for method chaining.</returns>
            public IApiCallBuilder WithEndpoint(string endpoint)
            {
                this.Log().Debug("Adding endpoint {Endpoint} to {Uri}", endpoint, uri);
                uri = new Uri(uri, endpoint);
                return this;
            }

            /// <summary>
            /// Adds a parameter to the API call.
            /// </summary>
            /// <param name="key">The key of the parameter.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>The current instance of <see cref="IApiCallBuilder"/> for method chaining.</returns>
            public IApiCallBuilder WithParams(string key, object value)
            {
                this.Log().Debug("Adding parameter {Key}={Value} to {Uri}", key, value, uri);
                string query;
                if (string.IsNullOrWhiteSpace(uri.Query))
                {
                    query = $"?{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}";
                }
                else
                {
                    query = uri.Query + $"&{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}";
                }

                uri = new Uri(uri.GetLeftPart(UriPartial.Path) + query);
                return this;
            }
        }
    }
}