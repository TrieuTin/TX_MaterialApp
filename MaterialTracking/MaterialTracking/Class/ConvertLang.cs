using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialTracking.Class
{
   public class ConvertLang
    {
        private static ConvertLang _convert;

        public static ConvertLang Convert { get => _convert == null ? _convert = new ConvertLang() : _convert; }
        ConvertLang()
        {

        }
        public string Translate_LYM(string vn,string myanmar)
        {

            switch (DB.StoreLocal.Instant.Myfac)
            {
                case DB.MyFactory.LYM:
                    return  myanmar;
                    
                default:
                    return vn;
              
            }                      
        }
    }
}
