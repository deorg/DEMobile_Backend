using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Concrete
{
    public class Security
    {
        public string generateOTP()
        {
            string numbers = "1234567890";
            string otp = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, numbers.Length);
                    character = numbers.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }
        public string generateRefCode()
        {
            string numbers = "1234567890";
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string characters = numbers + alphabets;
            string refCode = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (refCode.IndexOf(character) != -1);
                refCode += character;
            }
            return refCode;
        }
    }
}