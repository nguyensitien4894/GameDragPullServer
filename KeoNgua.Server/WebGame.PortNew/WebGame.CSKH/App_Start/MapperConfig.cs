using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.AccountWalletLogs;
using MsWebGame.CSKH.Models.Agencies;
using MsWebGame.CSKH.Models.Cards;
using MsWebGame.CSKH.Models.CashFlowOverviews;
using MsWebGame.CSKH.Models.Complains;
using MsWebGame.CSKH.Models.Games;
using MsWebGame.CSKH.Models.GiftCodes;
using MsWebGame.CSKH.Models.HistoryTranfers;
using MsWebGame.CSKH.Models.Mails;
using MsWebGame.CSKH.Models.SystemMails;
using MsWebGame.CSKH.Models.Telecoms;
using MsWebGame.CSKH.Models.SmsOperators;

namespace MsWebGame.CSKH.App_Start
{
    public class MapperConfig
    {

        public static void RegisterMapper()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Agency, AgencyModel>();
                cfg.CreateMap<HistoryTransfer, HistoryTransferModel>();
                cfg.CreateMap<Complain, ComplainModel>();
                cfg.CreateMap<GameGiftCode, GameGiftCodeModel>();
                cfg.CreateMap<TelecomOperators, TelecomOperatorModel>();
                cfg.CreateMap<Card, CardModel>();
                cfg.CreateMap<UserCardRecharge, UserCardRechardModel>();
                cfg.CreateMap<SystemMail, SystemMailModel>();
                cfg.CreateMap<Mail,MailModel>();
                cfg.CreateMap<CashflowOverview, CashFlowOverviewModel>();
                cfg.CreateMap<GameFunds, GameFundModel>();
                cfg.CreateMap<GameIndex, GameIndexModel>();
                cfg.CreateMap<SmsOperators, SmsOperatorModel>();

            });
        }
    }
}