using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LIBCORE.BusinessLayer;
using LIBCORE.DataRepository;
using LIBCORE.Domain;
using LIBCORE.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LIBCORE.Helper
{
    public sealed class Functions
    {
        private Functions() { }


        private static string RemoveSpecialChars(string text)
        {
            Regex regex = new Regex("[^a-zA-Z0-9 -]");
            return regex.Replace(text, "");
        }

        public static string GetWhereValue(string fieldName, string data, FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.String:
                    return "[" + fieldName + "] LIKE '%" + data + "%'";
                case FieldType.Date:
                    return "[" + fieldName + "] = '" + data + "'";
                case FieldType.Boolean:
                    if (data == "false")
                        return "([" + fieldName + "] = 0 OR [" + fieldName + "] IS NULL)";
                    else
                        return "[" + fieldName + "] = 1";
                case FieldType.Numeric:
                    if (data == "0")
                        return "([" + fieldName + "] = " + data + " OR [" + fieldName + "] IS NULL)";
                    else
                        return "[" + fieldName + "] = " + data;
                case FieldType.Decimal:
                    if (data == "0" || data == "0.0" || data == "0.00")
                        return "([" + fieldName + "] = " + data + " OR [" + fieldName + "] IS NULL)";
                    else
                        return "[" + fieldName + "] = " + data;
                default:
                    return "[" + fieldName + "] = '" + data + "'";
            }
        }

        public static void AddModelServices(IServiceCollection services) 
        {
            services.AddScoped<Member>();
            services.AddScoped<News>();
            services.AddScoped<CertificateType>();
            services.AddScoped<Certificate>();
            services.AddScoped<Calendar>();
            services.AddScoped<InExpen>();
            services.AddScoped<EmailService>();
        }
        public static void AddDataRepositoryAndBusinessLayerServices(IServiceCollection services, string connectionString)
        {
            // Đăng ký JwtTokenGenerator
            services.AddSingleton<JwtTokenGenerator>();

            // Đăng ký Repositories
            services.AddScoped<IMemberRepository>(provider =>
            {
                var jwtTokenGenerator = provider.GetRequiredService<JwtTokenGenerator>();
                var emailService = provider.GetRequiredService<EmailService>();
                return new MemberRepository(connectionString, jwtTokenGenerator, emailService); // ✅ Đầy đủ
            });

            services.AddScoped<INewsRepository>(provider => new NewsRepository(connectionString));
            services.AddScoped<ICertificateTypeRepository>(provider => new CertificateTypeRepository(connectionString));
            services.AddScoped<ICertificateRepository>(provider => new CertificateRepository(connectionString));
            services.AddScoped<ICalendarRepository>(provider => new CalendarRepository(connectionString));
            services.AddScoped<IInExpenRepository>(provider => new InExpenRepository(connectionString));

            // Đăng ký Business Layers
            services.AddScoped<IMemberBusinessLayer>(provider =>
            {
                var memberRepository = provider.GetRequiredService<IMemberRepository>();
                var jwtTokenGenerator = provider.GetRequiredService<JwtTokenGenerator>();
                var emailService = provider.GetRequiredService<EmailService>();
                return new MemberBusinessLayer(memberRepository, jwtTokenGenerator, emailService);
            });

            services.AddScoped<INewsBusinessLayer>(provider =>
            {
                var newsRepository = provider.GetRequiredService<INewsRepository>();
                return new NewsBusinessLayer(newsRepository);
            });

            services.AddScoped<ICertificateTypeBusinessLayer>(provider =>
            {
                var certificateTypeRepository = provider.GetRequiredService<ICertificateTypeRepository>();
                return new CertificateTypeBusinessLayer(certificateTypeRepository);
            });

            services.AddScoped<ICertificateBusinessLayer>(provider =>
            {
                var certificateRepository = provider.GetRequiredService<ICertificateRepository>();
                var certificateTypeBusinessLayer = provider.GetRequiredService<ICertificateTypeBusinessLayer>();
                return new CertificateBusinessLayer(certificateRepository, certificateTypeBusinessLayer);
            });

            services.AddScoped<ICalendarBusinessLayer>(provider =>
            {
                var calendarRepository = provider.GetRequiredService<ICalendarRepository>();
                return new CalendarBusinessLayer(calendarRepository);
            });

            services.AddScoped<IInExpenBusinessLayer>(provider =>
            {
                var inExpenRepository = provider.GetRequiredService<IInExpenRepository>();
                var memberBusinessLayer = provider.GetRequiredService<IMemberBusinessLayer>();
                return new InExpenBusinessLayer(inExpenRepository, memberBusinessLayer);
            });
        }
    }
}
