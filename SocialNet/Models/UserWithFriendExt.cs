﻿using SocialNet.Data.Models;

namespace SocialNet.Models
{
    public class UserWithFriendExt : User
    {
        public bool IsFriendWithCurrent { get; set; }
    }
}
