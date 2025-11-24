using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservaYa.Services //by chatGpt , thx  -- yo lo queria hacer por controller pero esto es mejor
{
    public static class EncriptarService
    {
        public static string EncriptarId(int id)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(id.ToString()));
        }

        public static int DescriptarId(string codificado)
        {
            return int.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(codificado)));
        }
    }
}