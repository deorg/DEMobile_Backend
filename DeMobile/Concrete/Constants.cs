using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Concrete
{
    public static class Constants
    {     
        public static class OracleDb
        {
            public static class Development
            {
                public const string conString = "User Id=DMOBILE;Password=DMOBILE;Data Source=35.247.128.114:1521/DEORCL;";
                public const string Host = "35.247.128.114";
                public const string Port = "1521";
                public const string Source = "DEORCL";
                public const string Username = "DMOBILE";
                public const string Password = "DMOBILE";
            }
            public static class Production
            {
                public const string conString = "User Id=DMOBILE;Password=DMOBILE;Data Source=localhost:1521/DEORCL;";
                public const string Host = "localhost";
                public const string Port = "1521";
                public const string Source = "DEORCL";
                public const string Username = "DMOBILE";
                public const string Password = "DMOBILE";
            }
        }
        public static class ChillPay
        {
            public static class Uat
            {
                public const string Host = "https://uatappsrv2.modern-pay.com";
                public const string MerchantCode = "M000052";            
                public const int Currency = 764;
                public const string LangCode = "TH";
                public const int RouteNo = 1;
                public const string ApiKey = "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk";
                public const string Md5SecretKey = "RyFYDwI3Se9y6_2FiBF4o2_hYgccTvjkt5TBo9mBmDor4IXNB46j5Fj3mIt7BjF_tviacnelruOrioqOpEY5G56qeL1a_xQb6zG1LFq0vq9rLAc2zHDoxpeHPOZE6tbDpYFeQRM_Wqt7vcIefg22S9b3cvIqXMR1Boy9JOlPHuy1n0SmM4AorOMF7T3AabnDRlQAZfKr8SQkyT8yEZR7g1vDKGLaiX6vD9BSPBEbGNb2GBuGdagd3SC1HM2e8Dc";
                public static class Api
                {
                    public const string CreatePayment = "api/v1/Payment/";
                    public const string CheckPaymentStatus = "api/v1/PaymentStatus/";
                }
            }
            public static class Production
            {
                public const string Host = "https://uatappsrv2.modern-pay.com";
                public const string MerchantCode = "M000052";
                public const int Currency = 764;
                public const string LangCode = "TH";
                public const int RouteNo = 1;
                public const string ApiKey = "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk";
                public const string Md5SecretKey = "RyFYDwI3Se9y6_2FiBF4o2_hYgccTvjkt5TBo9mBmDor4IXNB46j5Fj3mIt7BjF_tviacnelruOrioqOpEY5G56qeL1a_xQb6zG1LFq0vq9rLAc2zHDoxpeHPOZE6tbDpYFeQRM_Wqt7vcIefg22S9b3cvIqXMR1Boy9JOlPHuy1n0SmM4AorOMF7T3AabnDRlQAZfKr8SQkyT8yEZR7g1vDKGLaiX6vD9BSPBEbGNb2GBuGdagd3SC1HM2e8Dc";
                public static class Api
                {
                    public const string CreatePayment = "api/v1/Payment/";
                    public const string CheckPaymentStatus = "api/v1/PaymentStatus/";
                }
            }
        }
    }
}