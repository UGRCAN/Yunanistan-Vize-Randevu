using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace KosmosBG
{
    public class Worker : BackgroundService
    {
        private ApiSettings? _apiSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;
        private static readonly HttpClient client = new HttpClient();
        public Worker(IConfiguration configuration, ILogger<Worker> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _apiSettings = _configuration.GetSection("ApiSettings").Get<ApiSettings>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //await TestHttp();

            while (!stoppingToken.IsCancellationRequested)
            {
                
                
                int year = 2024;
                int month = _apiSettings.Month;
                int daysInMonth = DateTime.DaysInMonth(year, _apiSettings.Month);

                _logger.LogInformation("HTTP istekleri başladı.");
                //await SendSms("SMS deneme");
                for (int day = 1; day <= daysInMonth; day++)
                {
                    string dateString = new DateTime(year, _apiSettings.Month, day).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                    string url = $"https://api.kosmosvize.com.tr/api/AppointmentLayouts/GetAppointmnetHourQoutaInfo?nationalityNumber={_apiSettings.NationalityNumber}&dealerId={_apiSettings.DealerId}&date={dateString}&appointmentTypeId={_apiSettings.AppointmentTypeId}&onlyAvailable={_apiSettings.OnlyAvailable}";

                    

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    // HTTP başlıklarını ekleme
                    request.Headers.Add("authority", "api.kosmosvize.com.tr");
                    request.Headers.Add("accept", "application/json");
                    request.Headers.Add("accept-language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                    request.Headers.Add("origin", "https://basvuru.kosmosvize.com.tr");
                    request.Headers.Add("referer", "https://basvuru.kosmosvize.com.tr/");
                    request.Headers.Add("sec-ch-ua", "\"Not A;Brand\";v=\"99\", \"Google Chrome\";v=\"121\", \"Chromium\";v=\"121\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-fetch-dest", "empty");
                    request.Headers.Add("sec-fetch-mode", "cors");
                    request.Headers.Add("sec-fetch-site", "same-site");
                    request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");

                    try
                    {

                        HttpResponseMessage response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        //_logger.LogInformation(responseBody);
                        if (!string.IsNullOrEmpty(responseBody) && responseBody != "[]")
                        {
                            await SendSms(responseBody);
                        }

                    }
                    catch (HttpRequestException e)
                    {
                        _logger.LogError($"Hata: {e.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                _logger.LogInformation($"Tarama tamamlandı: {DateTime.Now}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); 
            }
        }

        private async Task TestHttp()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation(responseBody);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogInformation("\nException Caught!");
                    _logger.LogInformation("Message :{0} ", e.Message);
                }
            }
        }

        public async Task SendSms(string response)
        {


            // Twilio API Bilgileri
            string accountSid = _apiSettings.AccountSid;
            string authToken = _apiSettings.AuthToken;
            
            TwilioClient.Init(accountSid, authToken);

            var message = await MessageResource.CreateAsync(
                body: response,
                from: new Twilio.Types.PhoneNumber(_apiSettings.From),
                to: new Twilio.Types.PhoneNumber(_apiSettings.To)
                
            );


        }
    }

}
