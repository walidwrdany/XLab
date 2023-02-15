﻿using Microsoft.AspNetCore.Identity;

namespace XLab.Web.Data.Entities;

public class UserToken : IdentityUserToken<int>
{
    public User User { get; set; }
}