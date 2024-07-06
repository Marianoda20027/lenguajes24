using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace storeapi.Models{
    public class LoginModel{
        public string? userName { get;}
        public string? userPassword { get;}

        public LoginModel(string userName, string userPassword){
            if (string.IsNullOrEmpty(userName))        
                throw new ArgumentException($"{nameof(userName)} no puede ser nulo ni vacío");            

            if (string.IsNullOrEmpty(userPassword))
                throw new ArgumentException($"{nameof(userPassword)} no puede ser nulo ni vacío");
            this.userName = userName;
            this.userPassword = userPassword;
        }
    }
}