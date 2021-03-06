﻿using System;
using BlockchainWallet.Models.Domain;
using BlockchainWallet.Models.Dto;
using BlockchainWallet.Services;
using BlockchainWallet.Utils.Globals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockchainWallet.Controllers
{
    public class BalanceController : BaseController
    {
        private IOptions<NodeData> settings;

        public BalanceController(IOptions<NodeData> settings, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.settings = settings;
        }

        [HttpGet]
        public IActionResult Balance()
        {
            var dtoAsStr = this.TempData[TempDataKeys.BalanceDto] as string;
            BalanceDto dto = null;
            if (string.IsNullOrWhiteSpace(dtoAsStr))
            {
                dto = new BalanceDto();
                
            }
            else
            {
                dto = JsonConvert.DeserializeObject<BalanceDto>(dtoAsStr);
            }

            return this.View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Balance(BalanceDto dto)
        {
            // if valid ...
            dto.ShouldCheckBalance = true;
            this.TempData[TempDataKeys.BalanceDto] = JsonConvert.SerializeObject(dto);
            return this.RedirectToAction(nameof(this.Balance));
        }

        [HttpPost]
        public JsonResult BalanceAjax(string account)
        {
            var balanceCalculator = this.ServiceProvider.GetService<IBalanceCalculator>();
            var nodeData = this.settings.Value;

            decimal balance = 0m;
            
            //todo get from DB default Node parametars like : nodeUrl, starting page, block in on query(sizePerPage)

            foreach (var nodeAddress in nodeData.Url)
            {
                var tempResult = 0m;
                bool success = false;

                NodeInfo nodeInfo = new NodeInfo()
                {
                    UrlAddress = nodeAddress + nodeData.Endpoints.GetBlocks,
                    MaxBlocksInQuery = nodeData.MaxBlocksInQuery,
                    StartingPage = nodeData.StartingPage
                };


                (tempResult, success )= balanceCalculator.GetBalance(account, nodeInfo.UrlAddress, nodeInfo.StartingPage, nodeInfo.MaxBlocksInQuery);

                if (success)
                {
                    balance += tempResult;
                    break;
                }
            }

            
            var result = new { isSuccess = true, balance = balance };
            return this.Json(result);
        }
    }
}