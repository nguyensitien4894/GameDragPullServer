using System;
using System.Collections.Generic;
using System.Linq;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using Telerik.Web.Mvc.UI;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Controllers
{
    public class InfoHandler
    {
        private static readonly Lazy<InfoHandler> _instance = new Lazy<InfoHandler>(() => new InfoHandler());

        private static Dictionary<int, string> _mapGameIdToGameName;
        private static Dictionary<int, string> _mapIDToPrivilegeName;
        private static Dictionary<int, string> _mapIDToServicename;

        private readonly string[] _arrServiceName = {"0", "1", "2", "3","4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18",
            "19", "20","21","22", "Nạp thẻ", "Đổi thẻ", "Chuyển khoản", "Giftcode", "Thưởng event", "Rút két", "Nạp két", "" };

        public static InfoHandler Instance
        {
            get { return _instance.Value; }
        }

        private InfoHandler()
        {
            _mapGameIdToGameName = new Dictionary<int, string>();
            _mapIDToPrivilegeName = new Dictionary<int, string>();
            _mapIDToServicename = new Dictionary<int, string>();

            //Game
            var lstGame = ParamConfigDAO.Instance.GetListPrivilegeGameInfo(-1);
            if (lstGame != null && lstGame.Any())
            {
                foreach (var item in lstGame)
                {
                    _mapGameIdToGameName.Add(item.GameID, item.GameName);
                    if (!_mapIDToServicename.ContainsKey(item.GameID))
                    {
                        _mapIDToServicename.Add(item.GameID, item.GameName);
                    }
                }
            }

            //Privilege
            var lstPrivilege = ParamConfigDAO.Instance.GetListPrivilegeLevel(null);
            if (lstPrivilege != null && lstPrivilege.Any())
            {
                foreach (var item in lstPrivilege)
                {
                    _mapIDToPrivilegeName.Add(item.ID, item.PrivilegeName);
                }
            }

            //service name
            for (int i = 23; i < 30; i++)
            {
                if (!_mapIDToServicename.ContainsKey(i))
                {
                    _mapIDToServicename.Add(i, _arrServiceName[i]);
                }
            }
        }

        public string GetDeviceName(int deviceType)
        {
            string val = string.Empty;
            switch (deviceType)
            {
                case 1:
                    val = "Web";
                    break;
                case 2:
                    val = "Android";
                    break;
                case 3:
                    val = "Ios";
                    break;
                default:
                    val = "Undefined";
                    break;
            }

            return val;
        }

        public string GetGameName(int gameId)
        {
            string val = string.Empty;
            _mapGameIdToGameName.TryGetValue(gameId, out val);
            return val;
        }

        public string GetPrivilegeName(int privilegeId)
        {
            string val = string.Empty;
            _mapIDToPrivilegeName.TryGetValue(privilegeId, out val);
            return val;
        }

        public string GetServiceName(int id)
        {
            string val = string.Empty;
            _mapIDToServicename.TryGetValue(id, out val);
            return val;
        }

        public List<ConfigSelect> GetArtifactsBox()
        {
            int totalrecord = 0;
            var lstData = ManagerDAO.Instance.GetListArtifacts(0, null, null, true, null, 1, 500, out totalrecord);
            var lstRs = new List<ConfigSelect>();
            if (lstData != null && lstData.Any())
            {
                foreach (var item in lstData)
                {
                    var rs = new ConfigSelect();
                    rs.Value = item.ArtifactID;
                    rs.Name = item.ArtifactName;
                    lstRs.Add(rs);
                }
            }

            return lstRs;
        }

        public List<ConfigSelect> GetCampaignBox(int ServiceID)
        {
            var lstData = ParamConfigDAO.Instance.GetListGameCampaign(0, null, null, true, null, null, ServiceID);
            var lstRs = new List<ConfigSelect>();
            if (lstData != null && lstData.Any())
            {
                foreach (var item in lstData)
                {
                    var rs = new ConfigSelect();
                    rs.Value = item.CampaignID;
                    rs.Name = item.CampaignName;
                    lstRs.Add(rs);
                }
            }

            return lstRs;
        }

        public List<ConfigSelect> GetGameBox()
        {
            var lstData = ParamConfigDAO.Instance.GetListPrivilegeGameInfo(-1);
            var lstRs = new List<ConfigSelect>();
            if (lstData != null && lstData.Any())
            {
                foreach (var item in lstData)
                {
                    var rs = new ConfigSelect();
                    rs.Value = item.GameID;
                    rs.Name = item.GameName;
                    lstRs.Add(rs);
                }
            }

            return lstRs;
        }
        public List<ConfigSelect> GetAllGameService()
        {

                


                return new List<ConfigSelect>
                {
                      new ConfigSelect() {Value=1,Name="Giftcode",Code="Giftcode" },
                     new ConfigSelect() { Value = 2, Name = "Nạp thẻ", Code = "Nạp thẻ" },
                     new ConfigSelect() { Value = 3, Name = "VIP", Code = "VIP" },
                     new ConfigSelect() { Value = 4, Name = "Đổi thẻ", Code = "Đổi thẻ" },
                     new ConfigSelect() { Value = 5, Name = "Nạp bank", Code = "Nạp bank" },
                     new ConfigSelect() { Value = 6, Name = "Rút bank", Code = "Rút bank" },
                     new ConfigSelect() { Value = 8, Name = "Két", Code = "Két" },
                };

         
        }
        public List<SelectListItem> GetOrderHistoryPlay()
        {
            var lstRs = new List<SelectListItem>();
            lstRs.Add(new SelectListItem() { Value = "1",Text= "Theo thời gian" });
            lstRs.Add(new SelectListItem() { Value = "2", Text = "Theo giá trị" });
            return lstRs;
        }



        public List<SelectListItem> GetGameBaiSearch()
        {
            var lstRs = new List<SelectListItem>();
            lstRs.Add(new SelectListItem() { Value = "1", Text = "Theo phiên" });
            lstRs.Add(new SelectListItem() { Value = "2", Text = "Theo tên hiển thị" });
            return lstRs;
        }
        public List<SelectListItem> GetGameBai()
        {
            var lstRs = new List<SelectListItem>();
            lstRs.Add(new SelectListItem() { Value = "51", Text = "Ba Cây" });
            lstRs.Add(new SelectListItem() { Value = "54", Text = "TIEN_LEN_MN" });
            lstRs.Add(new SelectListItem() { Value = "55", Text = "MAU_BINH" });
            lstRs.Add(new SelectListItem() { Value = "57", Text = "POKER_TEXAS" });
            lstRs.Add(new SelectListItem() { Value = "61", Text = "BA_CAY_GA" });
            lstRs.Add(new SelectListItem() { Value = "62", Text = "BA_CAY_BIEN" });
            lstRs.Add(new SelectListItem() { Value = "63", Text = "Xóc Đĩa" });
            lstRs.Add(new SelectListItem() { Value = "66", Text = "TIEN_LEN_MN_SOLO" });
            return lstRs;
        }

        public List<ConfigSelect> GetRankBox()
        {
            var lstData = ParamConfigDAO.Instance.GetListPrivilegeLevel(null);
            var lstRs = new List<ConfigSelect>();
            if (lstData != null && lstData.Any())
            {
                foreach (var item in lstData)
                {
                    var rs = new ConfigSelect();
                    rs.Value = item.ID;
                    rs.Code = item.PrivilegeCode;
                    rs.Name = item.PrivilegeName;
                    lstRs.Add(rs);
                }
            }

            return lstRs;
        }

        public List<ConfigSelect> GetRoomBox()
        {
            string[] lstRoomName = { "Tất cả", "Phòng 100", "Phòng 1K", "Phòng 10K" };
            var lstRs = new List<ConfigSelect>();
            for (int i = 0; i < 4; i++)
            {
                var rs = new ConfigSelect();
                rs.Value = i;
                rs.Name = lstRoomName[i];
                lstRs.Add(rs);
            }
            return lstRs;
        }

        public List<ConfigSelect> GetWeekDayBox()
        {
            string[] lstDay = { "Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7" };
            var lstRs = new List<ConfigSelect>();
            for (int i = 0; i < lstDay.Length; i++)
            {
                var rs = new ConfigSelect();
                rs.Value = i + 1;
                rs.Name = lstDay[i];
                lstRs.Add(rs);
            }
            return lstRs;
        }

        public List<ConfigSelect> GetHomeNetworkBox()
        {
            string[] lstHomeNetwork = { "Tất cả", "Viettel", "Vinaphone", "Mobifone", "Vietnamobile" };
            var lstRs = new List<ConfigSelect>();
            for (int i = 0; i < lstHomeNetwork.Length; i++)
            {
                var rs = new ConfigSelect();
                rs.Value = i;
                rs.Name = lstHomeNetwork[i];
                lstRs.Add(rs);
            }
            return lstRs;
        }

        public List<ConfigSelect> GetCardValueBox()
        {
            string[] lstCardValue = { "Tất cả", "500", "200", "100", "50", "20", "10" };
            var lstRs = new List<ConfigSelect>();
            for (int i = 0; i < lstCardValue.Length; i++)
            {
                var rs = new ConfigSelect();
                if (i == 0)
                    rs.Value = 0;
                else
                    rs.Value = Int32.Parse(lstCardValue[i]);

                rs.Name = lstCardValue[i];
                lstRs.Add(rs);
            }
            return lstRs;
        }

        public List<ConfigSelect> GetTelecomStatus()
        {
            var lstRs = new List<ConfigSelect>();
            lstRs.Add(new ConfigSelect() { Value = -1, Name = "Tất cả" });
            lstRs.Add(new ConfigSelect() { Value = 1, Name = "Thành công" });
            lstRs.Add(new ConfigSelect() { Value = 2, Name = "Chờ duyệt" });
            lstRs.Add(new ConfigSelect() { Value = 0, Name = "Thất bại" });
            lstRs.Add(new ConfigSelect() { Value = 3, Name = "Hủy đổi thẻ" });
            return lstRs;
        }

        public List<DropDownItem> GetGateList()
        {
            List<DropDownItem> lstRs = new List<DropDownItem>();
            lstRs.Add(new DropDownItem() { Text = "Cổng 1", Value = "1" });
            lstRs.Add(new DropDownItem() { Text = "Cổng 2", Value = "2" });
            lstRs.Add(new DropDownItem() { Text = "Cổng 3", Value = "3" });
            return lstRs;
        }

        public List<SelectListItem> GetAgencyTotal(int serviceId)
        {
            try
            {
                int totalRecord = 0;
                var lstRs = AgencyDAO.Instance.GetListAgencyTotal(null, null, 0, -1, 0, serviceId, 1, 100, out totalRecord);
                return lstRs.Select(c => new SelectListItem()
                {
                    Value = c.AccountId.ToString(),
                    Text = c.FullName,

                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}