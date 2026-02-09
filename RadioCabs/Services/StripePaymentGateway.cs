using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RadioCabs.Models;

namespace RadioCabs.Services
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly StripeOptions _options;

        public StripePaymentGateway(HttpClient httpClient, IOptions<StripeOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value ?? new StripeOptions();

            _httpClient.BaseAddress = new Uri("https://api.stripe.com/");
            if (!string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.SecretKey);
            }
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.SecretKey);

        public async Task<PaymentSessionResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request, CancellationToken cancellationToken)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("Stripe is not configured. Provide a Stripe:SecretKey value.");
            }

            var payload = new Dictionary<string, string>
            {
                ["mode"] = "payment",
                ["success_url"] = request.SuccessUrl,
                ["cancel_url"] = request.CancelUrl,
                ["payment_method_types[0]"] = "card",
                ["line_items[0][quantity]"] = "1",
                ["line_items[0][price_data][currency]"] = "usd",
                ["line_items[0][price_data][unit_amount]"] = (request.Amount * 100).ToString(),
                ["line_items[0][price_data][product_data][name]"] = $"{request.Section} Payment - {request.Name}",
                ["metadata[section]"] = request.Section,
                ["metadata[entityId]"] = request.EntityId.ToString(),
                ["metadata[paymentType]"] = request.PaymentType
            };

            using var content = new FormUrlEncodedContent(payload);
            using var response = await _httpClient.PostAsync("v1/checkout/sessions", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;

            return new PaymentSessionResult
            {
                SessionId = root.GetProperty("id").GetString() ?? string.Empty,
                CheckoutUrl = root.GetProperty("url").GetString() ?? string.Empty
            };
        }

        public async Task<PaymentSessionResult> GetCheckoutSessionAsync(string sessionId, CancellationToken cancellationToken)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("Stripe is not configured. Provide a Stripe:SecretKey value.");
            }

            using var response = await _httpClient.GetAsync($"v1/checkout/sessions/{sessionId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;

            var metadata = new Dictionary<string, string>();
            if (root.TryGetProperty("metadata", out var metadataElement) && metadataElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in metadataElement.EnumerateObject())
                {
                    metadata[property.Name] = property.Value.GetString() ?? string.Empty;
                }
            }

            return new PaymentSessionResult
            {
                SessionId = sessionId,
                PaymentStatus = root.GetProperty("payment_status").GetString() ?? string.Empty,
                Metadata = metadata
            };
        }
    }
}
