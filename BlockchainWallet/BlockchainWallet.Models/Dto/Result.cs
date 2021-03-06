﻿using System.Collections.Generic;

namespace BlockchainWallet.Models.Dto
{
    public class Result
    {
        public Result()
        {
            this.Messages = new List<string>();    
        }

        public bool IsSuccess { get; set; }

        public dynamic Data { get; set; }

        public IList<string> Messages { get; set; }
    }
}
